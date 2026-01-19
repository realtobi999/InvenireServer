using InvenireServer.Application.Interfaces.Common;
using QRCoder;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace InvenireServer.Infrastructure.Utilities.QR;

/// <summary>
/// Default implementation of <see cref="IQuickResponseCodeGenerator"/>.
/// </summary>
public class QuickResponseCodeGenerator : IQuickResponseCodeGenerator
{
    private const int MinSize = IQuickResponseCodeGenerator.MinimumSize;
    private const int MaxSize = IQuickResponseCodeGenerator.MaximumSize;

    private const int MinFont = 8;
    private const float TextFraction = 1f / 6f;
    private const float FontFraction = 0.5f;

    private static readonly string FontPath = Path.Combine(AppContext.BaseDirectory, "assets", "fonts", "G_ari_bd.ttf");

    /// <summary>
    /// Generates a PNG-encoded QR code image.
    /// </summary>
    /// <param name="content">Content encoded in the QR code.</param>
    /// <param name="size">Image size in pixels.</param>
    /// <returns>PNG-encoded QR code image.</returns>
    public byte[] GenerateCode(string content, int size = MinSize)
    {
        return GenerateCodeWithLabels(content, [], size);
    }

    /// <summary>
    /// Generates a PNG-encoded QR code image with labels.
    /// </summary>
    /// <param name="content">Content encoded in the QR code.</param>
    /// <param name="labels">Labels rendered beneath the code.</param>
    /// <param name="size">Image size in pixels.</param>
    /// <returns>PNG-encoded QR code image.</returns>
    public byte[] GenerateCodeWithLabels(string content, IReadOnlyList<string> labels, int size = MinSize)
    {
        if (size < MinSize || size > MaxSize)
        {
            throw new ArgumentException($"Size must be between {MinSize} and {MaxSize}. Received: {size}.");
        }

        var labelSpace = labels.Count != 0 ? (int)(size * TextFraction) : 0;
        var qrSpace = size - labelSpace;

        var qrImage = LoadQr(content);
        var scale = Math.Min((float)qrSpace / qrImage.Width, (float)qrSpace / qrImage.Height);

        using var canvas = new Image<Rgba32>(size, size);
        canvas.Mutate(ctx =>
        {
            ctx.Fill(Color.White);
            DrawQr(ctx, qrImage, size, qrSpace, scale);
            if (labels.Count != 0) DrawLabels(ctx, [.. labels], size, qrSpace, labelSpace);
        });

        return ToPng(canvas);
    }

    private static Image<Rgba32> LoadQr(string content)
    {
        using var gen = new QRCodeGenerator();
        using var data = gen.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var png = new PngByteQRCode(data);
        return Image.Load<Rgba32>(png.GetGraphic(20));
    }

    private static void DrawQr(IImageProcessingContext ctx, Image<Rgba32> qr, int size, int qrSpace, float scale)
    {
        var w = (int)(qr.Width * scale);
        var h = (int)(qr.Height * scale);

        var x = (size - w) / 2;
        var y = (qrSpace - h) / 2;

        ctx.DrawImage(qr.Clone(c => c.Resize(w, h)), new Point(x, y), 1f);
    }

    private static void DrawLabels(IImageProcessingContext ctx, List<string> labels, int size, int top, int height)
    {
        var font = new FontCollection().Add(FontPath).CreateFont(Math.Max(height * FontFraction, MinFont), FontStyle.Bold);

        var measurements = labels.Select(l => TextMeasurer.MeasureSize(l, new TextOptions(font))).ToList();
        var totalHeight = measurements.Sum(m => m.Height) + (labels.Count - 1) * 4;

        var y = top + (height - totalHeight) / 2f;

        for (int i = 0; i < labels.Count; i++)
        {
            var label = labels[i];
            var labelSize = measurements[i];
            var x = (size - labelSize.Width) / 2f;

            ctx.DrawText(label, font, Color.Black, new PointF(x, y));
            y += labelSize.Height + 4;
        }
    }

    private static byte[] ToPng(Image<Rgba32> img)
    {
        using var ms = new MemoryStream();
        img.SaveAsPng(ms);
        return ms.ToArray();
    }
}
