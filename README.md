# BepInEx.UnityInput

Adds Harmony hooks so that BepInEx plugins may intercept Unity's Global Input system.

Development for this largely stems from interest of extending functionality of Illusion games, such as Koikatsu, which extensively make use of `UnityEngine.Input` in places where by best practice it shouldn't.  
In areas where you wish to use original game functionality through the simulation of button presses on the UI, you may need to occasionally fake an Input state.

## Requirements

* Install BepInEx 5.4 or later.

## Using this in your own Plugins

Plugins that require BepInEx.UnityInput should specify the dependency like so
```
    [BepInPlugin(GUID, "MyPlugin", Version)]
    [BepInDependency("github.lunared.bepinex.unityinput", BepInDependency.DependencyFlags.HardDependency)]
    internal class MyPlugin : BaseUnityPlugin
    {
    }
```

Once your plugin is set up to depend on the UnityInput one, you can now reference the Global methods where you need to.

The functions are all exposed similar to Unity's own Input system.
In this example, we can demonstrate how to click an Action button in Koikatsu's HSprite UI

```csharp
using UnityEngine;
using KKAPI.MainGame;
using BepInEx.Unity; // this has InputSimulator in it, which we use to override input

namespace MyPlugin 
{
    internal class MyPluginController : GameCustomFunctionController 
    {

        protected override void OnStartH(HSceneProc proc, bool freeH)
        {
            StartCoroutine(PickAction(proc));
        }

        IEnumerator PickAction(HSceneProc proc)
        {
            while (true)
            {
                // get the list of visible actions from one of the interfaces
                var choices = proc.sprite.sonyu.categoryActionButton.lstButton.Where(
                    button => button.isActiveAndEnabled && button.interactable
                ).ToList();

                // leverage simulating clicking in the menu
                var index = Random.RandomRangeInt(0, choices.Count);
                var nextAction = choices[index];

                // many koikatsu actions check for left click mouse up 
                // within their onClick handler
                InputSimulator.MouseButtonUp(0);
                // perform our action
                nextAction?.onClick?.Invoke();
                // reset the input state after performing the action
                InputSimulator.UnsetMouseButton(0);
                yield return new WaitForSeconds(1f);
            }
        }
    }
```
