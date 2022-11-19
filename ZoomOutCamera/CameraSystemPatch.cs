using HarmonyLib;
using Timberborn.CameraSystem;
using Timberborn.Common;
using UnityEngine;

namespace ZoomOutCamera
{
    class CameraSystemPatch
    {
        [HarmonyPatch(typeof(CameraComponent), "Awake")]
        public static class UpdateZoomLimit
        {
            private static void Postfix(ref FloatLimits ____defaultZoomLimits, ref FloatLimits ____relaxedZoomLimits, FloatLimits ____mapEditorZoomLimits)
            {
                var default_initial = ____defaultZoomLimits;
                var relaxed_initial = ____relaxedZoomLimits;
                ____defaultZoomLimits = ____relaxedZoomLimits;
                ____relaxedZoomLimits = ____mapEditorZoomLimits;
                Debug.Log("Increased zoom limits.");
                Debug.Log($"Default Zoom Limit {default_initial.Max} -> {____defaultZoomLimits.Max}");
                Debug.Log($"Relaxed Zoom Limit {relaxed_initial.Max} -> {____mapEditorZoomLimits.Max}");
                Debug.Log($"Map Editor Zoom Limit {____mapEditorZoomLimits.Max}, Unchanged");
            }
        }
    }
}
