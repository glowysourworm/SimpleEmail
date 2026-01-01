using SimpleEmail.Core.Component.Model;

using SimpleWpf.RecursiveSerializer.Component.Interface;
using SimpleWpf.RecursiveSerializer.Interface;

namespace SimpleEmail.Core.Model
{
    public class PrimaryConfiguration : IRecursiveSerializable
    {
        public List<EmailClientConfiguration> EmailAccounts { get; set; }

        public PrimaryConfiguration()
        {
            this.EmailAccounts = new List<EmailClientConfiguration>();
        }

        public PrimaryConfiguration(IPropertyReader reader)
        {
            this.EmailAccounts = reader.Read<List<EmailClientConfiguration>>("EmailAccounts");
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("EmailAccounts", this.EmailAccounts);
        }
    }
}
