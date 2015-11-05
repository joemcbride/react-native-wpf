using CefSharp;
using Newtonsoft.Json;
using ReactNative.Framework;
using System.Threading.Tasks;

namespace SampleApp
{
    public class CefJavascriptExecutor : IJavaScriptExecutor
    {
        private readonly IWebBrowser _webBrowser;

        public CefJavascriptExecutor(IWebBrowser webBrowser)
        {
            _webBrowser = webBrowser;
        }

        public async Task<string> Execute(string script, string sourceUrl)
        {
            var res = await _webBrowser.EvaluateScriptAsync(script);
            return "{0}".ToFormat(res.Result);
        }

        public async Task<string> ExecuteJSCall(string name, string method, object[] arguments)
        {
            var argJson = JsonConvert.SerializeObject(arguments);

            // react-native
//            var script = "(function(g){{\nreturn require('{0}').{1}.apply(undefined, {2});\n}})(this);".ToFormat(name, method, argJson);
            var script = "(function(g){{\nreturn g.{0}.{1}.apply(undefined, {2});\n}})(this);".ToFormat(name, method, argJson);
            var res = await _webBrowser.EvaluateScriptAsync(script);
            string json = null;
            if (res.Success)
            {
                json = JsonConvert.SerializeObject(res.Result);
            }
            return json;
        }

        public async Task InjectJSON(string objectName, string json)
        {
            var script = "(function(g){{ g.{0} = {1}; }})(this)".ToFormat(objectName, json);
            await _webBrowser.EvaluateScriptAsync(script);
        }

        public void Setup()
        {
        }
    }
}
