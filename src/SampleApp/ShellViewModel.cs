using System.IO;
using Caliburn.Micro;
using ReactNative.Framework.Views;

namespace SampleApp
{
    public class ShellViewModel : Screen, IShell
    {
        public ShellViewModel()
        {
            // JavaScriptCore
//            var root = IoC.Get<ReactRootViewModel>();
//            Screen = root;
//            var bundle = File.ReadAllText("..\\..\\..\\..\\..\\build\\main.jsbundle");
//            root.RunApplication("SampleApp", bundle, null);

            // CefSharp Demos
//            CefScreen = IoC.Get<CefSampleViewModel>();

            // WebSockets with Chrome
            SocketScreen = IoC.Get<WebSocketSampleViewModel>();
        }

        public object Screen { get; set; }

        public object CefScreen { get; set; }

        public object SocketScreen { get; set; }
    }
}
