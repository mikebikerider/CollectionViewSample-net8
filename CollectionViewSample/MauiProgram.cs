using Microsoft.Extensions.Logging;

namespace CollectionViewSample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>()
                        .ConfigureFonts(fonts =>
                        {
                            fonts.AddFont("MaterialIconsOutlined-Regular.otf", "MaterialIconsOutlined-Regular");
                            fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons-Regular");

                        })
            .ConfigureMauiHandlers(handlers =>
            {
                //The handler will only be called if the target platform is iOS
#if IOS
                handlers.AddHandler<Entry, EntryHandler>();
#endif
            });
            

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
