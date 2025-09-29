using InvenireServer.Application.Interfaces.Common;
using QRCoder;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace InvenireServer.Infrastructure.Utilities.QR;

public class QuickResponseCodeGenerator : IQuickResponseCodeGenerator
{
    private const int MinimumFontSize = 8;
    private const float TextSizeFraction = 0.5f;
    private const float TextSpaceFraction = 1f / 6f;
    private const int QrGraphicPixelsPerModule = 20;

    private static readonly string FontPath = Path.Combine(AppContext.BaseDirectory, "assets", "fonts", "G_ari_bd.ttf");

    public byte[] GenerateCode(string content, int width = 150, int height = 150)
    {
        return GenerateCodeWithLabel(content, null, width, height);
    }

    public byte[] GenerateCodeWithLabel(string content, string? label, int width = 150, int height = 150)
    {
        using var qr = Image.Load<Rgba32>(GenerateQrImageBytes(content));

        var textSpace = !string.IsNullOrEmpty(label) ? (int)(height * TextSpaceFraction) : 0;
        var qrSpaceHeight = height - textSpace;

        var scale = CalculateScale(qr.Width, qr.Height, width, qrSpaceHeight);
        var scaledQrWidth = (int)(qr.Width * scale);
        var scaledQrHeight = (int)(qr.Height * scale);

        using var image = new Image<Rgba32>(width, height);

        image.Mutate(ctx =>
        {
            ctx.Fill(Color.White);
            DrawCenteredQrCode(ctx, qr, width, qrSpaceHeight, scaledQrWidth, scaledQrHeight);

            if (!string.IsNullOrEmpty(label))
            {
                DrawCenteredLabel(ctx, label!, width, qrSpaceHeight, textSpace);
            }
        });

        return ConvertToPngBytes(image);
    }

    private static byte[] GenerateQrImageBytes(string content)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var png = new PngByteQRCode(data);
        return png.GetGraphic(QrGraphicPixelsPerModule);
    }

    private static float CalculateScale(int qrWidth, int qrHeight, int targetWidth, int targetHeight)
    {
        return Math.Min((float)targetWidth / qrWidth, (float)targetHeight / qrHeight);
    }

    private static void DrawCenteredQrCode(IImageProcessingContext ctx, Image<Rgba32> qrImage, int canvasWidth, int canvasHeight, int scaledWidth, int scaledHeight)
    {
        var x = (canvasWidth - scaledWidth) / 2;
        var y = (canvasHeight - scaledHeight) / 2;

        ctx.DrawImage(qrImage.Clone(c => c.Resize(scaledWidth, scaledHeight)), new Point(x, y), 1f);
    }

    private static void DrawCenteredLabel(IImageProcessingContext ctx, string label, int canvasWidth, int qrSpaceHeight, int textSpace)
    {
        var font = new FontCollection().Add(FontPath).CreateFont(Math.Max(textSpace * TextSizeFraction, MinimumFontSize), FontStyle.Bold);

        var text = new RichTextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = new PointF(canvasWidth / 2f, qrSpaceHeight + textSpace / 2f)
        };

        ctx.DrawText(text, label, Color.Black);
    }


    private static byte[] ConvertToPngBytes(Image<Rgba32> image)
    {
        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        return ms.ToArray();
    }
}
