using System;
using System.Collections.Generic;

namespace CovidTracer
{
    public static class Logger
    {
        // Number of logger messages kept before being discarded.
        public const int BACKLOG_SIZE = 250;

        public enum MessageType { Error, Info, Warning };

        public class Message : EventArgs
        {
            public readonly MessageType Type;
            public readonly string Value;

            public Message(MessageType type_, string value_)
            {
                Type = type_;
                Value = value_;
            }
        }

        public delegate void NewMessageHandler(Message message);
        /** Emitted a new nessage is logged. */
        public static event NewMessageHandler NewMessage;

        public static readonly Queue<Message> Backlog =
            new Queue<Message>();

        public static void Error(string value_)
        {
            Log(MessageType.Error, value_);
        }

        public static void Info(string value_)
        {
            Log(MessageType.Info, value_);
        }

        public static void Warning(string value_)
        {
            Log(MessageType.Warning, value_);
        }

        private static void Log(MessageType type_, string value_)
        {
            var message = new Message(type_, value_);

            lock (Backlog) {
                while (Backlog.Count >= BACKLOG_SIZE) {
                    Backlog.Dequeue();
                }

                Backlog.Enqueue(message);

                Console.WriteLine("[CovidTracer] " + value_);
                NewMessage?.Invoke(message);
            }
        }
    }
}
