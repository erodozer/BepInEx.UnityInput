using BepInEx.Bootstrap;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace BepInEx.Unity
{
    [BepInPlugin(GUID, "BepInEx.UnityInput", Version)]
    public class InputSimulator : BaseUnityPlugin
    {
        public const string GUID = "github.lunared.bepinex.unityinput";
        public const string Version = "0.1.0";

        public static new PluginInfo Info { get; private set; }

        private static Harmony _pluginTriggers;

        public void Awake()
        {
            _pluginTriggers = Harmony.CreateAndPatchAll(
                typeof(Triggers)
            );
        }

        public void OnDestroy()
        {
            _pluginTriggers?.UnpatchAll();
        }

        private static class Triggers
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Input), nameof(Input.GetMouseButtonUp))]
            public static void InterceptButtonUp(ref int button, ref bool __result, ref bool __runOriginal)
            {
                mouseUp.TryGetValue(button, out var isSet);
                if (isSet)
                {
                    __result = true;
                    __runOriginal = false;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Input), nameof(Input.GetMouseButtonDown))]
            public static void InterceptButtonDown(ref int button, ref bool __result, ref bool __runOriginal)
            {
                mouseDown.TryGetValue(button, out var isSet);
                if (isSet)
                {
                    __result = true;
                    __runOriginal = false;
                }
            }
        }

        private static readonly Dictionary<int, bool> mouseUp = new Dictionary<int, bool>()
        {
            { 0, false }, { 1, false }, { 2, false }
        };
        private static readonly Dictionary<int, bool> mouseDown = new Dictionary<int, bool>()
        {
            { 0, false }, { 1, false }, { 2, false }
        };

        public static Dictionary<int, bool> MouseUp
        {
            get { return mouseUp; }
        }

        public static Dictionary<int, bool> MouseDown
        {
            get { return mouseDown; }
        }

        public static void MouseButtonUp(int button)
        {
            mouseUp[button] = true;
        }

        public static void MouseButtonDown(int button)
        {
            mouseDown[button] = true;
        }

        public static void UnsetMouseButton(int button)
        {
            mouseUp[button] = false;
            mouseDown[button] = false;
        }
    }
}
