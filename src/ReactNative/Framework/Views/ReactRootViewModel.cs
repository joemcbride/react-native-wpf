using System.Threading.Tasks;

namespace ReactNative.Framework.Views
{
    public class ReactRootView : ReactItemsControl
    {
    }

    public class ReactRootViewModel : ReactContainerComponentBase
    {
        private readonly IReactBridge _bridge;

        public ReactRootViewModel(IUIManager uiManager, IReactBridge bridge)
        {
            _bridge = bridge;
            uiManager.RegisterRootView(this);
        }

        public async void RunApplication(string moduleName, string bundle, string sourceUrl)
        {
            await _bridge.LoadApplicationScript(bundle, sourceUrl);
            await _bridge.RunApplication(moduleName, ReactTag ?? 0, null);
        }
    }
}
