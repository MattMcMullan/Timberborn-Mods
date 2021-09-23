using HarmonyLib;
using Timberborn.CameraSystem;
using UnityEngine;

namespace ZoomOutCamera
{
    class CameraSystemPatch
    {
        [HarmonyPatch(typeof(CameraComponent), "Awake")]
        public static class UpdateZoomLimit
        {
            private static void Postfix(CameraComponent __instance)
            {
                var default_initial = __instance.DefaultZoomLimits;
                var relaxed_initial = __instance.RelaxedZoomLimits;
                __instance.DefaultZoomLimits = __instance.RelaxedZoomLimits;
                __instance.RelaxedZoomLimits = __instance.MapEditorZoomLimits;
                Debug.Log("Increased zoom limits.");
                Debug.Log($"Default Zoom Limit {default_initial.Max} -> {__instance.DefaultZoomLimits.Max}");
                Debug.Log($"Relaxed Zoom Limit {relaxed_initial.Max} -> {__instance.MapEditorZoomLimits.Max}");
                Debug.Log($"Map Editor Zoom Limit {__instance.MapEditorZoomLimits.Max}, Unchanged");
            }
        }
    }
}
