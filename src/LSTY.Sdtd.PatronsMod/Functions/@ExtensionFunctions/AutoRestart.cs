using FluentScheduler;
using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod.Functions
{
    public class AutoRestart : FunctionBase, ISubFunction
    {
        private string _scheduleName;
        private int _hours = 3;
        private int _minutes = 0;

        [ConfigNode(XmlNodeType.Attribute)]
        public int Hours 
        { 
            get => _hours; 
            set 
            { 
                if(_hours != value)
                {
                    if(value < 0 || value > 24)
                    {
                        throw new Exception("The hours must be greater than 0 and less than 24");
                    }

                    _hours = value;
                    AddJob();
                }
            } 
        }

        [ConfigNode(XmlNodeType.Attribute)]
        public int Minutes 
        { 
            get => _minutes; 
            set
            {
                if(_minutes != value)
                {
                    if (value < 0 || value > 60)
                    {
                        throw new Exception("The minutes must be greater than 0 and less than 60");
                    }

                    _minutes = value;
                    AddJob();
                }
            }
        }

        [ConfigNode(XmlNodeType.Attribute)]
        public int Delay { get; set; } = 5;

        [ConfigNode(XmlNodeType.Attribute)]
        public string GlobalTips { get; set; } = "[00FF00]The server will restart in {delay} seconds";

        private int _timeLeft = -1;

        public AutoRestart()
        {
            JobManager.JobException += info => CustomLogger.Error("An error just happened with a scheduled job: " + info.Exception);
            JobManager.Initialize();
            AddJob();
            JobManager.Start();

            availableVariables.Add("{delay}");
        }

        private void PrepareRestart()
        {
            JobManager.AddJob(() => 
            {
                lock (this)
                {
                    if (_timeLeft == -1)
                    {
                        _timeLeft = Delay < 0 ? 0 : Delay;
                        ModHelper.SendGlobalMessage(FormatCmd(GlobalTips, _timeLeft));
                        SdtdConsole.Instance.ExecuteSync("sa", null);
                        PrepareRestart();
                    }
                    else if (_timeLeft == 0)
                    {
                        _timeLeft = -1;
                        ModHelper.RestartServer();
                    }
                    else
                    {
                        --_timeLeft;
                        ModHelper.SendGlobalMessage(FormatCmd(GlobalTips, _timeLeft));
                        PrepareRestart();
                    }
                }
            }, (schedule) =>
            {
                schedule.ToRunOnceIn(1).Seconds();
            });
        }

        private void Schedule(Schedule schedule)
        {
            _scheduleName = nameof(AutoRestart);
            schedule.WithName(_scheduleName).ToRunEvery(1).Days().At(Hours, Minutes);
            if (IsEnabled == false)
            {
                schedule.Disable();
            }
        }

        private void AddJob()
        {
            if(_scheduleName != null)
            {
                JobManager.RemoveJob(_scheduleName);
            }

            JobManager.AddJob(PrepareRestart, Schedule);
        }

        private string FormatCmd(string message, int delay)
        {
            return message.Replace("{delay}", delay.ToString());
        }

        protected override void DisableFunction()
        {
            JobManager.GetSchedule(_scheduleName).Disable();
        }

        protected override bool EnableFunctionNonePlayer()
        {
            JobManager.GetSchedule(_scheduleName).Enable();
            return true;
        }
    }
}
