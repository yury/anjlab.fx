using System;

namespace AnjLab.FX.Tasks.Scheduling
{
    public interface ITrigger
    {
        DateTime ? GetNextTriggerTime(DateTime currentTime);
        string Tag {get;}
    }
}
