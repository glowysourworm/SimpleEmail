using System.Collections.ObjectModel;

using SimpleEmail.Controller.Interface;
using SimpleEmail.Core.Component.Interface;
using SimpleEmail.Event;
using SimpleEmail.ViewModel.Configuration;
using SimpleEmail.ViewModel.Email;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.Extensions.Command;
using SimpleWpf.Extensions.ObservableCollection;
using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.IocFramework.EventAggregation;
using SimpleWpf.Utilities;

namespace SimpleEmail.ViewModel
{
    [IocExportDefault]
    public class MainViewModel
    {
        private readonly IEmailModelService _emailModelService;

        /// <summary>
        /// Configuration object for the application
        /// </summary>
        public ConfigurationViewModel Configuration { get; set; }

        /// <summary>
        /// Primary list of email accounts
        /// </summary>
        public ObservableCollection<EmailAccountViewModel> EmailAccounts { get; private set; }

        // Selected Account / Email list
        public ObservableCollection<EmailStubViewModel> SelectedFolderEmail { get; private set; }

        // Commands
        public SimpleCommand NewAccountCommand { get; private set; }
        public SimpleCommand EditAccountSettingsCommand { get; private set; }
        public SimpleCommand EditThemeSettingsCommand { get; private set; }

        public SimpleCommand<EmailFolderViewModel> SelectFolderCommand { get; private set; }

        // Task Related

        /// <summary>
        /// This should signal that the application is waiting on a blocking task, which should prevent the user
        /// from doing anything on the UI.
        /// </summary>
        public bool PrimaryTaskRunning { get; private set; }

        /// <summary>
        /// This signals to the UI that there is a background task running, which may be displayed on the status
        /// bar, or is also in a task queue.
        /// </summary>
        public bool BackgroundTaskRunning { get; private set; }

        [IocImportingConstructor]
        public MainViewModel(IConfigurationManager configurationManager,
                             IIocEventAggregator eventAggregator,
                             IDialogController dialogController,
                             IEmailModelService emailModelService)
        {
            _emailModelService = emailModelService;

            this.SelectedFolderEmail = new ObservableCollection<EmailStubViewModel>();
            this.EmailAccounts = new ObservableCollection<EmailAccountViewModel>();
            this.Configuration = new ConfigurationViewModel(configurationManager.Get());        // Loads view model from model

            this.EmailAccounts.CollectionChanged += OnEmailAccountsChanged;

            // New Account
            this.NewAccountCommand = new SimpleCommand(() =>
            {
                var viewModel = new EmailAccountConfigurationViewModel();

                // Show editor
                var result = dialogController.ShowDialogWindowSync(DialogEventData.ShowNewEmailAccountEditor(viewModel));

                // Save
                if (result)
                {
                    // Add New Account to account list
                    this.Configuration.EmailAccountConfigurations.Add(viewModel);

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

            }, () => this.Configuration.EmailAccountConfigurations.Count > 0);

            // Edit Theme Settings
            this.EditThemeSettingsCommand = new SimpleCommand(() =>
            {
                // TODO
            });

            // Task Related Events
            eventAggregator.GetEvent<TaskEvent>().Subscribe(data =>
            {
                switch (data.Type)
                {
                    case TaskType.Initialization:
                    case TaskType.PrimaryTask:
                        this.PrimaryTaskRunning = data.Status == TaskEventType.Completed ? false : true;
                        break;
                    case TaskType.BackgroundTask:
                        this.BackgroundTaskRunning = data.Status == TaskEventType.Completed ? false : true;
                        break;
                    default:
                        throw new Exception("Unhandled TaskType:  MainViewModel.cs");
                }

                // Initialization
                if (data.Type == TaskType.Initialization &&
                    data.Status == TaskEventType.Completed)
                {
                    RefreshEmailAccounts();
                }
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

        private void RefreshEmailAccounts()
        {
            // Clear Existing Accounts
            this.EmailAccounts.Clear();

            // Get accounts from email model service
            var emailAccounts = _emailModelService.GetAccountList();

            foreach (var account in emailAccounts)
            {
                var viewModel = new EmailAccountViewModel();

                viewModel.EmailAddress = account.EmailAddress.ToString();
                viewModel.EmailFolders.AddRange(account.SpecialFolders.Select(folder =>
                {
                    return new EmailFolderViewModel(folder);
                }));
                viewModel.EmailFolders.AddRange(account.PersonalFolders.Select(folder =>
                {
                    return new EmailFolderViewModel(folder);
                }));

                this.EmailAccounts.Add(viewModel);
            }
        }

        private void OnEmailAccountsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshEmailAccountHooks();
            RefreshEmailFolderHooks();
        }

        private void OnEmailFoldersChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshEmailFolderHooks();
        }

        private void RefreshEmailAccountHooks()
        {
            foreach (var emailAccount in this.EmailAccounts)
            {
                emailAccount.EmailFolders.CollectionChanged -= OnEmailFoldersChanged;
                emailAccount.EmailFolders.CollectionChanged += OnEmailFoldersChanged;
            }
        }

        private void RefreshEmailFolderHooks()
        {
            foreach (var emailAccount in this.EmailAccounts)
            {
                foreach (var emailFolder in emailAccount.EmailFolders)
                {
                    RefreshEmailFolderHooksRecurse(emailFolder);
                }
            }
        }

        private void RefreshEmailFolderHooksRecurse(EmailFolderViewModel folder)
        {
            folder.PropertyChanged -= EmailFolder_PropertyChanged;
            folder.PropertyChanged += EmailFolder_PropertyChanged;

            foreach (var emailFolder in folder.SubFolders)
            {
                RefreshEmailFolderHooksRecurse(emailFolder);
            }
        }

        private async void EmailFolder_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var emailFolder = sender as EmailFolderViewModel;

            // Load Email List
            //
            if (emailFolder != null &&
                emailFolder.IsSelected &&
                e.PropertyName == "IsSelected")
            {
                // Email Service Call
                //
                var emailStubs = await _emailModelService.GetEmailStubs(emailFolder.EmailAddress, emailFolder.Id);

                if (emailStubs != null)
                {
                    this.SelectedFolderEmail.Clear();
                    this.SelectedFolderEmail.AddRange(emailStubs.Select(x => new EmailStubViewModel(x)).Actualize());
                }
            }
        }
    }
}
