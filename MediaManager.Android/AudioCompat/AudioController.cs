using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;

namespace Plugin.MediaManager.AudioCompat
{
    public class AudioController : IPlaybackController
    {
        public MediaControllerCompat mediaController;
        private MediaBrowserCompat mediaBrowser;
        private ConnectionCallback mConnectionCallback;
        private MediaControllerCallback mMediaControllerCallback;

        public AudioController()
        {
            mMediaControllerCallback = new MediaControllerCallback();

            // Connect a media browser just to get the media session token. There are other ways
            // this can be done, for example by sharing the session token directly.
            mConnectionCallback = new ConnectionCallback(() =>
            {
                mediaController = new MediaControllerCompat(Application.Context, mediaBrowser.SessionToken);
                mediaController.RegisterCallback(mMediaControllerCallback);
            });
            mediaBrowser = new MediaBrowserCompat(Application.Context, new ComponentName(Application.Context, nameof(AudioService)), mConnectionCallback, null);
            mediaBrowser.Connect();
        }

        public async Task Pause()
        {
            mediaController.GetTransportControls().Pause();
        }

        public async Task Play()
        {
            mediaController.GetTransportControls().Play();
        }

        public async Task PlayNext()
        {
            mediaController.GetTransportControls().SkipToNext();
        }

        public Task PlayPause()
        {
            throw new NotImplementedException();
        }

        public async Task PlayPrevious()
        {
            mediaController.GetTransportControls().SkipToPrevious();
        }

        public Task PlayPreviousOrSeekToStart()
        {
            throw new NotImplementedException();
        }

        public Task SeekTo(double seconds)
        {
            throw new NotImplementedException();
        }

        public Task SeekToStart()
        {
            throw new NotImplementedException();
        }

        public Task StepBackward()
        {
            throw new NotImplementedException();
        }

        public Task StepForward()
        {
            throw new NotImplementedException();
        }

        public async Task Stop()
        {
            mediaController.GetTransportControls().Stop();
        }

        public void ToggleRepeat()
        {
            throw new NotImplementedException();
        }

        public void ToggleShuffle()
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

            public override void OnSessionEvent(string @event, Bundle extras)
            {
                base.OnSessionEvent(@event, extras);
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
