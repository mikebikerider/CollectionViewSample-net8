using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using System.Diagnostics;

namespace CollectionViewSample;

public partial class NoScrollPage : ContentPage
{
    private static string gotop = "\ue25a";
    private static string gobottom = "\ue258";
    private static string hourglass = "\uea5b";
    private int loadnumber = 200;
    private int reloadnumber = 5000;
    private bool firsttime = true;
    private CVPageViewModel? vmodel;
    private bool glyphcorrection = false;
    private Thickness safeareainsets = new Thickness(0);
    public NoScrollPage()
    {
        InitializeComponent();
        vmodel = BindingContext as CVPageViewModel;
        if (vmodel != null)
        {
            double fsize = nameLabel.FontSize;
            vmodel.Cw0 = ScreenMetrics.MeasureTextWidth("5555", fsize) + fsize + 10;
            vmodel.Cw1 = ScreenMetrics.MeasureTextWidth("Column 1", fsize) + fsize + 10;
            vmodel.Cw2 = ScreenMetrics.MeasureTextWidth("Column 2", fsize) + fsize + 10;
        }

#if IOS
        Microsoft.Maui.Handlers.ScrollViewHandler.Mapper.AppendToMapping("Disable_Bounce", (handler, view) =>
        {
            handler.PlatformView.Bounces = false;
        });
        //currently no similar way of disabling CollectionView bouncing
#endif
#if ANDROID
        glyphCorrectionCheckBox.IsEnabled = true;
        glyphCorrectionCheckBox.IsChecked = false;
#endif

#if DEBUG
        loadnumber = 100;
        reloadnumber = 200;
#endif
    }
    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        await Task.Delay(100);

#if ANDROID
        collectionViewGrid.HeightRequest = height - bottomBorder.Height;
#else
        //the above line is not needed under iOS, however if issued in an app running on iPhone with the notch or dynamic island
        //then the bottomGrid will ber resized to size larger than width by the sum of the left and right inset
        safeareainsets = On<iOS>().SafeAreaInsets();
        collectionViewGrid.HeightRequest = height - bottomBorder.Height - safeareainsets.Bottom - safeareainsets.Top;
#endif
        if (Debugger.IsAttached)
            Debug.WriteLine("Height allocated: " + height.ToString());
    }
    public void SetAppTheme(AppTheme apptheme)
    {
        //On Android FontImageSource images for page in view during theme change will be updated correctly but after moving to another page and back graphics will disappear
        //font graphics on pages not in view during the theme change are not affected and will be rendered correctly when after moving to a different page and back
        //until the glyph is changed in code behind
        //does not happen on iOS

        try
        {
            if (vmodel != null)
            {
                if (vmodel.Cvc is List<CVcontent> cvc)
                {
                    if (cvc.Count > 0)
                    {
                        bool islightTheme = apptheme.Equals(AppTheme.Light);
                        for (int i = 0; i < vmodel.Cvc.Count; i++)
                        {
                            vmodel.Cvc[i].IsLightTheme = islightTheme;
                            cvc[i].IsSelected = cvc[i].IsSelected;
                        }
                    }
                }
            }
        }
        catch { }
    }
    private Task<double[]> ContentColumnsWidth(List<CVcontent> cvc, double fsize)
    {
        double w0 = ScreenMetrics.MeasureTextWidth("55555", fsize) + 10;
        double w1 = ScreenMetrics.MeasureTextWidth("Column 1", fsize) + 10;
        double w2 = ScreenMetrics.MeasureTextWidth("Column 2", fsize) + 10;
        for (int i = 0; i < cvc.Count; i++)
        {
            w0 = Math.Max(w0, ScreenMetrics.MeasureTextWidth(cvc[i].ItemNo, fsize) + 10); //right margin 10
            w1 = Math.Max(w1, ScreenMetrics.MeasureTextWidth(cvc[i].FirstName, fsize) + 10);
            w2 = Math.Max(w2, ScreenMetrics.MeasureTextWidth(cvc[i].LastName, fsize) + 10);
        }
   //     double cw = Math.Max(Width, w0 + w1 + w2);
        for (int i = 0; i < cvc.Count; i++)
        {
            cvc[i].Cw0 = w0;
            cvc[i].Cw1 = w1;
            cvc[i].Cw2 = w2;
        }
        double[] w = [w0, w1, w2];
        return Task.FromResult(w);
    }
    private void CollectionViewItem_Tapped(object sender, TappedEventArgs e)
    {
        if (settingsBorder.IsVisible)
            settingsBorder.IsVisible = false;
        if (sender is Label l)
        {
            if (l.BindingContext != null)
            {
                if (l.BindingContext is CVcontent cvc)
                    collectionView1.SelectedItem = cvc;
            }
        }
    }
    /*
    private string bytearray2string(byte[] b)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < b.Length; i++)
        {
            sb.Append(b[i].ToString("X2"));
        }
        return sb.ToString();
    }
    private string glyph2string(string glyph)
    {
        byte[] b = Encoding.BigEndianUnicode.GetBytes(glyph);
        return bytearray2string(b);
    }
    */
    private async void UpDown_Clicked(object sender, EventArgs e)
    {
        if (vmodel != null)
        {
            if (vmodel.Cvc != null)
            {
                if (vmodel.Cvc is List<CVcontent> cvc)
                {
                    if (cvc.Count > 0)
                    {
                        DeviceService.PlayClickSound();
                        if (vmodel != null)
                        {
                            vmodel.IsBusy = true;
                            await Task.Delay(100);
                            if (vmodel.UpDownText == "Bottom")
                            {
                                collectionView1.ScrollTo(cvc[^1], null, ScrollToPosition.End, false);
                                collectionView1.SelectedItem = cvc[^1];
                                vmodel.UpDownText = "Top";
                                vmodel.UpDownGlyph = gotop;
                            }
                            else if (vmodel.UpDownText == "Top")
                            {
                                collectionView1.ScrollTo(cvc[0], null, ScrollToPosition.Start, false);
                                collectionView1.SelectedItem = cvc[0];
                                vmodel.UpDownText = "Bottom";
                                vmodel.UpDownGlyph = gobottom;
                            }
                            vmodel.IsBusy = false;
                        }
                    }
                }
            }
        }
    }

    private void CollectionView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int index = -1;
        if (vmodel != null)
        {
            List<CVcontent> cvcontent = vmodel.Cvc;
            if (collectionView1.SelectedItem != null)
            {
                if (collectionView1.SelectedItem is CVcontent cvc)
                {
                    index = vmodel.Cvc.IndexOf(cvc);

                    if (index == 0)
                    {
                        vmodel.UpDownGlyph = gobottom;
                        vmodel.UpDownText = "Bottom";
                    }
                    else if (index == cvcontent.Count - 1)
                    {
                        vmodel.UpDownGlyph = gotop;
                        vmodel.UpDownText = "Top";
                    }
                }
            }
            for (int i = 0; i < cvcontent.Count; i++)
            {
                if (i == index)
                    cvcontent[i].IsSelected = true;
                else
                {
                    if (cvcontent[i].IsSelected)
                    {
                        cvcontent[i].IsSelected = false;
                    }
                }
            }
        }
    }

    private void Reload_Clicked(object sender, EventArgs e)
    {
        DeviceService.PlayClickSound();
        if (firsttime)
            loadCollectionView(loadnumber);
        else
            loadCollectionView(reloadnumber);
    }

    private void Page_Appearing(object sender, EventArgs e)
    {

        if (firsttime)
        {
            try
            {
                double fsize = numberItemsEntry.FontSize;
                numberItemsEntry.WidthRequest = ScreenMetrics.MeasureTextWidth("555555", fsize) + fsize + 10;
                numberItemsEntry.Text = reloadnumber.ToString();
                loadCollectionView(loadnumber);
            }
            catch { }
        }

        else
        {
#if ANDROID
            if (glyphcorrection)
            {
                if (vmodel != null)
                {
                    vmodel.UpdateUpDownGlyph();
                }
            }
#endif
        }

    }

    private async void loadCollectionView(int numberitems)
    {
        try
        {
            settingsBorder.IsVisible = false;
            collectionView1.SelectedItem = null;
            if (vmodel != null)
            {

                        vmodel.IsBusy = true;
                        vmodel.UpDownGlyph = hourglass;
                        vmodel.UpDownText = "Wait";
                        await Task.Delay(100);
                        bool isLightTheme = AppInfo.RequestedTheme.Equals(AppTheme.Light);
                        List<CVcontent> cvl = new List<CVcontent>();
                        for (int i = 0; i < numberitems; i++)
                        {
                            cvl.Add(new CVcontent { IsLightTheme = isLightTheme, ItemNumber = i + 1, FirstName = Path.GetRandomFileName().Replace(".", ""), LastName = Path.GetRandomFileName().Replace(".", "") });
                        }
                        double[] w = await ContentColumnsWidth(cvl, nameLabel.FontSize);
                        vmodel.Cw0 = w[0];
                        vmodel.Cw1 = w[1];
                        vmodel.Cvc = cvl;
                        List<CVcontent> cvc = vmodel.Cvc;
#if iOS
                        collectionViewGrid.WidthRequest = Math.Max(w[0] + w[1] + w[2], Width) - safeareainsets.Left - safeareainsets.Right;
                        collectionViewGrid.HeightRequest = Height - bottomBorder.Height - safeareainsets.Bottom - safeareainsets.Top;
#endif
                        await Task.Delay(100);
                        //this line trigggers SelectionChanged event that will change the upDownIFS glyph and the button appearance 
                        collectionView1.SelectedItem = cvc[0];
                        collectionView1.ScrollTo(cvc[0], null, ScrollToPosition.Start, false);
                        firsttime = false;
                        vmodel.IsBusy = false;
            }
        }
        catch (Exception x)
        {
            Debug.WriteLine(x.Message);
        }
    }

    private async void MeasureItems_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        activityIndicator.IsRunning = true;
        settingsBorder.IsVisible = false;
        DeviceService.PlayClickSound();
        await Task.Delay(500);

        if (e.Value)
            collectionView1.ItemSizingStrategy = ItemSizingStrategy.MeasureAllItems;
        else
            collectionView1.ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem;
        activityIndicator.IsRunning = false;
    }

    private async void GoButton_Clicked(object sender, EventArgs e)
    {
        DeviceService.PlayClickSound();
        await Shell.Current.GoToAsync("//hscroll");
    }

    private void SettingsButton_Clicked(object sender, EventArgs e)
    {
        DeviceService.PlayClickSound();
        if (settingsBorder.IsVisible)
            settingsBorder.IsVisible = false;
        else
            settingsBorder.IsVisible = true;
    }

    private void ShowNavigationBarChecked_Changed(object sender, CheckedChangedEventArgs e)
    {
        try
        {
            DeviceService.PlayClickSound();
            settingsBorder.IsVisible = false;
            Shell.SetNavBarIsVisible(this, navBarCheckBox.IsChecked);
        }
        catch (Exception x)
        {
            if (Debugger.IsAttached)
                Debug.WriteLine("NoScrlollPage.ShowNavigationBarChecked_Changed() error: " + x.Message);
        }
    }

    private void NumberItems_Changed(object sender, TextChangedEventArgs e)
    {
        try
        {
            int.TryParse(e.NewTextValue, out reloadnumber);
        }
        catch { }
    }

    private void SettingsLabel_Tapped(object sender, TappedEventArgs e)
    {
        settingsBorder.IsVisible = false;
    }

    private void NumberItems_Unfocused(object sender, FocusEventArgs e)
    {
        try
        {
            int num;
            if (numberItemsEntry.Text != null)
            {
                if (int.TryParse(numberItemsEntry.Text, out num))
                {
                    if (num > 0)
                    {
                        reloadnumber = num;
                        loadCollectionView(num);
                    }
                }
            }
        }
        catch { }
    }

    private void NumberItems_Completed(object sender, EventArgs e)
    {
        try
        {
            settingsBorder.IsVisible = false;
            DeviceService.PlayClickSound();
            Task.Delay(500);
            loadCollectionView(reloadnumber);
        }
        catch { }
    }

    private void GlyphCorrectionChecked_Changed(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            glyphcorrection = checkBox.IsChecked;
        }
    }
}