using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using MailKit;

using MimeKit;

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
                        EmailAddress = new EmailAddress(configuration.EmailAddress, EmailHosts.GmailApi),
                        PersonalFolders = topLevelFolders
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Gmail Service Error:  GetAccountDetail", ex);
            }
        }

        public Task<IMimeMessage> GetMessage(EmailAccountConfiguration configuration, string folderId, UniqueId emailUid)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IMessageSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, string folderId, IEnumerable<UniqueId> emailIds)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IMessageSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, string folderId)
        {
            throw new NotImplementedException();
        }

        public Task<IMessageSummary> GetSummaryAsync(EmailAccountConfiguration configuration, string folderId, UniqueId emailIds)
        {
            throw new NotImplementedException();
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
            });

            return service;
        }
    }
}
