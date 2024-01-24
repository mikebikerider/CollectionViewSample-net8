using Foundation;
using UIKit;


namespace CollectionViewSample
{
    public static class ScreenMetrics
    {
        public static double MeasureTextHeight(string text, double fontSize)
        {
            var nsText = new NSString(text);

            var boundSize = new SizeF(float.PositiveInfinity, float.PositiveInfinity);

            var options = NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin;
            UIFont font = UIFont.SystemFontOfSize((float)fontSize);
            var attributes = new UIStringAttributes
            {

                Font = font

            };

            var sizeF = nsText.GetBoundingRect(boundSize, options, attributes, null).Size;

            return (double)sizeF.Height;
        }
        public static double MeasureTextWidth(string text, double fontSize)
        {
            var nsText = new NSString(text);

            var boundSize = new SizeF(float.PositiveInfinity, float.PositiveInfinity);

            var options = NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin;

            UIFont font = UIFont.SystemFontOfSize((float)fontSize);
            var attributes = new UIStringAttributes
            {

                Font = font

            };

            var sizeF = nsText.GetBoundingRect(boundSize, options, attributes, null).Size;

            return (double)sizeF.Width;
        }

    }
}
