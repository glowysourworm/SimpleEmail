using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;

using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;

using MimeKit;

using SimpleEmail.Core.Component.Interface;
using SimpleEmail.Core.Model;
using SimpleEmail.Core.Model.Configuration;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.IocFramework.Application.Attribute;

namespace SimpleEmail.Core.Component
{
    [IocExportSpecific(typeof(IEmailClient), (int)EmailHosts.Gmail, InstancePolicy.ShareGlobal)]
    public class GmailClient : IEmailClient
    {
        /// <summary>
        /// Folder data must be specified to be retrieved using the client
        /// </summary>
        private readonly StatusItems _folderStatusItems = StatusItems.AppendLimit |
                                                          StatusItems.Count |
                                                          StatusItems.Unread;

        // This is effectively the root folder
        private const string GmailRoot = "[Gmail]";

        public GmailClient()
        {
        }

        public async Task<EmailAccount> GetAccountDetail(EmailAccountConfiguration configuration)
        {
            try
            {
                using (var client = await CreateIMAP(configuration))
                {
                    var personalFolders = new List<EmailFolder>();

                    // Add folders from all personal namespaces
                    foreach (var emailNamespace in client.PersonalNamespaces)
                    {
                        // Get mail folder from server (SUBFOLDERS ARE INCLUDED HERE)
                        var mailFolders = client.GetFolders(emailNamespace, _folderStatusItems, true);
                        var mailFoldersFlattened = new List<IMailFolder>();

                        // FLATTEN [Gmail] HIERARCHY
                        foreach (var folder in mailFolders)
                        {
                            if (folder.FullName == GmailRoot)
                                continue;

                            // These are Gmail's Default folders; and have to be identified
                            else if (folder.FullName.Contains(GmailRoot))
                            {
                                mailFoldersFlattened.Add(folder);
                            }

                            // Other root folders
                            else if (folder.ParentFolder.FullName == string.Empty)
                                mailFoldersFlattened.Add(folder);
                        }

                        // Recursively create EmailFolder(s). The API will recall the client during recursion.
                        //
                        var folders = mailFoldersFlattened.Select(folder =>
                        {
                            // Constructor becomes recursive to load all the subfolders
                            //
                            return new EmailFolder(folder);

                        }).Actualize();

                        // Add unique folders to result
                        foreach (var folder in folders)
                        {
                            if (!personalFolders.Any(x => x.Id == folder.Id))
                                personalFolders.Add(folder);
                        }
                    }

                    // Dispose client (also)
                    client.Disconnect(true);

                    return new EmailAccount()
                    {
                        EmailAddress = EmailAddress.Parse(configuration.EmailAddress),
                        PersonalFolders = new List<EmailFolder>(personalFolders)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw ex;
            }
        }

        public async Task<IEnumerable<IMessageSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, MailKit.SpecialFolder specialFolder, IEnumerable<UniqueId> emailIds)
        {
            try
            {
                using (var client = await CreateIMAP(configuration))
                {
                    // Open folder using IMAP client
                    var folder = client.GetFolder(specialFolder);

                    // Read Only
                    folder.Open(FolderAccess.ReadOnly);

                    // Retrieve message summaries
                    var messages = folder.Fetch(new List<UniqueId>(emailIds), MessageSummaryItems.All);

                    // Dispose client (also)
                    client.Disconnect(true);

                    return messages;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw ex;
            }
        }

        public async Task<IEnumerable<IMessageSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, MailKit.SpecialFolder specialFolder)
        {
            try
            {
                using (var client = await CreateIMAP(configuration))
                {
                    // Open folder using IMAP client
                    var folder = client.GetFolder(specialFolder);

                    // Read Only
                    folder.Open(FolderAccess.ReadOnly);

                    // Retrieve message summaries
                    var messages = folder.Fetch(0, folder.Count - 1, MessageSummaryItems.All);

                    // Dispose client (also)
                    client.Disconnect(true);

                    return messages;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw ex;
            }
        }
        public async Task<IEnumerable<IMessageSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, string folderPath)
        {
            try
            {
                using (var client = await CreateIMAP(configuration))
                {
                    // Open folder using IMAP client
                    var folder = client.GetFolder(folderPath);

                    // Read Only
                    folder.Open(FolderAccess.ReadOnly);

                    // Retrieve message summaries
                    var messages = folder.Fetch(0, folder.Count - 1, MessageSummaryItems.All);

                    // Dispose client (also)
                    client.Disconnect(true);

                    return messages;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw ex;
            }
        }

        public async Task<IMimeMessage> GetMessage(EmailAccountConfiguration configuration, UniqueId uid)
        {
            try
            {
                using (var client = await CreateIMAP(configuration))
                {
                    // Retrieve the essage directly using the UID
                    var folder = client.Inbox.Open(FolderAccess.ReadOnly);

                    // Retrieve message 
                    var message = await client.Inbox.GetMessageAsync(uid);

                    // Dispose client (also)
                    client.Disconnect(true);

                    return message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw ex;
            }
        }

        #region (private) Authentication Methods
        private async Task<ImapClient> CreateIMAP(EmailAccountConfiguration configuration)
        {
            var client = new ImapClient();

            // Connect to email server to establish client is valid
            client.Connect(configuration.ServerAddress,
                           configuration.ServerPort,
                           SecureSocketOptions.SslOnConnect);

            // Check for SSL
            if (!client.IsSecure)
            {
                throw new Exception("SSL authentication failed");
            }

            // Run primary authentication for Google API (using trusted 3rd party app)
            var oauth2 = await Authenticate(configuration);

            // Validate client with OAuth2 result
            client.Authenticate(oauth2);

            // Client must be disposed! (IDisposable)
            return client;
        }
        private async Task<SaslMechanism> Authenticate(EmailAccountConfiguration configuration)
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

            // Note: For a web app, you'll want to use AuthorizationCodeWebApp instead.
            var codeReceiver = new LocalServerCodeReceiver();
            var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);

            var credential = await authCode.AuthorizeAsync(configuration.EmailAddress, CancellationToken.None);

            if (credential.Token.IsStale)
                await credential.RefreshTokenAsync(CancellationToken.None);

            return new SaslMechanismOAuthBearer(credential.UserId, credential.Token.AccessToken);
        }
        #endregion
    }
}
