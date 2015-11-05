using System.Collections.Generic;

namespace ReactNative.Framework
{
    public interface IBridgeModule
    {
        void Initialize(IReactBridge bridge);

        IDictionary<string, object> ConstantsToExport();
    }
}
