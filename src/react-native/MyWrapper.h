#pragma once

#include "JSCoreMarshal.h"

JSValueRef wrapper_nativeLogging(JSContextRef ctx, JSObjectRef function, JSObjectRef thisObject,
    size_t argumentCount, const JSValueRef arguments[], JSValueRef* exception);
