using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;
using Android.Media.Browse;
using Android.OS;
using Plugin.MediaManager.Abstractions.Enums;
using System.Collections.Generic;

namespace Plugin.MediaManager.AudioCompat
{
    public class AudioPlayerImplementation : IAudioPlayer
    {
        public TimeSpan Buffered
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TimeSpan Duration
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TimeSpan Position
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Dictionary<string, string> RequestHeaders
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public MediaPlayerStatus Status
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFailedEventHandler MediaFailed;
        public event MediaFinishedEventHandler MediaFinished;
        public event PlayingChangedEventHandler PlayingChanged;
        public event StatusChangedEventHandler StatusChanged;

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public Task Play(IMediaFile mediaFile = null)
        {
            throw new NotImplementedException();
        }

        public Task Seek(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}