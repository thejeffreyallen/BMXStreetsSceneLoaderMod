using BMXStreetsSceneLoaderMod;
using MelonLoader;


[assembly: MelonInfo(typeof(Mod), "BMX Streets Scene Loader Mod", "1.0.0", "Lineryder")]

namespace BMXStreetsSceneLoaderMod
{
    /// <summary>
    /// Represents a modification (mod) that patches methods at application start using Harmony.
    /// </summary>
    /// <remarks>
    /// This class extends the MelonMod class and is used to apply patches to the application's methods
    /// during its initialization phase. It creates a new Harmony instance specifically identified by
    /// "BMX.Streets.Scene.Loader.Mod" to manage these patches. The OnApplicationStart method overrides the base class's
    /// method to initialize the patching process.
    /// </remarks>
    public class Mod : MelonMod
    {
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();

            var harmony = new HarmonyLib.Harmony("BMX.Streets.Scene.Loader.Mod");
            harmony.PatchAll();
        }

    }

}

