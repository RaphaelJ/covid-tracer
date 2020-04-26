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

using System.Collections.Generic;

using CovidTracer.Models.Time;
using CovidTracer.Services;

namespace CovidTracer.ViewModels
{
    public class DebugViewModel : BaseViewModel
    {
        public class MessageItem
        {
            public string TextColor { get; protected set; }
            public string Text { get; protected set; }

            public MessageItem(Logger.Message message)
            {
                switch (message.Type) {
                case Logger.MessageType.Error:
                    TextColor = "#721c24";
                    break;
                case Logger.MessageType.Info:
                    TextColor = null;
                    break;
                case Logger.MessageType.Warning:
                    TextColor = "#856404";
                    break;
                };

                Text = message.Value;
            }
        }

        TracerService tracerService;

        string tracerKey;
        public string TracerKey
        {
            get { return tracerKey; }
            set { SetProperty(ref tracerKey, value); }
        }

        string currentDayKey;
        public string CurrentDayKey
        {
            get { return currentDayKey; }
            set { SetProperty(ref currentDayKey, value); }
        }

        string currentHourKey;
        public string CurrentHourKey
        {
            get { return currentHourKey; }
            set { SetProperty(ref currentHourKey, value); }
        }

        int contactCount;
        public int ContactCount
        {
            get { return contactCount; }
            set { SetProperty(ref contactCount, value); }
        }

        int caseCount;
        public int CaseCount
        {
            get { return caseCount; }
            set { SetProperty(ref caseCount, value); }
        }

        public readonly Queue<MessageItem> MessageItems =
            new Queue<MessageItem>();

        public delegate void NewMessageItemHandler(MessageItem message);
        public event NewMessageItemHandler NewMessageItem;

        public DebugViewModel(TracerService tracerService_)
        {
            tracerService = tracerService_;

            Title = "Debug";

            var key = tracerService.Key;
            var dayKey = key.DerivateDailyKey(Date.Today);
            var hourKey = dayKey.DerivateHourlyKey(DateHour.Now);

            TracerKey = key.ToHumanReadableString();
            CurrentDayKey = dayKey.ToHumanReadableString();
            CurrentHourKey = hourKey.ToHumanReadableString();

            var stats = tracerService.Contacts.GetStats();
            ContactCount = (int) stats["contacts.Count"];
            CaseCount = (int) stats["cases.Count"];

            // Adds the messages from the backlog.
            lock (Logger.Backlog) {
                foreach (var message in Logger.Backlog) {
                    OnNewMessage(message);
                }

                Logger.NewMessage += OnNewMessage;
            }
        }

        ~DebugViewModel()
        {
            Logger.NewMessage -= OnNewMessage;
        }

        private void OnNewMessage(Logger.Message message)
        {
            var item = new MessageItem(message);
            MessageItems.Enqueue(item);
            NewMessageItem?.Invoke(item);
        }
    }
}