// react-native.h

#pragma once

#include "JavaScriptCore\JavaScriptCore.h"

using namespace System;
using namespace System::Collections::Generic;

namespace reactnative {
    public ref class ReactBridge
    {
    public:
        ReactBridge();
        String^ Execute(String^ script);
        String^ ExecuteJSCall(String^ name, String^ method, array<Object ^> ^ arguments);
        void InjectJSONText(String^ objectName, String^ json);

    private:
        JSGlobalContextRef ctx;

        void AddNativeHook(String^ name, JSObjectCallAsFunctionCallback hook);

        ~ReactBridge() {
            JSGlobalContextRelease(ctx);
            ctx = nullptr;
        }
    };
}
