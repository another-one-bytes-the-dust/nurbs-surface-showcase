using System.Drawing;
using CGLabPlatform;
using nurbs.Data;

namespace nurbs.Topology
{
    public class Lamp : AbstractFigure
    {
        public DVector3 LocalPosition;
        public DVector3 GlobalPosition;

        public override void Build()
        {
            Vertices = new Vertex[1, 1];
            Vertices[0, 0] = new Vertex(LocalPosition, Id);
        }

        public override void Update(DVector3 centerPoint, DVector3 angle, DVector3 scale)
        {
            DefaultUpdate(centerPoint, angle, scale);
            GlobalPosition = Vertices[0, 0].Global();
        }

        public override void Draw(GDIDeviceUpdateArgs e)
        {
            var light = StateSaver.LightSettingsInstance;
            var diffuse = Utilities.Draw.ColorFromVector(light.KDiffuse);
            var specular = Utilities.Draw.ColorFromVector(light.KSpecular);

            if (!Utilities.Draw.IsDark(diffuse) && light.EnableDiffuse)
                e.Graphics.FillEllipse(new SolidBrush(diffuse), new Rectangle((int)GlobalPosition.X - 6, (int)GlobalPosition.Y - 6, 12, 12));

            if (!Utilities.Draw.IsDark(specular) && light.EnableSpecular)
                e.Graphics.FillEllipse(new SolidBrush(specular), new Rectangle((int)GlobalPosition.X - 4, (int)GlobalPosition.Y - 4, 8, 8));
        }
    }
}
