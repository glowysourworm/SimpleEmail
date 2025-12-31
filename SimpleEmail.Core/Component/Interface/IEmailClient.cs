using MailKit;

using MimeKit;

using SimpleEmail.Core.Component.Model;

namespace SimpleEmail.Core.Component.Interface
{
    /// <summary>
    /// Describes a component that handles communication with email client; and utilizes the MailKit API.
    /// </summary>
    public interface IEmailClient
    {
        void Initialize(EmailClientConfiguration configuration);

        /// <summary>
        /// Primary configuration for the email client - specifies POP, or IMAP, and all required 
        /// parameters. This must cover all possible email server situations. (see design.txt)
        /// </summary>
        EmailClientConfiguration Configuration { get; }

        Task<IEnumerable<string>> GetFolders();
        Task<IEnumerable<string>> GetSubFolders(string folder);

        Task<IEnumerable<IMessageSummary>> GetSummariesAsync(MailKit.SpecialFolder folder);
        Task<IEnumerable<IMessageSummary>> GetSummariesAsync(string folder);

        /// <summary>
        /// Primary function to retrieve a message from the email server. Use this after acquiring
        /// the message's UID. Trying to get them all at once is not (currently) supported by the
        /// MailKit API; and may not be the best strategy for large email accounts.
        /// </summary>
        Task<IMimeMessage> GetMessage(UniqueId uid);
    }
}
