using Android.Views;
using Android.Util;
using Android.Widget;
using Android.Graphics;
using Android.Content;

namespace CollectionViewSample
{
    public static class ScreenMetrics
    {
        static public double MeasureTextHeight(string text, double fontSize)
        {
            //round up
            Context context = Platform.AppContext;
            float fsize = (float)Math.Ceiling(fontSize * FontScale);
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
            Context context = Platform.AppContext;
            //round up, decimal value can return wrong size
            //this way it should fit always
            float fsize = (float)Math.Ceiling(fontSize * FontScale);
            TextView textView = new(context) { Typeface = Typeface.Default };
            textView.SetTextSize(ComplexUnitType.Px, fsize);
            textView.SetText(text, TextView.BufferType.Normal);
            int height = 500;
            int widthMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            int heightMeasureSpec = Android.Views.View.MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.AtMost);
            textView.Measure(widthMeasureSpec, heightMeasureSpec);
            return textView.MeasuredWidth;
        }
        public static float FontScale
        {
            get
            {
                float scale;
                Context context = Platform.AppContext;
                Android.Content.Res.Resources? resources = context.Resources;
                if (resources != null)
                {
                    Android.Content.Res.Configuration? config = resources.Configuration;
                    if (config != null)
                    {
                        scale = config.FontScale;
                        if (scale > .4)
                            return scale;
                    }
                }
                return 1f;
            }
        }
    }
}
