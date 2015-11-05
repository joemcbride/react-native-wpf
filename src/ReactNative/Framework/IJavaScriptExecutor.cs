using System.Threading.Tasks;

namespace ReactNative.Framework
{
    public interface IJavaScriptExecutor
    {
        Task<string> Execute(string script, string sourceUrl);
        Task<string> ExecuteJSCall(string name, string method, object[] arguments);
        Task InjectJSON(string objectName, string json);
        void Setup();
    }
}