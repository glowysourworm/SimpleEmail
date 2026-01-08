using SimpleWpf.Extensions.Collection;

namespace SimpleEmail.Core.Model
{
    /// <summary>
    /// Primary location for caching the downloaded details of a single email account
    /// </summary>
    public class EmailAccountCache
    {
        public EmailAccount Account { get; private set; }

        /// <summary>
        /// Set to true to create stubs when adding email to the cache. Set to false if the
        /// stub must be first present before the email is added to protect the integrity of
        /// the cache - forcing the downloaded stubs to match the downloaded email.
        /// </summary>
        public bool AutoCreateStubs { get; private set; }

        // Primary folder cache
        Dictionary<string, EmailFolder> _folders;

        // [MEMORY] Primary email cache
        Dictionary<string, Email> _email;

        // [Less Memory] Primary email stub cache
        Dictionary<string, EmailSummary> _emailStubs;

        // Primary email cache by folder (shared Email references)
        Dictionary<string, Dictionary<string, Email>> _emailByFolder;

        // Primary email stub cache by folder (shared EmailStub references)
        Dictionary<string, Dictionary<string, EmailSummary>> _emailStubsByFolder;

        public EmailAccountCache()
            : this(new EmailAccount(), new List<EmailSummary>(), true)
        { }

        /// <summary>
        /// Creates a cache object with provided EmailAccount (which also fills out folder detail)
        /// </summary>
        public EmailAccountCache(EmailAccount account)
            : this(account, new List<EmailSummary>(), true)
        { }

        /// <summary>
        /// Creates an account cache with folders, and the provided email stubs
        /// </summary>
        public EmailAccountCache(EmailAccount account, IEnumerable<EmailSummary> emailStubs, bool autoCreateStubs)
        {
            this.Account = account;
            this.AutoCreateStubs = autoCreateStubs;

            _folders = new Dictionary<string, EmailFolder>();
            _email = new Dictionary<string, Email>();
            _emailStubs = new Dictionary<string, EmailSummary>();
            _emailByFolder = new Dictionary<string, Dictionary<string, Email>>();
            _emailStubsByFolder = new Dictionary<string, Dictionary<string, EmailSummary>>();

            foreach (var folder in account.SpecialFolders)
                AddFolder(folder);

            foreach (var folder in account.PersonalFolders)
                AddFolder(folder);

            foreach (var stub in emailStubs)
                AddEmailStub(stub);
        }

        /// <summary>
        /// Creates an account cache with folders, and the provided email to "fill the folders", which also creates
        /// stubs, and sets AutoCreateStubs = true.
        /// </summary>
        public EmailAccountCache(EmailAccount account, IEnumerable<Email> email)
        {
            this.Account = account;
            this.AutoCreateStubs = true;

            _folders = new Dictionary<string, EmailFolder>();
            _email = new Dictionary<string, Email>();
            _emailStubs = new Dictionary<string, EmailSummary>();
            _emailByFolder = new Dictionary<string, Dictionary<string, Email>>();
            _emailStubsByFolder = new Dictionary<string, Dictionary<string, EmailSummary>>();

            foreach (var folder in account.SpecialFolders)
                AddFolder(folder);

            foreach (var folder in account.PersonalFolders)
                AddFolder(folder);

            foreach (var mail in email)
                AddEmail(mail);
        }

        public EmailFolder GetFolder(string folderId)
        {
            return _folders[folderId];
        }
        public Email GetEmail(string emailId)
        {
            return _email[emailId];
        }
        public Email GetEmail(string folderId, string emailId)
        {
            return _emailByFolder[folderId][emailId];
        }
        public EmailSummary GetEmailStub(string emailId)
        {
            return _emailStubs[emailId];
        }
        public EmailSummary GetEmailStub(string folderId, string emailId)
        {
            return _emailStubsByFolder[folderId][emailId];
        }
        public IEnumerable<EmailSummary> GetEmailStubs(string folderId)
        {
            return _emailStubs.Where(x => x.Value.FolderId == folderId)
                              .Select(x => x.Value)
                              .Actualize();
        }

        public int GetEmailCount(string folderId)
        {
            return _emailByFolder[folderId].Count;
        }
        public int GetEmailStubCount(string folderId)
        {
            return _emailStubsByFolder[folderId].Count;
        }

        /// <summary>
        /// Adds folder, and all subfolders, to the cache. Any folder may later be referenced by 
        /// the EmailFolder.Id property.
        /// </summary>
        public void AddFolder(EmailFolder emailFolder)
        {
            if (_folders.ContainsKey(emailFolder.Id))
                throw new Exception("Email folder already present in EmailAccountCache");

            var folders = FlattenSubFolders(emailFolder);

            // TODO: NOTIFY WHERE WE ALREADY HAVE FOLDER ADDED
            foreach (var folder in folders)
            {
                _folders.Add(folder.Id, folder);
                _emailByFolder.Add(folder.Id, new Dictionary<string, Email>());
                _emailStubsByFolder.Add(folder.Id, new Dictionary<string, EmailSummary>());
            }
        }
        public void AddEmailStub(EmailSummary stub)
        {
            if (_emailStubs.ContainsKey(stub.Id))
                throw new Exception("Email stub already contained in the EmailAccountCache");

            if (_email.ContainsKey(stub.Id) && !this.AutoCreateStubs)
                throw new Exception("AutoCreateStubs is preventing the use of the EmailAccountCache because the email stub was not present before adding the actual email");

            _emailStubs.Add(stub.Id, stub);
            _emailStubsByFolder[stub.FolderId].Add(stub.Id, stub);
        }
        public void AddEmail(Email email)
        {
            if (_email.ContainsKey(email.Id))
                throw new Exception("Email already present in EmailAccountCache");

            if (!_folders.ContainsKey(email.FolderId))
                throw new Exception("Email folder not present in EmailAccountCache. Must add folder before adding email to the folder.");

            // Check email stubs
            if (!_emailStubs.ContainsKey(email.Id))
            {
                if (!this.AutoCreateStubs)
                    throw new Exception("AutoCreateStubs is preventing the use of the EmailAccountCache because the email stub was not present before adding the actual email");

                AddEmailStub(email.CreateStub());
            }

            _email.Add(email.Id, email);
            _emailByFolder[email.FolderId].Add(email.Id, email);
        }

        public bool ContainsFolder(string folderId)
        {
            return _folders.ContainsKey(folderId);
        }
        public bool ContainsEmail(string emailId)
        {
            return _email.ContainsKey(emailId);
        }
        public bool ContainsEmailStub(string emailId)
        {
            return _emailStubs.ContainsKey(emailId);
        }

        public void RemoveFolder(string folderId, bool removeIfNotEmpty = false)
        {
            if (!_folders.ContainsKey(folderId))
                throw new Exception("Email folder not present in EmailAccountCache");

            // "Non-Empty" means "Non-Empty by email stubs"
            //
            if (_emailStubsByFolder[folderId].Count != 0 && !removeIfNotEmpty)
                throw new Exception("Must either empty folder before removing folder, or set removeIfNotEmpty to true:  EmailAccountCache");

            // Check that email is removed since the user may have forced email removal
            foreach (var email in _emailByFolder[folderId])
                RemoveEmail(folderId, email.Key);

            _folders.Remove(folderId);
            _emailByFolder.Remove(folderId);
            _emailStubsByFolder.Remove(folderId);
        }
        public void RemoveEmailStub(string folderId, string emailStubId)
        {
            if (!_folders.ContainsKey(folderId))
                throw new Exception("Email folder not present in EmailAccountCache");

            if (!_emailStubsByFolder.ContainsKey(folderId))
                throw new Exception("Email stubs folder not present in EmailAccountCache");

            if (!_emailStubs.ContainsKey(emailStubId))
                throw new Exception("Email stub not present in EmailAccountCache. There was an email without a stub in the cache!");

            if (!_emailByFolder.ContainsKey(folderId))
                throw new Exception("Email folder not present in EmailAccountCache");

            // EMAIL STILL PRESENT! REMOVE BEFORE STUB!
            if (_email.ContainsKey(emailStubId))
                throw new Exception("Email still present in EmailAccountCache. Cannot yet remove the email stub!");

            _emailStubs.Remove(emailStubId);
            _emailStubsByFolder[folderId].Remove(emailStubId);
        }

        /// <summary>
        /// Removes email and email stub from the cache
        /// </summary>
        public void RemoveEmail(string folderId, string emailId)
        {
            if (!_folders.ContainsKey(folderId))
                throw new Exception("Email folder not present in EmailAccountCache");

            if (!_emailStubsByFolder.ContainsKey(folderId))
                throw new Exception("Email stubs folder not present in EmailAccountCache");

            if (!_emailByFolder.ContainsKey(folderId))
                throw new Exception("Email (stubs) folder not present in EmailAccountCache");

            if (!_email.ContainsKey(emailId))
                throw new Exception("Email not present in EmailAccountCache");

            if (!_emailStubs.ContainsKey(emailId))
                throw new Exception("Email stub not present in EmailAccountCache. There was an email without a stub in the cache!");

            _email.Remove(emailId);
            _emailByFolder[folderId].Remove(emailId);
            _emailStubs.Remove(emailId);
            _emailStubsByFolder[folderId].Remove(emailId);
        }

        private IEnumerable<EmailFolder> FlattenSubFolders(EmailFolder folder)
        {
            var result = new List<EmailFolder>();

            FlattenSubFoldersRecurse(folder, result);

            return result;
        }

        private void FlattenSubFoldersRecurse(EmailFolder folder, List<EmailFolder> result)
        {
            // Add
            result.Add(folder);

            foreach (var subFolder in folder.SubFolders)
            {
                // -> Add
                FlattenSubFoldersRecurse(subFolder, result);
            }
        }

    }
}
