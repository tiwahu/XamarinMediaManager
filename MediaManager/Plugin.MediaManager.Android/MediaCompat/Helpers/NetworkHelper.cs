using System;
using Android.Content;
using Android.Net;

namespace Plugin.MediaManager.MediaCompat.Helpers
{
    public class NetworkHelper
    {
        /**
         * @param context to use to check for network connectivity.
         * @return true if connected, false otherwise.
         */
        public static bool isOnline(Context context)
        {
            ConnectivityManager connMgr = (ConnectivityManager)
                context.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = connMgr.ActiveNetworkInfo;
            return (networkInfo != null && networkInfo.IsConnected);
        }
    }
}
