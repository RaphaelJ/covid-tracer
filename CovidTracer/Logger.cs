using System;

namespace CovidTracer
{
    public static class Logger
    {
        public enum MessageType { Error, Info, Warning };

        public class MessageArgs : EventArgs
        {
            public readonly MessageType Type;
            public readonly string Message;

            public MessageArgs(MessageType type_, string message_)
            {
                Type = type_;
                Message = message_;
            }
        }

        public delegate void NewMessageHandler(MessageArgs message);
        /** Emitted a new nessage is logged. */
        public static event NewMessageHandler NewMessage;

        public static void Error(string message)
        {
            Log(MessageType.Error, message);
        }

        public static void Info(string message)
        {
            Log(MessageType.Info, message);
        }

        public static void Warning(string message)
        {
            Log(MessageType.Warning, message);
        }

        private static void Log(MessageType type_, string message)
        {
            var args = new MessageArgs(type_, message);
            NewMessage?.Invoke(args);

            Console.WriteLine("[CovidTracer] " + message);
        }
    }
}
