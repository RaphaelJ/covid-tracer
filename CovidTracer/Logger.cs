// Copyright 2020 Raphael Javaux
//
// This file is part of CovidTracer.
//
// CovidTracer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// CovidTracer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with CovidTracer. If not, see<https://www.gnu.org/licenses/>.

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
