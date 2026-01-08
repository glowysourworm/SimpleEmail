using MailKit;

using MimeKit;

namespace SimpleEmail.Core.Model
{
    public class Email
    {
        public string Id { get; set; }               // MailKit's UniqueId
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
            this.Id = string.Empty;
            this.EmailAddress = new EmailAddress();
            this.FolderId = string.Empty;
            this.ThreadId = string.Empty;
            this.Flags = MessageFlags.None;
            this.Envelope = new EmailEnvelope();
            this.HtmlBody = string.Empty;
            this.SaveDate = DateTime.MinValue;
            this.ReferenceIds = new List<string>();
        }

        public Email(EmailSummary summary, EmailAddress emailAddress, DateTime internalDate, IEnumerable<string> keywords)
        {
            this.Id = summary.Id;
            this.EmailAddress = emailAddress;
            this.FolderId = summary.FolderId;
            this.ThreadId = summary.ThreadId;
            this.Flags = MessageFlags.None;
            this.Envelope = new EmailEnvelope(summary, internalDate, keywords);
            this.HtmlBody = summary.Body ?? string.Empty;
            this.SaveDate = DateTime.MinValue;
            this.ReferenceIds = new List<string>();
        }

        /// <summary>
        /// Constructor to build our email object from MailKit
        /// </summary>
        public Email(IMimeMessage message, IMessageSummary summary, string emailAddress)
        {
            this.Id = summary.UniqueId.ToString();
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

        public Email(IMimeMessage message, string folderId, string threadId, UniqueId emailUid, string emailAddress)
        {
            this.Id = emailUid.ToString();
            this.EmailAddress = EmailAddress.Parse(emailAddress);
            this.FolderId = folderId;
            this.ThreadId = threadId;
            this.Flags = MessageFlags.None;
            this.Envelope = new EmailEnvelope(message);

            this.HtmlBody = message.HtmlBody;
            this.SaveDate = DateTime.MinValue;
            this.ReferenceIds = message.References.ToList();
        }

        public EmailSummary CreateStub()
        {
            if (string.IsNullOrWhiteSpace(this.Id))
                throw new Exception("Trying to create stub for an empty email:  Email.cs");

            return new EmailSummary()
            {
                EmailAddress = this.EmailAddress,
                Id = this.Id,
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

            return other.Id.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
