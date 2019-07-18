using Android.Content;
using System;

namespace MediaManager
{
    public static partial class MediaManagerExtensions
    {
        public static void Init<TMediaBrowserService>(this IMediaManager mediaManager, Context context)
        {
            var androidMediaManager = ((MediaManagerImplementation)mediaManager);
            androidMediaManager.Context = context;
            androidMediaManager.MediaBrowserServiceType = typeof(TMediaBrowserService);
            androidMediaManager.Init();
        }
    }
}
