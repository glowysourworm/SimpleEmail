using MailKit;

using MimeKit;

namespace SimpleEmail.Core.Model
{
    public class Email
    {
        public UniqueId Uid { get; set; }               // MailKit's UniqueId
        public EmailAddress EmailAddress { get; set; }
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
        public Email(IMimeMessage message, IMessageSummary summary, string emailAddress)
        {
            this.Uid = summary.UniqueId;
            this.EmailAddress = EmailAddress.Parse(emailAddress);
            this.FolderId = summary.Folder.FullName;
            this.ThreadId = summary.ThreadId;
            this.Flags = MessageFlags.None;
            this.Envelope = new EmailEnvelope(summary.Envelope, summary.InternalDate, summary.Keywords);

            // TODO: VIEW HTML!
            //this.HtmlBody = message.Body?.ToString() ?? string.Empty;
            this.HtmlBody = message.HtmlBody;

            this.SaveDate = summary.SaveDate?.UtcDateTime ?? DateTime.MinValue;         // This should be the date that the email is saved on the server side
            //this.ReferenceIds = message.References.Select(x => x).ToList();
            this.ReferenceIds = new List<string>();
        }

        public EmailStub CreateStub()
        {
            if (this.Uid == new UniqueId())
                throw new Exception("Trying to create stub for an empty email:  Email.cs");

            return new EmailStub()
            {
                EmailAddress = this.EmailAddress,
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
