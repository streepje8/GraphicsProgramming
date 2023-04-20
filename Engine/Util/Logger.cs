using System.Diagnostics;

namespace Striped.Engine.Util;

public class LoggingContext
{
    public string stackTrace;
    public StackFrame currentStackFrame;

    public LoggingContext(int skipCount = 1)
    {
        currentStackFrame = new StackFrame(skipCount, true);
        stackTrace = Environment.StackTrace;
    }

    public override string ToString()
    {
        return "(" + currentStackFrame.GetMethod()?.Name + "@" + Path.GetFileName(currentStackFrame.GetFileName()) + ":" + currentStackFrame.GetFileLineNumber() + ") ";
    }
}

public static class Logger
{
    public static LoggingContext GatherContext() => new LoggingContext(2);

    public static void Log(object message, LoggingContext? context = null) => Info(message,context);
    public static void Info(object message, LoggingContext? context = null) => Log("INFO", Console.Out, message.ToString() ?? "null", context);
    public static void Warn(object message, LoggingContext? context = null) => Log("WARNING", Console.Out, message.ToString() ?? "null", context);
    public static void Err(object message, LoggingContext? context = null) => Log("ERROR", Console.Error, message.ToString() ?? "null", context);
    
    public static void Info(string message, LoggingContext? context = null) => Log("INFO", Console.Out, message, context);
    public static void Warn(string message, LoggingContext? context = null) => Log("WARNING", Console.Out, message, context);
    public static void Err(string message, LoggingContext? context = null) => Log("ERROR", Console.Error, message, context);
    public static void Except(Exception e, LoggingContext? context = null) => Log("ERROR", Console.Error, e.ToString(), context);
    private static void Log(string tag, TextWriter output, string message, LoggingContext? loggingContext)
    {
        DateTime currentTime = DateTime.Now;
        output.WriteLine("[" + tag + "/" + currentTime.ToString("HH:mm:ss") + "] " + (loggingContext != null ? loggingContext.ToString() : "") + message);
    }
}