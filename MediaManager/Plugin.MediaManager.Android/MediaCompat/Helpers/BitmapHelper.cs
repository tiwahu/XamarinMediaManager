using Java.Net;
using Java.IO;
using Android.Graphics;

namespace Plugin.MediaManager.MediaCompat.Helpers
{
    /*
    public class BitmapHelper
    {
        //private static final String TAG = LogHelper.makeLogTag(BitmapHelper.class);

    // Max read limit that we allow our input stream to mark/reset.
    private static int MAX_READ_LIMIT_PER_IMG = 1024 * 1024;

        public static Bitmap scaleBitmap(Bitmap src, int maxWidth, int maxHeight)
        {
            double scaleFactor = Math.Min(
                ((double)maxWidth) / src.Width, ((double)maxHeight) / src.Height);
            return Bitmap.CreateScaledBitmap(src,
                (int)(src.Width * scaleFactor), (int)(src.Height * scaleFactor), false);
        }

        public static Bitmap scaleBitmap(int scaleFactor, InputStream input)
        {
            // Get the dimensions of the bitmap
            BitmapFactory.Options bmOptions = new BitmapFactory.Options();

            // Decode the image file into a Bitmap sized to fill the View
            bmOptions.InJustDecodeBounds = false;
            bmOptions.InSampleSize = scaleFactor;

            return BitmapFactory.DecodeStream(input, null, bmOptions);
        }

        public static int findScaleFactor(int targetW, int targetH, InputStream input)
        {
            // Get the dimensions of the bitmap
            BitmapFactory.Options bmOptions = new BitmapFactory.Options();
            bmOptions.InJustDecodeBounds = true;
            BitmapFactory.DecodeStream(input, null, bmOptions);
            int actualW = bmOptions.OutWidth;
            int actualH = bmOptions.OutHeight;

            // Determine how much to scale down the image
            return Math.Min(actualW / targetW, actualH / targetH);
        }

    //@SuppressWarnings("SameParameterValue")
    public static Bitmap fetchAndRescaleBitmap(string uri, int width, int height)
            //throws IOException
        {
            URL url = new URL(uri);
        BufferedInputStream input = null;
        try {
            HttpURLConnection urlConnection = (HttpURLConnection)url.OpenConnection();
            input = new BufferedInputStream(urlConnection.InputStream);
            input.Mark(MAX_READ_LIMIT_PER_IMG);
            int scaleFactor = findScaleFactor(width, height, input);
            //LogHelper.d(TAG, "Scaling bitmap ", uri, " by factor ", scaleFactor, " to support ", width, "x", height, "requested dimension");
            input.Reset();
            return scaleBitmap(scaleFactor, input);
        } finally {
            if (input != null) {
                input.Close();
}
        }
    }
    
}*/
}
