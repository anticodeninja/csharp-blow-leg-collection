namespace IncorrectWaysToDie
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    public class MainViewModel
    {
        public ICommand NinjaWayToCrashCommand { get; }
        public ICommand HideMemoryWay1ToCrashCommand { get; }
        public ICommand HideMemoryWay2ToCrashCommand { get; }
        public ICommand MoreCrashesWayToCrashCommand { get; }
        public ICommand HideFromWerWayToCrashCommand { get; }
        public ICommand CorrectWayToCrashCommand { get; }

        public MainViewModel()
        {
            NinjaWayToCrashCommand = new NinjaWayToCrash();
            HideMemoryWay1ToCrashCommand = new HideMemoryWay1ToCrash();
            HideMemoryWay2ToCrashCommand = new HideMemoryWay2ToCrash();
            MoreCrashesWayToCrashCommand = new MoreCrashesWayToCrash();
            HideFromWerWayToCrashCommand = new HideFromWerWayToCrash();
            CorrectWayToCrashCommand = new CorrectWayToCrash();
        }

        private class NinjaWayToCrash : ICommand
        {
            public bool CanExecute(object parameter) => true;

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                AppDomain.CurrentDomain.UnhandledException += (sender, args)
                    => MessageBox.Show("Really?! Is something bad happen?!");
                Dispatcher.CurrentDispatcher.UnhandledException += (sender, args)
                    => MessageBox.Show("Really?! Is something bad happen?!");
                // Procdump also does not catch this exception

                void crash() => crash();
                crash();
            }
        }

        private class HideMemoryWay1ToCrash : ICommand
        {
            public bool CanExecute(object parameter) => true;

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    MessageBox.Show(args.ExceptionObject.ToString(), "Memory is hidden from you...");
                    Environment.Exit(0);
                };
                throw new Exception("Unhandled exception");
            }
        }

        private class HideMemoryWay2ToCrash : ICommand
        {
            public bool CanExecute(object parameter) => true;

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                Dispatcher.CurrentDispatcher.UnhandledException += (sender, args) =>
                {
                    args.Handled = true;
                    MessageBox.Show(args.Exception.ToString(), "Memory is hidden from you...");
                    Application.Current.Shutdown();
                };
                throw new Exception("Unhandled exception");
            }
        }

        private class MoreCrashesWayToCrash : ICommand
        {
            public bool CanExecute(object parameter) => true;

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                Dispatcher.CurrentDispatcher.UnhandledException += (sender, args) =>
                {
                    MessageBox.Show(args.Exception.ToString(), "Stack trace is replaced on incorrect for you...");
                    throw new Exception("Ololo"); // in real life it can be access to already broken resourse
                };
                throw new Exception("Unhandled exception");
            }
        }

        private class HideFromWerWayToCrash : ICommand
        {
            const uint SEM_NOGPFAULTERRORBOX = 0x0002;

            public bool CanExecute(object parameter) => true;

            public event EventHandler CanExecuteChanged;

            [DllImport("kernel32.dll")]
            internal static extern uint SetErrorMode(uint mode);

            public void Execute(object parameter)
            {
                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    MessageBox.Show(args.ExceptionObject.ToString(), "Crash will is hidden from WER...");
                };

                SetErrorMode(SetErrorMode(0) | SEM_NOGPFAULTERRORBOX);
                throw new Exception("Unhandled exception");
            }
        }

        private class CorrectWayToCrash : ICommand
        {
            public bool CanExecute(object parameter) => true;

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                MessageBox.Show("But seems there is not simple correct way", "Sorry...");
            }
        }
    }
}