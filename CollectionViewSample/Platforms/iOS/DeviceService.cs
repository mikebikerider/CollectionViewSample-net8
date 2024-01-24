using AudioToolbox;

namespace CollectionViewSample
{
    public static class DeviceService
    {
        private static void PlaySound(uint snd)
        {
            var sound = new SystemSound(snd);
            sound.PlaySystemSound();
        }
        public static void PlayClickSound()
        {
            var sound = new SystemSound(1104);
            sound.PlaySystemSound();
        }
    }
}
