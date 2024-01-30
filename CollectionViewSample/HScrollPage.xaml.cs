using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Diagnostics;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace CollectionViewSample
{
    public partial class HScrollPage : ContentPage
    {
        private readonly ImageSource uparrow = ImageSource.FromFile("uparrow.png");
        private readonly ImageSource uparrowgray = ImageSource.FromFile("uparrowgray.png");
        private readonly ImageSource downarrow = ImageSource.FromFile("downarrow.png");
        private readonly ImageSource downarrowgray = ImageSource.FromFile("downarrowgray.png");
        private readonly ImageSource load = ImageSource.FromFile("download.png");
        private readonly ImageSource loadgray = ImageSource.FromFile("downloadgray.png");
        private readonly ImageSource time = ImageSource.FromFile("time.png");
        private int loadnumber = 100;
        private int reloadnumber = 5000;
        private bool firsttime = true;

        public HScrollPage()
        {
            InitializeComponent();
#if DEBUG
            loadnumber = 50;
            reloadnumber = 200;
#endif            
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            //there is no need for overriding in Xamarin.Forms
            //iOS net8.0 after screen is rotated the bottom rows do not scroll into view
            base.OnSizeAllocated(width, height);
            if (collectionView1.ItemsSource != null)
            {
                if (collectionView1.ItemsSource is List<CVcontent> cvc)
                {
                    if (cvc.Count > 0)
                    {
                        double cw = Math.Max(width, cvc[0].Cw);
#if ANDROID
                        for (int i = 0; i < cvc.Count; i++)
                        {
                            cvc[i].W = cw;
                        }
                        //collectionViewGrid resizes correctly
                        //collectionView does not
                        collectionView1.WidthRequest = cw;
#else
                        //on iPhone with the nothch or dynamic island Grid inside a ScrollView is not resized corretly
                        //dimensions seem miscalcullated by safe area insets values and require correction
                        //nothing like that in Xamarin.Forms
                        Thickness safeareainsets = On<iOS>().SafeAreaInsets();
                        collectionViewGrid.HeightRequest = height - bottomBorder.Height - safeareainsets.Bottom;
                        Debug.WriteLine("Left inset: " + safeareainsets.Left.ToString());
                        Debug.WriteLine("Right inset: " + safeareainsets.Right.ToString());
                        if (width > height)
                            collectionViewGrid.WidthRequest = cw - safeareainsets.Left - safeareainsets.Right;
                        else
                            collectionViewGrid.WidthRequest = cw;
#endif
                    }
                }
            }


            //not need for that in Xamarin.Forms
#if ANDROID
            //in a real app the minimum bottomBar width must be calculated
            //bottomGrid has a star column that should adjust the width automaticlly without width calculation
            //this seems to be a residual of treating star columns and rows as auto introduced early on due to somebodies dumb suggestion
            bottomGrid.WidthRequest = width;
            //without this line loading is very slow to forever, scrolling jerky the app might even freeze when orientation changes
            collectionViewGrid.HeightRequest = height - bottomBorder.Height;
            //issuing the above line in iOS does no good
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
            Debug.WriteLine("Font size:" + fsize.ToString());
            double w0 = ScreenMetrics.MeasureTextWidth("4444", fsize) + fsize + 10;
            double w1 = ScreenMetrics.MeasureTextWidth("Column 1",fsize) + fsize + 10;
            double w2 = ScreenMetrics.MeasureTextWidth("Column 2",fsize) + fsize + 10;
            double w3 = ScreenMetrics.MeasureTextWidth("Column 3", fsize) + fsize + 10;
            for (int i = 0; i < cvc.Count; i++)
            {
                w0 = Math.Max(w0, ScreenMetrics.MeasureTextWidth(cvc[i].ItemNo, fsize) + fsize + 10);
                w1 = Math.Max(w1, ScreenMetrics.MeasureTextWidth(cvc[i].FirstName, fsize) + fsize + 10);
                w2 = Math.Max(w2, ScreenMetrics.MeasureTextWidth(cvc[i].LastName, fsize) + fsize + 10);
                w3 = Math.Max(w3, ScreenMetrics.MeasureTextWidth(cvc[i].Occupation,fsize) + fsize + 10);

            }
            double w = Math.Max(Width, w0 + w1 + w2 + w3);
            for (int i = 0; i < cvc.Count; i++)
            {
                cvc[i].Cw0 = w0;
                cvc[i].Cw1 = w1;
                cvc[i].Cw2 = w2;
                cvc[i].Cw3 = w3;
#if ANDROID
                cvc[i].W = w;
#endif
            }
            return [w0,w1,w2,w3];
        }
        private void CollectionViewItem_Tapped(object sender, TappedEventArgs e)
        {
            if (settingsBorder.IsVisible)
                settingsBorder.IsVisible = false;
            if (collectionView1.ItemsSource != null)
            {
                if (collectionView1.ItemsSource is  List<CVcontent>)
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
                if (collectionView1.ItemsSource is List<CVcontent> cvcont)
                {
                    if (cvcont.Count > 0)
                    {
                        DeviceService.PlayClickSound();
                        upDownButton.IsEnabled = false;
                        loadButton.IsEnabled = false;
#if ANDROID
                        loadButton.Source = loadgray;
#endif
                        collectionView1.Opacity = .5;
                        activityIndicator.IsRunning = true;
                        if (upDownLabel.Text == "Down")
                        {
#if ANDROID
                            upDownButton.Source = downarrowgray;
#endif
                            await Task.Delay(100);
                            collectionView1.ScrollTo(cvcont[^1],null, ScrollToPosition.MakeVisible, false);
                            upDownButton.Source = uparrow;
                            upDownLabel.Text = "Up";
                            upDownButton.IsEnabled = true;
                            activityIndicator.IsRunning = false;
                            collectionView1.Opacity = 1;
                            collectionView1.SelectedItem = cvcont[^1];
                        }
                        else if (upDownLabel.Text == "Up")
                        {
#if ANDROID
                            upDownButton.Source = uparrowgray;
#endif
                            await Task.Delay(100);
                            collectionView1.ScrollTo(cvcont[0], null, ScrollToPosition.Start, false);
                            upDownButton.Source = downarrow;
                            upDownLabel.Text = "Down";
                            upDownButton.IsEnabled = true;
                            activityIndicator.IsRunning = false;
                            collectionView1.Opacity = 1;
                            collectionView1.SelectedItem = cvcont[0];
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
#if ANDROID
            upDownButton.Source = time;
            upDownLabel.Text = "Wait";
#endif
            activityIndicator.IsRunning = true;
            collectionView1.Opacity = .5;
            if (firsttime)
                loadCollectionView(loadnumber);
            else
                loadCollectionView(reloadnumber);
        }
        private async void Page_Appearing(object sender, EventArgs e)
        {
            if (firsttime)
            {
                numberItemsEntry.Text = reloadnumber.ToString();
                collectionView1.ItemsSource = new List<CVcontent>();
                double fsize = numberItemsEntry.FontSize;
                numberItemsEntry.WidthRequest = ScreenMetrics.MeasureTextWidth("55555", fsize) + fsize + 10;
                await Task.Delay(500);
                loadCollectionView(loadnumber);
            }
        }
        
        private async void loadCollectionView(int numberitems)
        {
            try
            {
                if (collectionView1.ItemsSource != null)
                {
                    if (collectionView1.ItemsSource is List<CVcontent> _cvc)
                    {
                        settingsBorder.IsVisible = false;
                        collectionView1.SelectedItem = null;
                        loadButton.IsEnabled = false;
#if ANDROID
                        loadButton.Source = loadgray;
                        upDownButton.Source = time;
                        upDownLabel.Text = "Wait";
#endif
                        activityIndicator.IsRunning = true;
                        await Task.Delay(500);
                        List<CVcontent> cvc = new();
                        bool isLightTheme = AppInfo.RequestedTheme.Equals(AppTheme.Light);
                        for (int i = 0; i < numberitems; i++)
                        {
                            cvc.Add(new CVcontent { IsLightTheme = isLightTheme, ItemNumber = i + 1, FirstName = Path.GetRandomFileName().Replace(".", ""), LastName = Path.GetRandomFileName().Replace(".", ""), Occupation = Path.GetRandomFileName().Replace(".", "") });
                        }
                        double[] w = ContentColumnsWidth(cvc);
                        nameLabel.Text = "";
                        headerGrid.ColumnDefinitions[0].Width = new GridLength(w[0]);
                        headerGrid.ColumnDefinitions[1].Width = new GridLength(w[1]);
                        headerGrid.ColumnDefinitions[2].Width = new GridLength(w[2]);
#if ANDROID
                        collectionView1.WidthRequest = Math.Max(w[0] + w[1] + w[2] + w[3], Width);
#endif
                        loadButton.Source = load;
                        nameLabel.Text = "Column 1";
                        collectionView1.ItemsSource = cvc;
//#if IOS
                        collectionView1.SelectedItem = cvc[^1];
                        collectionView1.ScrollTo(cvc[^1], null, ScrollToPosition.End, false);
                        upDownButton.Source = uparrow;
                        upDownLabel.Text = "Up";
/*
#else
                        collectionView1.SelectedItem = cvc[0];
                        upDownButton.Source = downarrow;
                        loadButton.Source = load;
                        upDownLabel.Text = "Down";
#endif
                        */
                        loadButton.IsEnabled = true;
                        collectionView1.Opacity = 1;
                        firsttime = false;
                        activityIndicator.IsRunning = false;
                    }
                }
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
        }
        private async void MeasureItems_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            DeviceService.PlayClickSound();
            activityIndicator.IsRunning = true;
            settingsBorder.IsVisible = false;
            await Task.Delay(500);
            if (e.Value)
                collectionView1.ItemSizingStrategy = ItemSizingStrategy.MeasureAllItems;
            else
                collectionView1.ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem;
            activityIndicator.IsRunning = false;
            settingsBorder.IsVisible = false;
        }
        private async void GoButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//noscroll");
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
                    Debug.WriteLine("HScrlollPage.ShowNavigationBarChecked_Changed() error: " + x.Message);
            }
        }

        private void NumberItems_Changed(object sender, TextChangedEventArgs e)
        {
            //if text set in XAML after the TextChanged event handler then the event will be triggered while page not shown
            //not a behavior I am used to or agree with
            int.TryParse(e.NewTextValue, out reloadnumber);
        }

        private void SettingsLabel_Tapped(object sender, TappedEventArgs e)
        {
            settingsBorder.IsVisible = false;
        }

        private void NumberItems_Unfocused(object sender, FocusEventArgs e)
        {
#if IOS
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
#endif
        }

        private void NumberItems_Completed(object sender, EventArgs e)
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
    }
}
