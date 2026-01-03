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
        public Task<EmailAccount> GetAccountDetail(EmailAccountConfiguration configuration)
        {
            throw new NotImplementedException();
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
    }
}
