using System.Windows;

using SimpleEmail.Core.Component;
using SimpleEmail.Core.Component.Model;
using SimpleEmail.ViewModel;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.Extensions.ObservableCollection;

namespace SimpleEmail
{
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel = new MainViewModel();

        public MainWindow()
        {
            this.ViewModel = new MainViewModel();

            InitializeComponent();

            this.DataContext = this.ViewModel;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Default Password
            this.PasswordTB.Password = this.ViewModel.Configuration.Password;
        }

        private void PasswordTB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Configuration.Password = this.PasswordTB.Password;
        }

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
    }
}