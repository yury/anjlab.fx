using AnjLab.FX.Patterns.Generic;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Tasks.Scheduling
{
    public class Scheduler<TTask>
        where TTask: ICommand
    {
        private readonly KeyedFactory<string, TTask> _taskFactory;
        private readonly EventQueue _eventQueue = new EventQueue();

        public Scheduler(KeyedFactory<string, TTask> taskFactory)
        {
            Guard.ArgumentNotNull("taskFactory", taskFactory);

            _taskFactory = taskFactory;
            _eventQueue.EventOccurs += eventQueue_EventOccurs;
        }

        public void RegisterTriggers(params ITrigger [] triggers)
        {
            Lst.ForEach(triggers, _eventQueue.Register);
        }

        public void Clear()
        {
            _eventQueue.Clear();
        }

        void eventQueue_EventOccurs(object sender, EventArgs<string> e)
        {
            TTask task = _taskFactory.Create(e.Item);
            StartTask(task);
        }

        protected virtual void StartTask(TTask task)
        {
            task.Execute();
        }

        public void Start()
        {
            _eventQueue.Start();
        }

        public void Stop()
        {
            _eventQueue.Stop();
        }
    }
}
