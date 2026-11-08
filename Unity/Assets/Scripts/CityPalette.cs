using UnityEngine;

namespace Majoras.City
{
    // Palette roles follow docs/ART_DIRECTION.md: colored shadows, warm stone and teal accents.
    public static class CityPalette
    {
        public static readonly Color Ink = Hex(0x172A30);
        public static readonly Color Deep = Hex(0x24383E);
        public static readonly Color Paper = Hex(0xF0E6CB);
        public static readonly Color Muted = Hex(0xB6C1B6);
        public static readonly Color Brass = Hex(0xD5BC77);
        public static readonly Color Teal = Hex(0x65A8A1);
        public static readonly Color DaySky = Hex(0x87BBD4);
        public static readonly Color DuskSky = Hex(0x615F82);
        public static readonly Color DayAmbient = Hex(0x7A8E99);
        public static readonly Color DuskAmbient = Hex(0x666B87);

        public static Color Alpha(Color color, float alpha) { color.a = alpha; return color; }
        static Color Hex(uint rgb)
        {
            return new Color(((rgb >> 16) & 255) / 255f, ((rgb >> 8) & 255) / 255f, (rgb & 255) / 255f, 1);
        }
    }
}
