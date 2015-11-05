using System.Collections.Generic;
using System.Linq;

namespace ReactNative.Framework
{
    public interface IViewManager : IBridgeModule
    {
        IReactComponent View();
        IEnumerable<string> CustomBubblingEventTypes();
        IEnumerable<string> CustomDirectEventTypes();
    }

    public abstract class ViewManager : IViewManager
    {
        protected IReactBridge Bridge { get; set; }

        public object ViewManagerRegistry { get; set; }
        public object ViewRegistry { get; set; }

        public virtual void Initialize(IReactBridge bridge)
        {
            Bridge = bridge;
        }

        public virtual IDictionary<string, object> ConstantsToExport()
        {
            return null;
        }

        public virtual IEnumerable<string> CustomBubblingEventTypes()
        {
            yield return "click";
        }

        public virtual IEnumerable<string> CustomDirectEventTypes()
        {
            return Enumerable.Empty<string>();
        }

        public abstract IReactComponent View();
    }
}