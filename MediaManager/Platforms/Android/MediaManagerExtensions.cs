using Android.Content;
using System;
using System.Threading.Tasks;

namespace MediaManager
{
    public static partial class MediaManagerExtensions
    {
        public static async Task Init<TMediaBrowserService>(this IMediaManager mediaManager, Context context)
        {
            var androidMediaManager = ((MediaManagerImplementation)mediaManager);
            androidMediaManager.Context = context;
            androidMediaManager.MediaBrowserServiceType = typeof(TMediaBrowserService);
            await androidMediaManager.Init();
        }
    }
}
