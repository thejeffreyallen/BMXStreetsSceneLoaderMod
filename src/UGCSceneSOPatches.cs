using HarmonyLib;
using Il2Cpp;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using MelonLoader;


namespace BMXStreetsSceneLoaderMod
{
    class UGCSceneSOPatches
    {
        private static Scene mainScene;
        private static Scene? previousScene = null;
        /// <summary>
        /// A Harmony patch for the UGCSceneSO.Load method, which is responsible for dynamically loading
        /// assemblies based on the scene's bundle path. This class enhances the mod's capability to load
        /// scenes
        /// </summary>
        /// <remarks>
        /// This class uses a Harmony prefix to intercept the loading process
        /// of UGC scenes
        /// </remarks>
        [HarmonyPatch(typeof(UGCSceneSO), nameof(UGCSceneSO.Load))]
        class UGCSceneSOLoadPatch
        {
            /// <summary>
            /// Harmony prefix method that ...
            /// </summary>
            /// <param name="__instance">The instance of UGCSceneSO being loaded.</param>
            /// <returns>Always returns true to continue with the original method execution.</returns>
            [HarmonyPrefix]
            static bool Prefix(UGCSceneSO __instance)
            {
                if (!__instance.IsUnloaded)
                {
                    return true;
                }
                var assetBundle = AssetBundleManager.Load(__instance._bundlePath, 0);
                if (!assetBundle)
                {
                    return true;
                }

                if (!mainScene.IsValid())
                {
                    mainScene = SceneManager.GetSceneByName("BMXSWorldMain");
                    if (mainScene.isLoaded)
                    {
                        Logger.Log($"Deactivating scene: {mainScene.name}");
                        DeactivateScene(mainScene);
                    }
                }

                Logger.Log($"Path value: {assetBundle.GetAllScenePaths()[0]}");
                __instance.SetLoading();

                MelonCoroutines.Start(LoadSceneAsyncAndActivate(__instance, assetBundle));

                return false;
            }

            private static IEnumerator LoadSceneAsyncAndActivate(UGCSceneSO __instance, AssetBundle assetBundle)
            {
                // Start the async scene load operation
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(assetBundle.GetAllScenePaths()[0], LoadSceneMode.Additive);

                // Optionally set allowSceneActivation to false if you want to control when the scene activates
                asyncOperation.allowSceneActivation = true;

                // Wait until the async operation is done (progress will be 1 when the scene is ready)
                while (!asyncOperation.isDone)
                {
                    // You can optionally check progress here
                    // asyncOperation.progress is the progress of loading (between 0.0 and 0.9), and the scene activation will happen when it's 1.0
                    yield return null;  // Wait for the next frame
                }

                // Small delay to ensure everything is ready
                yield return new WaitForSeconds(0.5f);

                // Once the scene is loaded, retrieve the Scene struct using the scene path
                Scene loadedScene = SceneManager.GetSceneByPath(assetBundle.GetAllScenePaths()[0]);

                if (loadedScene.IsValid() && loadedScene.isLoaded)
                {
                    Logger.Log($"Scene valid: {loadedScene.IsValid()}, Scene loaded: {loadedScene.isLoaded}");
                    // Call the method you previously used as a callback
                    OnLevelLoaded(__instance, loadedScene);
                }
                else
                {
                    // Handle scene not being valid, if necessary
                    Logger.LogError("Scene loading failed or scene is not valid.");
                    yield return new WaitForEndOfFrame();
                }
            }

            private static void OnLevelLoaded(UGCSceneSO __instance, Scene scene)
            {
                try
                {
                    Logger.Log($"Scene valid: {scene.IsValid()}, Scene loaded: {scene.isLoaded}");

                    __instance.SetLoaded();
                    __instance.OnSceneLoaded?.Invoke();
                    __instance.OnSceneLoaded_Scene?.Invoke(scene);
                    __instance._scene = scene;
                    GameObject[] rootGameObjects = scene.GetRootGameObjects();
                    for (int i = 0; i < rootGameObjects.Length; i++)
                    {
                        foreach (Transform trans in rootGameObjects[i].GetComponentsInChildren<Transform>(true))
                        {
                            if (trans.name.ToLower().Contains("spawnpoint") || trans.name.ToLower().Contains("spawpoint") || trans.name.ToLower().Contains("spawn point"))
                            {
                                __instance._sessionMarker.SetLocation(trans.position, trans.rotation);
                                __instance._sessionMarker.PlaceMarker(trans.position, trans.rotation);
                                __instance._sessionMarker.RequestTeleportTo();
                            }
                        }
                    }
                    if (previousScene != null)
                    {
                        SceneManager.UnloadScene((Scene)previousScene);
                    }
                    previousScene = scene;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error when loading scene: {ex.Message} :\n\n {ex.StackTrace}");
                }
            }

            private static void DeactivateScene(Scene scene)
            {
                foreach (GameObject obj in scene.GetRootGameObjects())
                {
                    obj.SetActive(false); // Deactivate all root objects
                }
            }
        }
    }
}

