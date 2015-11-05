namespace ReactNative.Framework.Views
{
    public class ReactContainerView : ReactItemsControl
    {
    }

    public class ReactContainerViewModel : ReactContainerComponentBase
    {
    }

    [ReactModule("ReactContainer")]
    public class ContainerViewManager : ViewManager
    {
        public override IReactComponent View()
        {
            return new ReactContainerViewModel();
        }
    }
}