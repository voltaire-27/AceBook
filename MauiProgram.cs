using AceBook.View;
using AceBook.ViewModel;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using The49.Maui.BottomSheet;
using UraniumUI;

namespace AceBook
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseUraniumUI() // Add UraniumUI handlers
                .UseUraniumUIMaterial() // Add Material Design for UraniumUI
                .UseMauiCommunityToolkit()
                .UseSkiaSharp() // Enable SkiaSharp
                 .UseBottomSheet()
                   .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFontAwesomeIconFonts();
                });

         

            // Register pages and view models
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<RegisterPage>();
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddTransient<AddPost>();

            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<RegisterViewModel>();
            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddTransient<AddPostViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}