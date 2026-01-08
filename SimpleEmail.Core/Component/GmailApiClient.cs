using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Requests;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using Microsoft.IdentityModel.Tokens;

using SimpleEmail.Core.Component.Interface;
using SimpleEmail.Core.Model;
using SimpleEmail.Core.Model.Configuration;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.IocFramework.Application.Attribute;

namespace SimpleEmail.Core.Component
{
    [IocExportSpecific(typeof(IEmailClient), (int)EmailHosts.GmailApi, InstancePolicy.ShareGlobal)]
    public class GmailApiClient : IEmailClient
    {
        protected const string HEADER_TO = "To";
        protected const string HEADER_FROM = "From";
        protected const string HEADER_SUBJECT = "Subject";
        protected const string HEADER_DATE = "Date";

        protected const string MESSAGE_SUMMARY_FIELDS = "id,threadId,historyId,internalDate,payload.partId,payload.mimeType,payload.headers,payload.parts";

        /// <summary>
        /// Gmail API's batch request limit
        /// </summary>
        protected const uint BATCH_LIMIT = 20;

        /// <summary>
        /// Gmail API's max message list results
        /// </summary>
        protected const uint MESSAGE_MAX_RESULTS = 500;

        public async Task<EmailAccount> GetAccountDetail(EmailAccountConfiguration configuration)
        {
            try
            {
                using (var service = await Authenticate(configuration))
                {
                    var profile = service.Users.GetProfile(configuration.EmailAddress);

                    var labels = await service.Users
                                              .Labels
                                              .List(configuration.EmailAddress)
                                              .ExecuteAsync();

                    var resultLabels = labels.Labels
                                             .Where(x => !IsFolderExcluded(x.Name))
                                             .Select(x => service.Users
                                                                .Labels
                                                                .Get(configuration.EmailAddress, x.Id)
                                                                .Execute()).Actualize();



                    // Only inlucde top-level folders. The rest are added recursively.
                    var topLevelFolders = resultLabels.Where(label => !label.Name.Contains('/'))
                                                      .Select(label => new EmailFolder(configuration.EmailAddress, label, resultLabels))
                                                      .ToList();

                    return new EmailAccount()
                    {
                        EmailAddress = EmailAddress.Parse(configuration.EmailAddress),
                        PersonalFolders = topLevelFolders
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Gmail Service Error:  GetAccountDetail", ex);
            }
        }

        public async Task<Email> GetMessage(EmailAccountConfiguration configuration, string folderId, string emailId)
        {
            try
            {
                // Use Google's API to run its authentication service
                using (var service = await Authenticate(configuration))
                {
                    var message = service.Users.Messages.Get(configuration.EmailAddress, emailId).Execute();

                    return ParseMessage(message, EmailAddress.Parse(configuration.EmailAddress), folderId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Gmail Service Error:  GetAccountDetail", ex);
            }
        }

        public async Task<IEnumerable<EmailSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, string folderId, IEnumerable<string> emailIds)
        {
            try
            {
                return await GetSummariesAsyncImpl(configuration, folderId, emailIds);
            }
            catch (Exception ex)
            {
                throw new Exception("Gmail Service Error:  GetAccountDetail", ex);
            }
        }

        public async Task<IEnumerable<EmailSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, string folderId)
        {
            try
            {
                var emailIds = new List<string>();
                var emailAddress = EmailAddress.Parse(configuration.EmailAddress);

                // Get Message Ids
                using (var service = await Authenticate(configuration))
                {
                    var messageRequest = service.Users.Messages.List(configuration.EmailAddress);

                    // Folder Filter
                    messageRequest.LabelIds = folderId;
                    messageRequest.MaxResults = MESSAGE_MAX_RESULTS;

                    string? nextPageToken = null;

                    do
                    {
                        // Set next page token
                        messageRequest.PageToken = nextPageToken;

                        var messageList = messageRequest.Execute();

                        // Go through message Id's and retrieve the rest of the data
                        foreach (var entry in messageList.Messages)
                        {
                            emailIds.Add(entry.Id);
                        }

                        // Next Page
                        nextPageToken = messageList.NextPageToken;
                    }
                    while (!string.IsNullOrEmpty(nextPageToken));
                }

                // Retrieve the Messages
                return await GetSummariesAsyncImpl(configuration, folderId, emailIds);
            }
            catch (Exception ex)
            {
                throw new Exception("Gmail API Error:  GetSummariesAsync", ex);
            }
        }

        public Task<EmailSummary> GetSummaryAsync(EmailAccountConfiguration configuration, string folderId, string emailIds)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<EmailSummary>> GetSummariesAsyncImpl(EmailAccountConfiguration configuration, string folderId, IEnumerable<string> emailIds)
        {
            var emailAddress = EmailAddress.Parse(configuration.EmailAddress);

            // Use Google's API to run its authentication service
            using (var service = await Authenticate(configuration))
            {
                var result = new List<EmailSummary>();
                var batches = new List<BatchRequest>();

                BatchRequest currentBatch = null;
                string? nextPageToken = null;

                do
                {
                    // Go through message Id's and retrieve the rest of the data
                    foreach (var emailId in emailIds)
                    {
                        // Next Batch
                        if (currentBatch == null || (currentBatch != null && currentBatch.Count == BATCH_LIMIT))
                        {
                            currentBatch = new BatchRequest(service);
                            batches.Add(currentBatch);
                        }

                        // Get Messasge
                        var request = service.Users.Messages.Get(configuration.EmailAddress, emailId);

                        // Specify fields to minimize download size
                        request.Fields = MESSAGE_SUMMARY_FIELDS;

                        currentBatch.Queue<Message>(request, (content, error, index, responseMessage) =>
                        {
                            if (error != null)
                                throw new Exception("Batch Error:  (probably 429 limit exceeded)");

                            var summary = ParseSummary(content, emailAddress, folderId);

                            result.Add(summary);
                        });
                    }
                }
                while (!string.IsNullOrEmpty(nextPageToken));

                // -> Add results as they come back. We could do fire-and-forget
                foreach (var request in batches)
                {
                    // KLUDGE!!!  THROTTLING GMAIL REQUESTS! OTHERWISE WE GET A 429 ERROR!
                    //
                    await request.ExecuteAsync();
                }

                return result;
            }
        }

        private async Task<GmailService> Authenticate(EmailAccountConfiguration configuration)
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = configuration.ClientId,
                ClientSecret = configuration.ClientSecret
            };

            var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                // Cache tokens in ~/.local/share/google-filedatastore/CredentialCacheFolder on Linux/Mac
                //
                // TODO: Locate the proper cache directory, and store these there
                //
                DataStore = new FileDataStore(Environment.CurrentDirectory, false),
                Scopes = new[] { "https://mail.google.com/" },
                ClientSecrets = clientSecrets,
                LoginHint = configuration.EmailAddress  // Email address was used
            });

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets,
                                    new[] { GmailService.ScopeConstants.GmailReadonly },
                                    configuration.EmailAddress,
                                    CancellationToken.None,
                                    new FileDataStore(clientSecrets.ClientId + ".json"));

            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Simple Email",
                GZipEnabled = true
            });

            return service;
        }

        private Email ParseMessage(Message message, EmailAddress emailAddress, string folderId)
        {
            MessagePart? primaryPart = null;

            if (message.Payload.Parts == null)
            {
                primaryPart = message.Payload;
            }
            else
            {
                // Message Parts have to be interpreted. The body is usually the largest.
                primaryPart = message.Payload.Parts.MaxBy(part => part.Body.Size) ?? null;
            }

            if (primaryPart == null)
                throw new Exception("Unable to locate primary email message part:  GmailApiClient.cs");

            var summary = ParseMessagePart(message.Payload.Headers, primaryPart, emailAddress, folderId, message.ThreadId, message.Id);

            return new Email(summary, emailAddress, new DateTime(message.InternalDate ?? 0, DateTimeKind.Utc), new string[] { });
        }

        private EmailSummary ParseSummary(Message message, EmailAddress emailAddress, string folderId)
        {
            MessagePart? primaryPart = null;

            if (message.Payload.Parts == null)
            {
                primaryPart = message.Payload;
            }
            else
            {
                // Message Parts have to be interpreted. The body is usually the largest.
                primaryPart = message.Payload.Parts.MaxBy(part => part.Body.Size) ?? null;
            }

            if (primaryPart == null)
                throw new Exception("Unable to locate primary email message part:  GmailApiClient.cs");

            return ParseMessagePart(message.Payload.Headers, primaryPart, emailAddress, folderId, message.ThreadId, message.Id);
        }

        private EmailSummary ParseMessagePart(IList<MessagePartHeader> primaryHeaders, MessagePart messagePart, EmailAddress emailAddress, string folderId, string threadId, string emailId)
        {
            var toHeader = primaryHeaders.First(x => x.Name == HEADER_TO);
            var fromHeader = primaryHeaders.First(x => x.Name == HEADER_FROM);
            var subjectHeader = primaryHeaders.First(x => x.Name == HEADER_SUBJECT);
            var dateHeader = primaryHeaders.First(x => x.Name == HEADER_DATE);

            // Take the Body Html if present
            var messageBody = messagePart.Body?.Data == null ? string.Empty : Base64UrlEncoder.Decode(messagePart.Body.Data);

            var date = DateTime.MinValue;

            DateTime.TryParse(dateHeader.Value, out date);

            return new EmailSummary()
            {
                EmailAddress = emailAddress,
                Date = date,
                FolderId = folderId,
                From = fromHeader.Value,
                Id = emailId,
                Subject = subjectHeader.Value,
                ThreadId = threadId,
                Body = messageBody,
                MimeType = MimeType.Parse(messagePart.MimeType)
            };
        }

        private bool IsFolderExcluded(string labelName)
        {
            return labelName.Contains("CATEGORY_") || labelName == "UNSTARRED" || labelName == "STARRED";
        }
    }
}
