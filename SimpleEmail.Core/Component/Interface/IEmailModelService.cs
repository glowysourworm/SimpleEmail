using MailKit;

using SimpleEmail.Core.Model;
using SimpleEmail.Core.Model.Configuration;

namespace SimpleEmail.Core.Component.Interface
{
    /// <summary>
    /// Component to manage the primary email in-memory cache store
    /// </summary>
    public interface IEmailModelService
    {
        /// <summary>
        /// Initializes the email service with the stored account configurations.
        /// </summary>
        Task Initialize(IEnumerable<EmailAccountConfiguration> accountConfigurations);

        /// <summary>
        /// Returns the email addresses for the current user. These will be the "keys" for each of the
        /// EmailAccount objects held, in memory, by the cache.
        /// </summary>
        IEnumerable<EmailAccount> GetAccountList();

        /// <summary>
        /// Returns an EmailAccount object for the email address.
        /// </summary>
        EmailAccount GetAccount(string emailAddress);

        /// <summary>
        /// Returns email stubs for the email address, for the provided folder.
        /// </summary>
        Task<IEnumerable<EmailStub>> GetEmailStubs(string emailAddress, string emailFolderId);

        /// <summary>
        /// Returns Email object for the provided email unique id.
        /// </summary>
        Task<Email> GetEmail(string emailAddress, UniqueId emailId);
    }
}
