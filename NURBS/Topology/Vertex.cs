using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CGLabPlatform;
using nurbs.Data;

namespace nurbs.Topology
{
    public class Vertex
    {
        public DVector3 LocalPoint;
        public DVector3 Normal;
        public Color Color { get; private set; } 
        public int Id { get; }
        
        public static readonly List<DMatrix4> Transform = new List<DMatrix4>();
        public readonly List<Face> Faces;
        
        public Vertex(DVector3 localPoint, int id)
        {
            this.Id = id;
            this.LocalPoint = localPoint;
            Faces = new List<Face>();
        }

        public DVector3 Local() => LocalPoint;

        public DVector3 Global() => (DVector3)(Transform[Id] * new DVector4(LocalPoint, 1));

        public void Update() => CalcNormal();

        private void CalcNormal()
        {
            var p = Faces.Aggregate(DVector3.Zero, (res, face) => res += face.Normal);
            Normal = (p / Faces.Count).Normalized();
        }

        public void DrawNormal(GDIDeviceUpdateArgs e, double size) =>
            e.Surface.DrawLine(Color.Coral.ToArgb(), Global(), Global() + Normal * size);

        public void CalcColor()
        {
            var color = DVector3.Zero;
            var light = StateSaver.LightSettingsInstance;
            
            if (light.EnableAmbient)
                color += new DVector3(DVector3.Multiply(light.KAmbient, light.IA));

            DVector3 len = -(this.Global() - light.Position);
            DVector3 normal = Normal;

            double eyeNormalCos = len.DotProduct(normal);
            double normalLightCos = -Normal.DotProduct(this.Global() - light.Position);

            double distance = (this.Global() - light.Position).GetLength();
            double kd = distance + light.K;

            if (light.EnableDiffuse && eyeNormalCos > 0)
                color += DVector3.Multiply(light.KDiffuse, light.IL) * 
                    normalLightCos / kd;

            DVector3 reflection = (len - 2 * normal * (normal.DotProduct(len) /
                                                       normal.DotProduct(normal))).Normalized();

            var eyeReflectCos = reflection.DotProduct(new DVector3(0, 0, -1));

            if (light.EnableSpecular && eyeReflectCos > 0 && eyeNormalCos > 0)
                color += light.K *
                    light.IL.Multiply(light.KSpecular) *
                    Math.Pow(eyeReflectCos, light.p) / kd;

            Color = Utilities.Draw.ColorFromVector(color);
        }
    }
}