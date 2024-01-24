using Microsoft.Maui.Platform;
using System.Drawing;
using UIKit;

namespace CollectionViewSample
{
    public class EntryHandler : Microsoft.Maui.Handlers.EntryHandler
    {
        protected override void ConnectHandler(MauiTextField platformView)
        {
            base.ConnectHandler(platformView);
            //Create a new toolbar.
            var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

            //Create a new UIBarButton with a delegate to clear the focus on the Entry by calling a method that forces the text to stop being edited.
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                PlatformView.EndEditing(true);
            });

            //Add the button to the toolbar previosly created
            toolbar.Items =
            [
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            ];
            PlatformView.InputAccessoryView = toolbar;
        }
    }
}
