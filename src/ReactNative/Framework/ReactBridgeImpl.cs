using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ReactNative.Framework
{
    public class ReactBridgeImpl : IReactBridge
    {
        private readonly IReactAssemblyProvider _assemblyProvider;
        private readonly IModuleLoader _moduleLoader;
        private readonly IJavaScriptExecutor _executor;
        private readonly IUIManager _uiManager;
        private List<ModuleData> _modules;

        private readonly Timer _timer;

        public ReactBridgeImpl(
            IReactAssemblyProvider assemblyProvider,
            IModuleLoader moduleLoader,
            IJavaScriptExecutor executor,
            IUIManager uiManager)
        {
            _assemblyProvider = assemblyProvider;
            _moduleLoader = moduleLoader;
            _executor = executor;
            _uiManager = uiManager;

            _timer = new Timer(16.67);
//            _timer = new Timer(5000);
            _timer.Elapsed += (e, a) =>
            {
                PollQueue();
            };
        }

        public IEnumerable<ModuleData> Modules
        {
            get { return _modules; }
        }

        public async Task<bool> LoadApplicationScript(string bundle, string sourceUrl)
        {
            Reset();

            _modules = _moduleLoader.Load(this, _assemblyProvider.Assemblies()).ToList();

            _modules.Apply(m => m.Instance.Initialize(this));

            System.Diagnostics.Debug.WriteLine("Executor Setup");
            _executor.Setup();

            System.Diagnostics.Debug.WriteLine("Injecting Modules");
            InjectModules();

            System.Diagnostics.Debug.WriteLine("Executing Script");
            await _executor.Execute(bundle, sourceUrl);

            System.Diagnostics.Debug.WriteLine("flushing");
            var results = await _executor.ExecuteJSCall("BatchedBridge", "flushedQueue", null);
            HandleBuffer(results);
            _timer.Enabled = true;

            return true;
        }

        public async Task RunApplication(string moduleName, long rootTag, IDictionary<string, object> initialProperties)
        {
            var appParams = new Dictionary<string, object>
            {
                {"rootTag", rootTag},
                {"initialProps", initialProperties}
            };

            var json = await _executor.ExecuteJSCall(
                "AppRegistry",
                "runApplication",
                new object[] {moduleName, appParams});

            HandleBuffer(json);
        }

        public Task<string> Execute(string script, string sourceUrl)
        {
            return _executor.Execute(script, sourceUrl);
        }

        public Task<string> ExecuteJSCall(string name, string method, object[] arguments)
        {
            return _executor.ExecuteJSCall(name, method, arguments);
        }

        public void InjectModules()
        {
            var moduleConfig = _moduleLoader.ModuleConfig(_modules);
            var json = JsonConvert.SerializeObject(moduleConfig, Formatting.Indented);

            System.Diagnostics.Debug.WriteLine("__fbBatchedBridgeConfig:\n{0}".ToFormat(json));

            _executor.InjectJSON("__fbBatchedBridgeConfig", json);
        }

        public void HandleBuffer(string json)
        {
            if (string.IsNullOrWhiteSpace(json) || json == "null" || json == "undefined")
            {
                return;
            }

            var requestsArray = JArray.Parse(json);

            if (requestsArray != null)
            {
                var modulesIds = requestsArray[0];
                var methodIds = requestsArray[1];
                var paramsArrays = requestsArray[2];

                for (var i = 0; i < modulesIds.Count(); i++)
                {
                    var moduleId = modulesIds[i].ToObject<int>();
                    var methodId = methodIds[i].ToObject<int>();
                    var arguments = ConvertArguments(paramsArrays[i]);

                    HandleRequestNumber(moduleId, methodId, arguments);
                }
            }
        }

        private object[] ConvertArguments(JToken token)
        {
            var arguments = token.ToObject<object[]>();
            for (var y = 0; y < arguments.Length; y++)
            {
                var arg = arguments[y];
                if (arg is JObject)
                {
                    arguments[y] = ((JObject) arg).ToObject<Dictionary<string, object>>();
                }

                // JArray is converted in ModuleMethod class
            }

            return arguments;
        }

        public void HandleRequestNumber(int moduleId, int methodId, object[] arguments)
        {
            var module = _modules[moduleId];
            var method = module.Methods[methodId];
            method.Invoke(module.Instance, arguments);
        }

        public async void PollQueue()
        {
            var json = await _executor.ExecuteJSCall("BatchedBridge", "flushedQueue", null);
            HandleBuffer(json);
        }

        public void Reset()
        {
            _timer.Enabled = false;
            _uiManager.Reset();
        }
    }
}