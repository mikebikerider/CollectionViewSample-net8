using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using System.Diagnostics;

namespace CollectionViewSample;

public partial class NoScrollPage : ContentPage
{
    private static string gotop = "\ue25a";
    private static string gobottom = "\ue258";

    private int loadnumber = 200;
    private int reloadnumber = 5000;
    private bool firsttime = true;
    private CVPageViewModel? vmodel;
    private Thickness safeareainsets = new Thickness(0);
    public NoScrollPage()
    {
        InitializeComponent();
        double fsize = numberItemsEntry.FontSize;
        numberItemsEntry.WidthRequest = ScreenMetrics.MeasureTextWidth("555555", fsize) + fsize + 10;
        vmodel = BindingContext as CVPageViewModel;
        if (vmodel != null)
        {
            vmodel.Fsize = nameLabel.FontSize;
            vmodel.Cw0 = ScreenMetrics.MeasureTextWidth("55555", fsize) + fsize + 10;
            vmodel.Cw1 = ScreenMetrics.MeasureTextWidth("Column 1", fsize) + fsize + 10;
            vmodel.Cw2 = ScreenMetrics.MeasureTextWidth("Column 2", fsize) + fsize + 10;
        }

#if IOS
        glyphCorrectionCheckBox.IsEnabled = false;
#else
        if (Preferences.ContainsKey("AndroidGlyphCorrection"))
            glyphCorrectionCheckBox.IsChecked = Preferences.Get("AndroidGlyphCorrection", false);
#endif

#if DEBUG
        loadnumber = 100;
        reloadnumber = Preferences.Get("Reloadnumber",200);
#else
        reloadnumber = Preferences.Get("Reloadnumber", 5000);
#endif
    }
    protected override async void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        await Task.Delay(100);
        double safewidth = width;

#if ANDROID
        collectionViewGrid.HeightRequest = height - bottomBorder.Height;
#else
        safewidth = width - safeareainsets.Left - safeareainsets.Right;
        //otherwise the grid will be sized to size larger than width by the sum of the left and right inset
        safeareainsets = On<iOS>().SafeAreaInsets();
        collectionViewGrid.HeightRequest = height - bottomBorder.Height - safeareainsets.Bottom - safeareainsets.Top;
#endif
        if (vmodel != null)
            vmodel.SafeWidth = safewidth;
        if (Debugger.IsAttached)
            Debug.WriteLine("Height allocated: " + height.ToString());
    }
    public void SetAppTheme(AppTheme apptheme)
    {
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

    private async void UpDown_Clicked(object sender, EventArgs e)
    {
        if (settingsBorder.IsVisible)
            settingsBorder.IsVisible = false;
        if (vmodel != null)
        {
            if (vmodel.Cvc != null)
            {
                if (vmodel.Cvc is List<CVcontent> cvc)
                {
                    if (cvc.Count > 0)
                    {
                        if (vmodel != null)
                        {
                            DeviceService.PlayClickSound();
                            vmodel.IsBusy = true;
                            await Task.Delay(100);
                            if (vmodel.UpDownText == "Bottom")
                            {
                                collectionView1.ScrollTo(cvc[^1], null, ScrollToPosition.End, false);
                                collectionView1.SelectedItem = cvc[^1];
                            }
                            else
                            {
                                collectionView1.ScrollTo(cvc[0], null, ScrollToPosition.Start, false);
                                collectionView1.SelectedItem = cvc[0];
                            }
                            vmodel.IsBusy = false;
                        }
                    }
                }
            }
        }
    }

    private async void CollectionView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int index;
        if (vmodel != null)
        {
            List<CVcontent> cvcontent = vmodel.Cvc;
            if (collectionView1.SelectedItem != null)
            {
                if (collectionView1.SelectedItem is CVcontent cvc)
                {
                    index = vmodel.Cvc.IndexOf(cvc);
                    for (int i = 0; i < cvcontent.Count; i++)
                    {
                        if (i == index)
                            cvcontent[i].IsSelected = true;
                        else
                        {
                            if (cvcontent[i].IsSelected)
                                cvcontent[i].IsSelected = false;
                        }
                    }

                    //changing glyph of a disabled button may cause complications
                    if (index == 0)
                    {
                        if (vmodel.IsBusy)
                        {
                            vmodel.IsBusy = false;
                            await Task.Delay(100);
                        }
                        vmodel.UpDownGlyph = gobottom;
                        vmodel.UpDownText = "Bottom";
                    }
                    else if (index == cvcontent.Count - 1)
                    {
                        if (vmodel.IsBusy)
                        {
                            vmodel.IsBusy = false;
                            await Task.Delay(100);
                        }
                        vmodel.UpDownGlyph = gotop;
                        vmodel.UpDownText = "Top";
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
        {
            reloadnumber = Preferences.Get("Reloadnumber",reloadnumber);
            loadCollectionView(reloadnumber);
        }
    }

    private async void Page_Appearing(object sender, EventArgs e)
    {
        reloadnumber = Preferences.Get("Reloadnumber", reloadnumber);
        numberItemsEntry.Text = reloadnumber.ToString();
        if (firsttime)
        {
            try
            {
                //otherwise bottom bar graphics are not displayed until ColllectionView loads
                await Task.Delay(100);
                loadCollectionView(loadnumber);
            }
            catch { }
        }

        else
        {
#if ANDROID
            if (Preferences.Get("AndroidGlyphCorrection",false))
            {
                if (vmodel != null)
                    vmodel.UpdateUpDownGlyph();
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
                vmodel.UpDownText = "Wait";
                vmodel.Fsize = nameLabel.FontSize;
                await Task.Delay(100);
                bool isLightTheme = AppInfo.RequestedTheme.Equals(AppTheme.Light);
                List<CVcontent> cvl = new List<CVcontent>();
                for (int i = 0; i < numberitems; i++)
                {
                    cvl.Add(new CVcontent { IsLightTheme = isLightTheme, ItemNumber = i + 1, FirstName = Path.GetRandomFileName().Replace(".", ""), LastName = Path.GetRandomFileName().Replace(".", "") });
                }
                vmodel.Cvc = cvl;
                await Task.Delay(100);
                List<CVcontent> cvc = vmodel.Cvc;
#if iOS
                        collectionViewGrid.WidthRequest = vmodel.ContentWidth; //Math.Max(w[0] + w[1] + w[2], Width) - safeareainsets.Left - safeareainsets.Right;
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
        if (settingsBorder.IsVisible)
            settingsBorder.IsVisible = false;
        await Shell.Current.GoToAsync("//hscroll");
    }

    private void SettingsButton_Clicked(object sender, EventArgs e)
    {
        DeviceService.PlayClickSound();
        if (settingsBorder.IsVisible)
            settingsBorder.IsVisible = false;
        else
        {
#if ANDROID
            glyphCorrectionCheckBox.IsChecked = Preferences.Get("AndroidGlyphCorrection", false);
#endif
            settingsBorder.IsVisible = true;
        }
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
                        Preferences.Set("Reloadnumber", num);
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
        Preferences.Set("AndroidGlyphCorrection", e.Value);
    }
}