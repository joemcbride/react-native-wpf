/*
* Copyright(c) 2009, Peter Nelson(charn.opcode@gmail.com)
* All rights reserved.
* 
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met :
* 
* 1. Redistributions of source code must retain the above copyright notice,
*    this list of conditions and the following disclaimer.
* 2. Redistributions in binary form must reproduce the above copyright notice,
*    this list of conditions and the following disclaimer in the documentation
*    and / or other materials provided with the distribution.
* 
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
* ARE DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
* LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
* CONSEQUENTIAL DAMAGES(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
* SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
* CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR OTHERWISE)
* ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
* POSSIBILITY OF SUCH DAMAGE.
*/

#include "stdafx.h"
#include "string.h"
#include "JSCoreMarshal.h"

using namespace reactnative;

JSStringRef JSCoreMarshal::StringToJSString(String ^ value)
{
    if (value != nullptr)
    {
        JSChar * chars = (JSChar *)Marshal::StringToBSTR(value).ToPointer();
        JSStringRef str = JSStringCreateWithCharacters(chars, wcslen(chars));
        Marshal::FreeBSTR(IntPtr(chars));

        // TODO: find out if we should return JSStringRetain(str) instead
        return str;
    }
    else
    {
        return NULL;
    }
}

String ^ JSCoreMarshal::JSStringToString(JSStringRef string)
{
    size_t len = JSStringGetLength(string);

    if (len == 0)
    {
        return nullptr;
    }

    JSChar * cStr = (JSChar *)JSStringGetCharactersPtr(string);

    // TODO: does this copy the string, or point to it?
    // Do we need to clean up afterwards?
    return Marshal::PtrToStringAuto(IntPtr((void *)cStr), len);
}

String^ JSCoreMarshal::JSValueToJSONString(JSContextRef context, JSValueRef value, int indent)
{
    JSStringRef string = JSValueCreateJSONString(context, value, indent, NULL);
    return JSCoreMarshal::JSStringToString(string);
}

String^ JSCoreMarshal::JSValueToString(JSContextRef context, JSValueRef value)
{
    JSStringRef string = JSValueToStringCopy(context, value, NULL);
    return JSCoreMarshal::JSStringToString(string);
}

Exception^ JSCoreMarshal::JSErrorToException(JSContextRef context, JSValueRef jsError)
{
    String^ errorMessage = jsError ? JSCoreMarshal::JSValueToString(context, jsError) : L"unknown JS error";
    String^ details = jsError ? JSCoreMarshal::JSValueToJSONString(context, jsError, 2) : L"no details";

    return gcnew Exception(errorMessage + L"\n" + details);
}
