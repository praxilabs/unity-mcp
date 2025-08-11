namespace Praxilabs.DeviceSideMenu
{
    [System.Flags]
    public enum TabType
    {
        None = 0,
        SafetyProcedures = 1 << 0,
        Description = 1 << 1,
        Readings = 1 << 2,
        Controls = 1 << 3
    }
}