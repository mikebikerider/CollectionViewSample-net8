using Android.Views;
using Android.Util;
using Android.Widget;
using Android.Graphics;

namespace CollectionViewSample
{
    public static class ScreenMetrics
    {
        static public double MeasureTextHeight(string text, double fontSize)
        {
            //round up
            float fsize = (float)Math.Ceiling(fontSize * TextScaleFactor);
            var context = Platform.AppContext;
            var textView = new TextView(context) { Typeface = Typeface.Default };
            textView.SetTextSize(ComplexUnitType.Px, fsize);
            textView.SetText(text, TextView.BufferType.Normal);
            int width = 500;
            int widthMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.AtMost);
            int heightMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            textView.Measure(widthMeasureSpec, heightMeasureSpec);
            return textView.MeasuredHeight;
        }

        public static double MeasureTextWidth(string text, double fontSize)
        {
            //round up, decimal value returns wrong size
            //this way it should fit always
            float fsize = (float)Math.Ceiling(fontSize * TextScaleFactor);
            var context = Platform.AppContext;
            TextView textView = new(context) { Typeface = Typeface.Default };
            textView.SetTextSize(ComplexUnitType.Px, fsize);
            textView.SetText(text, TextView.BufferType.Normal);
            int height = 500;
            int widthMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            int heightMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.AtMost);
            textView.Measure(widthMeasureSpec, heightMeasureSpec);
            return textView.MeasuredWidth;

        }
        private static double TextScaleFactor
        {
            get
            {
                DisplayMetrics dm = Platform.AppContext.Resources.DisplayMetrics;
                float sd = Math.Max(1, dm.ScaledDensity);
                float density = Math.Max(1, dm.Density);
                return sd / density;
            }
        }
    }
}
