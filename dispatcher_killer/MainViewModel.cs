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

        public MainViewModel()
        {
            SyntheticKillCommand = new SyntheticKill();
            LikeInRealLifeKillCommand = new LikeInRealLifeKill();
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
    }
}