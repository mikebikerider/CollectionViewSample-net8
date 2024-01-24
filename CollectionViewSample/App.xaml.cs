namespace CollectionViewSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            try
            {
                if (Current != null)
                {
                    Current.RequestedThemeChanged += (s, a) =>
                    {
                        if (MainPage != null)
                        {
                            AppShell appShell = (AppShell)MainPage;
                            if (appShell != null)
                            {
                                if (appShell.noscrollPage != null)
                                    appShell.noscrollPage.SetAppTheme(a.RequestedTheme);
                                if (appShell.hscrollPage != null)
                                    appShell.hscrollPage.SetAppTheme(a.RequestedTheme);
                            }
                        }
                    };
                }
            }
            catch { }
            MainPage = new AppShell();
        }
    }
}
