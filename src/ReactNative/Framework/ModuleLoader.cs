using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;

namespace ReactNative.Framework
{
    public interface IModuleLoader
    {
        IEnumerable<ModuleData> Load(IReactBridge bridge, IEnumerable<Assembly> assemblies);
        List<ModuleData> Build(IEnumerable<Assembly> assemblies);
        IDictionary<string, object> ModuleConfig(IEnumerable<ModuleData> modules);
    }

    public class ModuleLoader : IModuleLoader
    {
        public IEnumerable<ModuleData> Load(IReactBridge bridge, IEnumerable<Assembly> assemblies)
        {
            var modules = Build(assemblies);
            return modules;
        }

        public List<ModuleData> Build(IEnumerable<Assembly> assemblies)
        {
            var moduleTypes = assemblies
                .SelectMany(a =>
                    a.GetTypes().Where(t => Attribute.IsDefined((MemberInfo) t, typeof(ReactModuleAttribute)))
                )
                .ToList();

            var modules = moduleTypes
                .Apply((moduleType, index) =>
                {
                    var attr = moduleType.GetCustomAttribute<ReactModuleAttribute>();
                    var typeToRegister = attr.Type ?? moduleType;
                    var module = (IBridgeModule) IoC.GetInstance(typeToRegister, null);

                    Debug.Assert(module != null,
                        "Null module for {0}, is it registered in the container?"
                        .ToFormat(typeToRegister.Name));

                    var name = !string.IsNullOrWhiteSpace(attr.Name) ? attr.Name : moduleType.Name;
                    return new ModuleData(index, name, module);
                })
                .ToList();

            return modules;
        }

        public IDictionary<string, object> ModuleConfig(IEnumerable<ModuleData> modules)
        {
            var moduleConfig = new Dictionary<string, object>();
            modules.Apply(module => moduleConfig[module.Name] = module.Config());

            var container = new Dictionary<string, object>();
            container["remoteModuleConfig"] = moduleConfig;
            return container;
        }
    }
}