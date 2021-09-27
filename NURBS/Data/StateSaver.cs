namespace nurbs.Data
{
    public static class StateSaver
    {
        public static LightData LightSettingsInstance { get; } = new LightData();
        public static LightData ShadeInstance { get; } = new LightData();
        public static FigureData FigureInstance { get; } = new FigureData();
    }
}