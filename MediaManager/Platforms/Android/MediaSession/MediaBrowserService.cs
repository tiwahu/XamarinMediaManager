using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2.UI;
using MediaManager.Platforms.Android.Media;

namespace MediaManager.Platforms.Android.MediaSession
{
    //[Service(Exported = true)]
    //[IntentFilter(new[] { global::Android.Service.Media.MediaBrowserService.ServiceInterface })]
    public abstract class MediaBrowserService : MediaBrowserServiceCompat
    {
        protected MediaManagerImplementation MediaManager => CrossMediaManager.Android;
        protected MediaDescriptionAdapter MediaDescriptionAdapter { get; set; }
        protected PlayerNotificationManager PlayerNotificationManager {
            get => (MediaManager.NotificationManager as Notifications.NotificationManager).PlayerNotificationManager;
            set => (MediaManager.NotificationManager as Notifications.NotificationManager).PlayerNotificationManager = value;
        }
        protected MediaControllerCompat MediaController => MediaManager.MediaBrowserManager.MediaController;

        protected NotificationListener NotificationListener { get; set; }

        public readonly string ChannelId = "audio_channel";
        public readonly int ForegroundNotificationId = 1;
        public bool IsForeground = false;
        protected Notification _notification;

        public MediaBrowserService()
        {
        }

        protected MediaBrowserService(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            PrepareMediaSession();
            PrepareNotificationManager();

            MediaManager.StateChanged += MediaManager_StateChanged;
        }

        private void MediaManager_StateChanged(object sender, MediaManager.Playback.StateChangedEventArgs e)
        {
            switch (e.State)
            {
                case global::MediaManager.Playback.MediaPlayerState.Failed:
                case global::MediaManager.Playback.MediaPlayerState.Stopped:
                    if (IsForeground && MediaController.PlaybackState.State == PlaybackStateCompat.StateStopped)
                    {
                        StopForeground(true);
                        StopSelf();
                        IsForeground = false;
                    }
                    break;
                case global::MediaManager.Playback.MediaPlayerState.Loading:
                case global::MediaManager.Playback.MediaPlayerState.Buffering:
                case global::MediaManager.Playback.MediaPlayerState.Playing:
                    if(!IsForeground)
                    {
                        // NOTE: need to use specified MediaBrowserServiceType...
                        ContextCompat.StartForegroundService(MediaManager.Context, new Intent(MediaManager.Context, Java.Lang.Class.FromType(MediaManager.MediaBrowserServiceType)));
                        PlayerNotificationManager.SetOngoing(true);
                        PlayerNotificationManager.Invalidate();

                        //TODO: This might need to be called: https://stackoverflow.com/questions/44425584/context-startforegroundservice-did-not-then-call-service-startforeground
                        // calling it...
                        StartForeground(ForegroundNotificationId, _notification);

                        IsForeground = true;
                    }
                    break;
                case global::MediaManager.Playback.MediaPlayerState.Paused:
                    if (IsForeground)
                    {
                        StopForeground(false);
                        PlayerNotificationManager.SetOngoing(false);
                        IsForeground = false;
                    }
                    break;
                default:
                    break;
            }
        }

        protected virtual void PrepareMediaSession()
        {
            var mediaSession = new MediaSessionCompat(this, nameof(MediaBrowserService));
            MediaManager.MediaSession = mediaSession;
            mediaSession.SetSessionActivity(MediaManager.SessionActivityPendingIntent);
            mediaSession.Active = true;

            SessionToken = mediaSession.SessionToken;

            mediaSession.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons |
                                   MediaSessionCompat.FlagHandlesTransportControls);
        }

        protected virtual int? SmallIconResourceId => null;

        protected virtual int? Color => null;

        protected virtual bool? Colorized => null;

        protected virtual bool? UseChronometer => null;


        protected virtual void PrepareNotificationManager()
        {
            MediaDescriptionAdapter = new MediaDescriptionAdapter();
            PlayerNotificationManager = Com.Google.Android.Exoplayer2.UI.PlayerNotificationManager.CreateWithNotificationChannel(
                this,
                ChannelId,
                Resource.String.exo_download_notification_channel_name,
                ForegroundNotificationId,
                MediaDescriptionAdapter);

            //Needed for enabling the notification as a mediabrowser.
            NotificationListener = new NotificationListener();
            NotificationListener.OnNotificationStartedImpl = (notificationId, notification) =>
            {
                _notification = notification;
                // NOTE: need to use specified MediaBrowserServiceType...
                ContextCompat.StartForegroundService(MediaManager.Context, new Intent(MediaManager.Context, Java.Lang.Class.FromType(MediaManager.MediaBrowserServiceType)));
                StartForeground(notificationId, notification);
                IsForeground = true;
            };
            NotificationListener.OnNotificationCancelledImpl = (notificationId) =>
            {
                //TODO: in new exoplayer use StopForeground(false) or use dismissedByUser
                StopForeground(true);
                StopSelf();
                IsForeground = false;
            };

            PlayerNotificationManager.SetFastForwardIncrementMs((long)MediaManager.StepSize.TotalMilliseconds);
            PlayerNotificationManager.SetRewindIncrementMs((long)MediaManager.StepSize.TotalMilliseconds);
            
            //? deprecated?
            PlayerNotificationManager.SetNotificationListener(NotificationListener);

            PlayerNotificationManager.SetMediaSessionToken(SessionToken);
            PlayerNotificationManager.SetOngoing(true);
            PlayerNotificationManager.SetUsePlayPauseActions(MediaManager.NotificationManager.ShowPlayPauseControls);
            PlayerNotificationManager.SetUseNavigationActions(MediaManager.NotificationManager.ShowNavigationControls);

            //PlayerNotificationManager.SetUseActionsInComapactView(true);

            // CUSTOM SUPPORT
            PlayerNotificationManager.SetVisibility((int)NotificationVisibility.Public);

            var smallIconResourceId = this.SmallIconResourceId;
            if (smallIconResourceId.HasValue)
                this.PlayerNotificationManager.SetSmallIcon(smallIconResourceId.Value);

            if (this.Color.HasValue)
                this.PlayerNotificationManager.SetColor(this.Color.Value);

            if (this.Colorized.HasValue)
                this.PlayerNotificationManager.SetColorized(this.Colorized.Value);

            if (this.UseChronometer.HasValue)
                this.PlayerNotificationManager.SetUseChronometer(this.UseChronometer.Value);

            // CUSTOM SUPPORT

            //Must be called to start the connection
            //! switched
            //(MediaManager.NotificationManager as Notifications.NotificationManager).Player = MediaManager.Player;
            this.PlayerNotificationManager.SetPlayer(MediaManager.Player);
        }

        public override StartCommandResult OnStartCommand(Intent startIntent, StartCommandFlags flags, int startId)
        {
            if (startIntent != null)
            {
                MediaButtonReceiver.HandleIntent(MediaManager.MediaSession, startIntent);
            }
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            // Service is being killed, so make sure we release our resources

            // enabling this disposal, too...
            PlayerNotificationManager.SetPlayer(null);
            PlayerNotificationManager.Dispose();

            MediaManager.StateChanged -= MediaManager_StateChanged;
            MediaManager.MediaSession.Active = false;
            MediaManager.MediaSession.Release();
            StopForeground(true);
            IsForeground = false;
        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            return new BrowserRoot(nameof(ApplicationContext.ApplicationInfo.Name), null);
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            var mediaItems = new JavaList<MediaBrowserCompat.MediaItem>();

            foreach (var item in MediaManager.MediaQueue)
                mediaItems.Add(item.ToMediaBrowserMediaItem());

            result.SendResult(mediaItems);
        }
    }
}
