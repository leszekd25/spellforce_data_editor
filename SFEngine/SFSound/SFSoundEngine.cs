﻿using NAudio.Wave;
using System;
using System.IO;

namespace SFEngine.SFSound
{
    public class SFSoundEngine : IDisposable
    {
        private MemoryStream sound_data = null;
        private WaveOutEvent player = null;
        private WaveStream sound_loader = null;
        public bool loaded { get; private set; } = false;

        public int LoadSoundMP3(StreamResource sound)
        {
            if (loaded)
            {
                UnloadSound();
            }

            sound_data = new MemoryStream(sound.sound_data, sound.offset, sound.sound_data.Length - sound.offset);
            sound_data.Seek(0, SeekOrigin.Begin);
            sound_loader = new Mp3FileReader(sound_data);
            player = new WaveOutEvent();
            player.Init(sound_loader);
            loaded = true;

            return 0;
        }

        public int LoadSoundWAV(StreamResource sound)
        {
            if (loaded)
            {
                UnloadSound();
            }

            sound_data = new MemoryStream(sound.sound_data, sound.offset, sound.sound_data.Length - sound.offset);
            sound_data.Seek(0, SeekOrigin.Begin);
            sound_loader = new WaveFileReader(sound_data);
            player = new WaveOutEvent();
            player.Init(sound_loader);
            loaded = true;

            return 0;
        }

        public int PlaySound()
        {
            if (!loaded)
            {
                return -1;
            }

            player.Play();
            return 0;
        }

        public int PauseSound()
        {
            if (!loaded)
            {
                return -1;
            }

            player.Pause();
            return 0;
        }

        public void SetSound(double miliseconds)
        {
            if (!loaded)
            {
                return;
            }

            miliseconds = Math.Min(sound_loader.TotalTime.TotalMilliseconds, Math.Max(0, miliseconds));
            sound_loader.CurrentTime = TimeSpan.FromMilliseconds(miliseconds);
        }

        public double GetSoundDuration()
        {
            if (!loaded)
            {
                return 10000000000;
            }

            return sound_loader.TotalTime.TotalMilliseconds;
        }

        public double GetSoundPosition()
        {
            if (!loaded)
            {
                return 0;
            }

            return sound_loader.CurrentTime.TotalMilliseconds;
        }

        public PlaybackState GetSoundStatus()
        {
            if (!loaded)
            {
                return PlaybackState.Stopped;
            }

            return player.PlaybackState;
        }

        public int UnloadSound()
        {
            if (!loaded)
            {
                return -1;
            }

            try
            {
                player.Stop();
                player.Dispose();
                sound_loader.Close();
                sound_loader.Dispose();
                sound_data.Close();
                sound_data.Dispose();
                loaded = false;
            }
            catch (Exception)
            {
                return -2;
            }

            GC.Collect(2, GCCollectionMode.Forced, false);

            return 0;
        }

        public void Dispose()
        {
            UnloadSound();
        }
    }
}
