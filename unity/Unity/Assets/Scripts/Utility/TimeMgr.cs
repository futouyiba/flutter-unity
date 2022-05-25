using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ET
{

    public class Timer
    {
        public long tillTime;
        public Action dlg;

        public Timer(long tillTime, Action dlg)
        {
            this.tillTime = tillTime;
            this.dlg = dlg;
        }
    }
    public class TimeMgr : MonoBehaviour
    {
        //
        // [SerializeField] private int dbg_timer_count;
        //
        // [SerializeField] private long current_ts;
        //
        // [SerializeField] private long nearest_timer;
        //instances
        private static TimeMgr _instance;

        public static TimeMgr instance
        {
            get
            {
                if (!_instance)
                {
                    Debug.LogError("time manager instance does not exist");
                    return null;
                }

                return _instance;
            }
        }
        
        //dicts
        private Dictionary<int, Timer> timers;

        private int _cur_id;

        private int get_id
        {
            get
            {
                return _cur_id++;
            }
        }

        private long minTime;

        private void Awake()
        {
            this.timers = new Dictionary<int, Timer>();
            _cur_id = 0;
            minTime = -1;
        }


        // Start is called before the first frame update
        void Start()
        {
            if (_instance)
            {
                Debug.LogError($"time manager already exists");
                this.Dispose();
                return;
            }
            _instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            // //debug==
            // dbg_timer_count = timers.Count;
            // current_ts = Dt2Ts(DateTime.Now);
            // long neareast_tilltime = -1;
            // foreach (var timer in timers)
            // {
            //     if (neareast_tilltime == -1) neareast_tilltime = timer.Value.tillTime;
            //     else if (timer.Value.tillTime < neareast_tilltime) neareast_tilltime = timer.Value.tillTime;
            // }
            // nearest_timer = neareast_tilltime-current_ts;
            
            //debug--
            
            if (this.timers.Count == 0) return;
            var curTs = Dt2Ts(DateTime.Now);
            if (curTs < minTime || minTime == -1) return;

            long changedMinTime = -1;
            Queue<int> timeOutIds = new Queue<int>();
            foreach (var timer in timers)
            {
                if (timer.Value.tillTime > curTs)
                {//更新mintime
                    if(changedMinTime==-1)
                        changedMinTime = timer.Value.tillTime;
                    else
                    {
                        if (timer.Value.tillTime < changedMinTime) 
                            changedMinTime = timer.Value.tillTime;
                    }
                }
                else
                {//过期了，加入timerOuts
                    timeOutIds.Enqueue(timer.Key);
                }
            }

            if (timeOutIds.Count > 0) minTime = changedMinTime;

            while (timeOutIds.Count > 0)
            {
                var timeOut = timeOutIds.Dequeue();
                var timeOutTimer = timers[timeOut];
                timers.Remove(timeOut);
                timeOutTimer.dlg?.Invoke();
            }

        }

        void Dispose()
        {
            this.enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="afterTime">time in milisec</param>
        /// <param name="dlg"></param>
        /// <returns></returns>
        public int AddTimer(int afterTime, Action dlg)
        {
            var curTs = Dt2Ts(DateTime.Now);
            var tillTime = curTs + afterTime;
            var createTimer = new Timer(tillTime, dlg);
            var timerID = get_id;
            timers.Add(timerID,createTimer);
            UpdateMinTime(tillTime);
            Debug.Log($"timer {timerID} added!");
            return timerID;
        }

        public bool RemoveTimer(int timerId)
        {
            var result = timers.TryGetValue(timerId, out Timer timerGot);
            if (!result)
            {
                Debug.LogError($"no id={timerId} timer found");
                return false;
            }

            timers.Remove(timerId);
            if (timerGot.tillTime == minTime)
            {//刷新mintime
                long changedMinTime = -1;
                foreach (var timer in timers)
                {
                    if (timer.Value.tillTime < changedMinTime || changedMinTime == -1)
                    {
                        changedMinTime = timer.Value.tillTime;
                    }
                }

                minTime = changedMinTime;
            }

            return true;
        }

        private bool UpdateMinTime(long newTime)
        {
            bool willUpdate = false;
            if (newTime < Dt2Ts(DateTime.Now))
            {//更新了一个过去的时间，不行
                willUpdate = false;
            }
            else if (newTime < minTime|| minTime==-1)
            {
                willUpdate = true;
            }

            if (willUpdate)
            {
                minTime = newTime;
            }

            return willUpdate;

        }
        
        public static long Dt2Ts(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }


        
    }
}
