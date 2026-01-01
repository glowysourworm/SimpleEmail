namespace SimpleEmail.Core.Model
{
    public class EmailAddress
    {
        public const string GMAIL_HOST_NAME = "gmail.com";
        public const string HOTMAIL_HOST_NAME = "hotmail.com";

        /// <summary>
        /// Everything before the @ symbol
        /// </summary>
        public string User { get; private set; }

        /// <summary>
        /// Everything after the @ symbol (user@host.com) => (host.com)
        /// </summary>
        public string HostAddress { get; private set; }

        /// <summary>
        /// Email host identifier. Each of these must have client support (IEmailClient)
        /// </summary>
        public EmailHosts EmailHost { get; private set; }

        public EmailAddress()
        {
            this.User = "newaccount";
            this.HostAddress = GetHostName(EmailHosts.Hotmail);
        }
        public EmailAddress(string userName, EmailHosts supportedHost)
        {
            this.User = userName;
            this.HostAddress = GetHostName(supportedHost);
        }

        public static string GetHostName(EmailHosts supportedHost)
        {
            switch (supportedHost)
            {
                case EmailHosts.Gmail:
                    return GMAIL_HOST_NAME;
                case EmailHosts.Hotmail:
                    return HOTMAIL_HOST_NAME;
                default:
                    throw new Exception("Host support not implemented!");
            }
        }

        public static EmailHosts GetHost(string hostName)
        {
            switch (hostName)
            {
                case GMAIL_HOST_NAME:
                    return EmailHosts.Gmail;
                case HOTMAIL_HOST_NAME:
                    return EmailHosts.Hotmail;
                default:
                    throw new Exception("Host support not implemented!");
            }
        }

        public static bool IsHost(string hostName)
        {
            switch (hostName)
            {
                case GMAIL_HOST_NAME:
                case HOTMAIL_HOST_NAME:
                    return true;
                default:
                    return false;
            }
        }

        public static EmailAddress Parse(string address)
        {
            EmailAddress? result = null;

            if (!Validate(address, out result))
                return null;

            return result;
        }

        public static bool Validate(string address, out EmailAddress? result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(address))
                return false;

            if (!address.Contains('@'))
                return false;

            var parsedAddress = address.Split('@', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parsedAddress.Length != 2)
                return false;

            if (!IsHost(parsedAddress[1]))
                return false;

            result = new EmailAddress(parsedAddress[0], GetHost(parsedAddress[1]));

            return true;
        }

        public override string ToString()
        {
            return this.User + "@" + this.HostAddress;
        }
    }
}
