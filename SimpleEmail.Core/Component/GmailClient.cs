using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;

using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;

using MimeKit;

using SimpleEmail.Core.Component.Interface;
using SimpleEmail.Core.Component.Model;

using SimpleWpf.Extensions.Collection;

namespace SimpleEmail.Core.Component
{
    public class GmailClient : IEmailClient
    {
        public EmailClientConfiguration Configuration { get; private set; }

        public GmailClient(string user, string password)
        {
            this.Configuration = new EmailClientConfiguration();
        }

        public GmailClient(EmailClientConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public async Task<IEnumerable<string>> GetFolders()
        {
            try
            {
                using (var client = await CreateIMAP(this.Configuration))
                {
                    // Get mail folder from server
                    var mailFolders = client.GetFolders(new FolderNamespace('/', ""));

                    // Dispose client (also)
                    client.Disconnect(true);

                    return mailFolders.Select(x => x.FullName)
                                      .Actualize();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw ex;
            }
        }

        public async Task<IEnumerable<string>> GetSubFolders(string folder)
        {
            try
            {
                using (var client = await CreateIMAP(this.Configuration))
                {
                    var folders = new List<string>();

                    // Get mail folder from server
                    var mailFolder = client.GetFolder(folder);

                    // Gets sub-folders of the mail folder (non-recursive)
                    foreach (var subFolder in mailFolder.GetSubfolders(false))
                    {
                        folders.Add(subFolder.FullName);
                    }

                    // Dispose client (also)
                    client.Disconnect(true);

                    return folders;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw ex;
            }
        }

        public async Task<IEnumerable<IMessageSummary>> GetSummariesAsync(SpecialFolder specialFolder)
        {
            try
            {
                using (var client = await CreateIMAP(this.Configuration))
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

        public async Task<IEnumerable<IMessageSummary>> GetSummariesAsync(string folderPath)
        {
            try
            {
                using (var client = await CreateIMAP(this.Configuration))
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

        public async Task<IMimeMessage> GetMessage(UniqueId uid)
        {
            try
            {
                using (var client = await CreateIMAP(this.Configuration))
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
        private async Task<ImapClient> CreateIMAP(EmailClientConfiguration configuration)
        {
            var client = new ImapClient();

            // Connect to email server to establish client is valid
            client.Connect(this.Configuration.ServerAddress,
                           this.Configuration.ServerPort,
                           SecureSocketOptions.SslOnConnect);

            // Check for SSL
            if (!client.IsSecure)
            {
                throw new Exception("SSL authentication failed");
            }

            // Run primary authentication for Google API (using trusted 3rd party app)
            var oauth2 = await Authenticate(this.Configuration);

            // Validate client with OAuth2 result
            client.Authenticate(oauth2);

            // Client must be disposed! (IDisposable)
            return client;
        }
        private async Task<SaslMechanism> Authenticate(EmailClientConfiguration configuration)
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
                DataStore = new FileDataStore("CacheFolderLocation", false),
                Scopes = new[] { "https://mail.google.com/" },
                ClientSecrets = clientSecrets,
                LoginHint = configuration.User  // Email address was used
            });

            // Note: For a web app, you'll want to use AuthorizationCodeWebApp instead.
            var codeReceiver = new LocalServerCodeReceiver();
            var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);

            var credential = await authCode.AuthorizeAsync(configuration.User, CancellationToken.None);

            if (credential.Token.IsStale)
                await credential.RefreshTokenAsync(CancellationToken.None);

            return new SaslMechanismOAuthBearer(credential.UserId, credential.Token.AccessToken);
        }
        #endregion
    }
}
