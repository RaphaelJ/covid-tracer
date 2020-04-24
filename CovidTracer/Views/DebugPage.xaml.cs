using System.ComponentModel;

using Xamarin.Essentials;
using Xamarin.Forms;

using CovidTracer.Services;
using CovidTracer.ViewModels;

namespace CovidTracer.Views
{
    [DesignTimeVisible(false)]
    public partial class DebugPage : ContentPage
    {
        TracerService tracerService;

        public DebugPage(TracerService tracerService_)
        {
            tracerService = tracerService_;

            var vm = new DebugViewModel(tracerService);
            BindingContext = vm;

            InitializeComponent();

            foreach (var item in vm.MessageItems) {
                OnNewMessageItem(item);
            }

            vm.NewMessageItem += OnNewMessageItem;
        }

        ~DebugPage()
        {
            ((DebugViewModel)BindingContext).NewMessageItem -= OnNewMessageItem;
        }

        /** Adds new messages on top. */
        private void OnNewMessageItem(DebugViewModel.MessageItem msg)
        {
            MainThread.BeginInvokeOnMainThread(() => {
                var label = new Label();
                label.Margin = 10;
                label.Text = msg.Text;

                if (msg.TextColor != null) {
                    label.TextColor = Color.FromHex(msg.TextColor);
                }

                var cell = new ViewCell();
                cell.View = label;
                Messages.Insert(0, cell);
            });
        }
    }
}