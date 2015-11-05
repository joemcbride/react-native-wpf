using reactnative;
using ReactNative.Framework;
using System.Threading.Tasks;

namespace ReactNative.JavaScriptCore
{
    public class JavaScriptCoreExecutor : IJavaScriptExecutor
    {
        private reactnative.ReactBridge _bridge;

        public Task<string> Execute(string script, string sourceUrl)
        {
            var result = _bridge.Execute(script);
            return Task.FromResult(result);
        }

        public Task<string> ExecuteJSCall(string name, string method, object[] arguments)
        {
            var result = _bridge.ExecuteJSCall(name, method, arguments);
            return Task.FromResult(result);
        }

        public Task InjectJSON(string objectName, string json)
        {
            _bridge.InjectJSONText(objectName, json);
            return Task.FromResult(true);
        }

        public void Setup()
        {
            _bridge = new ReactBridge();
        }
    }
}
