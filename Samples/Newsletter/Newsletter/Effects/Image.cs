using Newsletter.Effects.Traits;

namespace Newsletter.Effects;

/// <summary>
/// Json parser
/// </summary>
public static class Image<M, RT>
    where RT : 
        Has<M, ImageIO>
    where M :
        Monad<M>
{
    public static readonly K<M, ImageIO> trait =
        Has<M, RT, ImageIO>.ask;

    public static K<M, byte[]> scaleToMaximumWidthJpeg(byte[] input, int maxWidthInPixels) =>
        trait.Map(t => t.ScaleToMaximumWidthJpeg(input, maxWidthInPixels));
}
