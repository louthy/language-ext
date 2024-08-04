using Newsletter.Effects.Traits;

namespace Newsletter.Effects;

/// <summary>
/// Json parser
/// </summary>
public static class Image<RT>
    where RT : Has<Eff<RT>, ImageIO>
{
    public static readonly Eff<RT, ImageIO> trait =
        Stateful.getsM<Eff<RT>, RT, ImageIO>(rt => rt.Trait).As();

    public static Eff<RT, byte[]> scaleToMaximumWidthJpeg(byte[] input, int maxWidthInPixels) =>
        trait.Map(t => t.ScaleToMaximumWidthJpeg(input, maxWidthInPixels));
}
