using System;
using System.Collections.Generic;
using System.Reflection;

namespace ReactNative.Framework
{
    public interface IReactAssemblyProvider
    {
        IEnumerable<Assembly> Assemblies();
    }

    public class ReactAssemblyProvider : IReactAssemblyProvider
    {
        private readonly Func<IEnumerable<Assembly>> _provideAssemblies;

        public ReactAssemblyProvider(Func<IEnumerable<Assembly>> provideAssemblies)
        {
            _provideAssemblies = provideAssemblies;
        }

        public IEnumerable<Assembly> Assemblies()
        {
            return _provideAssemblies();
        }
    }
}
