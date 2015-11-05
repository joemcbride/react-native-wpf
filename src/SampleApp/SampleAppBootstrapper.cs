using Caliburn.Micro;
using ReactNative.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using ReactNative.Framework.Views;
using ReactNative.JavaScriptCore;

namespace SampleApp
{
    public class SampleAppBootstrapper : BootstrapperBase
    {
        private SimpleContainer _container;

        public SampleAppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container = new SimpleContainer();
            _container.Instance(_container);
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();

            _container.Singleton<IUIManager, UIManager>();
            _container.Singleton<IEventDispatcher, EventDispatcher>();

            _container.Singleton<TextBlockViewManager>();
            _container.Singleton<ButtonViewManager>();
            _container.Singleton<ContainerViewManager>();
            _container.Singleton<StackPanelViewManager>();

            _container.Instance<IReactAssemblyProvider>(new ReactAssemblyProvider(SelectAssemblies));

//            _container.PerRequest<IJavaScriptExecutor, JavaScriptCoreExecutor>();
            _container.PerRequest<IJavaScriptExecutor, WebSocketExecutor>();
            _container.Singleton<IReactBridge, ReactBridgeImpl>();
            _container.PerRequest<IModuleLoader, ModuleLoader>();
            _container.PerRequest<ReactRootViewModel>();
            _container.PerRequest<ReactTextBlockViewModel>();
            _container.PerRequest<ReactButtonViewModel>();
            _container.PerRequest<ReactContainerViewModel>();

            _container.PerRequest<IShell, ShellViewModel>();
            _container.PerRequest<CefSampleViewModel>();
            _container.PerRequest<WebSocketSampleViewModel>();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var result = base.SelectAssemblies().ToList();
            result.Add(typeof(IReactBridge).Assembly);
            return result;
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }
    }
}
