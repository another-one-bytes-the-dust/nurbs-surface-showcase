using CGLabPlatform;

namespace nurbs.Data
{
    public class LightData
    {
        public bool EnableAmbient { get; set; } = true;
        public bool EnableDiffuse { get; set; } = true;
        public bool EnableSpecular { get; set; } = true;
        public Shading ShadingType { get; set; } = Shading.Flat;

        public DVector3 Position { get; set; } = DVector3.Zero;
        public DVector3 KAmbient { get; set; } = DVector3.One;
        public DVector3 KDiffuse { get; set; } = DVector3.One;
        public DVector3 KSpecular { get; set; } = DVector3.One;

        public DVector3 IA { get; set; } = DVector3.One;
        public DVector3 IL { get; set; } = DVector3.One;

        public double p = 6, K = 500;
    }
}
