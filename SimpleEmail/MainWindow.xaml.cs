using System.Windows;

using SimpleEmail.ViewModel;

using SimpleWpf.IocFramework.Application;
using SimpleWpf.IocFramework.Application.Attribute;

namespace SimpleEmail
{
    [IocExportDefault]
    public partial class MainWindow : Window
    {
        public readonly MainViewModel ViewModel;

        public MainWindow()
        {
            this.ViewModel = IocContainer.Get<MainViewModel>();

            InitializeComponent();

            this.DataContext = this.ViewModel;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /*
        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var config = new EmailClientConfiguration()
            {
                ServerAddress = this.ViewModel.Configuration.ServerAddress,
                ServerPort = this.ViewModel.Configuration.ServerPort,
                Password = this.ViewModel.Configuration.Password,
                User = this.ViewModel.Configuration.User,
                ClientId = this.ViewModel.Configuration.ClientId,
                ClientSecret = this.ViewModel.Configuration.ClientSecret,
            };
            var client = new GmailClient(config);

            var messageSummaries = await client.GetSummariesAsync("INBOX");

            this.ViewModel.PrimaryMail.Clear();
            this.ViewModel.PrimaryMail.AddRange(messageSummaries.Select(x => new EmailViewModel()
            {
                Subject = x.NormalizedSubject,
                From = x.Envelope
                        .Sender
                        .Select(x => x.Name)
                        .Join("; ", x => x),
                Timestamp = x.Date.ToUniversalTime().DateTime
            }));

            this.PrimaryTabControl.SelectedIndex = 1;
        }
        */
    }
}