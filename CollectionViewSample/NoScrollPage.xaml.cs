using System.Diagnostics;
using System.Text;

namespace CollectionViewSample;

public partial class NoScrollPage : ContentPage
{
    private static string gotop = "\ue25a";
    private static string gobottom = "\ue258";
    private static string hourglass = "\uea5b";
    private int loadnumber = 100;
    private int reloadnumber = 5000;
    private bool firsttime = true;
    private bool glyphcorrection = false;
    public NoScrollPage()
    {
        InitializeComponent();
#if IOS
        Microsoft.Maui.Handlers.ScrollViewHandler.Mapper.AppendToMapping("Disable_Bounce", (handler, view) =>
        {
            handler.PlatformView.Bounces = false;
        });
        //currently no similar way of disabling CollectionView bouncing
#endif
#if ANDROID
        glyphCorrectionCheckBox.IsEnabled = true;
#endif

#if DEBUG
        loadnumber = 50;
        reloadnumber = 200;
#endif        
    }
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

#if ANDROID
        //the need for this code in MAUI is an embarassment
        //in a real app the minimum bottomBar width is calculated and WidthRequest be the bigger of width and calculated width
        //no need for that in Xamarin.Forms and MAUI iOS
        bottomGrid.WidthRequest = width;
#else
        //the above line is not needed under iOS, however if issued in an app running on iPhone with the notch or dynamic island
        //then the bottomGrid will ber resized to size larger than width by the sum of the left and right inset
#endif
    }
    public async void SetAppTheme(AppTheme apptheme)
    {
        //On Android FontImageSource images for page in view during theme change will be updated correctly but after moving to another page and back graphics will disappear
        //font graphics on pages not in view during the theme change are not affected and will be rendered correctly when after moving to a different page and back
        //until the glyph is changed in code behind
        //does not happen on iOS

        await MainThread.InvokeOnMainThreadAsync(() => {
            try
            {
                bool islightTheme = apptheme.Equals(AppTheme.Light);
                if (collectionView1.ItemsSource != null)
                {
                    if (collectionView1.ItemsSource is List<CVcontent> cvc)
                    {
                        for (int i = 0; i < cvc.Count; i++)
                        {
                            cvc[i].IsLightTheme = islightTheme;
                            cvc[i].IsSelected = cvc[i].IsSelected;
                        }
                    }
                }
            }
            catch { }
        }); 
    }
    private Task<double[]> ContentColumnsWidth(List<CVcontent> cvc, double fsize)
    {
//        Debug.WriteLine("Font size:" + fsize.ToString());
        double w0 = ScreenMetrics.MeasureTextWidth("5555", fsize) + fsize + 10;
        double w1 = ScreenMetrics.MeasureTextWidth("Column 1", fsize) + fsize;
        double w2 = ScreenMetrics.MeasureTextWidth("Column 2", fsize) + fsize;
        for (int i = 0; i < cvc.Count; i++)
        {
            w0 = Math.Max(w0, ScreenMetrics.MeasureTextWidth(cvc[i].ItemNo, fsize) + fsize + 10); //right margin 10
            w1 = Math.Max(w1, ScreenMetrics.MeasureTextWidth(cvc[i].FirstName, fsize) + fsize);
            w2 = Math.Max(w2, ScreenMetrics.MeasureTextWidth(cvc[i].LastName, fsize) + fsize);
        }
        double cw = Math.Max(Width, w0 + w1 + w2);
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
        if (collectionView1.ItemsSource != null)
        {
            if (collectionView1.ItemsSource is List<CVcontent>)
            {
                if (sender is Label l)
                {
                    if (l.BindingContext != null)
                    {
                        if (l.BindingContext is CVcontent cvc)
                            collectionView1.SelectedItem = cvc;
                    }
                }
            }
        }
    }
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
    private async void UpDown_Clicked(object sender, EventArgs e)
    {
        if (collectionView1.ItemsSource != null)
        {
            if (collectionView1.ItemsSource is List<CVcontent> cvc)
            {
                if (cvc.Count > 0)
                {
                    DeviceService.PlayClickSound();
                    upDownButton.IsEnabled = false;
                    loadButton.IsEnabled = false;
                    collectionView1.Opacity = .5;
                    activityIndicator.IsRunning = true;
                    await Task.Delay(100);
                    if (upDownLabel.Text == "Down")
                    {
                        collectionView1.ScrollTo(cvc[^1], null, ScrollToPosition.End, false);
                        await Task.Delay(100);
                        collectionView1.SelectedItem = cvc[^1];
                    }
                    else if (upDownLabel.Text == "Up")
                    {
                        collectionView1.ScrollTo(cvc[0], null, ScrollToPosition.Start, false);
                        await Task.Delay(100);
                        collectionView1.SelectedItem = cvc[0];
                    }
                    upDownButton.IsEnabled = true;
                    activityIndicator.IsRunning = false;
                    collectionView1.Opacity = 1;
                    loadButton.IsEnabled = true;
                }
            }
        }
    }

    private void CollectionView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (collectionView1.ItemsSource != null)
        {
            if (collectionView1.ItemsSource is List<CVcontent> cvcontent)
            {
                int index = -1;
                if (collectionView1.SelectedItem != null)
                {
                    if (collectionView1.SelectedItem is CVcontent cvc)
                    {
                        index = cvcontent.IndexOf(cvc);
                        if (index == 0)
                        {
                            upDownFIS.Glyph = gobottom;
                            upDownLabel.Text = "Down";
                        }
                        else if (index == cvcontent.Count - 1)
                        {
                            upDownFIS.Glyph = gotop;
                            upDownLabel.Text = "Up";
                        }
                        upDownButton.IsEnabled = true;
                        loadButton.IsEnabled = true;
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
                collectionView1.ItemsSource = new List<CVcontent>();
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
                string glyph = upDownFIS.Glyph;
                upDownFIS.Glyph = "";
                upDownFIS.Glyph = glyph;

                glyph = loadFIS.Glyph;
                loadFIS.Glyph = "";
                loadFIS.Glyph = glyph;

                glyph = settingsFIS.Glyph;
                settingsFIS.Glyph = "";
                settingsFIS.Glyph = glyph;

                glyph = navbarFIS.Glyph;
                navbarFIS.Glyph = "";
                navbarFIS.Glyph = glyph;

                glyph = cancelFIS.Glyph;
                cancelFIS.Glyph = "";
                cancelFIS.Glyph = glyph;

                glyph = goFIS.Glyph;
                goFIS.Glyph = "";
                goFIS.Glyph = glyph;
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
            upDownFIS.Glyph = hourglass;
            loadButton.IsEnabled = false;
            goButton.IsEnabled = false;
            upDownButton.IsEnabled = false;
            upDownLabel.Text = "Wait";
            collectionView1.Opacity = .5;
            activityIndicator.IsRunning = true;
            await Task.Delay(100);
            List<CVcontent> cvc = new List<CVcontent>();
            bool isLightTheme = AppInfo.RequestedTheme.Equals(AppTheme.Light);
            for (int i = 0; i < numberitems; i++)
            {
                cvc.Add(new CVcontent { IsLightTheme = isLightTheme, ItemNumber = i + 1, FirstName = Path.GetRandomFileName().Replace(".", ""), LastName = Path.GetRandomFileName().Replace(".", "") });
            }
            double[] w = await ContentColumnsWidth(cvc,nameLabel.FontSize);
            nameLabel.Text = "";
            headerGrid.ColumnDefinitions[0].Width = new GridLength(w[0]);
            headerGrid.ColumnDefinitions[1].Width = new GridLength(w[1]);
            await Task.Delay(100);
            //dumb trick to force header repaint after column definitions changed
            //is not required in Xamarin.Forms
            nameLabel.Text = "Column 1";
            await Task.Delay(100);
            collectionView1.ItemsSource = cvc;
            await Task.Delay(100);
            if (collectionView1.ItemsSource != null)
            {
                if (collectionView1.ItemsSource is List<CVcontent> cvcontent)
                {
                    //this line trigggers SelectionChanged event that will change the upDownIFS glyph and the button appearance 
                    collectionView1.SelectedItem = cvcontent[^1];
                    collectionView1.ScrollTo(cvcontent[^1], null, ScrollToPosition.End, false);
                    loadButton.IsEnabled = true;
                    upDownButton.IsEnabled = true;
                    goButton.IsEnabled = true;
                    collectionView1.Opacity = 1;
                    firsttime = false;
                }
            }
            activityIndicator.IsRunning = false;
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