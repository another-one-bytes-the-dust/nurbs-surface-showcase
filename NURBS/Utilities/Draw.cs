using System;
using System.Drawing;
using CGLabPlatform;

namespace nurbs.Utilities
{
    public static class Draw
    {
        public static void DrawCircle(DVector2 v, int radius, Brush brush, GDIDeviceUpdateArgs e)
        {
            e.Graphics.FillEllipse(brush,
                new Rectangle((int)(v.X - radius / 2),
                                 (int)(v.Y - radius / 2), radius, radius));
        }

        public static void DrawPoint(DVector2 v, Brush foreColor, Brush backColor, GDIDeviceUpdateArgs e)
        {
            DrawCircle(v, 8, backColor, e);
            DrawCircle(v, 6, foreColor, e);
        }

        public static void DrawOrdinaryPoint(DVector2 v, GDIDeviceUpdateArgs e) =>
            DrawPoint(v, Brushes.Orange, Brushes.Black, e);

        public static void DrawSelectedPoint(DVector2 v, GDIDeviceUpdateArgs e) =>
            DrawPoint(v, Brushes.White, Brushes.Orange, e);

        public static Color ColorFromVector(DVector3 color) =>
            Color.FromArgb(NormalizeColorProjection(color.X),
                NormalizeColorProjection(color.Y),
                NormalizeColorProjection(color.Z));

        public static byte NormalizeColorProjection(double colorProjection) =>
            (byte)Math.Min(255, Math.Max(colorProjection * 255, 0));

        public static Color Multiply(Color color, double value) =>
            Color.FromArgb(NormalizeColorProjection(color.R * value),
                NormalizeColorProjection(color.G * value),
                NormalizeColorProjection(color.B * value));

        public static bool IsDark(Color color) => Math.Max(Math.Max(color.R, color.G), color.B) < 10;
    }
}