using CGLabPlatform;

namespace nurbs.Data
{
    public class FigureData
    {
        public double Scale { get; set; }
        public DVector3 CenterPoint { get; set; } = DVector3.Zero;
        public DVector3 AxisScale { get; set; } = DVector3.One;
        public double AngleX { get; set; } = 1000;
        public double AngleY { get; set; } = 1000;
        public double AngleZ { get; set; } = 1000;
    }
}