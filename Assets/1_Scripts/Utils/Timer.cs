using System;
using UnityEngine;

//An Abstact class for a timer system - allows other classes to create and manage their own timers
namespace Utilities
{
    public abstract class Timer 
    {
        protected float _intialTime;
        protected float Time { get; set; }
        public bool IsTimerRunning { get; protected set; }

        public float Progress => Time / _intialTime;

        public float InvertedProgress => 1 - Progress;

        public Action OnTimerStart = delegate { };
        public Action OnTimerEnd = delegate { };

        //Constructor
        protected Timer(float value)
        {
            _intialTime = value;
            IsTimerRunning = false;

        }

        public void StartTimer()
        {
            Time = _intialTime;
            if (!IsTimerRunning)
            {
                IsTimerRunning = true;
                OnTimerStart.Invoke();
            }
        }

        public void StopTimer()
        {
            if (IsTimerRunning)
            {
                IsTimerRunning = false;
                OnTimerEnd.Invoke();
            }
        }

        public void ResumeTimer() => IsTimerRunning = true;

        public void PauseTimer() => IsTimerRunning = false;

        //We need to pass the deltaTime to the tick method and run out own update loop
        public abstract void Tick(float deltaTime);

    }

    //countdownTimer
        public class CountdownTimer : Timer
        {

            //CountdownTimer needs to be created with a value - this will set initial time from the inherited class
            public CountdownTimer(float value) : base(value) { }

            public override void Tick(float deltaTime)
            {
                if (IsTimerRunning && Time > 0)
                {
                    Time -= deltaTime;
                }

                if (IsTimerRunning && Time <= 0)
                {
                    StopTimer();
                }
            }

            public bool IsFinished() => Time <= 0;

            public void Reset() => Time = _intialTime;

            public void ResetWithNewTime(float newTime)
            {
                _intialTime = newTime;
                Reset();
            }

        }

        //StopwatchTimer
        public class StopwatchTimer : Timer
        {
            public StopwatchTimer() : base(0) { }

            public override void Tick(float deltaTime)
            {
                if (IsTimerRunning)
                {
                    Time += deltaTime;
                }
            }

            public void Reset() => Time = 0;

            public float GetTime() => Time;

        }
}

