using MailKit;

using MimeKit;

using SimpleEmail.Core.Model;
using SimpleEmail.Core.Model.Configuration;

namespace SimpleEmail.Core.Component.Interface
{
    /// <summary>
    /// Describes a component that handles communication with email client; and utilizes the MailKit API.
    /// </summary>
    public interface IEmailClient
    {
        /// <summary>
        /// Queries the account detils to fill out an EmailAccount, which will be stored in the database to
        /// use it locally; and re-verified by the core components at startup and synchronization.
        /// </summary>
        Task<EmailAccount> GetAccountDetail(EmailAccountConfiguration configuration);

        Task<IEnumerable<string>> GetFolders(EmailAccountConfiguration configuration);
        Task<IEnumerable<string>> GetSubFolders(EmailAccountConfiguration configuration, string folder);

        // THIS NEEDS TO BE THE COMPLETE MESSAGE GET. CHECK WHAT IS CONTAINED IN MIME MESSAGE!
        Task<IEnumerable<IMessageSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, MailKit.SpecialFolder folder, IEnumerable<UniqueId> emailIds);
        Task<IEnumerable<IMessageSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, MailKit.SpecialFolder folder);
        Task<IEnumerable<IMessageSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, string folder);

        /// <summary>
        /// Primary function to retrieve a message from the email server. Use this after acquiring
        /// the message's UID. Trying to get them all at once is not (currently) supported by the
        /// MailKit API; and may not be the best strategy for large email accounts.
        /// </summary>
        Task<IMimeMessage> GetMessage(EmailAccountConfiguration configuration, UniqueId uid);
    }
}
