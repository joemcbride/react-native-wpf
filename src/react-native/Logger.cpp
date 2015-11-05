#include "stdafx.h"
#include "Logger.h"

Logger::Logger()
{
}

void Logger::WriteLog(LogLevel level, String^ message)
{
    String^ levelName = Enum::GetName(LogLevel::typeid, (Object^)level);
    Console::WriteLine("[{0}] [{1}] {2}", DateTime::Now.ToString(L"HH:mm:ss"), levelName, message);
}

void Logger::Log(String^ message)
{
    Logger:WriteLog(LogLevel::Log, message);
}

void Logger::Error(String^ message)
{
    Logger:WriteLog(LogLevel::Error, message);
}

void Logger::Error(String^ message, Exception^ exc)
{
    String^ levelName = Enum::GetName(LogLevel::typeid, (Object^)LogLevel::Error);
    String^ msg = !String::IsNullOrEmpty(message) ? message + L" " : L"";
    Console::WriteLine("[{0}] [{1}] {2}{3}", DateTime::Now.ToString(L"HH:mm:ss"), levelName, msg, exc);
}

void Logger::Error(Exception^ exc)
{
    Logger::Error(L"", exc);
}
