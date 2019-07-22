﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using MediaManager.Media;
using MediaManager.Platforms.Android;
using MediaManager.Platforms.Android.Media;
using MediaManager.Platforms.Android.MediaSession;
using MediaManager.Platforms.Android.Playback;
using MediaManager.Playback;
using MediaManager.Queue;
using MediaManager.Volume;

[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessWifiState)]
[assembly: UsesPermission(Android.Manifest.Permission.ForegroundService)]
namespace MediaManager
{
    [global::Android.Runtime.Preserve(AllMembers = true)]
    public class MediaManagerImplementation : MediaManagerBase, IMediaManager<SimpleExoPlayer>
    {
        public MediaManagerImplementation()
        {
        }

        private Context _context = Application.Context;
        public virtual Context Context
        {
            get => _context;
            set
            {
                if (SetProperty(ref _context, value))
                    SessionActivityPendingIntent = BuildSessionActivityPendingIntent();
            }
        }

        private Type _mediaBrowserServiceType = null;

        public Type MediaBrowserServiceType
        {
            get => _mediaBrowserServiceType;
            set => this.SetProperty(ref _mediaBrowserServiceType, value);
        }

        private int _notificationIconResource = Resource.Drawable.exo_notification_play;

        public int NotificationIconResource
        {
            get => _notificationIconResource;
            set
            {
                SetProperty(ref _notificationIconResource, value);
                var playerNotificationManager = (NotificationManager as MediaManager.Platforms.Android.Notifications.NotificationManager)?.PlayerNotificationManager;
                playerNotificationManager?.SetSmallIcon(_notificationIconResource);
            }
        }

        private MediaSessionCompat _mediaSession;
        public MediaSessionCompat MediaSession
        {
            get => _mediaSession;
            set => SetProperty(ref _mediaSession, value);
        }

        private MediaControllerCompat _mediaController;
        public MediaControllerCompat MediaController
        {
            get => _mediaController;
            set => SetProperty(ref _mediaController, value);
        }

        private PendingIntent _sessionActivityPendingIntent;
        public virtual PendingIntent SessionActivityPendingIntent
        {
            get
            {
                if (_sessionActivityPendingIntent == null)
                {
                    _sessionActivityPendingIntent = BuildSessionActivityPendingIntent();
                }
                return _sessionActivityPendingIntent;
            }
            set => SetProperty(ref _sessionActivityPendingIntent, value);
        }

        public virtual PendingIntent BuildSessionActivityPendingIntent()
        {
            Intent sessionIntent;
            // Build a PendingIntent that can be used to launch the UI.
            if (Context is Activity activity)
                sessionIntent = new Intent(Context, activity.GetType());
            else
                sessionIntent = Context.PackageManager.GetLaunchIntentForPackage(Context.PackageName);
            return PendingIntent.GetActivity(Context, 0, sessionIntent, 0);
        }

        public override void Init()
        {
            EnsureInit();
        }

        public async Task EnsureInit()
        {
            if (!IsInitialized && !(IsInitialized = await MediaBrowserManager.Init()))
                throw new Exception("Cannot Initialize MediaManager");
        }

        private MediaBrowserManager _mediaBrowserManager;
        public virtual MediaBrowserManager MediaBrowserManager
        {
            get
            {
                if (_mediaBrowserManager == null)
                    _mediaBrowserManager = new MediaBrowserManager(this.MediaBrowserServiceType);
                return _mediaBrowserManager;
            }
            set => SetProperty(ref _mediaBrowserManager, value);
        }

        public override TimeSpan StepSize
        {
            get => base.StepSize;
            set
            {
                base.StepSize = value;
                var playerNotificationManager = (NotificationManager as MediaManager.Platforms.Android.Notifications.NotificationManager)?.PlayerNotificationManager;
                playerNotificationManager?.SetFastForwardIncrementMs((long)value.TotalMilliseconds);
                playerNotificationManager?.SetRewindIncrementMs((long)value.TotalMilliseconds);
            }
        }

        private IMediaPlayer _mediaPlayer;
        public override IMediaPlayer MediaPlayer
        {
            get
            {
                if (_mediaPlayer == null)
                    _mediaPlayer = new AndroidMediaPlayer();
                return _mediaPlayer;
            }
            set => SetProperty(ref _mediaPlayer, value);
        }

        public AndroidMediaPlayer AndroidMediaPlayer => (AndroidMediaPlayer)MediaPlayer;
        public SimpleExoPlayer Player => AndroidMediaPlayer.Player;

        private IVolumeManager _volumeManager;
        public override IVolumeManager VolumeManager
        {
            get
            {
                if (_volumeManager == null)
                    _volumeManager = new VolumeManager();
                return _volumeManager;
            }
            set => SetProperty(ref _volumeManager, value);
        }

        private IMediaExtractor _mediaExtractor;
        public override IMediaExtractor MediaExtractor
        {
            get
            {
                if (_mediaExtractor == null)
                    _mediaExtractor = new MediaExtractor();
                return _mediaExtractor;
            }
            set => SetProperty(ref _mediaExtractor, value);
        }


        private INotificationManager _notificationManager;
        public override INotificationManager NotificationManager
        {
            get
            {
                if (_notificationManager == null)
                    _notificationManager = new MediaManager.Platforms.Android.Notifications.NotificationManager();

                return _notificationManager;
            }
            set => SetProperty(ref _notificationManager, value);
        }

        public override TimeSpan Position => TimeSpan.FromMilliseconds(MediaController?.PlaybackState?.Position ?? 0);

        public override TimeSpan Duration => MediaController?.Metadata?.ToMediaItem()?.Duration ?? TimeSpan.Zero;

        public override float Speed {
            get => MediaController?.PlaybackState?.PlaybackSpeed ?? 0;
            set => Player.PlaybackParameters.Speed = value;
        }

        public override async Task Pause()
        {
            await EnsureInit();

            MediaController.GetTransportControls().Pause();
        }

        public override async Task Play()
        {
            await EnsureInit();

            if (this.IsStopped())
                MediaController.GetTransportControls().Prepare();

            MediaController.GetTransportControls().Play();
        }

        public override async Task<IMediaItem> Play(string uri)
        {
            await EnsureInit();

            var mediaItem = await MediaExtractor.CreateMediaItem(uri);
            await AddMediaItemsToQueue(new List<IMediaItem> { mediaItem }, true);

            MediaController.GetTransportControls().Prepare();
            return mediaItem;
        }

        public override async Task Play(IMediaItem mediaItem)
        {
            await EnsureInit();
            await AddMediaItemsToQueue(new List<IMediaItem> { mediaItem }, true);

            MediaController.GetTransportControls().Prepare();
            return;
        }

        public override async Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items)
        {
            await EnsureInit();

            var mediaItems = new List<IMediaItem>();
            foreach (var uri in items)
            {
                mediaItems.Add(await MediaExtractor.CreateMediaItem(uri));
            }

            await AddMediaItemsToQueue(mediaItems, true);

            await MediaQueue.FirstOrDefault()?.FetchMetaData();
            MediaController.GetTransportControls().Prepare();
            return MediaQueue;
        }

        public override async Task Play(IEnumerable<IMediaItem> items)
        {
            await EnsureInit();

            await AddMediaItemsToQueue(items, true);

            MediaQueue.CurrentIndex = 0;
            MediaQueue.CurrentPosition = TimeSpan.Zero;

            MediaController.GetTransportControls().Prepare();
            return;
        }

        public override async Task Play(IEnumerable<IMediaItem> items, int index)
        {
            await EnsureInit();

            MediaController.GetTransportControls().Stop();

            await AddMediaItemsToQueue(items, true);

            MediaQueue.CurrentIndex = index;
            MediaQueue.CurrentPosition = TimeSpan.Zero;

            MediaController.GetTransportControls().Prepare();
            return;
        }

        public override async Task<IMediaItem> Play(FileInfo file)
        {
            await EnsureInit();

            var mediaItem = await MediaExtractor.CreateMediaItem(file);
            var mediaItemToPlay = await AddMediaItemsToQueue(new List<IMediaItem> { mediaItem }, true);

            MediaController.GetTransportControls().Prepare();
            return mediaItem;
        }

        public override async Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo)
        {
            await EnsureInit();

            var mediaItems = new List<IMediaItem>();
            foreach (var file in directoryInfo.GetFiles())
            {
                var mediaItem = await MediaExtractor.CreateMediaItem(file);
                mediaItems.Add(mediaItem);
            }

            await AddMediaItemsToQueue(mediaItems, true);

            await MediaQueue.FirstOrDefault()?.FetchMetaData();
            MediaController.GetTransportControls().Prepare();
            return MediaQueue;
        }

        public override async Task<bool> PlayNext()
        {
            await EnsureInit();

            if (Player.NextWindowIndex == Player.CurrentWindowIndex)
            {
                await SeekTo(TimeSpan.FromSeconds(0));
                return true;
            }

            if (Player.NextWindowIndex == -1)
            {
                return false;
            }

            MediaController.GetTransportControls().SkipToNext();

            return true;
        }

        public override async Task<bool> PlayPrevious()
        {
            await EnsureInit();

            if (Player.PreviousWindowIndex == Player.CurrentWindowIndex)
            {
                await SeekTo(TimeSpan.FromSeconds(0));
                return true;
            }

            if (Player.PreviousWindowIndex == -1)
            {
                return false;
            }

            MediaController.GetTransportControls().SkipToPrevious();

            return true;
        }

        public override async Task<bool> PlayQueueItem(IMediaItem mediaItem)
        {
            await EnsureInit();

            if (!MediaQueue.Contains(mediaItem))
                return false;

            MediaController.GetTransportControls().SkipToQueueItem(MediaQueue.IndexOf(mediaItem));
            return true;
        }

        public override async Task SeekTo(TimeSpan position)
        {
            await EnsureInit();

            MediaController.GetTransportControls().SeekTo((long)position.TotalMilliseconds);
        }

        public override async Task Stop()
        {
            await EnsureInit();

            MediaController.GetTransportControls().Stop();
            (NotificationManager as MediaManager.Platforms.Android.Notifications.NotificationManager).Player = null;
        }

        public override RepeatMode RepeatMode
        {
            get
            {
                return (RepeatMode)MediaController?.RepeatMode;
            }
            set
            {
                MediaController?.GetTransportControls()?.SetRepeatMode((int)value);
            }
        }

        public override ShuffleMode ShuffleMode
        {
            get
            {
                return (ShuffleMode)MediaController?.ShuffleMode;
            }
            set
            {
                MediaController?.GetTransportControls()?.SetShuffleMode((int)value);
            }
        }
    }
}
