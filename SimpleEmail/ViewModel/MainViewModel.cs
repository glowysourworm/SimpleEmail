using System.Collections.ObjectModel;

using SimpleEmail.Controller.Interface;
using SimpleEmail.Core.Component.Interface;
using SimpleEmail.Core.Model;
using SimpleEmail.Event;

using SimpleWpf.Extensions.Command;
using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.IocFramework.EventAggregation;

namespace SimpleEmail.ViewModel
{
    [IocExportDefault]
    public class MainViewModel
    {
        /// <summary>
        /// Configuration object for the application
        /// </summary>
        public ConfigurationViewModel Configuration { get; set; }

        /// <summary>
        /// Primary email list
        /// </summary>
        public ObservableCollection<EmailViewModel> PrimaryMail { get; private set; }

        public SimpleCommand EditAccountSettingsCommand { get; private set; }
        public SimpleCommand EditThemeSettingsCommand { get; private set; }

        [IocImportingConstructor]
        public MainViewModel(IConfigurationManager configurationManager,
                             IIocEventAggregator eventAggregator,
                             IDialogController dialogController)
        {
            this.PrimaryMail = new ObservableCollection<EmailViewModel>();
            this.Configuration = new ConfigurationViewModel();

            // Edit Account Settings
            //
            this.EditAccountSettingsCommand = new SimpleCommand(() =>
            {
                dialogController.ShowDialogWindowSync(DialogEventData.ShowConfigurationEditor(this.Configuration));
            });

            // Edit Theme Settings
            this.EditThemeSettingsCommand = new SimpleCommand(() =>
            {
                // TODO
            });
        }

        /// <summary>
        /// Initializes the main view model with the supplied configuration
        /// </summary>
        public MainViewModel(PrimaryConfiguration configuration)
        {

        }
    }
}
