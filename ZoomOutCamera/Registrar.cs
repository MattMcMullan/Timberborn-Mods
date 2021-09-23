using BepInEx;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace ZoomOutCamera
{
    [BepInPlugin("Matt.McMullan.Timberborn.ZoomOutCamera", "Zoom Out The Camera", "1.0.0.0")]
    [BepInProcess("Timberborn.exe")]
    public class Registrar : BaseUnityPlugin
    {
        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Debug.Log("Loaded ZoomOutMore.");
        }
    }
}
