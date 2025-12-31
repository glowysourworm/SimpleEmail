using SimpleEmail.Core.Component.Model;

using SimpleWpf.SimpleCollections.Collection;

namespace SimpleEmail.Core.Model
{
    public class PrimaryConfiguration
    {
        public SimpleList<EmailClientConfiguration> EmailAccounts { get; set; }

        public PrimaryConfiguration()
        {
            this.EmailAccounts = new SimpleList<EmailClientConfiguration>();
        }
    }
}
