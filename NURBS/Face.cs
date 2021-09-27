using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CGLabPlatform;

namespace nurbs
{
    public class Face : IComparable<Face>, IComparer<Face>
    {
        public readonly List<Vertex> Vertices;
        public DVector3 Normal { get; set; }
        public int Id { get; }

        public Face(List<Vertex> vertices)
        {
            this.Vertices = vertices;
            this.Id = vertices[0].Id;
            vertices.ForEach(v => v.Faces.Add(this));
        }

        public bool IsVisible() => Normal.Z >= 0.0;

        public void Update()
        {
            Normal = new DVector3(Vertices[0].Global() - Vertices[1].Global()) *
                     new DVector3(Vertices[1].Global() - Vertices[2].Global());

            if (!Normal.ApproxEqual(DVector3.Zero))
                Normal.Normalize();
            Vertices.ForEach(v => v.Update());
        }

        public DVector3 GlobalMedianPoint()
        {
            var p = Vertices.Aggregate(DVector3.Zero, (res, elem) => res += elem.Global());
            return p / Vertices.Count;
        }

        public DVector3 LocalMedianPoint()
        {
            var p = Vertices.Aggregate(DVector3.Zero, (res, elem) => res += elem.Local());
            return p / Vertices.Count;
        }

        public void DrawWireframe(GDIDeviceUpdateArgs e)
        {
            DrawTriangleFrame(Color.White, 0, 1, 2, e);
            if (Vertices.Count == 3) return;

            DrawTriangleFrame(Color.White, 3, 1, 2, e);
        }

        public void DrawFlat(GDIDeviceUpdateArgs e)
        {
            Vertex v = new Vertex(LocalMedianPoint(), Id);
            v.Normal = IsVisible() ? Normal : DVector3.Zero;
            v.CalcColor();
        
            DrawTriangle(v.Color, 0, 1, 2, e);
            if (Vertices.Count == 3) return;

            DrawTriangle(v.Color, 3, 1, 2, e);
        }

        public void DrawGouraud(GDIDeviceUpdateArgs e)
        {
            DrawGradientTriangle(0, 1, 2, e);
            if (Vertices.Count == 3) return;
            DrawGradientTriangle(3, 1, 2, e);
        }

        public void DrawNormal(GDIDeviceUpdateArgs e, double size) =>
            e.Surface.DrawLine(Color.MediumVioletRed.ToArgb(),
                GlobalMedianPoint(),
                GlobalMedianPoint() + Normal * size);

        public int CompareTo(Face other) => 
            this.GlobalMedianPoint().Z.CompareTo(other.GlobalMedianPoint().Z);

        public int Compare(Face face1, Face face2) => 
            face1.GlobalMedianPoint().Z.CompareTo(face2.GlobalMedianPoint().Z);

        private void DrawTriangleFrame(Color color, int index1, int index2, int index3, GDIDeviceUpdateArgs e)
        {
            e.Surface.DrawLine(color.ToArgb(), Vertices[index1].Global(), Vertices[index2].Global());
            e.Surface.DrawLine(color.ToArgb(), Vertices[index1].Global(), Vertices[index3].Global());
        }

        private void DrawTriangle(Color color, int index1, int index2, int index3, GDIDeviceUpdateArgs e)
        {
            e.Surface.DrawTriangle(color.ToArgb(),
                Vertices[index1].Global().X, Vertices[index1].Global().Y,
                Vertices[index2].Global().X, Vertices[index2].Global().Y,
                Vertices[index3].Global().X, Vertices[index3].Global().Y);
        }

        private void DrawGradientTriangle(int index1, int index2, int index3, GDIDeviceUpdateArgs e)
        {
            e.Surface.DrawTriangle(Vertices[index1].Color.ToArgb(), Vertices[index1].Global().X, Vertices[index1].Global().Y,
                Vertices[index2].Color.ToArgb(), Vertices[index2].Global().X, Vertices[index2].Global().Y,
                Vertices[index3].Color.ToArgb(), Vertices[index3].Global().X, Vertices[index3].Global().Y);
        }
    }
}