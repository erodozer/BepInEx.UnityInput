using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace BepInEx.Unity
{
    public enum PressState
    {
        None,
        Up,
        Down,
        Held,
    }

    [BepInPlugin(GUID, "BepInEx.UnityInput", Version)]
    public class InputSimulator : BaseUnityPlugin
    {
        public const string GUID = "github.lunared.bepinex.unityinput";
        public const string Version = "0.2.0";

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

        public static KeyCode StrToKeyCode(string name)
        {
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), name);
        }

        private static class Triggers
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Input), nameof(Input.GetMouseButtonUp))]
            public static void InterceptButtonUp(ref int button, ref bool __result, ref bool __runOriginal)
            {
                mouseButton.TryGetValue(button, out var isSet);
                if (isSet == PressState.Up)
                {
                    __result = true;
                    __runOriginal = false;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Input), nameof(Input.GetMouseButtonDown))]
            public static void InterceptButtonDown(ref int button, ref bool __result, ref bool __runOriginal)
            {
                mouseButton.TryGetValue(button, out var isSet);
                if (isSet == PressState.Down)
                {
                    __result = true;
                    __runOriginal = false;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Input), nameof(Input.GetMouseButton))]
            public static void InterceptButtonPress(ref int button, ref bool __result, ref bool __runOriginal)
            {
                mouseButton.TryGetValue(button, out var isSet);
                if (isSet == PressState.Down || isSet == PressState.Held)
                {
                    __result = true;
                    __runOriginal = false;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Input), nameof(Input.GetKeyUp), typeof(string))]
            public static void InterceptKeyUp(ref string name, ref bool __result, ref bool __runOriginal)
            {
                keys.TryGetValue(StrToKeyCode(name), out var isSet);
                if (isSet == PressState.Up)
                {
                    __result = true;
                    __runOriginal = false;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Input), nameof(Input.GetKeyUp), typeof(KeyCode))]
            public static void InterceptKeyUp(ref KeyCode key, ref bool __result, ref bool __runOriginal)
            {
                keys.TryGetValue(key, out var isSet);
                if (isSet == PressState.Up)
                {
                    __result = true;
                    __runOriginal = false;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), typeof(string))]
            public static void InterceptKeyDown(ref string name, ref bool __result, ref bool __runOriginal)
            {
                keys.TryGetValue(StrToKeyCode(name), out var isSet);
                if (isSet == PressState.Down)
                {
                    __result = true;
                    __runOriginal = false;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), typeof(KeyCode))]
            public static void InterceptKeyDown(ref KeyCode key, ref bool __result, ref bool __runOriginal)
            {
                keys.TryGetValue(key, out var isSet);
                if (isSet == PressState.Down)
                {
                    __result = true;
                    __runOriginal = false;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Input), nameof(Input.GetKey), typeof(string))]
            public static void InterceptKeyHeld(ref string name, ref bool __result, ref bool __runOriginal)
            {
                keys.TryGetValue(StrToKeyCode(name), out var isSet);
                if (isSet == PressState.Down || isSet == PressState.Held)
                {
                    __result = true;
                    __runOriginal = false;
                }
            }
        }

        private static readonly Dictionary<int, PressState> mouseButton = new Dictionary<int, PressState>()
        {
            { 0, PressState.None }, { 1, PressState.None }, { 2, PressState.None }
        };

        private static readonly Dictionary<KeyCode, PressState> keys = new Dictionary<KeyCode, PressState>();

        public static void MouseButtonUp(int button)
        {
            mouseButton[button] = PressState.Up;
        }

        public static void MouseButtonDown(int button)
        {
            mouseButton[button] = PressState.Up;
        }

        public static void MouseButtonHold(int button)
        {
            mouseButton[button] = PressState.Held;
        }

        public static void UnsetMouseButton(int button)
        {
            mouseButton[button] = PressState.None;
        }

        public static void KeyUp(KeyCode key)
        {
            keys[key] = PressState.Up;
        }

        public static void KeyDown(KeyCode key)
        {
            keys[key] = PressState.Down;
        }

        public static void KeyHold(KeyCode key)
        {
            keys[key] = PressState.Held;
        }

        public static void UnsetKey(KeyCode key)
        {
            keys[key] = PressState.None;
        }
    }
}
