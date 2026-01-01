using SimpleWpf.RecursiveSerializer.Component.Interface;
using SimpleWpf.RecursiveSerializer.Interface;

namespace SimpleEmail.Core.Model.Configuration
{
    public class EmailAccountConfiguration : IRecursiveSerializable
    {
        public string ServerAddress { get; set; }
        public ushort ServerPort { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public EmailAccountConfiguration()
        {
            this.ServerAddress = string.Empty;
            this.ServerPort = 0;
            this.EmailAddress = string.Empty;
            this.Password = string.Empty;
            this.ClientId = string.Empty;
            this.ClientSecret = string.Empty;
        }

        public EmailAccountConfiguration(IPropertyReader reader)
        {
            this.ServerAddress = reader.Read<string>("ServerAddress");
            this.ServerPort = reader.Read<ushort>("ServerPort");
            this.EmailAddress = reader.Read<string>("EmailAddress");
            this.Password = reader.Read<string>("Password");
            this.ClientId = reader.Read<string>("ClientId");
            this.ClientSecret = reader.Read<string>("ClientSecret");
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("ServerAddress", this.ServerAddress);
            writer.Write("ServerPort", this.ServerPort);
            writer.Write("EmailAddress", this.EmailAddress);
            writer.Write("Password", this.Password);
            writer.Write("ClientId", this.ClientId);
            writer.Write("ClientSecret", this.ClientSecret);
        }
    }
}
