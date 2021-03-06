using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using AssemblyRef = System.Reflection.Assembly;

namespace uEssentialsAntiTracker
{
    public class AntiTracker : RocketPlugin
    {
        public Harmony HarmonyInstance;

        public override void LoadPlugin()
        {
            base.LoadPlugin();
            HarmonyInstance = new Harmony("uEssentialsAntiTrack");
            try
            {
                AssemblyRef asm = AssemblyRef.Load("UEssentials");
                if (asm == null) throw new FileNotFoundException();
                Type anylitics = asm.GetType("Essentials.Misc.Analytics");

                if (anylitics != null)
                {
                    MethodInfo sendAnylticicsEvent = anylitics.GetMethod("SendEvent", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (sendAnylticicsEvent != null)
                    {
                        HarmonyInstance.CreateProcessor(sendAnylticicsEvent).AddPrefix(typeof(AntiTracker).GetMethod("BlockMethod")).Patch();
                        Logger.Log("uEssentials analytics blocked.");
                    }
                    else
                    {
                        Logger.Log("Failed to find Analytics SendEvent Method.");
                    }
                }
                else
                {
                    Logger.Log("Failed to find Analytics Type.");
                }
            }
            catch (FileNotFoundException)
            {
                Logger.Log("uEssentials is not installed.");
            }
        }

        public override void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            base.UnloadPlugin(state);
            HarmonyInstance?.UnpatchAll("uEssentialsAntiTrack");
        }

        public static bool BlockMethod() => false;
    }
}