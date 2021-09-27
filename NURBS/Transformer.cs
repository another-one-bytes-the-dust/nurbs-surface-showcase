using System;
using CGLabPlatform;

namespace nurbs
{
    public static class Transformer
    {
        public static DMatrix4 Translate(double x, double y, double z) =>
            new DMatrix4(1, 0, 0,  x,
                0, 1, 0,  y,
                0, 0, 1,  z,
                0, 0, 0,  1);
        public static DMatrix4 Translate(DVector3 v) => Translate(v.X, v.Y, v.Z);

        public static DMatrix4 Scale(double x, double y, double z) => new DMatrix4(x, 0.0, 0.0, 0.0,
            0.0, y, 0.0, 0.0,
            0.0, 0.0, z, 0.0,
            0.0, 0.0, 0.0, 1.0);

        public static DMatrix4 Scale(DVector3 v) => Scale(v.X, v.Y, v.Z);
        
        public static DMatrix4 Rotate(DVector3 angle) =>
            RotateX(angle.X * Math.PI / 180) *
            RotateY(angle.Y * Math.PI / 180) *
            RotateZ(angle.Z * Math.PI / 180);
        
        public static DMatrix4 RotateX(double angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new DMatrix4(1.0,  0.0, 0.0, 0.0,
                0.0,  cos, sin, 0.0,
                0.0, -sin, cos, 0.0,
                0.0,  0.0, 0.0, 1.0);
        }

        public static DMatrix4 RotateY(double angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new DMatrix4( cos, 0.0, sin, 0.0,
                0.0, 1.0, 0.0, 0.0,
                -sin, 0.0, cos, 0.0,
                0.0, 0.0, 0.0, 1.0);
        }

        public static DMatrix4 RotateZ(double angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new DMatrix4(cos, -sin, 0.0, 0.0,
                sin, cos, 0.0, 0.0,
                0.0, 0.0, 1.0, 0.0,
                0.0, 0.0, 0.0, 1.0);
        }
    }
}
