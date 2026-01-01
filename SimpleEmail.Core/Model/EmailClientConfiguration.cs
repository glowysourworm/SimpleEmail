using SimpleWpf.RecursiveSerializer.Component.Interface;
using SimpleWpf.RecursiveSerializer.Interface;

namespace SimpleEmail.Core.Component.Model
{
    public class EmailClientConfiguration : IRecursiveSerializable
    {
        public string ServerAddress { get; set; }
        public ushort ServerPort { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public EmailClientConfiguration()
        {
        }

        public EmailClientConfiguration(IPropertyReader reader)
        {
            this.ServerAddress = reader.Read<string>("ServerAddress");
            this.ServerPort = reader.Read<ushort>("ServerPort");
            this.User = reader.Read<string>("User");
            this.Password = reader.Read<string>("Password");
            this.ClientId = reader.Read<string>("ClientId");
            this.ClientSecret = reader.Read<string>("ClientSecret");
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("ServerAddress", this.ServerAddress);
            writer.Write("ServerPort", this.ServerPort);
            writer.Write("User", this.User);
            writer.Write("Password", this.Password);
            writer.Write("ClientId", this.ClientId);
            writer.Write("ClientSecret", this.ClientSecret);
        }
    }
}
