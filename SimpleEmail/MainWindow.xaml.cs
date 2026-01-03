using System.Windows;

using CefSharp;

using SimpleEmail.Core.Component.Interface;
using SimpleEmail.ViewModel;
using SimpleEmail.ViewModel.Email;
using SimpleEmail.Views;

using SimpleWpf.IocFramework.Application.Attribute;

namespace SimpleEmail
{
    [IocExportDefault]
    public partial class MainWindow : Window
    {
        private readonly IEmailModelService _emailModelService;
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        [IocImportingConstructor]
        public MainWindow(IEmailModelService emailModelService, MainViewModel mainViewModel)
        {
            _emailModelService = emailModelService;
            _mainViewModel = mainViewModel;

            InitializeComponent();

            this.DataContext = _mainViewModel;
        }

        private async void MailGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = (e.OriginalSource as FrameworkElement).DataContext as EmailStubViewModel;

            if (viewModel != null)
            {
                var email = await _emailModelService.GetEmail(viewModel.EmailAddress, viewModel.FolderId, viewModel.Uid);

                // View Email
                var emailView = new EmailView();
                var window = new Window();

                window.Content = emailView;
                emailView.Browser.LoadHtml(email.HtmlBody);
                emailView.Browser.BrowserSettings.Javascript = CefState.Disabled;

                window.Show();
            }
        }
    }
}