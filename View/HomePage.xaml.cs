using AceBook.ViewModel;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace AceBook.View;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var paint = new SKPaint
        {
            Color = SKColors.DarkGray,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        float radius = 4f;
        float centerX = e.Info.Width / 2;
        float centerY = e.Info.Height / 2;
        float spacing = 12f; // Adjust spacing between dots

        // Draw three dots in a horizontal alignment
        canvas.DrawCircle(centerX - spacing, centerY, radius, paint);
        canvas.DrawCircle(centerX, centerY, radius, paint);
        canvas.DrawCircle(centerX + spacing, centerY, radius, paint);
    }

}