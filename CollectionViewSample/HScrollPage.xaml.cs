using System.Diagnostics;
using System.Text;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;


namespace CollectionViewSample
{
    public partial class HScrollPage : ContentPage
    {
        private static string gotop = "\ue25a";
        private static string gobottom = "\ue258";
        private int loadnumber = 100;
        private int reloadnumber = 5000;
        private bool firsttime = true;
        private CVPageViewModel? vmodel;
        private Thickness safeareainsets = new Thickness(0);

        public HScrollPage()
        {
            InitializeComponent();
            vmodel = BindingContext as CVPageViewModel;
#if DEBUG
            loadnumber = 50;
            reloadnumber = Preferences.Get("Reloadnumber",100);     
#else
            reloadnumber = Preferences.Get("Reloadnumber", 5000);
#endif
            double fsize = numberItemsEntry.FontSize;

            if (vmodel != null)
                vmodel.Fsize = nameLabel.FontSize;

#if IOS
            glyphCorrectionCheckBox.IsEnabled = false;
            Microsoft.Maui.Handlers.ScrollViewHandler.Mapper.AppendToMapping("Disable_Bounce", (handler, view) =>
            {
                handler.PlatformView.Bounces = false;
            });
            //no handler for disabling CollectionView bouncing
#else
            if (Preferences.ContainsKey("AndroidGlyphCorrection"))
                glyphCorrectionCheckBox.IsChecked = Preferences.Get("AndroidGlyphCorrection", false);
#endif

        }

        protected async override void OnSizeAllocated(double width, double height)
        {
            //there is no need for overriding in Xamarin.Forms
            //iOS net8.0 after screen is rotated the bottom rows do not scroll into view
            base.OnSizeAllocated(width, height);
            double safewidth = width;
            if (vmodel != null)
            {
                vmodel.IsBusy = true;
                await Task.Delay(100);
#if ANDROID
                //without this line loading is very slow to forever
                vmodel.CvgHeight = height - bottomBorder.Height;
#else
                //on iPhone with the nothch or dynamic island Grid inside a ScrollView is not resized corretly
                //dimensions seem miscalcullated by safe area insets values and require correction
                safeareainsets = On<iOS>().SafeAreaInsets();
                safewidth = width - safeareainsets.Left - safeareainsets.Right;
#endif

               vmodel.SafeWidth = safewidth;
                if (vmodel.CvcCount > 0)
                {
                    await Task.Delay(100);
                    vmodel.CorrectStarColumnWidth(width);
                }
#if IOS
                vmodel.CvgHeight = height - bottomBorder.Height - safeareainsets.Bottom - safeareainsets.Top;
#endif
                vmodel.IsBusy = false;
            }
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

        private void CollectionViewItem_Tapped(object sender, TappedEventArgs e)
        {
            if (vmodel != null)
            {
                if (vmodel.SettingsVisible)
                    vmodel.SettingsVisible = false;
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
        private async void UpDown_Clicked(object sender, EventArgs e)
        {
            if (vmodel != null)
            {
                if (vmodel.SettingsVisible)
                    vmodel.SettingsVisible = false;
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


        private async void CollectionView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                        if (index == 0)
                        {
                            //changing glyph of a disabled button may cause complications
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
            if (vmodel != null)
            {
                DeviceService.PlayClickSound();
                if (vmodel.SettingsVisible)
                    vmodel.SettingsVisible = false;
                if (firsttime)
                    loadCollectionView(loadnumber);
                else
                {
                    reloadnumber = Preferences.Get("Reloadnumber", reloadnumber);
                    loadCollectionView(reloadnumber);
                }
            }
        }
        private async void Page_Appearing(object sender, EventArgs e)
        {
            reloadnumber = Preferences.Get("Reloadnumber", reloadnumber);
            numberItemsEntry.Text = reloadnumber.ToString();
            if (firsttime)
            {
                //otherwise bottom bar graphics are not displayed until ColllectionView loads
                await Task.Delay(100);
                loadCollectionView(loadnumber);
            }
            else
            {
#if ANDROID
                if (Preferences.Get("AndroidGlyphCorrection", false))
                {
                    if (vmodel != null)
                        vmodel.UpdateUpDownGlyph();
                }
#endif
            }
        }
        private string RandomString(int length)
        {
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder sb = new StringBuilder();
            Random random = new Random((int)DateTime.Now.Ticks);
            for (var i = 0; i < length; i++)
            {
                char c = pool[random.Next(0, pool.Length)];
                sb.Append(c);
            }
            return sb.ToString();
        }
        private async void loadCollectionView(int numberitems)
        {
            try
            {
                collectionView1.SelectedItem = null;
                if (vmodel != null)
                {
                    vmodel.SettingsVisible = false;
                    vmodel.IsBusy = true;
                    vmodel.UpDownText = "Wait";
                    await Task.Delay(100);
                    vmodel.Cvc = new();
                    await Task.Delay(100);
                    bool isLightTheme = AppInfo.RequestedTheme.Equals(AppTheme.Light);
                    List<CVcontent> cvl = new List<CVcontent>();
                    for (int i = 0; i < numberitems; i++)
                    {
                        cvl.Add(new CVcontent { IsLightTheme = isLightTheme, ItemNumber = i + 1, FirstName = RandomString(10), LastName = RandomString(15), Occupation = RandomString(20) });
                    }
                    vmodel.Cvc = cvl;
                    await Task.Delay(100);
                    List<CVcontent> cvc = vmodel.Cvc;
                    await Task.Delay(100);
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
            if (vmodel != null)
            {

                DeviceService.PlayClickSound();
                vmodel.IsBusy = true;
                vmodel.SettingsVisible = false;
                await Task.Delay(500);
                if (e.Value)
                    collectionView1.ItemSizingStrategy = ItemSizingStrategy.MeasureAllItems;
                else
                    collectionView1.ItemSizingStrategy = ItemSizingStrategy.MeasureFirstItem;
                vmodel.IsBusy = false;
            }
        }
        private async void GoButton_Clicked(object sender, EventArgs e)
        {
            if (vmodel != null)
            {
                if (vmodel.SettingsVisible)
                    vmodel.SettingsVisible = false;
                await Shell.Current.GoToAsync("//noscroll");
            }
        }
        private void SettingsButton_Clicked(object sender, EventArgs e)
        {
            if (vmodel != null)
            {
                
            DeviceService.PlayClickSound();
                if (vmodel.SettingsVisible)
                    vmodel.SettingsVisible = false;
                else
                {
#if ANDROID
                glyphCorrectionCheckBox.IsChecked = Preferences.Get("AndroidGlyphCorrection", false);
#endif
                    vmodel.SettingsVisible = true;
                }
            }
        }

        private async void ShowNavigationBarChecked_Changed(object sender, CheckedChangedEventArgs e)
        {
            //this event handler can be triggered by setting IsChecked = true in XAML is after Checked_Changed
            //only in DEBUG mode. Was very confusing - would crash when compiled in DEBUG regardless of HotReload or launched by VS or outside VS, but run fine in Release
            //must be .NET 8.0 XAML order of things 'Enhancement'
            if (vmodel != null)
            {
                try
                {
                    DeviceService.PlayClickSound();
                    vmodel.SettingsVisible = false;
                    Shell.SetNavBarIsVisible(this, navBarCheckBox.IsChecked);
                    if (Debugger.IsAttached)
                    {
                        await Task.Delay(200);
                        Debug.WriteLine("Height: " + Height.ToString());
                    }
                }
                catch (Exception x)
                {
                    if (Debugger.IsAttached)
                        Debug.WriteLine("HScrlollPage.ShowNavigationBarChecked_Changed() error: " + x.Message);
                }
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
            if (vmodel != null)
                vmodel.SettingsVisible = false;
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
                        Preferences.Set("Reloadnumber", num);
                        loadCollectionView(num);
                    }
                }
            }
        }
        private void GlyphCorrectionChecked_Changed(object sender, CheckedChangedEventArgs e)
        {
            Preferences.Set("AndroidGlyphCorrection", e.Value);
        }
    }
}
