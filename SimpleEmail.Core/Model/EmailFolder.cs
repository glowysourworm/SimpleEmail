using MailKit;

namespace SimpleEmail.Core.Model
{
    public class EmailFolder
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int MessageCount { get; set; }
        public int MessageUnreadCount { get; set; }
        public MessageFlags AcceptedFlags { get; set; }
        public EmailFolder? ParentFolder { get; set; }

        public EmailFolder()
        {
            this.Id = string.Empty;
            this.Name = string.Empty;
            this.ParentFolder = null;
            this.MessageCount = 0;
            this.MessageUnreadCount = 0;
            this.AcceptedFlags = MessageFlags.None;
        }
        public EmailFolder(IMailFolder folder)
        {
            this.Id = folder.FullName;              // Id was null from the client
            this.Name = folder.Name;
            this.ParentFolder = folder.ParentFolder != null ? new EmailFolder(folder.ParentFolder) : null;
            this.MessageCount = folder.Count;
            this.MessageUnreadCount = folder.Unread;
            this.AcceptedFlags = MessageFlags.None;
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
