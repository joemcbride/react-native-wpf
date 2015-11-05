using Caliburn.Micro;
using CefSharp;
using CefSharp.Wpf;
using Newtonsoft.Json;
using ReactNative.Framework;
using ReactNative.Framework.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SampleApp
{
    public class JSObject
    {
        private string _someValue;

        public string SomeValue
        {
            get { return _someValue; }
            set
            {
                _someValue = value;
            }
        }
    }

    public class CefSampleViewModel : Screen
    {
        private ReactRootViewModel _reactScreen;
        private string _message;
        private IWpfWebBrowser _webBrowser;
        private string _script;
        private IJavaScriptExecutor _javaScriptExecutor;
        private string _address;

        public CefSampleViewModel()
        {
            _script = "myGlobal.echo = function(arg) {\n  console.log('echo ' + JSON.stringify(arg));\n};";
        }

        public IWpfWebBrowser WebBrowser
        {
            get { return _webBrowser; }
            set
            {
                _webBrowser = value;
                NotifyOfPropertyChange(() => WebBrowser);

                if (_webBrowser != null)
                {
                    LoadReact(_webBrowser);
                }
            }
        }

        public string Script
        {
            get { return _script; }
            set
            {
                _script = value;
                NotifyOfPropertyChange(() => Script);
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                NotifyOfPropertyChange(() => Address);
            }
        }

        public void Go()
        {
            if (_webBrowser != null)
            {
                _webBrowser.Load(Address);
            }
        }

        public void ShowDevTools()
        {
            if (_webBrowser != null)
            {
                _webBrowser.ShowDevTools();
            }
        }

        public async Task LoadReact(IWebBrowser browser)
        {
            _javaScriptExecutor = new CefJavascriptExecutor(browser);

            await LoadPageAsync(browser, "<html><head></head><body><div>Reactify</div></body></html>");
        }

        public static Task LoadPageAsync(IWebBrowser browser, string html = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                //Wait for while page to finish loading not just the first frame
                if (!args.IsLoading)
                {
                    browser.LoadingStateChanged -= handler;
                    tcs.TrySetResult(true);
                }
            };

            browser.LoadingStateChanged += handler;

            if (!string.IsNullOrWhiteSpace(html))
            {
                browser.LoadHtml(html, "http://react.local");
            }
            return tcs.Task;
        }

        public void FlushQueue()
        {
            _javaScriptExecutor
                .ExecuteJSCall("BatchedBridge", "flushedQueue", null)
                .ContinueWith(res =>
                {
                    AddMessage(res.Result);
                });
        }

        public void AddGlobal()
        {
            var moduleConfig = new Dictionary<string, object>();
            moduleConfig["one"] = 123;
            moduleConfig["two"] = new Dictionary<string, object> { {"three", 456} };

            var configJSON = JsonConvert.SerializeObject(moduleConfig);
            _javaScriptExecutor.InjectJSON("myGlobal", configJSON);
        }

        public void ExecuteGlobal()
        {
            var moduleConfig = new Dictionary<string, object>();
            moduleConfig["two"] = new Dictionary<string, object> { {"three", 456} };
            _javaScriptExecutor.ExecuteJSCall("myGlobal", "echo", new object[] {moduleConfig});
        }

        public void Run()
        {
            ExecuteScript(Script);
        }

        private async void ExecuteScript(string script)
        {
            try
            {
                var response = await _webBrowser.EvaluateScriptAsync(script);
                if (response.Success && response.Result is IJavascriptCallback)
                {
                    response = await ((IJavascriptCallback)response.Result).ExecuteAsync("This is a callback from EvaluateJavaScript");
                }

                var message = "{0}".ToFormat(response.Success
                    ? (response.Result ?? "null")
                    : response.Message);
                AddMessage(message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while evaluating Javascript: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddMessage(string message)
        {
            var sb = new StringBuilder(Message);
            sb.AppendLine("{0}: {1}".ToFormat(DateTime.Now, message));
            Message = sb.ToString();
        }
    }
}
