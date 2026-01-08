namespace SimpleEmail.Core.Model
{
    public enum MimeTypes
    {
        Text,
        Html,
        Json,
        Binary
    }

    public class MimeType
    {
        public const string Text = "text/plain";
        public const string Html = "text/html";
        public const string Json = "application/json";
        public const string Binary = "application/binary";

        public string Value { get; set; }
        public MimeTypes Type { get; set; }

        /// <summary>
        /// Initializes the mime type to text/plain
        /// </summary>
        public MimeType()
        {
            this.Value = MimeType.Text;
            this.Type = MimeTypes.Text;
        }

        public MimeType(string mimeValue, MimeTypes mimeType)
        {
            this.Value = mimeValue;
            this.Type = mimeType;
        }

        public static MimeType Parse(string mimeTypeString)
        {
            switch (mimeTypeString)
            {
                case MimeType.Text:
                    return new MimeType(mimeTypeString, MimeTypes.Text);

                case MimeType.Html:
                    return new MimeType(mimeTypeString, MimeTypes.Html);

                case MimeType.Json:
                    return new MimeType(mimeTypeString, MimeTypes.Json);

                case MimeType.Binary:
                    return new MimeType(mimeTypeString, MimeTypes.Binary);

                default:
                    throw new Exception("Unhandled MimeType String:  " + mimeTypeString);
            }
        }
    }
}
