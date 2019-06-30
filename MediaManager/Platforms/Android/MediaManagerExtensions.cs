using Android.Content;
using System;

namespace MediaManager
{
    public static partial class MediaManagerExtensions
    {
        public static void Init<TMediaBrowserService>(this IMediaManager mediaManager, Context context)
        {
            ((MediaManagerImplementation)mediaManager).Context = context;
            ((MediaManagerImplementation)mediaManager).MediaBrowserServiceType = typeof(TMediaBrowserService);
            mediaManager.Init();
        }
    }
}
