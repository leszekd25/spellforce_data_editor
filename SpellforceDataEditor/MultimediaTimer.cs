﻿// https://stackoverflow.com/questions/24839105/high-resolution-timer-in-c-sharp
// High resolution timer, a replacement to System.Timer
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SpellforceDataEditor
{
    public class MultimediaTimer : IDisposable
    {
        private bool disposed = false;
        private int interval, resolution;
        private UInt32 timerId;

        // Hold the timer callback to prevent garbage collection.
        private readonly MultimediaTimerCallback Callback;

        public MultimediaTimer()
        {
            Callback = new MultimediaTimerCallback(TimerCallbackMethod);
            Resolution = 5;
            Interval = 10;
        }

        ~MultimediaTimer()
        {
            Dispose(false);
        }

        public int Interval
        {
            get
            {
                return interval;
            }
            set
            {
                CheckDisposed();

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                interval = value;
                if (Resolution > Interval)
                {
                    Resolution = value;
                }
            }
        }

        // Note minimum resolution is 0, meaning highest possible resolution.
        public int Resolution
        {
            get
            {
                return resolution;
            }
            set
            {
                CheckDisposed();

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                resolution = value;
            }
        }

        public bool IsRunning
        {
            get { return timerId != 0; }
        }

        public void Start()
        {
            CheckDisposed();

            if (IsRunning)
            {
                throw new InvalidOperationException("Timer is already running");
            }

            // Event type = 0, one off event
            // Event type = 1, periodic event
            UInt32 userCtx = 0;
            timerId = NativeMethods.TimeSetEvent((uint)Interval, (uint)Resolution, Callback, ref userCtx, 1);
            if (timerId == 0)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
        }

        public void Stop()
        {
            CheckDisposed();

            if (!IsRunning)
            {
                throw new InvalidOperationException("Timer has not been started");
            }

            StopInternal();
        }

        private void StopInternal()
        {
            NativeMethods.TimeKillEvent(timerId);
            timerId = 0;
        }

        public event EventHandler Elapsed;

        public void Dispose()
        {
            Dispose(true);
        }

        private void TimerCallbackMethod(uint id, uint msg, ref uint userCtx, uint rsv1, uint rsv2)
        {
            Elapsed?.Invoke(this, EventArgs.Empty);
        }

        private void CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("MultimediaTimer");
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            if (IsRunning)
            {
                StopInternal();
            }

            if (disposing)
            {
                Elapsed = null;
                GC.SuppressFinalize(this);
            }
        }
    }

    internal delegate void MultimediaTimerCallback(UInt32 id, UInt32 msg, ref UInt32 userCtx, UInt32 rsv1, UInt32 rsv2);
    internal static class NativeMethods
    {
        [DllImport("winmm.dll", SetLastError = true, EntryPoint = "timeSetEvent")]
        internal static extern UInt32 TimeSetEvent(UInt32 msDelay, UInt32 msResolution, MultimediaTimerCallback callback, ref UInt32 userCtx, UInt32 eventType);

        [DllImport("winmm.dll", SetLastError = true, EntryPoint = "timeKillEvent")]
        internal static extern void TimeKillEvent(UInt32 uTimerId);
    }
}
