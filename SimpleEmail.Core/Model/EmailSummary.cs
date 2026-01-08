using MailKit;

using MimeKit;

namespace SimpleEmail.Core.Model
{
    public class EmailSummary
    {
        public EmailAddress EmailAddress { get; set; }
        public string Id { get; set; }
        public string FolderId { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        public string ThreadId { get; set; }
        public MimeType MimeType { get; set; }

        /// <summary>
        /// This is the (optional) body of the message. Some API's do not allow getting a smaller
        /// summary back from the email service
        /// </summary>
        public string? Body { get; set; }

        public EmailSummary()
        {
            this.EmailAddress = new EmailAddress();
            this.Id = string.Empty;
            this.FolderId = string.Empty;
            this.Subject = string.Empty;
            this.From = string.Empty;
            this.To = string.Empty;
            this.Date = DateTime.MinValue;
            this.ThreadId = string.Empty;
            this.MimeType = new MimeType();
            this.Body = null;
        }

        public EmailSummary(IMessageSummary summary, string folderId, EmailAddress emailAddress)
        {
            this.EmailAddress = emailAddress;
            this.Id = summary.UniqueId.ToString();
            this.FolderId = folderId;
            this.Subject = summary.NormalizedSubject;
            this.From = summary.Envelope.From.FirstOrDefault()?.Name ?? string.Empty;
            this.To = summary.Envelope.To.FirstOrDefault()?.Name ?? string.Empty;
            this.Date = summary.Date.UtcDateTime;
            this.ThreadId = summary.ThreadId;
            this.Body = summary.HtmlBody?.ToString() ?? null;
            this.MimeType = MimeType.Parse(summary.Body.ContentType.Name);
        }

        public EmailSummary(IMimeMessage message, string threadId, string folderId, EmailAddress emailAddress)
        {
            this.EmailAddress = emailAddress;
            this.Id = message.MessageId;
            this.FolderId = folderId;
            this.Subject = message.Subject;
            this.From = message.From.ToString();
            this.To = message.To.ToString();
            this.Date = message.Date.UtcDateTime;
            this.ThreadId = threadId;
            this.Body = message.HtmlBody?.ToString() ?? null;
            this.MimeType = MimeType.Parse(message.Body.ContentType.Name);
        }

        public EmailSummary(EmailSummary summary)
        {
            this.EmailAddress = summary.EmailAddress;
            this.Id = summary.Id;
            this.FolderId = summary.FolderId;
            this.Subject = summary.Subject;
            this.From = summary.From;
            this.To = summary.To;
            this.Date = summary.Date;
            this.ThreadId = summary.ThreadId;
            this.Body = summary.Body;
            this.MimeType = summary.MimeType;
        }
    }
}
