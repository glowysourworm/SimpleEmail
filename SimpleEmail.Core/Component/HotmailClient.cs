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

        public Task<Email> GetMessage(EmailAccountConfiguration configuration, string folderId, string emailUid)
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
    }
}
