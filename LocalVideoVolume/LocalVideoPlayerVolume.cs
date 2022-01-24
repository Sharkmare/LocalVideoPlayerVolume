using HarmonyLib;
using NeosModLoader;
using FrooxEngine;

namespace LocalVideoPlayerVolume
{
    public class LocalVideoPlayerVolume : NeosMod
    {
        public override string Name => "LocalVideoPlayerVolume";
        public override string Author => "Sox";
        public override string Version => "1.0.0";
        public override string Link => "";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.Sox.LocalVideoPlayerVolume");
            harmony.PatchAll();
        }
		
        [HarmonyPatch(typeof(VideoPlayer), "SetupMainAudioOutput")]
        class LocalVideoPlayerVolumePatch
        {
            static void Postfix(VideoPlayer __instance, SyncRef<AudioOutput> ____mainAudioOutput)
            {
             ValueUserOverride<float> volumeoverride =__instance.Slot.AttachComponent<ValueUserOverride<float>>();
                volumeoverride.CreateOverrideOnWrite.Value = true;
                volumeoverride.Target.Target = ____mainAudioOutput.Target.Volume;
                volumeoverride.Default.Value = 1f;
            }
        }
    }
}