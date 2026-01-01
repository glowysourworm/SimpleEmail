namespace SimpleEmail.Core.Model
{
    public class EmailAccount
    {
        public EmailAddress EmailAddress { get; set; }
        public List<EmailFolder> SpecialFolders { get; set; }
        public List<EmailFolder> PersonalFolders { get; set; }

        public EmailAccount()
        {
            this.EmailAddress = new EmailAddress();
            this.SpecialFolders = new List<EmailFolder>();
            this.PersonalFolders = new List<EmailFolder>();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is not EmailAccount)
                return false;

            var other = obj as EmailAccount;

            return other.EmailAddress.ToString().Equals(this.EmailAddress.ToString());
        }

        public override int GetHashCode()
        {
            return this.EmailAddress.ToString().GetHashCode();
        }
    }
}
