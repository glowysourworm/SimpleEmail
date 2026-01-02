using MailKit;

using SimpleWpf.Extensions.Collection;

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
    }
}
