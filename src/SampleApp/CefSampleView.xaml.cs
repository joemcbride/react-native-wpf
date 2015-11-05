using System.Windows.Controls;

namespace SampleApp
{
    public partial class CefSampleView : UserControl
    {
        public CefSampleView()
        {
            InitializeComponent();

            browser.RegisterJsObject("bound", new JSObject {SomeValue = "A value"});
            browser.ConsoleMessage += Browser_ConsoleMessage;
        }

        private void Browser_ConsoleMessage(object sender, CefSharp.ConsoleMessageEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("CEF ({0}): {1}", e.Line, e.Message);
        }
    }
}
