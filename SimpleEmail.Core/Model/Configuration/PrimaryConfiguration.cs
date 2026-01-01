using SimpleWpf.RecursiveSerializer.Component.Interface;
using SimpleWpf.RecursiveSerializer.Interface;

namespace SimpleEmail.Core.Model.Configuration
{
    public class PrimaryConfiguration : IRecursiveSerializable
    {
        public List<EmailAccountConfiguration> EmailAccounts { get; set; }

        public PrimaryConfiguration()
        {
            this.EmailAccounts = new List<EmailAccountConfiguration>();
        }

        public PrimaryConfiguration(IPropertyReader reader)
        {
            this.EmailAccounts = reader.Read<List<EmailAccountConfiguration>>("EmailAccounts");
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("EmailAccounts", this.EmailAccounts);
        }
    }
}
