using BepInEx;
using BepInEx.Logging;

[BepInPlugin("com.yourname.testplugin","TestPlugin","0.0.1")]
public class TestPlugin : BaseUnityPlugin
{
    internal static ManualLogSource Log;
    private void Awake()
    {
        Log = Logger;
        Log.LogInfo("TEST PLUGIN Awake - if you see this, BepInEx loaded the DLL.");
    }
}
