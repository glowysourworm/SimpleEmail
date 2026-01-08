using MailKit;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.Utilities;

using GmailFolder = Google.Apis.Gmail.v1.Data.Label;
using HotmailFolder = Microsoft.Graph.Models.MailFolder;

namespace SimpleEmail.Core.Model
{
    public class EmailFolder
    {
        public string EmailAddress { get; set; }
        public string Id { get; set; }
        public string? ParentId { get; set; }
        public string Name { get; set; }
        public int MessageCount { get; set; }
        public int MessageUnreadCount { get; set; }
        public MessageFlags AcceptedFlags { get; set; }
        public List<EmailFolder> SubFolders { get; set; }

        public EmailFolder()
        {
            this.EmailAddress = string.Empty;
            this.Id = string.Empty;
            this.ParentId = null;
            this.Name = string.Empty;
            this.MessageCount = 0;
            this.MessageUnreadCount = 0;
            this.AcceptedFlags = MessageFlags.None;
            this.SubFolders = new List<EmailFolder>();
        }
        public EmailFolder(string emailAddress, IMailFolder folder)
        {
            this.EmailAddress = emailAddress;
            this.Id = folder.FullName;
            this.Name = folder.Name;
            this.ParentId = folder.ParentFolder != null ? folder.ParentFolder.FullName : null;
            this.MessageCount = folder.Count;
            this.MessageUnreadCount = folder.Unread;
            this.AcceptedFlags = MessageFlags.None;

            // Assumes GetSubFolders will work with the open client! API may have a way to preload folder tree!
            this.SubFolders = new List<EmailFolder>(folder.GetSubfolders(StatusItems.Count | StatusItems.MailboxId | StatusItems.Unread, true)
                                                          .Select(x => new EmailFolder(emailAddress, x))
                                                          .Actualize());
        }
        public EmailFolder(string emailAddress, GmailFolder folder, IEnumerable<GmailFolder> allFolders)
        {
            this.EmailAddress = emailAddress;
            this.Id = folder.Id;
            this.Name = folder.Name;
            this.ParentId = null;
            this.MessageCount = folder.MessagesTotal ?? 0;
            this.MessageUnreadCount = folder.MessagesUnread ?? 0;

            this.SubFolders = allFolders.Where(x =>
            {
                var matchCount = 0;

                // [Label]/[SubLabel]/[next level sub-label] (excluded)
                var success = StringHelpers.RegexMatchIC(folder.Name + "/([a-zA-Z0-9_]+)", x.Name, out matchCount);

                return success && matchCount == 1;
            })
            .Select(x =>
            {
                var subFolder = new EmailFolder(emailAddress, x, allFolders);

                // Remove parent folder name from the subfolder
                subFolder.Name = subFolder.Name.Replace(folder.Name + "/", "");

                return subFolder;
            })
            .ToList();
        }

        public EmailFolder(HotmailFolder folder, IEnumerable<HotmailFolder> subFolders, string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(folder.Id))
                throw new ArgumentException("EmailFolder.Id must not be null");

            if (string.IsNullOrWhiteSpace(folder.DisplayName))
                throw new ArgumentException("EmailFolder.DisplayName must not be null");

            this.EmailAddress = emailAddress;
            this.Id = folder.Id;
            this.Name = folder.DisplayName;
            this.ParentId = null;
            this.MessageCount = folder.TotalItemCount ?? 0;
            this.MessageUnreadCount = folder.UnreadItemCount ?? 0;

            this.SubFolders = subFolders.Select(subFolder => new EmailFolder(subFolder, new HotmailFolder[] { }, emailAddress)).ToList();
        }


        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is not EmailFolder)
                return false;

            var other = obj as EmailFolder;

            return other.Id.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Id={0} Name={1}", this.Id, this.Name);
        }
    }
}
