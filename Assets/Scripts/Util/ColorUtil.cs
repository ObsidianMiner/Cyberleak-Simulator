// This code was AI generated to reduce the time taken here.
using System;
using UnityEngine;

/// <summary>
/// OKLab perceptual color space by Björn Ottosson (https://bottosson.github.io/posts/oklab/).
/// L is lightness (~0..1), a is green(-)/red(+), b is blue(-)/yellow(+).
/// Euclidean distance in OKLab approximates perceptual difference well.
/// </summary>
[Serializable]
public readonly struct OKLab : IEquatable<OKLab>
{
    public readonly float L;
    public readonly float a;
    public readonly float b;

    public OKLab(float L, float a, float b)
    {
        this.L = L;
        this.a = a;
        this.b = b;
    }

    public static readonly OKLab Black = new OKLab(0f, 0f, 0f);
    public static readonly OKLab White = new OKLab(1f, 0f, 0f);

    // ---------------------------------------------------------------------
    // sRGB  ->  OKLab
    // ---------------------------------------------------------------------

    /// <summary>Convert an sRGB color (components 0..1) to OKLab.</summary>
    public static OKLab FromRGB(float r, float g, float b)
    {
        // 1) sRGB gamma decode -> linear RGB
        float lr = SRGBToLinear(r);
        float lg = SRGBToLinear(g);
        float lb = SRGBToLinear(b);

        // 2) linear RGB -> LMS cone response
        float l = 0.4122214708f * lr + 0.5363325363f * lg + 0.0514459929f * lb;
        float m = 0.2119034982f * lr + 0.6806995451f * lg + 0.1073969566f * lb;
        float s = 0.0883024619f * lr + 0.2817188376f * lg + 0.6299787005f * lb;

        // 3) non-linearity (cube root, defined for negatives too)
        float l_ = Cbrt(l);
        float m_ = Cbrt(m);
        float s_ = Cbrt(s);

        // 4) LMS' -> OKLab
        return new OKLab(
            L: 0.2104542553f * l_ + 0.7936177850f * m_ - 0.0040720468f * s_,
            a: 1.9779984951f * l_ - 2.4285922050f * m_ + 0.4505937099f * s_,
            b: 0.0259040371f * l_ + 0.7827717662f * m_ - 0.8086757660f * s_
        );
    }

    public static OKLab FromRGB(Color color) => FromRGB(color.r, color.g, color.b);

    public static OKLab FromRGB(Color32 color)
        => FromRGB(color.r / 255f, color.g / 255f, color.b / 255f);

    /// <summary>Build from already-linearized RGB values (0..1), skipping the gamma decode.</summary>
    public static OKLab FromLinearRGB(float lr, float lg, float lb)
    {
        float l = 0.4122214708f * lr + 0.5363325363f * lg + 0.0514459929f * lb;
        float m = 0.2119034982f * lr + 0.6806995451f * lg + 0.1073969566f * lb;
        float s = 0.0883024619f * lr + 0.2817188376f * lg + 0.6299787005f * lb;

        float l_ = Cbrt(l);
        float m_ = Cbrt(m);
        float s_ = Cbrt(s);

        return new OKLab(
            0.2104542553f * l_ + 0.7936177850f * m_ - 0.0040720468f * s_,
            1.9779984951f * l_ - 2.4285922050f * m_ + 0.4505937099f * s_,
            0.0259040371f * l_ + 0.7827717662f * m_ - 0.8086757660f * s_
        );
    }

    // ---------------------------------------------------------------------
    // OKLab  ->  sRGB
    // ---------------------------------------------------------------------

    /// <summary>
    /// Convert to sRGB components (0..1). Values may fall outside [0,1] for
    /// out-of-gamut colors — clamp with Mathf.Clamp01 if you need displayable output.
    /// </summary>
    public void ToRGB(out float r, out float g, out float b)
    {
        // 1) OKLab -> LMS'
        float l_ = L + 0.3963377774f * a + 0.2158037573f * this.b;
        float m_ = L - 0.1055613458f * a - 0.0638541728f * this.b;
        float s_ = L - 0.0894841775f * a - 1.2914855480f * this.b;

        // 2) undo cube root
        float l = l_ * l_ * l_;
        float m = m_ * m_ * m_;
        float s = s_ * s_ * s_;

        // 3) LMS -> linear RGB
        float lr = +4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s;
        float lg = -1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s;
        float lb = -0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s;

        // 4) gamma encode
        r = LinearToSRGB(lr);
        g = LinearToSRGB(lg);
        b = LinearToSRGB(lb);
    }

    /// <summary>Linear RGB (no gamma encode) — useful for rendering / math.</summary>
    public void ToLinearRGB(out float r, out float g, out float b)
    {
        float l_ = L + 0.3963377774f * a + 0.2158037573f * this.b;
        float m_ = L - 0.1055613458f * a - 0.0638541728f * this.b;
        float s_ = L - 0.0894841775f * a - 1.2914855480f * this.b;

        float l = l_ * l_ * l_;
        float m = m_ * m_ * m_;
        float s = s_ * s_ * s_;

        r = +4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s;
        g = -1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s;
        b = -0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s;
    }

    public Color ToColor()
    {
        ToLinearRGB(out float r, out float g, out float b);
        return new Color(r, g, b, 1f);
    }

    public Color ToUIRGBColor()
    {
        ToRGB(out float r, out float g, out float b);
        return new Color(r, g, b, 1f);
    }
    public Color ToColor(float alpha)
    {
        ToRGB(out float r, out float g, out float b);
        return new Color(r, g, b, alpha);
    }

    /// <summary>Convert to sRGB and clamp into the displayable gamut.</summary>
    public Color ToColorClamped()
    {
        ToRGB(out float r, out float g, out float b);
        return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), 1f);
    }

    public Color32 ToColor32()
    {
        ToRGB(out float r, out float g, out float b);
        return new Color32(
            (byte)Mathf.RoundToInt(Mathf.Clamp01(r) * 255f),
            (byte)Mathf.RoundToInt(Mathf.Clamp01(g) * 255f),
            (byte)Mathf.RoundToInt(Mathf.Clamp01(b) * 255f),
            255);
    }

    // ---------------------------------------------------------------------
    // Distance / difference
    // ---------------------------------------------------------------------

    /// <summary>Squared Euclidean distance in OKLab.</summary>
    public float DistanceSquared(OKLab other)
    {
        float dL = L - other.L;
        float da = a - other.a;
        float db = b - other.b;
        return dL * dL + da * da + db * db;
    }

    /// <summary>
    /// Euclidean distance in OKLab. Roughly perceptual — ~0.02 is a
    /// just-noticeable difference for most colors.
    /// </summary>
    public float Distance(OKLab other) => Mathf.Sqrt(DistanceSquared(other));

    /// <summary>Alias for <see cref="Distance"/> — the standard color-difference naming.</summary>
    public float DeltaE(OKLab other) => Distance(other);

    /// <summary>Distance to an sRGB color.</summary>
    public float Distance(Color color) => Distance(FromRGB(color));

    /// <summary>True if the two colors are within <paramref name="threshold"/> in OKLab space.</summary>
    public bool IsSimilar(OKLab other, float threshold)
        => DistanceSquared(other) <= threshold * threshold;

    public static float Distance(OKLab x, OKLab y) => x.Distance(y);
    public static float DeltaE(OKLab x, OKLab y) => x.Distance(y);

    /// <summary>
    /// Returns the arithmetic mean of an array of OKLab colors.
    /// </summary>
    public static OKLab Average(OKLab[] colors)
    {
        if (colors == null)
            throw new ArgumentNullException(nameof(colors));

        if (colors.Length == 0)
            throw new ArgumentException("Cannot average an empty color array.", nameof(colors));

        float L = 0f;
        float a = 0f;
        float b = 0f;

        for (int i = 0; i < colors.Length; i++)
        {
            L += colors[i].L;
            a += colors[i].a;
            b += colors[i].b;
        }

        float invCount = 1f / colors.Length;

        return new OKLab(
            L * invCount,
            a * invCount,
            b * invCount);
    }

    // ---------------------------------------------------------------------
    // Blending / interpolation (perceptually nicer than RGB lerp)
    // ---------------------------------------------------------------------

    public static OKLab Lerp(OKLab x, OKLab y, float t)
    {
        t = Mathf.Clamp01(t);
        return new OKLab(
            x.L + (y.L - x.L) * t,
            x.a + (y.a - x.a) * t,
            x.b + (y.b - x.b) * t);
    }

    public static OKLab LerpUnclamped(OKLab x, OKLab y, float t)
        => new OKLab(
            x.L + (y.L - x.L) * t,
            x.a + (y.a - x.a) * t,
            x.b + (y.b - x.b) * t);

    // ---------------------------------------------------------------------
    // Object overrides
    // ---------------------------------------------------------------------

    public bool Equals(OKLab other) => L == other.L && a == other.a && b == other.b;
    public override bool Equals(object obj) => obj is OKLab o && Equals(o);
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = L.GetHashCode();
            hash = (hash * 397) ^ a.GetHashCode();
            hash = (hash * 397) ^ b.GetHashCode();
            return hash;
        }
    }

    public static bool operator ==(OKLab x, OKLab y) => x.Equals(y);
    public static bool operator !=(OKLab x, OKLab y) => !x.Equals(y);

    public override string ToString() => $"OKLab({L:F4}, {a:F4}, {b:F4})";

    // ---------------------------------------------------------------------
    // Internal helpers
    // ---------------------------------------------------------------------

    /// <summary>sRGB gamma decode: display value -> linear.</summary>
    private static float SRGBToLinear(float c)
        => c <= 0.04045f ? c / 12.92f : Mathf.Pow((c + 0.055f) / 1.055f, 2.4f);

    /// <summary>sRGB gamma encode: linear -> display value.</summary>
    private static float LinearToSRGB(float c)
        => c <= 0.0031308f ? 12.92f * c : 1.055f * Mathf.Pow(c, 1f / 2.4f) - 0.055f;

    /// <summary>
    /// Real cube root. Mathf.Pow returns NaN for negative bases, but the LMS
    /// step legitimately produces negatives for out-of-gamut colors.
    /// </summary>
    private static float Cbrt(float x)
        => x < 0f ? -Mathf.Pow(-x, 1f / 3f) : Mathf.Pow(x, 1f / 3f);
}