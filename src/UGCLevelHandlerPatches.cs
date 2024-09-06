using HarmonyLib;
using Il2CppMG_Core.UGC;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2Cpp;

namespace BMXStreetsSceneLoaderMod
{
    public class UGCLevelHandlerPatches
    {

        [HarmonyPatch(typeof(UGCLevelHandler), nameof(UGCLevelHandler.GetAllAssetBundlePaths))]
        class UGCLevelHandler_LoadModMaps_Patch
        {
            /// <summary>
            /// Harmony prefix method that ...
            /// </summary>
            /// <param name="__instance">The instance of UGCLevelHandler class.</param>
            /// <returns>Always returns false to override original method execution.</returns>
            [HarmonyPrefix]
            static bool Prefix(ref Il2CppStringArray __result)
            {
                string mapsDocumentsAssetsLocation = GetDocumentsMapsDirectoryPath();
                string modIOLocation = Application.persistentDataPath + "/mod.io/";
                bool flag = Directory.Exists(modIOLocation);
                DirectoryInfo directory = new FileInfo(mapsDocumentsAssetsLocation).Directory;
                if (directory != null)
                {
                    directory.Create();
                }
                string[] documentsMapBundlePaths = GetAllAssetBundlePathsAtDirectory(mapsDocumentsAssetsLocation);
                string[] bundleFilePaths;
                if (flag && documentsMapBundlePaths.Count() > 0)
                {
                    string[] modIOBundlePaths = GetAllAssetBundlePathsAtDirectory(modIOLocation);
                    bundleFilePaths = documentsMapBundlePaths.Concat(modIOBundlePaths).ToArray<string>();
                }
                else
                {
                    bundleFilePaths = documentsMapBundlePaths;
                }
                /*string[] array2 = bundleFilePaths;
                for (int i = 0; i < array2.Length; i++)
                {
                    Logger.Log(array2[i]);
                }*/

                __result = bundleFilePaths;

                return false;
            }

            private static string GetDocumentsMapsDirectoryPath()
            {
                string newFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BMX Streets");
                if (!Directory.Exists(newFolderPath))
                {
                    Directory.CreateDirectory(newFolderPath);
                }
                string nextDirPath = Path.Combine(newFolderPath, "Maps");
                if (!Directory.Exists(nextDirPath))
                {
                    Directory.CreateDirectory(nextDirPath);
                }
                return nextDirPath;
            }

            private static string[] GetAllAssetBundlePathsAtDirectory(string directory)
            {
                Logger.Log("[UGCBundleHandler] GetAllAssetBundlePathsAtDirectory: " + directory);
                IEnumerable<string> files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
                if (files == null || files.Count() == 0)
                { 
                    return Array.Empty<string>();
                }
                return files.Where(x => string.IsNullOrEmpty(Path.GetExtension(x)) || Path.GetExtension(x) == ".bundle").ToArray();
            }


            public static bool TryGetScreenShotPathForBundle(string bundlePath, out string screenShotPath)
            {
                string path = Path.GetDirectoryName(bundlePath);
                string[] pngFilePaths = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
                string[] jpgFilePaths = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
                string[] bundleFilePaths = pngFilePaths.Concat(jpgFilePaths).ToArray<string>();
                if (bundleFilePaths.Length != 0 && Path.GetFileNameWithoutExtension(bundleFilePaths[0]) == Path.GetFileNameWithoutExtension(bundlePath))
                {
                    screenShotPath = bundleFilePaths[0];
                    return true;
                }

                Logger.LogError($"Failed to get screenshot: {Path.GetFileNameWithoutExtension(bundleFilePaths[0])} : {Path.GetFileNameWithoutExtension(bundlePath)}");
                screenShotPath = string.Empty;
                return false;
            }

            /*public static void ModMapDataFromBundlePath(UGCLevelHandler __instance, string bundleFilePath)
            {
                AssetBundle bundle = AssetBundleManager.Load(bundleFilePath, 0);
                Debug.Log("[UGCBundleHandler] Attempt to ModMapDataFromBundlePath: " + bundleFilePath);
                Debug.Log("Bundle Loaded ?  :" + (bundle != null).ToString());
                if (bundle)
                {
                    string[] scenes = bundle.GetAllScenePaths();
                    Debug.Log("Num Scenes :" + scenes.Length.ToString());
                    if (scenes.Length != 0)
                    {
                        string sceneName = scenes[0];
                        foreach (var map in __instance._modMaps)
                        {
                            if (map.Path == sceneName)
                            {
                                Texture2D screenShot = null;
                                string screenShotPath;
                                if (__instance.TryGetScreenShotPathForBundle(bundleFilePath, out screenShotPath))
                                {
                                    Debug.Log("Bundle Has Screenshot  :" + screenShotPath);
                                    __instance.CreateTexture2DFromFile(screenShotPath, out screenShot);
                                }
                                else
                                {
                                    screenShot = __instance._defualtScreenShot;
                                }
                                Il2CppSystem.Collections.Generic.List<UGCSceneSO> modMaps = __instance._modMaps;
                                UGCSceneSO ugcsceneSO = modMaps[modMaps.Count - 1];

                                UGCSceneSO modMapSceneData = UGCSceneSO.CreateUGCSceneSO(bundleFilePath, screenShot, __instance._sessionMarker, ugcsceneSO._sceneManager);
                                __instance._modLevelsContainer.AddDataToList(modMapSceneData);
                                __instance._modMaps.Add(modMapSceneData);
                                if (__instance._onModMapLoadedGE != null)
                                {
                                    ugcsceneSO.OnSceneLoaded += new Action(delegate ()
                                    {
                                        __instance._onModMapLoadedGE.Raise();
                                    });
                                }
                                ugcsceneSO.OnSceneLoaded_Scene += new Action<Scene>(__instance.InjectManagerToScene);
                            }
                        }
                    }
                }
            }*/
        }

        [HarmonyPatch(typeof(UGCLevelHandler), nameof(UGCLevelHandler.TryGetScreenShotPathForBundle))]
        class UGCLevelHandler_TryGetScreenShotPathForBundle_Patch
        {
            [HarmonyPrefix]
            static bool Prefix(string bundlePath, ref bool __result, out string screenShotPath)
            {
                string path = Path.GetDirectoryName(bundlePath);
                string[] pngFilePaths = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
                string[] jpgFilePaths = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
                string[] bundleFilePaths = pngFilePaths.Concat(jpgFilePaths).ToArray<string>();
                if (bundleFilePaths.Length != 0)
                {
                    screenShotPath = bundleFilePaths[0];
                    __result = true;
                    return false;
                }

                Logger.LogError($"Failed to get screenshot: {bundleFilePaths[0]} : {bundlePath}");
                screenShotPath = string.Empty;

                __result = false;
                return false;
            }
        }

        [HarmonyPatch(typeof(UGCLevelHandler), nameof(UGCLevelHandler.CreateTexture2DFromFile))]
        class UGCLevelHandler_CreateTexture2DFromFile_Patch
        {
            [HarmonyPrefix]
            static bool Prefix(string path, ref bool __result, out Texture2D texture2D)
            {
                texture2D = null;
                bool validExt = Path.GetExtension(path) != "jpg" || Path.GetExtension(path) != "png";
                if (!File.Exists(path) || !validExt)
                {
                    Logger.LogError("[UGCBundleHandler] CreateTexture2DFromFile Failed for :" + path);
                    __result = false;
                    return false;
                }
                byte[] binaryImageData = File.ReadAllBytes(path);
                texture2D = new Texture2D(1920, 1080);
                texture2D.LoadImage(binaryImageData);

                __result = true;
                return false;
            }
        }
    }
}
