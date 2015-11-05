using Caliburn.Micro;

namespace ReactNative.Framework
{
    public class ReactContainerComponentBase : ReactComponentBase, IReactContainerComponent
    {
        private readonly BindableCollection<IReactComponent> _views = new BindableCollection<IReactComponent>();

        public IObservableCollection<IReactComponent> Views
        {
            get { return _views; }
        }

        public void AddView(IReactComponent view)
        {
            _views.Add(view);
        }

        public void AddViewAtIndex(IReactComponent view, int index)
        {
            _views.Insert(index, view);
        }

        public void RemoveView(IReactComponent child)
        {
            _views.Remove(child);
        }
    }
}