using System.Windows;

using SimpleEmail.ViewModel;

using SimpleWpf.IocFramework.Application.Attribute;

namespace SimpleEmail
{
    [IocExportDefault]
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        [IocImportingConstructor]
        public MainWindow(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            InitializeComponent();

            this.DataContextChanged += MainWindow_DataContextChanged;

            this.DataContext = _mainViewModel;
        }

        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            HookEmailCollectionsChanged();
        }

        private void HookEmailCollectionsChanged()
        {
            //_mainViewModel.EmailAccounts.CollectionChanged -= RefreshCollectionBindings;
            //_mainViewModel.EmailAccounts.CollectionChanged += RefreshCollectionBindings;

            //foreach (var account in _mainViewModel.EmailAccounts)
            //{
            //    account.EmailFolders.CollectionChanged -= RefreshCollectionBindings;
            //    account.EmailFolders.CollectionChanged += RefreshCollectionBindings;
            //}
        }

        // Hook IsSelected for TreeViewItem email folder
        private void RefreshCollectionBindings(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private void OnEmailFolderSelected(object sender, RoutedEventArgs e)
        {
            //var treeViewItem = (TreeViewItem)sender;
            //var emailFolder = (EmailFolderViewModel)treeViewItem.DataContext;

            //// Set Selected Folder
            //_mainViewModel.SetSelectedEmailFolder(emailFolder.Id);
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