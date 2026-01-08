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

        // THIS NEEDS TO BE THE COMPLETE MESSAGE GET. CHECK WHAT IS CONTAINED IN MIME MESSAGE!
        Task<EmailSummary> GetSummaryAsync(EmailAccountConfiguration configuration, string folderId, string emailIds);
        Task<IEnumerable<EmailSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, string folderId, IEnumerable<string> emailIds);
        Task<IEnumerable<EmailSummary>> GetSummariesAsync(EmailAccountConfiguration configuration, string folderId);

        /// <summary>
        /// Primary function to retrieve a message from the email server. Use this after acquiring
        /// the message's UID. Trying to get them all at once is not (currently) supported by the
        /// MailKit API; and may not be the best strategy for large email accounts.
        /// </summary>
        Task<Email> GetMessage(EmailAccountConfiguration configuration, string folderId, string emailUid);
    }
}
