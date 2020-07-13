using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Timers;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace ApnaBawarchiKhana.Server.Helper
{
    public class InMemorySink : ILogEventSink
    {
        private readonly ITextFormatter _textFormatter = new MessageTemplateTextFormatter("{Timestamp} [{Level}] {Message}{Exception}", CultureInfo.InvariantCulture);

        private Timer timer;

        public static ConcurrentQueue<string> Events { get; } = new ConcurrentQueue<string>();


        public InMemorySink()
        {
            timer =new Timer(300000);
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            var renderSpace = new StringWriter();
            _textFormatter.Format(logEvent, renderSpace);
            Events.Enqueue(renderSpace.ToString());
        }

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Events.Clear();
            }
            catch
            {
            }
         
        }
    }
}
