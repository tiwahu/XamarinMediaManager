using Android.Content;
using System;

namespace MediaManager
{
    public static partial class MediaManagerExtensions
    {
        public static void Init<TMediaBrowserService>(this IMediaManager mediaManager, Context context)
        {
            mediaManager.SetContext(context);
            mediaManager.SetMediaManagerType(typeof(TMediaBrowserService));
            mediaManager.Init();
        }

        public static void SetContext(this IMediaManager mediaManager, Context context)
        {
            (mediaManager as MediaManagerImplementation).Context = context;
        }

        public static void SetMediaManagerType(this IMediaManager mediaManager, Type mediaBrowserServiceType)
        {
            (mediaManager as MediaManagerImplementation).MediaBrowserServiceType = mediaBrowserServiceType;
        }

        public static Context GetContext(this IMediaManager mediaManager)
        {
            return (mediaManager as MediaManagerImplementation).Context;
        }
    }
}
