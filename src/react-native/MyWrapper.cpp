#include "stdafx.h"
#include "MyWrapper.h"
#include "Logger.h"

using namespace reactnative;

JSValueRef wrapper_nativeLogging(JSContextRef context, JSObjectRef function, JSObjectRef thisObject,
    size_t argumentCount, const JSValueRef arguments[], JSValueRef* exception)
{
    if (argumentCount > 0) {
        JSStringRef messageRef = JSValueToStringCopy(context, arguments[0], NULL);
        if (messageRef == nullptr) {
            return JSValueMakeUndefined(context);
        }

        LogLevel level = LogLevel::Trace;
        String^ message = JSCoreMarshal::JSStringToString(messageRef);
        JSStringRelease(messageRef);

        if (argumentCount > 1) {
            level = (LogLevel)(Int32)JSValueToNumber(context, arguments[1], NULL);
        }

        Logger::WriteLog(level, message);
    }

    return JSValueMakeUndefined(context);
}
