using System.Numerics;

namespace ExtraChat.Util;

internal static class ColourUtil {
    internal static uint RgbaToAbgr(uint rgba) {
        return (rgba >> 24) // red 
               | ((rgba & 0x0000ff00) << 8) // blue
               | ((rgba & 0x00ff0000) >> 8) // green
               | ((rgba & 0x000000ff) << 24); // alpha
    }

    internal static (double, double, double, double) ExplodeRgba(uint rgba) {
        // separate RGBA values
        var r = (byte) ((rgba >> 24) & 0xff);
        var g = (byte) ((rgba >> 16) & 0xff);
        var b = (byte) ((rgba >> 8) & 0xff);
        var a = (byte) (rgba & 0xff);

        // convert RGBA to floats
        var rf = r / 255d;
        var gf = g / 255d;
        var bf = b / 255d;
        var af = a / 255d;

        return (rf, gf, bf, af);
    }

    internal static Vector4 RgbaToHsl(uint rgba) {
        var (rf, gf, bf, af) = ExplodeRgba(rgba);

        // determine hue
        var max = Math.Max(rf, Math.Max(gf, bf));
        var min = Math.Min(rf, Math.Min(gf, bf));
        var chroma = max - min;
        var hPrime = 0d;
        if (chroma == 0) {
            hPrime = 0d;
        } else if (Math.Abs(rf - max) < 0.0001) {
            hPrime = ((gf - bf) / chroma) % 6;
        } else if (Math.Abs(gf - max) < 0.0001) {
            hPrime = 2 + (bf - rf) / chroma;
        } else if (Math.Abs(bf - max) < 0.0001) {
            hPrime = 4 + (rf - gf) / chroma;
        }

        var h = hPrime * 60f;

        // determine lightness
        var l = (min + max) / 2f;

        // determine saturation
        double s;
        if (l is 0 or 1) {
            s = 0d;
        } else {
            s = chroma / (1 - Math.Abs(2 * l - 1));
        }

        return new Vector4((float) h, (float) s, (float) l, (float) af);
    }

    internal static Vector4 RgbaToHsv(uint rgba) {
        var (rf, gf, bf, af) = ExplodeRgba(rgba);

        // determine hue
        var max = Math.Max(rf, Math.Max(gf, bf));
        var min = Math.Min(rf, Math.Min(gf, bf));
        var chroma = max - min;
        var hPrime = 0d;
        if (chroma == 0) {
            hPrime = 0d;
        } else if (Math.Abs(rf - max) < 0.0001) {
            hPrime = ((gf - bf) / chroma) % 6;
        } else if (Math.Abs(gf - max) < 0.0001) {
            hPrime = 2 + (bf - rf) / chroma;
        } else if (Math.Abs(bf - max) < 0.0001) {
            hPrime = 4 + (rf - gf) / chroma;
        }

        var h = hPrime * 60f;

        // determine lightness
        var v = max;

        // determine saturation
        double s;
        if (v is 0) {
            s = 0d;
        } else {
            s = chroma / v;
        }

        return new Vector4((float) h, (float) s, (float) v, (float) af);
    }

    internal static double Luma(uint rgba) {
        var (r, g, b, _) = ExplodeRgba(rgba);
        return 0.2627 * r + 0.6780 * g + 0.0593 * b;
    }

    internal static (int, int, int) Step(uint rgba) {
        var (r, g, b, _) = ExplodeRgba(rgba);
        var lum = Math.Sqrt(0.241 * r + 0.691 * g + 0.068 * b);
        var hsv = RgbaToHsv(rgba);
        const int reps = 8;
        var h2 = (int) (hsv.X * reps);
        var lum2 = (int) (lum * reps);
        var v2 = (int) (hsv.Z * reps);

        if (h2 % 2 == 1) {
            v2 = reps - v2;
            lum2 = reps - lum2;
        }

        return (h2, lum2, v2);
    }
}
