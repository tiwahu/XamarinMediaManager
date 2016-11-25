using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Audio;

namespace Plugin.MediaManager.MediaCompat
{
    public class AudioPlayerImplementation : IAudioPlayer
    {
        private MediaControllerCompat mediaController;
        private MediaBrowserCompat mMediaBrowser;
        private ConnectionCallback mConnectionCallback;
        private MediaControllerCallback mMediaControllerCallback;
        private Context context;

        public AudioPlayerImplementation(Context context)
        {
            mMediaControllerCallback = new MediaControllerCallback();

            // Connect a media browser just to get the media session token. There are other ways
            // this can be done, for example by sharing the session token directly.
            mConnectionCallback = new ConnectionCallback(() => { 
                mediaController = new MediaControllerCompat(context, mMediaBrowser.SessionToken);
                mediaController.RegisterCallback(mMediaControllerCallback);
            });
            mMediaBrowser = new MediaBrowserCompat(context, new ComponentName(context, nameof(MusicService)), mConnectionCallback, null);
            mMediaBrowser.Connect();
        }

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

        public Task Play(IMediaFile mediaFile)
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

        private class MediaControllerCallback : MediaControllerCompat.Callback
        {
            public override void OnPlaybackStateChanged(PlaybackStateCompat state)
            {
                base.OnPlaybackStateChanged(state);
            }

            public override void OnMetadataChanged(MediaMetadataCompat metadata)
            {
                base.OnMetadataChanged(metadata);
            }
        }

        private class ConnectionCallback : MediaBrowserCompat.ConnectionCallback
        {
            Action OnConnect;

            public ConnectionCallback(Action onConnect)
            {
                OnConnect = onConnect;
            }

            public ConnectionCallback(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
            {

            }

            public override void OnConnected()
            {
                try
                {
                    OnConnect?.Invoke();
                }
                catch (RemoteException e)
                {
                    
                }
            }
        }
    }
}
