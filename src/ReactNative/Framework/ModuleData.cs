using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReactNative.Framework
{
    public class ModuleData
    {
        private readonly IBridgeModule _module;
        private ModuleMethod[] _methods;

        public ModuleData(int id, string name, IBridgeModule module)
        {
            Id = id;
            Name = name;
            _module = module;
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public IBridgeModule Instance {
            get { return _module; }
        }

        public ModuleMethod[] Methods
        {
            get
            {
                if (_methods == null)
                {
                    var moduleType = _module.GetType();
                    var methods = moduleType
                        .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .Where(m => Attribute.IsDefined((MemberInfo) m, typeof (ReactMethodAttribute)));

                    _methods = methods.Select(method =>
                    {
                        var attr = method.GetCustomAttribute<ReactMethodAttribute>();
                        var name = !string.IsNullOrWhiteSpace(attr.Name) ? attr.Name : method.Name;

                        return new ModuleMethod
                        {
                            Name = method.Name,
                            JavaScriptName = name,
                            MethodInfo = method
                        };
                    }).ToArray();
                }
                return _methods;
            }
        }

        public IDictionary<string, object> Config()
        {
            var config = new Dictionary<string, object>();
            config["moduleID"] = Id;

            var constants = Instance.ConstantsToExport();
            if (constants != null)
            {
                config["constants"] = constants;
            }

            var methods = new Dictionary<string, object>();

            Methods.Apply((method, index) =>
            {
                methods[method.JavaScriptName] = new
                {
                    methodID = index,
                    type = "remote"
                };
            });

            config["methods"] = methods;

            return config;
        }
    }
}