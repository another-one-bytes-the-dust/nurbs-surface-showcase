using System;
using CGLabPlatform;

namespace nurbs.Topology
{
    public class Grid : Surface
    {
        public readonly DVector4[,] DVertices = new DVector4[4, 4];
        private readonly Random random = new Random();
        private bool firstInit = true;

        public override void Build()
        {
            if (firstInit) Init();
            else Rebuild();

            firstInit = false;
            FillFaces(ref Faces, ref Vertices, 4, 4);
        }

        public override void Draw(GDIDeviceUpdateArgs e)
        {
            foreach (var face in Faces)  face.DrawWireframe(e);
        }

        private void Init() 
        {
            Vertices = new Vertex[4, 4];
            Faces = new Face[3 * 3];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    float u = (float)i / 3, v = (float)j / 3;
                    var vec = RoundedVector(u, (1 - 2 * random.NextDouble()) / 4, v, 2)
                              - new DVector3(0.5, 0, 0.5);
                    DVertices[i, j] = new DVector4(vec.X, vec.Y, vec.Z, 1);
                    Vertices[i, j] = new Vertex(new DVector3(vec.X, -vec.Y, vec.Z), this.Id);
                }
            }
        }

        private void Rebuild()
        {
            Vertices = new Vertex[4, 4];
            Faces = new Face[3 * 3];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var vec = DVertices[i, j];
                    Vertices[i, j] = new Vertex(new DVector3(vec.X, -vec.Y, vec.Z), this.Id);
                }
            }
        }

        private DVector3 RoundedVector(double x, double y, double z, int signs) => 
            new DVector3(Math.Round(x, signs), Math.Round(y, signs), Math.Round(z, signs)); 
    }
}