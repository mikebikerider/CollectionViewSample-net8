using System.Diagnostics;
namespace CollectionViewSample;

public partial class NoScrollPage : ContentPage
{
    private readonly ImageSource uparrow = ImageSource.FromFile("uparrow.png");
    private readonly ImageSource uparrowgray = ImageSource.FromFile("uparrowgray.png");
    private readonly ImageSource downarrow = ImageSource.FromFile("downarrow.png");
    private readonly ImageSource downarrowgray = ImageSource.FromFile("downarrowgray.png");
    private readonly ImageSource load = ImageSource.FromFile("download.png");
    private readonly ImageSource loadgray = ImageSource.FromFile("downloadgray.png");
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
        //not need for that in Xamarin.Forms and MAUI iOS
        bottomGrid.WidthRequest = width;
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
    private double[] ContentColumnsWidth(List<CVcontent> cvc)
    {
        double fsize = nameLabel.FontSize;
        double w0 = ScreenMetrics.MeasureTextWidth("5555", fsize) + fsize + 10;
        double w1 = ScreenMetrics.MeasureTextWidth("Column 1", fsize) + fsize;
        double w2 = ScreenMetrics.MeasureTextWidth("Column 2", fsize) + fsize;
        for (int i = 0; i < cvc.Count; i++)
        {
            w0 = Math.Max(w0, ScreenMetrics.MeasureTextWidth(cvc[i].ItemNo, fsize) + fsize + 10); //right margin 10
            w1 = Math.Max(w1, ScreenMetrics.MeasureTextWidth(cvc[i].FirstName, fsize) + fsize);
            w2 = Math.Max(w2, ScreenMetrics.MeasureTextWidth(cvc[i].LastName, fsize) + fsize);
        }
        double w = Math.Max(Width, w0 + w1 + w2);
        for (int i = 0; i < cvc.Count; i++)
        {
            cvc[i].Cw0 = w0;
            cvc[i].Cw1 = w1;
            cvc[i].Cw2 = w2;
        }
        return [w0, w1, w2];
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
                    DeviceService.PlayClickSound();
                    upDownButton.IsEnabled = false;
                    loadButton.IsEnabled = false;
                    collectionView1.Opacity = .5;
                    activityIndicator.IsRunning = true;
                    if (upDownLabel.Text == "Down")
                    {
#if ANDROID
                        upDownButton.Source = downarrowgray;
#endif
                        await Task.Delay(100);
                        collectionView1.ScrollTo(cvc[^1], null, ScrollToPosition.End, false);
                        upDownButton.Source = uparrow;
                        upDownLabel.Text = "Up";
                        upDownButton.IsEnabled = true;
                        activityIndicator.IsRunning = false;
                        collectionView1.Opacity = 1;
                        collectionView1.SelectedItem = cvc[^1];
                    }
                    else if (upDownLabel.Text == "Up")
                    {
#if ANDROID
                        upDownButton.Source = uparrowgray;
#endif
                        await Task.Delay(100);
                        upDownButton.Source = downarrow;
                        upDownLabel.Text = "Down";
                        upDownButton.IsEnabled = true;
                        activityIndicator.IsRunning = false;
                        collectionView1.Opacity = 1;
                        collectionView1.SelectedItem = cvc[0];
                        collectionView1.ScrollTo(cvc[0], null, ScrollToPosition.Start, false);
                    }
                    loadButton.IsEnabled = true;
#if ANDROID
                    loadButton.Source = load; ;
#endif
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
                            upDownButton.Source = "downarrow.png";
                            upDownLabel.Text = "Down";
                        }
                        else if (index == cvcontent.Count - 1)
                        {
                            upDownButton.Source = "uparrow.png";
                            upDownLabel.Text = "Up";
                        }
                        upDownButton.IsEnabled = true;
#if ANDROID
                        loadButton.Source = load;
#endif
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
#if ANDROID
        loadButton.Source = loadgray;
#endif
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
                numberItemsEntry.WidthRequest = ScreenMetrics.MeasureTextWidth("55555", fsize) + fsize + 10;
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
            settingsBorder.IsVisible = false;
            collectionView1.SelectedItem = null;
            loadButton.IsEnabled = false;
#if ANDROID
            loadButton.Source = loadgray;
#endif
            upDownLabel.Text = "Wait";
            activityIndicator.IsRunning = true;
            await Task.Delay(100);
            List<CVcontent> cvc = new List<CVcontent>();
            bool isLightTheme = AppInfo.RequestedTheme.Equals(AppTheme.Light);
            for (int i = 0; i < numberitems; i++)
            {
                cvc.Add(new CVcontent { IsLightTheme = isLightTheme, ItemNumber = i + 1, FirstName = Path.GetRandomFileName().Replace(".", ""), LastName = Path.GetRandomFileName().Replace(".", "") });
            }
            double[] w = ContentColumnsWidth(cvc);
            nameLabel.Text = "";
            headerGrid.ColumnDefinitions[0].Width = new GridLength(w[0]);
            headerGrid.ColumnDefinitions[1].Width = new GridLength(w[1]);
            //dumb trick to force header repaint after column definitions changed
            //is not required in Xamarin.Forms
            nameLabel.Text = "Column 1";
            collectionView1.ItemsSource = cvc;
            collectionView1.SelectedItem = cvc[^1];
            collectionView1.ScrollTo(cvc[^1], null, ScrollToPosition.End, false);
            upDownButton.Source = uparrow;
            upDownLabel.Text = "Up";
            loadButton.IsEnabled = true;
            collectionView1.Opacity = 1;
            firsttime = false;
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