using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReactNative.Framework
{
    public interface IReactBridge
    {
        IEnumerable<ModuleData> Modules { get; }

        Task<bool> LoadApplicationScript(string bundle, string sourceUrl);

        Task RunApplication(string moduleName, long rootTag, IDictionary<string, object> initialProperties);

        Task<string> Execute(string script, string sourceUrl);

        Task<string> ExecuteJSCall(string name, string method, object[] arguments);

        void Reset();
    }
}