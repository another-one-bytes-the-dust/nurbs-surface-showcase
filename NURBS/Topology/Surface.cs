using System.Collections.Generic;
using CGLabPlatform;

namespace nurbs.Topology
{
    public abstract class Surface : AbstractFigure
    {
        public int Width { get; set; } = 8;
        public int Height { get; set; } = 8;
        
        public readonly double[] knotU = { 0, 0, 0, 0, 0.5, 1.01, 1.01, 1.01 };
        public readonly double[] knotV = { 0, 0, 0, 0, 0.5, 1.01, 1.01, 1.01 };
        
        protected void FillFaces(ref Face[] faces, ref Vertex[,] vertices, int limI, int limJ) 
        {
            for (int i = 0; i < limI - 1; i++)
            for (int j = 0; j < limJ - 1; j++)
                faces[i * (limJ - 1) + j] = new Face(new List<Vertex>() {
                    vertices[i, j], vertices[i, (j + 1) % limJ],
                    vertices[i + 1, j], vertices[i + 1, (j + 1) % limJ] });
        }

        public override void Update(DVector3 centerPoint, DVector3 angles, DVector3 size) =>
            base.DefaultUpdate(centerPoint, angles, size * 3);
    }
}