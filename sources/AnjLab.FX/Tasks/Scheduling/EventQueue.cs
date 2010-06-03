using System;
using System.Collections.Generic;
using System.Threading;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Tasks.Scheduling
{
    public class EventQueue
    {
        private readonly List<Pair<ITrigger, DateTime>> _innerQueue;

        public event EventHandler<EventArgs<string>> ExceptionHandler;
        public event EventHandler<EventArgs<string>> EventOccurs;
        private readonly object _syncObj = new object();
        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        Thread _thread;

        protected void OnBeginEventOccurs(string tag)
        {
            if (EventOccurs != null)
                EventOccurs.BeginInvoke(this, EventArg.New(tag), null, null);
        }

        public EventQueue()
        {
            _innerQueue = new List<Pair<ITrigger, DateTime>>();
        }

        public void Register(ITrigger trigger)
        {
            Guard.ArgumentNotNull("trigger", trigger);
            DateTime date = DateTime.Now;
            
            DateTime? time = trigger.GetNextTriggerTime(date);
            
            if (time == null)
                return;
            
            lock (_syncObj)
            {
                var pair = new Pair<ITrigger, DateTime>(trigger, time.Value);

                if (_innerQueue.Count == 0)
                {
                    _innerQueue.Add(pair);
                    if (_started)
                        _waitHandle.Set();
                    return;
                }

                for (int i = 0; i < _innerQueue.Count; i++)
                {
                    if (_innerQueue[i].B < pair.B)
                        continue;

                    _innerQueue.Insert(i, pair);
                    if (i == 0 && _started)
                    {
                        _waitHandle.Set();
                    }
                    return;
                }
                _innerQueue.Add(pair);
            }
        }

        public void Clear()
        {
            lock (_syncObj)
            {
                _innerQueue.Clear();
                if (_started)
                    _waitHandle.Set();
            }
        }

        public void Start(bool background)
        {
            _started = true;
            _thread = new Thread(WorkingThread) {IsBackground = background};
            _thread.Start();
        }

        public void Stop()
        {
            _started = false;
            _waitHandle.Set();
        }

        private bool _started;

        public void WorkingThread()
        {
            try
            {
                while (_started)
                {
                    if (_innerQueue.Count == 0)
                    {
                        _waitHandle.WaitOne();
                    }
                    else
                    {
                        Pair<ITrigger, DateTime> next;

                        lock (_syncObj)
                        {
                            next = _innerQueue[0];
                        }

                        TimeSpan wait = next.B - DateTime.Now;
                        bool eventOccured = true;
                        if (wait.Ticks > 0)
                           eventOccured = !_waitHandle.WaitOne(wait, false);

                        if (eventOccured)
                        {
                            lock (_syncObj)
                            {
                                _innerQueue.Remove(next);
                            }
                            
                            OnBeginEventOccurs(next.A.Tag);

                            Register(next.A);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                OnException(e);
            }
        }

        private void OnException(Exception exception)
        {
            Console.WriteLine(exception);

            if (ExceptionHandler != null)
            {
                ExceptionHandler(this, new EventArgs<string>(exception.ToString()));
            }
        }
    }
}
