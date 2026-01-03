using MailKit;

using SimpleEmail.Core.Component.Interface;
using SimpleEmail.Core.Model;
using SimpleEmail.Core.Model.Configuration;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.IocFramework.Application.Attribute;

namespace SimpleEmail.Core.Component
{
    [IocExport(typeof(IEmailModelService))]
    public class EmailModelService : IEmailModelService
    {
        private readonly IEmailClient _gmailClient;
        private readonly IEmailClient _hotmailClient;

        // (Secondary) Account Configuration Store. If a user adds an account, the Initialize method must be re-called.
        //
        Dictionary<string, EmailAccountConfiguration> _emailConfigurations;

        // PRIMARY DATA SOURCE! (KEY IS EMAIL ADDRESS)
        Dictionary<string, EmailAccountCache> _emailAccounts;

        [IocImportingConstructor]
        public EmailModelService([IocImport(typeof(IEmailClient), (int)EmailHosts.Gmail)] IEmailClient gmailClient,
                                 [IocImport(typeof(IEmailClient), (int)EmailHosts.Hotmail)] IEmailClient hotmailClient)
        {
            _emailAccounts = new Dictionary<string, EmailAccountCache>();
            _emailConfigurations = new Dictionary<string, EmailAccountConfiguration>();

            _gmailClient = gmailClient;
            _hotmailClient = hotmailClient;
        }

        public async Task Initialize(IEnumerable<EmailAccountConfiguration> accountConfigurations)
        {
            // Empty current cache. (This should not contain anything)
            _emailAccounts.Clear();
            _emailConfigurations.Clear();

            // Create cache for each email account
            foreach (var configuration in accountConfigurations)
            {
                _emailConfigurations.Add(configuration.EmailAddress, configuration);

                // Get email account details
                var emailAccount = await GetClient(EmailAddress.GetHostFromAddress(configuration.EmailAddress)).GetAccountDetail(configuration);

                // Create new cache object (which adds folder detail)
                var cache = new EmailAccountCache(emailAccount);

                // EmailAddress is the unique key, which is a bit annoying since we're object wrapping it. The
                // ViewModel domain will send us a string, which must be interpreted as the unique ID...
                _emailAccounts.Add(cache.Account.EmailAddress.ToString(), cache);
            }
        }

        public EmailAccount GetAccount(string emailAddress)
        {
            if (!_emailAccounts.ContainsKey(emailAddress))
                throw new Exception("Email address not found in local store");

            return _emailAccounts[emailAddress].Account;
        }

        public IEnumerable<EmailAccount> GetAccountList()
        {
            return _emailAccounts.Values.Select(x => x.Account).Actualize();
        }

        public async Task<Email> GetEmail(string emailAddress, string folderId, UniqueId emailId)
        {
            if (!_emailAccounts.ContainsKey(emailAddress))
                throw new Exception("Email address not found in local store");

            var cache = _emailAccounts[emailAddress];

            // Download
            if (!cache.ContainsEmail(emailId))
            {
                var configuration = _emailConfigurations[emailAddress];

                var message = await GetClient(cache.Account.EmailAddress.EmailHost).
                                    GetMessage(configuration, folderId, emailId);

                var stub = await GetClient(cache.Account.EmailAddress.EmailHost).
                                 GetSummaryAsync(configuration, folderId, emailId);

                if (message == null)
                    throw new Exception("Email request failed for Email " + emailId.ToString());

                var result = new Email(message, stub, emailAddress);

                // Add to cache
                cache.AddEmail(result);

                return result;
            }
            else
                return cache.GetEmail(emailId);
        }

        public async Task<IEnumerable<EmailStub>> GetEmailStubs(string emailAddress, string emailFolderId)
        {
            if (!_emailAccounts.ContainsKey(emailAddress))
                throw new Exception("Email address not found in local store");

            var cache = _emailAccounts[emailAddress];

            if (!cache.ContainsFolder(emailFolderId))
                throw new Exception("Email address not found in local store");

            // Download
            //
            // Check to see if all stubs have been downloaded. TODO: Find out how email checksums are
            // done to validate that number (in case there have been emails deleted and added)
            //
            if (cache.GetFolder(emailFolderId).MessageCount != cache.GetEmailStubCount(emailFolderId))
            {
                var configuration = _emailConfigurations[emailAddress];
                var summaries = await GetClient(cache.Account.EmailAddress.EmailHost).
                                      GetSummariesAsync(configuration, emailFolderId);

                if (summaries == null || !summaries.Any())
                    throw new Exception("Email request failed for " + emailAddress);

                var emailStubs = summaries.Select(x => new EmailStub(x, emailFolderId, cache.Account.EmailAddress)).Actualize();

                // Add to cache
                foreach (var stub in emailStubs)
                {
                    // DOUBLE CHECK (SERVICE NEEDS TO BE PROOFED)
                    if (!cache.ContainsEmailStub(stub.Uid))
                        cache.AddEmailStub(stub);
                }

                return emailStubs;
            }
            else
                return cache.GetEmailStubs(emailFolderId);
        }

        private IEmailClient GetClient(EmailHosts emailHost)
        {
            switch (emailHost)
            {
                case EmailHosts.Hotmail:
                    return _hotmailClient;
                case EmailHosts.Gmail:
                    return _gmailClient;
                default:
                    throw new Exception("Unhandled email host type: EmailModelService.cs");
            }
        }
    }
}
