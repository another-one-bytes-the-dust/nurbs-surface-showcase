using System;
using System.Linq;
using CGLabPlatform;
using nurbs.Utilities;

namespace nurbs.Topology
{
    public abstract class AbstractFigure
    {
        private static int counter;
        public int Id { get; }
        public Vertex[,] Vertices;
        public Face[] Faces;

        protected AbstractFigure() {
            Id = counter++;
            Vertex.Transform.Add(DMatrix4.Identity);
        }

        public abstract void Build();
        public abstract void Update(DVector3 centerPoint, DVector3 angles, DVector3 scale);
        public abstract void Draw(GDIDeviceUpdateArgs e);

        protected void DefaultUpdate(DVector3 centerPoint, DVector3 angle, DVector3 scale)
        {
            DMatrix4 rotationMatrix = Transformer.Rotate(angle);
            DMatrix4 scaleMatrix = Transformer.Scale(scale);
            Vertex.Transform[Id] = DMatrix4.Identity * 
                                   Transformer.Translate(centerPoint) * 
                                   rotationMatrix * 
                                   scaleMatrix;

            if (this.Faces == null) return;

            foreach (var face in this.Faces) face.Update();

            Array.Sort(this.Faces);
        }

        public void DrawNormals(GDIDeviceUpdateArgs e)
        {
            Faces.Where(face => face.IsVisible()).ToList().ForEach(face => face.DrawNormal(e, 30));

            foreach (Vertex elem in Vertices)
            {
                if (!elem.Faces.Any(face => face.IsVisible())) continue;
                elem.DrawNormal(e, 30);
            }
        }

        public void DrawWireframe(GDIDeviceUpdateArgs e)
        {
            foreach (var face in Faces)
            {
                face.Normal = face.IsVisible() ? face.Normal : face.Normal * -1;
                face.DrawWireframe(e);
            }
        }
    }
}