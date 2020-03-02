// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL was not distributed
// with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright 2018-2020 Artem Yamshanov, me [at] anticode.ninja

namespace DispatcherKiller
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    public class MainViewModel
    {
        public ICommand SyntheticKillCommand { get; }
        public ICommand LikeInRealLifeKillCommand { get; }
        public ICommand SyntheticKill2Command { get; }
        public ICommand LikeInRealLifeKill2Command { get; }

        public MainViewModel()
        {
            SyntheticKillCommand = new SyntheticKill();
            LikeInRealLifeKillCommand = new LikeInRealLifeKill();
            SyntheticKill2Command = new SyntheticKill2();
            LikeInRealLifeKill2Command = new LikeInRealLifeKill2();
        }

        // Fun Fact: it is a bad idea to write wrappers around sync/async invokers with the same priority
        internal static void InvokeSync(Action callback)
        {
            Application.Current.Dispatcher.Invoke(callback, DispatcherPriority.Normal);
        }

        internal static void InvokeAsync(Action callback)
        {
            Application.Current.Dispatcher.InvokeAsync(callback, DispatcherPriority.Normal);
        }

        private class SyntheticKill : ICommand
        {
            public bool CanExecute(object parameter) => true;

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                void Killer()
                {
                    InvokeAsync(Killer);
                    InvokeSync(() => MessageBox.Show("It will be never invoked!"));
                }

                Killer();
            }
        }

        private class LikeInRealLifeKill : ICommand
        {
            public bool CanExecute(object parameter) => true;

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                var syncEvent1 = new AutoResetEvent(true);
                var syncEvent2 = new AutoResetEvent(false);

                void Worker()
                {
                    syncEvent1.Set();
                    syncEvent2.WaitOne();
                    InvokeSync(() => MessageBox.Show("It will be never invoked!"));
                }

                void LikeVeryOverLoadQueue(object state)
                {
                    for (;;)
                    {
                        syncEvent1.WaitOne();
                        InvokeAsync(Worker);
                        syncEvent2.Set();
                    }
                }

                ThreadPool.QueueUserWorkItem(LikeVeryOverLoadQueue, null);
            }
        }

        private class SyntheticKill2 : ICommand
        {
            public bool CanExecute(object parameter) => true;

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                using (Application.Current.Dispatcher.DisableProcessing())
                    InvokeSync(() => MessageBox.Show("It will be never invoked!"));
            }
        }

        private class LikeInRealLifeKill2 : ICommand
        {
            public bool CanExecute(object parameter) => true;

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                ThirdPartyElement.Callback = () => InvokeSync(() => MessageBox.Show("It will be never invoked!"));
                Application.Current.MainWindow.Width += 1;
            }
        }
    }
}
