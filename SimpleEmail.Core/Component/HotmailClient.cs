using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;

using SimpleEmail.Core.Component.Interface;
using SimpleEmail.Core.Model;
using SimpleEmail.Core.Model.Configuration;

using SimpleWpf.IocFramework.Application.Attribute;

namespace SimpleEmail.Core.Component
{
    [IocExportSpecific(typeof(IEmailClient), (int)EmailHosts.Hotmail, InstancePolicy.ShareGlobal)]
    public class HotmailClient : IEmailClient
    {
        public async Task<EmailAccount> GetAccountDetail(EmailAccountConfiguration configuration)
        {
            try
            {
                using (var service = await Authenticate(configuration))
                {
                    var users = await service.Users.GetAsync();

                    return new EmailAccount()
                    {
                        EmailAddress = EmailAddress.Parse(configuration.EmailAddress),
                        PersonalFolders = users.Value.First().MailFolders.Select(folder =>
                        {
                            return new EmailFolder(folder, folder.ChildFolders, configuration.EmailAddress);

                        }).ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Gmail Service Error:  GetAccountDetail", ex);
            }
        }

        public Task<Email> GetMessage(EmailAccountConfiguration configuration, string folderId, string emailId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EmailSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, string folderId, IEnumerable<string> emailIds)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EmailSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, string folderId)
        {
            throw new NotImplementedException();
        }

        public Task<EmailSummary> GetSummaryAsync(EmailAccountConfiguration configuration, string folderId, string emailId)
        {
            throw new NotImplementedException();
        }

        public async Task<GraphServiceClient> Authenticate(EmailAccountConfiguration configuration)
        {
            // Set our scopes to the default
            //var scopes = new[] { "https://graph.microsoft.com/.default" };

            // Create TokenCredential
            var provider = new BasicAuthenticationProvider(configuration.EmailAddress, configuration.Password);
            //var credential = new ClientSecretCredential();            

            // Create Graph client
            var graphClient = new GraphServiceClient(provider);


            return graphClient;
        }
    }
}
