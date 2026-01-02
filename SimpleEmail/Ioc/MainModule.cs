using SimpleEmail.Core.Component.Interface;
using SimpleEmail.Event;

using SimpleWpf.IocFramework.Application;
using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.IocFramework.EventAggregation;
using SimpleWpf.IocFramework.RegionManagement.Interface;

namespace SimpleEmail.Ioc
{
    [IocExportDefault]
    public class MainModule : ModuleBase
    {
        private readonly IIocRegionManager _regionManager;
        private readonly IIocEventAggregator _eventAggregator;
        private readonly IConfigurationManager _configurationManager;
        private readonly IEmailModelService _emailModelService;

        [IocImportingConstructor]
        public MainModule(IIocRegionManager regionManager,
                          IIocEventAggregator eventAggregator,
                          IConfigurationManager configurationManager,
                          IEmailModelService emailModelService)
            : base(regionManager, eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _configurationManager = configurationManager;
            _emailModelService = emailModelService;
        }

        public override void Initialize()
        {
            base.Initialize();

            //_regionManager.LoadNamedInstance(MAIN_REGION, typeof(MainView));

            // ManagerView -> FileTreeView (needs loading)
            //var treeView = _regionManager.LoadNamedInstance<FileTreeView>("ManagerViewFileTreeViewRegion", true);

            // -> to set the search pattern
            //treeView.SearchPattern = "*.mp3";
        }

        // Entry point to the application (post-bootstrapper)
        public override async Task RunAsync()
        {
            base.Run();

            var configuration = _configurationManager.Get();

            // Publish Task Started
            _eventAggregator.GetEvent<TaskEvent>().Publish(new TaskEventData()
            {
                Type = TaskType.Initialization,
                Status = TaskEventType.Started
            });

            // Use this to initialize the service layer
            await _emailModelService.Initialize(configuration.EmailAccounts);

            // Publish Task Completed
            _eventAggregator.GetEvent<TaskEvent>().Publish(new TaskEventData()
            {
                Type = TaskType.Initialization,
                Status = TaskEventType.Completed
            });
        }
    }
}
