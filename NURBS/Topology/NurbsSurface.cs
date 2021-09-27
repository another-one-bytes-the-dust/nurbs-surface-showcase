using System;
using System.Linq;
using CGLabPlatform;
using nurbs.Data;

namespace nurbs.Topology
{
    class NurbsSurface : Surface
    {
        public DVector4[,] Grid = new DVector4[4, 4];

        public override void Build()
        {
            Vertices = new Vertex[Height, Width];
            Faces = new Face[(Height - 1) * (Width - 1)];
            
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    float u = (float)i / (Height - 1), v = (float)j / (Width - 1);
                    var vec = Get(u, v);
                    Vertices[i, j] = new Vertex(new DVector3(vec.X, -vec.Y, vec.Z), this.Id);
                }
            }

            FillFaces(ref Faces, ref Vertices, Height, Width);
        }

        public override void Draw(GDIDeviceUpdateArgs e)
        {
            foreach (var face in Faces)
                face.Normal = face.IsVisible() ? face.Normal : face.Normal * -1;


            if (StateSaver.LightSettingsInstance.ShadingType == Shading.Flat)
                foreach (var face in Faces)
                    face.DrawFlat(e);

            else
            {
                foreach (var vertex in Vertices)
                {
                    vertex.Update();
                    vertex.CalcColor();
                }

                Faces.Where(face => face.IsVisible()).ToList().ForEach(face => face.DrawGouraud(e));
            }
        }

        private DVector4 Get(float u, float v)
        {
            var res = DVector4.Zero;
            
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var point = Grid[i, j];
                    var vert = new DVector4(point.X * point.W, 
                                            point.Y * point.W, 
                                            point.Z * point.W, point.W);
                    res += vert * BSpline(u, i, 4, knotU) * BSpline(v, j, 4, knotV);
                }
            }

            if (res[3] < 0.001f) res[3] = 1.0f;
            return res / res.W;
        }

        private double BSpline(float t, int i, int k, double[] T)
        {
            if (k == 1)
                return (t >= T[i] && t < T[i + 1]) ? 1.0 : 0.0;

            double a1 = (t - T[i]) * BSpline(t, i, k - 1, T);
            double a2 = T[i + k - 1] - T[i];

            double b1 = (T[i + k] - t) * BSpline(t, i + 1, k - 1, T);
            double b2 = T[i + k] - T[i + 1];

            double a, b, eps = 0.001f;
            a = (Math.Abs(a1) < eps && Math.Abs(a2) < eps) ? 0.0 : a1 / a2;
            b = (Math.Abs(b1) < eps && Math.Abs(b2) < eps) ? 0.0 : b1 / b2;
            return a + b;
        }
    }
}
