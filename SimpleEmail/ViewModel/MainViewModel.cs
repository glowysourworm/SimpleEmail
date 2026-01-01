using System.Collections.ObjectModel;

using SimpleEmail.Controller.Interface;
using SimpleEmail.Core.Component.Interface;
using SimpleEmail.Event;

using SimpleWpf.Extensions.Command;
using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.IocFramework.EventAggregation;
using SimpleWpf.Utilities;

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

        public SimpleCommand NewAccountCommand { get; private set; }

        public SimpleCommand EditAccountSettingsCommand { get; private set; }
        public SimpleCommand EditThemeSettingsCommand { get; private set; }

        [IocImportingConstructor]
        public MainViewModel(IConfigurationManager configurationManager,
                             IIocEventAggregator eventAggregator,
                             IDialogController dialogController)
        {
            this.PrimaryMail = new ObservableCollection<EmailViewModel>();
            this.Configuration = new ConfigurationViewModel(configurationManager.Get());        // Loads view model from model

            // New Account
            this.NewAccountCommand = new SimpleCommand(() =>
            {
                var viewModel = new EmailAccountSettingsViewModel();

                // Show editor
                var result = dialogController.ShowDialogWindowSync(DialogEventData.ShowNewEmailAccountEditor(viewModel));

                // Save
                if (result)
                {
                    // Add New Account to account list
                    this.Configuration.EmailAccountSettings.Add(viewModel);

                    // Map view model back to configuration
                    var configuration = this.Configuration.CreateConfiguration();

                    // Set -> Save to primary configuration using IConfigurationManager
                    configurationManager.Set(configuration);
                    configurationManager.Save();

                    CheckCommandsReady();
                }
            });

            // Edit Account Settings
            //
            this.EditAccountSettingsCommand = new SimpleCommand(() =>
            {
                // Copy view model for editing
                var configuration = new ConfigurationViewModel();

                // Create copy using AutoMapper
                BasicHelpers.MapOnto(this.Configuration, configuration);

                // Show editor
                var result = dialogController.ShowDialogWindowSync(DialogEventData.ShowConfigurationEditor(configuration));

                // Save
                if (result)
                {
                    // Save in memory to view model
                    BasicHelpers.MapOnto(configuration, this.Configuration);

                    // Set -> Save to primary configuration using IConfigurationManager
                    configurationManager.Set(configuration.CreateConfiguration());
                    configurationManager.Save();

                    CheckCommandsReady();
                }

            }, () => this.Configuration.EmailAccountSettings.Count > 0);

            // Edit Theme Settings
            this.EditThemeSettingsCommand = new SimpleCommand(() =>
            {
                // TODO
            });
        }

        /// <summary>
        /// Command readiness depends on the amount of email accounts, so this should be
        /// executed after email settings are edited, in case any are deleted or created.
        /// </summary>
        private void CheckCommandsReady()
        {
            this.NewAccountCommand.RaiseCanExecuteChanged();
            this.EditAccountSettingsCommand.RaiseCanExecuteChanged();
            this.EditThemeSettingsCommand.RaiseCanExecuteChanged();
        }

    }
}
