using MailKit;

using MimeKit;

using SimpleWpf.RecursiveSerializer.Component.Interface;
using SimpleWpf.RecursiveSerializer.Interface;

namespace SimpleEmail.Core.Model
{
    /// <summary>
    /// This is modified slightly from MailKit to include most of the meta-data about the email.
    /// </summary>
    public class EmailEnvelope : IRecursiveSerializable
    {
        public List<string> To { get; private set; }
        public List<string> From { get; private set; }
        public List<string> Cc { get; private set; }
        public List<string> Bcc { get; private set; }
        public List<string> ReplyTo { get; private set; }
        public string InReplyTo { get; private set; }
        public string PrimaryTo { get; private set; }
        public string PrimaryFrom { get; private set; }
        public string NormalizedSubject { get; private set; }
        public EmailDirection Direction { get; private set; }
        public DateTime Date { get; private set; }
        public DateTime? InternalDate { get; private set; }
        public List<string> Keywords { get; private set; }

        public EmailEnvelope()
        {
            this.To = new List<string>();
            this.From = new List<string>();
            this.Cc = new List<string>();
            this.Bcc = new List<string>();
            this.ReplyTo = new List<string>();
            this.InReplyTo = string.Empty;
            this.PrimaryTo = string.Empty;
            this.PrimaryFrom = string.Empty;
            this.NormalizedSubject = string.Empty;
            this.Direction = EmailDirection.Incoming;
            this.Date = DateTime.MinValue;
            this.InternalDate = DateTime.MinValue;
            this.Keywords = new List<string>();
        }

        public EmailEnvelope(Envelope envelope, DateTimeOffset? internalDate, IEnumerable<string> keywords)
        {
            this.To = FromInternetAddressList(envelope.To);
            this.From = FromInternetAddressList(envelope.From);
            this.Cc = FromInternetAddressList(envelope.Cc);
            this.Bcc = FromInternetAddressList(envelope.Bcc);
            this.ReplyTo = FromInternetAddressList(envelope.ReplyTo);

            this.InReplyTo = envelope.InReplyTo;

            this.PrimaryTo = envelope.To.FirstOrDefault()?.Name ?? string.Empty;
            this.PrimaryFrom = envelope.From.FirstOrDefault()?.Name ?? string.Empty;
            this.NormalizedSubject = envelope.Subject;
            this.Direction = EmailDirection.Incoming;
            this.Date = envelope.Date?.UtcDateTime ?? DateTime.MinValue;
            this.InternalDate = internalDate?.UtcDateTime ?? null;
            this.Keywords = new List<string>(keywords);
        }

        public EmailEnvelope(IPropertyReader reader)
        {
            this.To = reader.Read<List<string>>("To");
            this.From = reader.Read<List<string>>("From");
            this.Cc = reader.Read<List<string>>("Cc");
            this.Bcc = reader.Read<List<string>>("Bcc");
            this.ReplyTo = reader.Read<List<string>>("ReplyTo");
            this.InReplyTo = reader.Read<string>("InReplyTo");

            this.PrimaryTo = reader.Read<string>("PrimaryTo");
            this.PrimaryFrom = reader.Read<string>("PrimaryFrom");
            this.NormalizedSubject = reader.Read<string>("NormalizedSubject");
            this.Direction = reader.Read<EmailDirection>("Direction");
            this.Date = reader.Read<DateTime>("Date");
            this.InternalDate = reader.Read<DateTime>("InternalDate");
            this.Keywords = reader.Read<List<string>>("Keywords");
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("To", this.To);
            writer.Write("From", this.From);
            writer.Write("Cc", this.Cc);
            writer.Write("Bcc", this.Bcc);
            writer.Write("ReplyTo", this.ReplyTo);
            writer.Write("InReplyTo", this.InReplyTo);

            writer.Write("PrimaryTo", this.PrimaryTo);
            writer.Write("PrimaryFrom", this.PrimaryFrom);
            writer.Write("NormalizedSubject", this.NormalizedSubject);
            writer.Write("Direction", this.Direction);
            writer.Write("Date", this.Date);
            writer.Write("InternalDate", this.InternalDate);
            writer.Write("Keywords", this.Keywords);
        }

        /// <summary>
        /// Sets the data structure:  This will use the MailKit.Envelope structure to set up all
        /// the required data here.
        /// </summary>
        public void Set(MessageSummary summary, EmailDirection direction)
        {
            SetRecipientList(this.To, summary.Envelope.To);
            SetRecipientList(this.From, summary.Envelope.From);
            SetRecipientList(this.Cc, summary.Envelope.Cc);
            SetRecipientList(this.Bcc, summary.Envelope.Bcc);
            SetRecipientList(this.ReplyTo, summary.Envelope.ReplyTo);

            this.InReplyTo = summary.Envelope.InReplyTo;
            this.PrimaryFrom = summary.Envelope.From.FirstOrDefault()?.Name ?? string.Empty;
            this.PrimaryTo = summary.Envelope.To.FirstOrDefault()?.Name ?? string.Empty;
            this.NormalizedSubject = summary.NormalizedSubject;
            this.Direction = direction;
            this.Date = summary.Date.UtcDateTime;
            this.InternalDate = summary.InternalDate?.UtcDateTime;

            this.Keywords.Clear();
            this.Keywords.AddRange(summary.Keywords);
        }

        private void SetRecipientList(List<string> target, InternetAddressList source)
        {
            target.Clear();
            target.AddRange(source.Select(x => x.Name));
        }

        private List<string> FromInternetAddressList(InternetAddressList source)
        {
            var result = new List<string>();

            result.AddRange(source.Select(x => x.Name));

            return result;
        }
    }
}
