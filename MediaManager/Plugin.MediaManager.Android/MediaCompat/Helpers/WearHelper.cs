using System;
using Android.OS;

namespace Plugin.MediaManager.MediaCompat.Helpers
{
    public class WearHelper
    {
        private static string WEAR_APP_PACKAGE_NAME = "com.google.android.wearable.app";
        /*
        public static bool isValidWearCompanionPackage(String packageName)
        {
            return WEAR_APP_PACKAGE_NAME.Equals(packageName);
        }

        public static void setShowCustomActionOnWear(Bundle customActionExtras, bool showOnWear)
        {
            if (showOnWear)
            {
                customActionExtras.PutBoolean(
                        MediaControlConstants.EXTRA_CUSTOM_ACTION_SHOW_ON_WEAR, true);
            }
            else {
                customActionExtras.Remove(MediaControlConstants.EXTRA_CUSTOM_ACTION_SHOW_ON_WEAR);
            }
        }

        public static void setUseBackgroundFromTheme(Bundle extras, bool useBgFromTheme)
        {
            if (useBgFromTheme)
            {
                extras.PutBoolean(MediaControlConstants.EXTRA_BACKGROUND_COLOR_FROM_THEME, true);
            }
            else {
                extras.Remove(MediaControlConstants.EXTRA_BACKGROUND_COLOR_FROM_THEME);
            }
        }

        public static void setSlotReservationFlags(Bundle extras, bool reserveSkipToNextSlot,
                                                   bool reserveSkipToPrevSlot)
        {
            if (reserveSkipToPrevSlot)
            {
                extras.PutBoolean(MediaControlConstants.EXTRA_RESERVE_SLOT_SKIP_TO_PREVIOUS, true);
            }
            else {
                extras.Remove(MediaControlConstants.EXTRA_RESERVE_SLOT_SKIP_TO_PREVIOUS);
            }
            if (reserveSkipToNextSlot)
            {
                extras.PutBoolean(MediaControlConstants.EXTRA_RESERVE_SLOT_SKIP_TO_NEXT, true);
            }
            else {
                extras.Remove(MediaControlConstants.EXTRA_RESERVE_SLOT_SKIP_TO_NEXT);
            }
        }*/
    }
}
