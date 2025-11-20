using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Img = SixLabors.ImageSharp.Image;
using Newsletter.Effects.Traits;

namespace Newsletter.Effects.Impl;

public class Image : ImageIO
{
    public static readonly ImageIO Default = new Image();
    
    public byte[] ScaleToMaximumWidthJpeg(ReadOnlySpan<byte> inputBytes, int maxWidthInPixels)
    {
        using var image = Img.Load(inputBytes);
        if (image.Width < maxWidthInPixels) return inputBytes.ToArray();
        var height = (int)((double)image.Height * maxWidthInPixels / image.Width);
        image.Mutate(x => x.Resize(maxWidthInPixels,  height));
        using var stream = new MemoryStream();
        image.SaveAsJpeg(stream);
        return stream.ToArray();
    }
}
