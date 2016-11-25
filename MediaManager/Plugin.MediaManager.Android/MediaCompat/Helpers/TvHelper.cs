using System;
using Android.App;
using Android.Content;
using Android.Content.Res;

namespace Plugin.MediaManager.MediaCompat.Helpers
{
    public class TvHelper
    {

    /**
     * Returns true when running Android TV
     *
     * @param c Context to detect UI Mode.
     * @return true when device is running in tv mode, false otherwise.
     */
        public static bool isTvUiMode(Context c)
        {
            UiModeManager uiModeManager = (UiModeManager)c.GetSystemService(Context.UiModeService);
            if (uiModeManager.CurrentModeType == UiMode.TypeTelevision)
            {
                //LogHelper.d(TAG, "Running in TV mode");
                return true;
            }
            else {
                //LogHelper.d(TAG, "Running on a non-TV mode");
                return false;
            }
        }
    }
}
