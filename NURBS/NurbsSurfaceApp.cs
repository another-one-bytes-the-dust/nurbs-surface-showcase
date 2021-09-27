using System;
using System.Drawing;
using System.Windows.Forms;
using CGLabPlatform;
using nurbs.Data;
using nurbs.Topology;

namespace nurbs
{
    public enum Shading { Flat, Gouraud };

    public abstract class NurbsSurfaceApp : GFXApplicationTemplate<NurbsSurfaceApp>
    {

        [DisplayTextBoxProperty(Default: "", Name: "Point")]
        public virtual string Textbox { get; set; }

        [DisplayNumericProperty(Default: new[] { 0d, 0d, 0d }, Increment: 2, Name: "Shift")]
        public abstract DVector3 CenterPoint { get; set; }

        [DisplayNumericProperty(Default: new[] { 1d, 1d, 1d }, Increment: 0.01, Minimum: 0.001, Maximum: 1, Name: "AxisScale")]
        public abstract DVector3 AxisScale { get; set; }

        [DisplayNumericProperty(Default: 1, Increment: 0.1, Minimum: 0.01, Maximum: 10, Name: "Scale")]
        public abstract double Scale { get; set; }

        [DisplayNumericProperty(Default: 0, Increment: 6d, Name: "Rotate Ox")]
        public double AngleX
        {
            get => Get<double>();
            set
            {
                while (value < 0) value += 360;
                while (value >= 360) value -= 360;
                Set(value);
            }
        }

        [DisplayNumericProperty(Default: 0, Increment: 6d, Name: "Rotate Oy")]
        public double AngleY
        {
            get { return Get<double>(); }
            set
            {
                while (value < 0) value += 360;
                while (value >= 360) value -= 360;
                Set(value);
            }
        }

        [DisplayNumericProperty(Default: 0, Increment: 6d, Name: "Rotate Oz")]
        public double AngleZ
        {
            get => Get<double>();
            set
            {
                while (value < 0) value += 360;
                while (value >= 360) value -= 360;
                Set(value);
            }
        }

        [DisplayNumericProperty(Default: new[] { 25d, 25d }, Increment: 2, Minimum: 6, Name: "Approx")]
        public abstract DVector2 Approximation { get; set; }

        [DisplayNumericProperty(Default: new[] { 200d, 200d, 0d }, Increment: 2, Name: "Light pos")]
        public abstract DVector3 LightPosition { get; set; }

        [DisplayCheckerProperty(false, "Show curves")]
        public bool CurvesVisible { get; set; } = true;

        [DisplayCheckerProperty(true, "Show carcass")]
        public bool WireframeVisible { get; set; }

        private readonly Shading shading = Shading.Gouraud;

        [DisplayNumericProperty(Default: new[] { 0.3d, 0.3d, 0.0d }, Increment: 0.1, Minimum: 0, Maximum: 1, Name: "kA")]
        public abstract DVector3 KAmbient { get; set; }

        [DisplayNumericProperty(Default: new[] { 1.0d, 0.8d, 0.0d }, Increment: 0.1, Minimum: 0, Maximum: 1, Name: "kD")]
        public abstract DVector3 KDiffuse { get; set; }

        [DisplayNumericProperty(Default: new[] { 1.5d, 1.8d, 1.2d }, Increment: 0.1, Minimum: 0, Maximum: 2, Name: "kS")]
        public abstract DVector3 KSpecular { get; set; }

        [DisplayCheckerProperty(true, "Enable ambient")]
        public bool EnableAmbient { get; set; } = true;

        [DisplayCheckerProperty(true, "Enable diffuse")] 
        public bool EnableDiffuse { get; set; } = true;

        [DisplayCheckerProperty(true, "Enable specular")]
        public bool EnableSpecular { get; set; } = true;

        [DisplayNumericProperty(Default: new[] { 0.6d, 0.0d, 0.0d }, Increment: 0.1, Minimum: 0, Name: "iA")]
        public abstract DVector3 IA { get; set; }

        [DisplayNumericProperty(Default: new[] { 1.5d, 1.8d, 1.2d }, Increment: 0.1, Minimum: 0, Name: "iL")]
        public abstract DVector3 IL { get; set; }

        [DisplayNumericProperty(Default: 5.9d, Increment: 0.1, Minimum: 1, Name: "power")]
        public abstract double p { get; set; }

        [DisplayNumericProperty(Default: 90d, Increment: 10, Minimum: 1, Name: "K")]
        public abstract double K { get; set; }

        [STAThread] static void Main() { RunApplication(); }

        private readonly Lamp lamp = new Lamp();
        private readonly NurbsSurface nurbs = new NurbsSurface();
        private readonly Grid grid = new Grid();

        private Vertex selected;
        private Vertex highlighted;

        private int selectedI = -1;
        private int selectedJ = -1;
        private bool enterSucceeded;

        private const double Tolerance = 1e-2;
        private const double AngleTolerance = 1e-1;
    
        protected override void OnMainWindowLoad(object sender, EventArgs args)
        {
            CenterPoint = new DVector3(0, 0, 0);

            base.RenderDevice.BufferBackCol = 0x20;
            base.ValueStorage.Font = new Font("Courier New", 12f);
            base.ValueStorage.ForeColor = Color.Black;
            base.ValueStorage.RowHeight = 30;

            base.ValueStorage.RightColWidth = 40;
            base.VSPanelWidth = 400;
            base.MainWindow.Size = new Size(1100, 600);

            base.RenderDevice.MouseMoveWithRightBtnDown += (s, e) =>
            {
                AngleX += e.MovDeltaY;
                AngleY += e.MovDeltaX;
            };

            base.RenderDevice.MouseMoveWithMiddleBtnDown += (s, e) => AngleZ += 0.01 * e.MovDeltaX;

            base.RenderDevice.MouseWheel += (s, e) => Scale += e.Delta * 0.0005;

            RenderDevice.HotkeyRegister(Keys.Up,   (s, e) => CenterPoint += new DVector3(0, 0, 10));
            RenderDevice.HotkeyRegister(Keys.Down, (s, e) => CenterPoint -= new DVector3(0, 0, -10));

            base.RenderDevice.MouseMove += (s, e) =>
            {
                if (grid.Vertices == null) return;

                var vert = SelectVertex(new DVector3(e.X, e.Y, 0), false);

                highlighted = vert;
            };

            base.RenderDevice.MouseClick += (s, e) =>
            {
                if (grid.Vertices == null) return;

                if (e.Button == MouseButtons.Left)
                {
                    selected = SelectVertex(new DVector3(e.X, e.Y, 0), true);
                    var dVert = grid.DVertices[selectedI, selectedJ];
                    Textbox = dVert.X + " " + dVert.Y + " " + dVert.Z + " " + dVert.W;
                }
            };

            this.RenderDevice.HotkeyRegister(Keys.Enter, (s, e) =>
            {
                if (selected == null) return;
                DVector4? vec = Parser.Parse(Textbox);

                if (vec != null) grid.DVertices[selectedI, selectedJ] = (DVector4)vec;
                enterSucceeded = true;
            });
        }

        protected override void OnDeviceUpdate(object s, GDIDeviceUpdateArgs e)
        {
            var shift = new DVector3(e.Surface.Width / 2, e.Surface.Height / 2, CenterPoint.Z);
            var autoscale = Math.Min(e.Surface.Height, e.Surface.Width) / 6;

            var centerPosition = new DVector3(CenterPoint.X, -CenterPoint.Y, CenterPoint.Z);
            var lampPosition = new DVector3(LightPosition.X, -LightPosition.Y, LightPosition.Z);

            var light = StateSaver.LightSettingsInstance;
            var figureInstance = StateSaver.FigureInstance;
            var shadeInstance = StateSaver.ShadeInstance;

            lamp.LocalPosition = lampPosition / autoscale;
            lamp.Build();

            bool rebuild = NeedRebuild() || enterSucceeded;
            bool update = NeedUpdate();
            bool reshade = NeedReshade();

            if (rebuild)
            {
                nurbs.Height = (int)Approximation.X;
                nurbs.Width = (int)Approximation.Y;
                grid.Build();
                nurbs.Grid = grid.DVertices;
                nurbs.Build();

                if (enterSucceeded)
                    selected = grid.Vertices[selectedI, selectedJ];
                enterSucceeded = false;
            }

            var angles = new DVector3(AngleX, AngleY, AngleZ);
        
            lamp.Update(centerPosition + shift, angles, DVector3.One * autoscale);
            nurbs.Update(centerPosition + shift, angles, AxisScale * Scale * autoscale);
            grid.Update(centerPosition + shift, angles, AxisScale * Scale * autoscale);

            SaveState(figureInstance);
            light.Position = lamp.GlobalPosition;
            SaveShade(light);
            if (rebuild || update || reshade) SaveShade(shadeInstance);

            nurbs.Draw(e);

            if (CurvesVisible) nurbs.DrawWireframe(e);
            if (WireframeVisible) grid.Draw(e);

            if (highlighted != null)
                Utilities.Draw.DrawOrdinaryPoint(new DVector2(highlighted.Global().X, 
                                                                highlighted.Global().Y), e);

            if (selected != null)
                Utilities.Draw.DrawSelectedPoint(new DVector2(selected.Global().X, 
                                                                selected.Global().Y), e);

            lamp.Draw(e);
        }

        private bool NeedRebuild() => Math.Abs(Approximation.X - nurbs.Height) > Tolerance || 
                                      Math.Abs(Approximation.Y - nurbs.Width)  > Tolerance;

        private bool NeedUpdate()
        {
            var update = StateSaver.FigureInstance;
            return !update.CenterPoint.ApproxEqual(CenterPoint) ||
                   !update.AxisScale.ApproxEqual(AxisScale) ||
                   Math.Abs(update.AngleX - AngleX) > AngleTolerance ||
                   Math.Abs(update.AngleY - AngleY) > AngleTolerance ||
                   Math.Abs(update.AngleZ - AngleZ) > AngleTolerance ||
                   Math.Abs(update.Scale - Scale) > Tolerance;
        }

        private bool NeedReshade()
        {
            var shade = StateSaver.ShadeInstance;
            return shade.ShadingType != shading || 
                   !shade.Position.ApproxEqual(StateSaver.LightSettingsInstance.Position) ||
                   shade.EnableAmbient != EnableAmbient ||
                   shade.EnableDiffuse != EnableDiffuse ||
                   shade.EnableSpecular != EnableSpecular ||
                   !shade.KAmbient.ApproxEqual(KAmbient) ||
                   !shade.KDiffuse.ApproxEqual(KDiffuse) ||
                   !shade.KSpecular.ApproxEqual(KSpecular) ||
                   !shade.IA.ApproxEqual(IA) ||
                   !shade.IL.ApproxEqual(IL) ||
                   Math.Abs(shade.p - p) > Tolerance ||
                   Math.Abs(shade.K - K) > Tolerance;
        }
        private void SaveState(FigureData update)
        {
            update.Scale = Scale;
            update.CenterPoint = CenterPoint;
            update.AxisScale = AxisScale;
            update.AngleX = AngleX;
            update.AngleY = AngleY;
            update.AngleZ = AngleZ;
        }

        private void SaveShade(LightData light)
        {
            light.ShadingType = shading;            light.Position = StateSaver.LightSettingsInstance.Position;
            light.EnableAmbient = EnableAmbient;    light.EnableDiffuse = EnableDiffuse;
            light.EnableSpecular = EnableSpecular;  light.KAmbient = KAmbient;
            light.KDiffuse = KDiffuse;              light.KSpecular = KSpecular;
            light.IA = IA;                          light.IL = IL;
            light.p = p;                            light.K = K;
        }

        public Vertex SelectVertex(DVector3 cursor, bool save)
        {
            int savedI = -1;
            int savedJ = -1;
            var globalCursor = CenterPoint + cursor;
            globalCursor.Z = 0;

            double minLen = double.MaxValue;
            Vertex minVertex = null;

            for (int i = 0; i < grid.Vertices.GetLength(0); i++) 
            {
                for (int j = 0; j < grid.Vertices.GetLength(1); j++)
                {
                    var vert = grid.Vertices[i, j];
                    var projectionOnXY = new DVector3(vert.Global().X, vert.Global().Y, 0);
                    var len = (projectionOnXY - globalCursor).GetLength();
                    
                    if (!(len < minLen)) continue;
                    
                    minLen = len;
                    minVertex = vert;
                    savedI = i;
                    savedJ = j;
                }
            }

            if (save) { selectedI = savedI; selectedJ = savedJ; }

            return minVertex;
        }
    }
}