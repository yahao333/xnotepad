using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotesApp.Helpers
{
    public static class Logger
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        static Logger()
        {
#if DEBUG
        // 在 Debug 模式下,配置Trace将输出写入调试窗口
        //Trace.Listeners.Add(new DefaultTraceListener());
#else
            // 在 Release 模式下,配置Trace将输出写入文件
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"{DateTime.Now:yyyyMMdd_HHmmss}.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
            var textWriterTraceListener = new TextWriterTraceListener(logFilePath);
            Trace.Listeners.Add(textWriterTraceListener);
            Trace.AutoFlush = true; // 确保日志立即被写入文件
#endif
        }


        public static void Write(string message, string timeZone = "UTC+08:00")
        {
            _semaphore.Wait();
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                Trace.WriteLine($"{timestamp}   {message} (Time zone: {timeZone})");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
