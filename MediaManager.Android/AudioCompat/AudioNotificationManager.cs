using System;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Plugin.MediaManager.AudioCompat
{
    public class AudioNotificationManager : IMediaNotificationManager
    {
        public AudioNotificationManager()
        {
        }

        public void StartNotification(IMediaFile mediaFile)
        {
            throw new NotImplementedException();
        }

        public void StopNotifications()
        {
            throw new NotImplementedException();
        }

        public void UpdateNotifications(IMediaFile mediaFile, MediaPlayerStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
