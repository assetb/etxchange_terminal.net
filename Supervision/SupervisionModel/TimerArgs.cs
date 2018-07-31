namespace SupervisionModel
{
    public class TimerArgs
    {
        //public TimerCallback callBack;
        //public object state;
        public int dueTime = 0;
        public int period;


        //public TimerBO(TimerCallback callBack, object state, int dueTime, int period)

        /// <summary>
        /// Arguments for parallel process
        /// </summary>
        /// <param name="dueTime">miliseconds over which process will start</param>
        /// <param name="period">interval before new invoke</param>
        public TimerArgs(int dueTime, int period)
        {
            //this.callBack = callBack;
            //this.state = state;
            this.dueTime = dueTime;
            this.period = period;
        }


        //public TimerBO(TimerCallback callBack, object state, int dueTime)
        public TimerArgs(int dueTime)
        {
            //this.callBack = callBack;
            //this.state = state;
            this.dueTime = dueTime;
        }


        //Timer timer;
        //public Timer Timer {
        //    get { if (timer == null) timer = new Timer(callBack, state, dueTime, period); return timer; }
        //    set { if (value != timer) timer = value; }
        //}
    }
}
