using Android.Views;


namespace CollectionViewSample
{
    public static class DeviceService
    {
        private static void playSound(SoundEffects sf)
        {
            try
            {
                if (Platform.CurrentActivity != null)
                {
                    if (Platform.CurrentActivity.Window != null)
                    {
                        var root = Platform.CurrentActivity.Window.DecorView.RootView;
                        if (root != null)
                        {
                            if (root.SoundEffectsEnabled)
                                root.PlaySoundEffect(sf);
                        }
                    }
                }
            }
            catch { }
        }
        public static void PlayClickSound()
        {
            playSound(SoundEffects.Click);
        }
    }
}
