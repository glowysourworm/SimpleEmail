using MailKit;

using SimpleWpf.Extensions.Collection;

namespace SimpleEmail.Core.Model
{
    public class Email
    {
        public UniqueId Uid { get; set; }               // MailKit's UniqueId
        public string EmailId { get; set; }             // This must relate to MailKit.UniqueId
        public string FolderId { get; set; }
        public string ThreadId { get; set; }
        public MessageFlags Flags { get; set; }
        public EmailEnvelope Envelope { get; set; }
        public string HtmlBody { get; set; }
        public DateTime SaveDate { get; set; }
        public List<string> ReferenceIds { get; set; }  // MailKit has a list of reference emails. I'm assuming the EmailId is used for this.

        public Email()
        {
            this.Uid = new UniqueId();
            this.EmailId = string.Empty;
            this.FolderId = string.Empty;
            this.ThreadId = string.Empty;
            this.Flags = MessageFlags.None;
            this.Envelope = new EmailEnvelope();
            this.HtmlBody = string.Empty;
            this.SaveDate = DateTime.MinValue;
            this.ReferenceIds = new List<string>();
        }

        /// <summary>
        /// Constructor to build our email object from MailKit
        /// </summary>
        public Email(IMessageSummary message)
        {
            this.Uid = message.UniqueId;
            this.EmailId = message.EmailId;
            this.FolderId = message.Folder.Id;
            this.ThreadId = message.ThreadId;
            this.Flags = message.Flags ?? MessageFlags.None;
            this.Envelope = new EmailEnvelope(message.Envelope, message.InternalDate, message.Keywords);

            // TODO: VIEW HTML!

            this.SaveDate = message.SaveDate?.UtcDateTime ?? DateTime.MinValue;         // This should be the date that the email is saved on the server side
            this.ReferenceIds = message.References.Select(x => x).ToList();
        }

        public EmailStub CreateStub()
        {
            if (this.Uid == new UniqueId())
                throw new Exception("Trying to create stub for an empty email:  Email.cs");

            return new EmailStub()
            {
                Uid = this.Uid,
                Date = this.Envelope.Date,
                FolderId = this.FolderId,
                From = this.Envelope.PrimaryFrom,
                Subject = this.Envelope.NormalizedSubject,
                ThreadId = this.ThreadId
            };
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is not Email)
                return false;

            var other = obj as Email;

            return other.Uid.Equals(this.Uid);
        }

        public override int GetHashCode()
        {
            return this.Uid.GetHashCode();
        }
    }
}
