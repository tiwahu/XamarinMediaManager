using System;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;

namespace Plugin.MediaManager.MediaCompat.Helpers
{
    public class ResourceHelper
    {
        /**
         * Get a color value from a theme attribute.
         * @param context used for getting the color.
         * @param attribute theme attribute.
         * @param defaultColor default to use.
         * @return color value
         */
        public static int getThemeColor(Context context, int attribute, int defaultColor)
        {
            int themeColor = 0;
            string packageName = context.PackageName;
            try
            {
                Context packageContext = context.CreatePackageContext(packageName, 0);
                ApplicationInfo applicationInfo =
                    context.PackageManager.GetApplicationInfo(packageName, 0);
                packageContext.SetTheme(applicationInfo.Theme);
                Resources.Theme theme = packageContext.Theme;
                TypedArray ta = theme.ObtainStyledAttributes(new int[] { attribute });
                themeColor = ta.GetColor(0, defaultColor);
                ta.Recycle();
            }
            catch (PackageManager.NameNotFoundException e)
            {
                e.PrintStackTrace();
            }
            return themeColor;
        }
    }
}
