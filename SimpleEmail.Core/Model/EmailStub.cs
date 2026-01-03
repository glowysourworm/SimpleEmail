using MailKit;

namespace SimpleEmail.Core.Model
{
    public class EmailStub
    {
        public EmailAddress EmailAddress { get; set; }
        public UniqueId Uid { get; set; }
        public string FolderId { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public DateTime Date { get; set; }
        public string ThreadId { get; set; }

        public EmailStub()
        {
            this.EmailAddress = new EmailAddress();
            this.Uid = new UniqueId();
            this.FolderId = string.Empty;
            this.Subject = string.Empty;
            this.From = string.Empty;
            this.Date = DateTime.MinValue;
            this.ThreadId = string.Empty;
        }

        public EmailStub(IMessageSummary summary, string folderId, EmailAddress emailAddress)
        {
            this.EmailAddress = emailAddress;
            this.Uid = summary.UniqueId;
            this.FolderId = folderId;
            this.Subject = summary.NormalizedSubject;
            this.From = summary.Envelope.From.FirstOrDefault()?.Name ?? string.Empty;
            this.Date = summary.Date.UtcDateTime;
            this.ThreadId = summary.ThreadId;
        }
    }
}
