namespace Newsletter.Effects.Traits;

public interface ImageIO
{
    byte[] ScaleToMaximumWidthJpeg(ReadOnlySpan<byte> inputBytes, int maxWidthInPixels);
}
