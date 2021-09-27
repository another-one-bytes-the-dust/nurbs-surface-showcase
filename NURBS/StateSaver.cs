namespace nurbs
{
    public static class StateSaver
    {
        public static LightData LightSettingsInstance { get; } = new LightData();
        public static LightData ShadeInstance { get; } = new LightData();
        public static UpdateData UpdateInstance { get; } = new UpdateData();
    }
}