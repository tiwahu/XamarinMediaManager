using System;
using System.Linq;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Mediasession;
using Com.Google.Android.Exoplayer2.Source;
using MediaManager.Platforms.Android.Media;

namespace MediaManager.Platforms.Android.Player
{
    public class MediaSessionConnectorPlaybackPreparer : Java.Lang.Object, MediaSessionConnector.IPlaybackPreparer
    {
        protected readonly IExoPlayer _player;
        protected readonly ConcatenatingMediaSource _mediaSource;
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;

        public MediaSessionConnectorPlaybackPreparer(IExoPlayer player, ConcatenatingMediaSource mediaSource)
        {
            _player = player;
            _mediaSource = mediaSource;
        }

        protected MediaSessionConnectorPlaybackPreparer(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public long SupportedPrepareActions =>
                PlaybackStateCompat.ActionPrepare |
                PlaybackStateCompat.ActionPrepareFromMediaId |
                PlaybackStateCompat.ActionPrepareFromSearch |
                PlaybackStateCompat.ActionPrepareFromUri |
                PlaybackStateCompat.ActionPlayFromMediaId |
                PlaybackStateCompat.ActionPlayFromSearch |
                PlaybackStateCompat.ActionPlayFromUri;

        public string[] GetCommands()
        {
            return null;
        }

        public void OnCommand(IPlayer p0, string p1, Bundle p2, ResultReceiver p3)
        {
        }

        public void OnPrepare()
        {
            _mediaSource.Clear();

            var mediaItems = MediaManager.MediaQueue.Select(x => x.ToMediaSource()).ToList();

            _mediaSource.AddMediaSources(mediaItems);

            var windowIndex = System.Math.Max(0, System.Math.Min(MediaManager.MediaQueue.CurrentIndex, mediaItems.Count - 1));
            //var positionMs = (long)(MediaManager.MediaQueue.CurrentPosition?.TotalMilliseconds ?? C.TimeUnset);
            var positionMs = (long)0;

            _player.SeekTo(windowIndex, positionMs);

            _player.Prepare(_mediaSource, !(windowIndex > 0 || positionMs > 0), true);

            //Only in case of Prepare set PlayWhenReady to true because we use this to load in the whole queue
            if(MediaManager.AutoPlay)
                _player.PlayWhenReady = true;
            else
                _player.PlayWhenReady = false;
        }

        public async void OnPrepareFromMediaId(string mediaId, Bundle p1)
        {
            _mediaSource.Clear();
            int windowIndex = 0;

            var index = MediaManager.MediaQueue
                .Select((o, i) => new { Value = o, Index = i })
                .FirstOrDefault(o => o.Value.MediaId == mediaId)?.Index;

            if (index.HasValue)
            {
                windowIndex = index.Value;

                foreach (var mediaItem in MediaManager.MediaQueue)
                {
                    _mediaSource.AddMediaSource(mediaItem.ToMediaSource());
                }
            }
            else
            {
                // lookup playlist for media id...via MediaManager?

                var items = await MediaSession.MediaBrowserService.Instance?.ItemsForMediaId(mediaId);
                if (items != null)
                {
                    MediaManager.MediaQueue.Clear();
                    foreach (var item in items)
                    {
                        MediaManager.MediaQueue.Add(item);
                    }

                    foreach (var mediaItem in MediaManager.MediaQueue)
                    {
                        _mediaSource.AddMediaSource(mediaItem.ToMediaSource());
                    }
                }
            }

            _player.Prepare(_mediaSource);
            _player.SeekTo(windowIndex, 0);
        }

        public void OnPrepareFromSearch(string searchTerm, Bundle p1)
        {
            _mediaSource.Clear();
            foreach (var mediaItem in MediaManager.MediaQueue.Where(x => x.Title == searchTerm))
            {
                _mediaSource.AddMediaSource(mediaItem.ToMediaSource());
            }
            _player.Prepare(_mediaSource);
        }

        public void OnPrepareFromUri(global::Android.Net.Uri mediaUri, Bundle p1)
        {
            _mediaSource.Clear();
            var windowIndex = 0;
            foreach (var mediaItem in MediaManager.MediaQueue)
            {
                var uri = global::Android.Net.Uri.Parse(mediaItem.MediaUri);
                if (uri.Equals(mediaUri))
                    windowIndex = MediaManager.MediaQueue.IndexOf(mediaItem);

                _mediaSource.AddMediaSource(mediaItem.ToMediaSource());
            }
            _player.Prepare(_mediaSource);
            _player.SeekTo(windowIndex, 0);
        }
    }
}
