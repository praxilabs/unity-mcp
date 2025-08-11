/// <summary>
/// This class is added to parent gameObject of the managers to keep 
/// them from being destroyed when scene changes instead of turning all
/// lab loader scene managers into singletons
/// </summary>
 
public class LabLoaderManagersParent : Singleton<LabLoaderManagersParent>
{
}
