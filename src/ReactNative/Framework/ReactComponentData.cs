using System.Collections.Generic;
using System.Reflection;
using Caliburn.Micro;

namespace ReactNative.Framework
{
    public interface IReactComponentData
    {
        string Name { get; }
        IViewManager Manager { get; }
        IReactComponent CreateViewWithTag(long tag);
        void SetPropsForView(IDictionary<string, object> props, IReactComponent view);
        IDictionary<string, object> ViewConfig();
    }

    public class ReactComponentData : IReactComponentData
    {
        private IReactComponent _defaultView;

        public ReactComponentData(IViewManager manager)
        {
            Manager = manager;

            var managerType = manager.GetType();
            var moduleAttr = managerType.GetCustomAttribute<ReactModuleAttribute>();
            var name = moduleAttr.Name;

            if (string.IsNullOrWhiteSpace(name))
            {
                name = managerType.Name;

                if (name.EndsWith("Manager"))
                {
                    name = name.Substring(0, name.Length - "Manager".Length);
                }
            }

            Name = name;
        }

        public string Name { get; private set; }
        public IViewManager Manager { get; private set; }

        public IReactComponent CreateViewWithTag(long tag)
        {
            var view = Manager.View();
            view.ReactTag = tag;
            return view;
        }

        public void SetPropsForView(IDictionary<string, object> props, IReactComponent view)
        {
            if (view == null || props == null)
            {
                return;
            }

            if (_defaultView == null)
            {
                _defaultView = CreateViewWithTag(-1);
            }

            Execute.OnUIThread(() =>
            {
                props.Keys.Apply(key =>
                {
                    SetPropertyForKey(view, key, props[key]);
                });
            });
        }

        public void SetPropertyForKey(object view, string key, object value)
        {
            var propInfo = view
                .GetType()
                .GetProperty(key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            propInfo.SetValue(view, value);
        }

        public IDictionary<string, object> ViewConfig()
        {
            return null;
        }
    }
}