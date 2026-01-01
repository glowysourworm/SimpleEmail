


using MailKit;

using MimeKit;

using SimpleEmail.Core.Component.Interface;
using SimpleEmail.Core.Model;
using SimpleEmail.Core.Model.Configuration;

using SimpleWpf.IocFramework.Application.Attribute;

namespace SimpleEmail.Core.Component
{
    [IocExportSpecific(typeof(IEmailClient), (int)EmailHosts.Hotmail, InstancePolicy.ShareGlobal)]
    public class HotmailClient : IEmailClient
    {
        public HotmailClient()
        {
        }

        public Task<EmailAccount> GetAccountDetail(EmailAccountConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetFolders(EmailAccountConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public Task<IMimeMessage> GetMessage(EmailAccountConfiguration configuration, UniqueId uid)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetSubFolders(EmailAccountConfiguration configuration, string folder)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IMessageSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, SpecialFolder folder, IEnumerable<UniqueId> emailIds)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IMessageSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, SpecialFolder folder)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IMessageSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, string folder)
        {
            throw new NotImplementedException();
        }
    }
}
