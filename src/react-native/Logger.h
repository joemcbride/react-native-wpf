#pragma once

using namespace System;

public enum class LogLevel
{
    Trace = 0,
    Log = 1,
    Info = 2,
    Warn = 3,
    Error = 4
};

ref class Logger
{
public:
    Logger();

    static void WriteLog(LogLevel level, String^ message);
    static void Log(String^ message);
    static void Error(String^ message);
    static void Error(String^ message, Exception^ exc);
    static void Error(Exception^ exc);
};

