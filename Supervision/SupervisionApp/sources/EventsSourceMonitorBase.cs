using SupervisionModel;
using AltaLog;
using System;
using System.Timers;

namespace SupervisionApp
{
    public abstract class EventsSourceMonitorBase
    {
        protected MonitorBO MonitorArgs;

        protected EventsSourceMonitorBase(MonitorBO monitorArgs)
        {
            MonitorArgs = monitorArgs;
        }
        public event EventHandler SimpleEvent;
        public abstract void Start();
        protected void OnEvent()
        {
            SimpleEvent?.Invoke(this, new EventArgs());
        }

        private Timer _timer;

        protected void RunTimer()
        {
            //Debug.WriteLine("Запуск timer " + _timerNo);
            AppJournal.Write("Запуск timer ...");

            if (MonitorArgs != null) {
                _timer = new Timer(MonitorArgs.TimerArgs.period) {
                    AutoReset = true,
                    Enabled = true
                };
                _timer.Elapsed += Timer_Elapsed;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Execute();
        }

        protected abstract void Execute();

        public void Close()
        {
                if (_timer != null) {
                    _timer.Stop();
                    _timer.Close();
                    //timer.Dispose();
                }
        }
    }
}
