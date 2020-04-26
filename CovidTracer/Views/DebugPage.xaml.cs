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