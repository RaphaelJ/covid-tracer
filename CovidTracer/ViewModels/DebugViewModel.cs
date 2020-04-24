using CovidTracer.Models.Keys;
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

            public MessageItem(Logger.MessageArgs message)
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

                Text = message.Message;
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

            Logger.NewMessage += OnNewMessage;
        }

        ~DebugViewModel()
        {
            Logger.NewMessage -= OnNewMessage;
        }

        private void OnNewMessage(Logger.MessageArgs message)
        {
            NewMessageItem?.Invoke(new MessageItem(message));
        }
    }
}