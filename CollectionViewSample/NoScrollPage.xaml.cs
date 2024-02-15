using System.Diagnostics;
using System.Threading.Tasks;

namespace CollectionViewSample;

public partial class NoScrollPage : ContentPage
{
    private static string gotop = "\ue25a";
    private static string gobottom = "\ue258";
    private int loadnumber = 100;
    private int reloadnumber = 5000;
    private bool firsttime = true;
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
        await MainThread.InvokeOnMainThreadAsync(() => {
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
        }); 
    }
    private Task<double[]> ContentColumnsWidth(List<CVcontent> cvc, double fsize)
    {
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
    private async void UpDown_Clicked(object sender, EventArgs e)
    {
        if (collectionView1.ItemsSource != null)
        {
            if (collectionView1.ItemsSource is List<CVcontent> cvc)
            {
                if (cvc.Count > 0)
                {
                    if (upDownButton.Source != null)
                    {
                        if (upDownButton.Source is FontImageSource fis)
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
                                fis.Glyph = gotop;
                                upDownLabel.Text = "Up";
                            }
                            else if (upDownLabel.Text == "Up")
                            {    
                                collectionView1.ScrollTo(cvc[0], null, ScrollToPosition.Start, false);
                                await Task.Delay(100);
                                collectionView1.SelectedItem = cvc[0];
                                fis.Glyph = gobottom;
                                upDownLabel.Text = "Down";
                            }
                            upDownButton.IsEnabled = true;
                            activityIndicator.IsRunning = false;
                            collectionView1.Opacity = 1;
                            loadButton.IsEnabled = true;
                        }
                    }
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
                        if (upDownButton.Source != null)
                        {
                            if (upDownButton.Source is FontImageSource fis)
                            {
                                index = cvcontent.IndexOf(cvc);
                                if (index == 0)
                                {
                                    fis.Glyph = gobottom;
                                    upDownLabel.Text = "Down";
                                }
                                else if (index == cvcontent.Count - 1)
                                {
                                    fis.Glyph = gotop;
                                    upDownLabel.Text = "Up";
                                }
                            }
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
        loadButton.IsEnabled = false;
        upDownButton.IsEnabled = false;
        upDownLabel.Text = "Wait";
        activityIndicator.IsRunning = true;
        collectionView1.Opacity = .5;
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
    }

    private async void loadCollectionView(int numberitems)
    {
        try
        {
            //otherwise bottom buttons will appear blank until CollectionView loads.
            await Task.Delay(100);
            settingsBorder.IsVisible = false;
            collectionView1.SelectedItem = null;
            loadButton.IsEnabled = false;
            upDownButton.IsEnabled = false;
            upDownLabel.Text = "Wait";
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
                    collectionView1.SelectedItem = cvcontent[^1];
                    collectionView1.ScrollTo(cvcontent[^1], null, ScrollToPosition.End, false);
                    if (upDownButton.Source != null)
                    {
                        if (upDownButton.Source is FontImageSource fis)
                            fis.Glyph = gotop;
                    }
                    upDownLabel.Text = "Up";
                    loadButton.IsEnabled = true;
                    upDownButton.IsEnabled = true;
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
        //this event handler can be triggered by setting IsChecked = true in XAML is after Checked_Changed
        //only in DEBUG mode. Was very confusing - would crash when compiled in DEBUG regardless of HotReload or launched by VS or outside VS, but run fine in Release
        //must be .NET 8.0 XAML order of things 'Enhancement'
        try
        {
            DeviceService.PlayClickSound();
            goButton.IsVisible = !navBarCheckBox.IsChecked;
            goLabel.IsVisible = !navBarCheckBox.IsChecked;
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
}