// This is the main DLL file.

#include "stdafx.h"
#include <string>

#include "react-native.h"
#include "JSCoreMarshal.h"
#include "MyWrapper.h"
#include "Logger.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Windows::Threading;
using namespace reactnative;

ReactBridge::ReactBridge() {
    ctx = JSGlobalContextCreate(nullptr);

    AddNativeHook(L"nativeLoggingHook", wrapper_nativeLogging);
}

String^ ReactBridge::Execute(String^ scriptText)
{
    JSValueRef exception = nullptr;
    JSStringRef script = JSCoreMarshal::StringToJSString(scriptText);
    JSValueRef result = JSEvaluateScript(ctx, script, nullptr, nullptr, 0, &exception);
    JSStringRelease(script);

    String^ strResult;

    if (exception != nullptr) {
        Exception^ exc = JSCoreMarshal::JSErrorToException(ctx, exception);
        Logger::Error(exc);
        return strResult;
    }

    if (result != nullptr)
    {
        JSStringRef str = JSValueToStringCopy(ctx, result, nullptr);
        strResult = JSCoreMarshal::JSStringToString(str);
        JSStringRelease(str);
    }
    return strResult;
}

String^ ReactBridge::ExecuteJSCall(String^ name, String^ method, array<Object ^>^ arguments)
{
    String^ argsString = Newtonsoft::Json::JsonConvert::SerializeObject(arguments);

    JSValueRef errorJSRef = nullptr;
    JSValueRef resultJSRef = nullptr;
    JSGlobalContextRef contextJSRef = ctx;
    JSObjectRef globalObjectJSRef = JSContextGetGlobalObject(ctx);

    // get require
    JSStringRef requireNameJSStringRef = JSCoreMarshal::StringToJSString(L"require");
    JSValueRef requireJSRef = JSObjectGetProperty(contextJSRef, globalObjectJSRef, requireNameJSStringRef, &errorJSRef);
    JSStringRelease(requireNameJSStringRef);

    // get module
    JSStringRef moduleNameJSStringRef = JSCoreMarshal::StringToJSString(name);
    JSValueRef moduleNameJSRef = JSValueMakeString(contextJSRef, moduleNameJSStringRef);
    JSValueRef moduleJSRef = JSObjectCallAsFunction(contextJSRef, (JSObjectRef)requireJSRef, NULL, 1, (const JSValueRef *)&moduleNameJSRef, &errorJSRef);
    JSStringRelease(moduleNameJSStringRef);

    // get method
    JSStringRef methodNameJSStringRef = JSCoreMarshal::StringToJSString(method);
    JSValueRef methodJSRef = JSObjectGetProperty(contextJSRef, (JSObjectRef)moduleJSRef, methodNameJSStringRef, &errorJSRef);
    JSStringRelease(methodNameJSStringRef);

    // invoke method no arguments
    if (arguments == nullptr || arguments->Length == 0) {
        resultJSRef = JSObjectCallAsFunction(contextJSRef, (JSObjectRef)methodJSRef, (JSObjectRef)moduleJSRef, 0, NULL, &errorJSRef);
    }

    // invoke method with 1 argument
    else if (arguments != nullptr && arguments->Length == 1)
    {
        JSStringRef argsJSStringRef = JSCoreMarshal::StringToJSString(argsString);
        JSValueRef argsJSRef = JSValueMakeFromJSONString(contextJSRef, argsJSStringRef);
        resultJSRef = JSObjectCallAsFunction(contextJSRef, (JSObjectRef)methodJSRef, (JSObjectRef)moduleJSRef, 1, &argsJSRef, &errorJSRef);
        JSStringRelease(argsJSStringRef);
    }

    // invoke method with multiple arguments
    else
    {
        // apply invoke with array of arguments
        JSStringRef applyNameJSStringRef = JSCoreMarshal::StringToJSString("apply");
        JSValueRef applyJSRef = JSObjectGetProperty(contextJSRef, (JSObjectRef)methodJSRef, applyNameJSStringRef, &errorJSRef);
        JSStringRelease(applyNameJSStringRef);

        if (applyJSRef != NULL && errorJSRef == NULL) {
            // invoke apply
            JSStringRef argsJSStringRef = JSCoreMarshal::StringToJSString(argsString);
            JSValueRef argsJSRef = JSValueMakeFromJSONString(contextJSRef, argsJSStringRef);

            JSValueRef args[2];
            args[0] = JSValueMakeNull(contextJSRef);
            args[1] = argsJSRef;

            resultJSRef = JSObjectCallAsFunction(contextJSRef, (JSObjectRef)applyJSRef, (JSObjectRef)methodJSRef, 2, args, &errorJSRef);
            JSStringRelease(argsJSStringRef);
        }
    }

    if (errorJSRef != nullptr)
    {
        Exception^ exc = JSCoreMarshal::JSErrorToException(ctx, errorJSRef);
        Logger::Error(L"ExecuteJSCall", exc);
    }

    JSStringRef str = JSValueCreateJSONString(ctx, resultJSRef, 0, nullptr);
    String^ jsonString = nullptr;

    if (str != nullptr)
    {
        jsonString = JSCoreMarshal::JSStringToString(str);
        JSStringRelease(str);
    }

    return jsonString;
}

void ReactBridge::InjectJSONText(String^ objectName, String^ json)
{
    JSStringRef execJSString = JSCoreMarshal::StringToJSString(json);
    JSValueRef valueToInject = JSValueMakeFromJSONString(ctx, execJSString);
    JSStringRelease(execJSString);

    if (valueToInject == nullptr) {
        Logger::Error("InjectJSONText - invalid JSON?");
        return;
    }

    JSValueRef exception = nullptr;
    JSObjectRef globalObject = JSContextGetGlobalObject(ctx);
    JSStringRef JSName = JSCoreMarshal::StringToJSString(objectName);
    JSObjectSetProperty(ctx, globalObject, JSName, valueToInject, kJSPropertyAttributeNone, &exception);
    JSStringRelease(JSName);

    if (exception != nullptr) {
        Exception^ exc = JSCoreMarshal::JSErrorToException(ctx, exception);
        Logger::Error(L"InjectJSONText - could not set global", exc);
    }
}

void ReactBridge::AddNativeHook(String^ name, JSObjectCallAsFunctionCallback hook)
{
    JSObjectRef globalObject = JSContextGetGlobalObject(ctx);

    JSStringRef JSName = JSCoreMarshal::StringToJSString(name);
    JSValueRef cb = JSObjectMakeFunctionWithCallback(ctx, JSName, hook);
    JSObjectSetProperty(ctx, globalObject, JSName, cb, kJSPropertyAttributeNone, NULL);
    JSStringRelease(JSName);
}
