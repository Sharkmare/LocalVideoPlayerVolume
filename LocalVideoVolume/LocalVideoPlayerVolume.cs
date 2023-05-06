using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using BaseX;
using static NeosAssets;

namespace LocalVideoPlayerVolume
{
    public class LocalVideoPlayerVolume : NeosMod
    {
        public static ModConfiguration Config;

        [AutoRegisterConfigKey]
        public static ModConfigurationKey<float> Volume = new ModConfigurationKey<float>("Local Default Volume", "Sets the Local Volume for everyone", () => 0f);
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<float> MaxDistance = new ModConfigurationKey<float>("Maximum Falloff", "Sets the Maximum Falloff", () => 10f);
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<float> MinDistance = new ModConfigurationKey<float>("Minimum Falloff", "Sets the Minimum Falloff", () => 0f);
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<int> RolloffMode = new ModConfigurationKey<int>("Rolloff Mode", "Sets the Rolloff Mode (0 = Linear, 1 = Log)", () => 0);
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<int> DistanceSpace = new ModConfigurationKey<int>("Distance Space", "Sets the Distance Space (0 = Local, 1 = Global)", () => 0);

        public override string Name => "LocalVideoPlayerVolume";
        public override string Author => "Sox & darbdarb & Sharkmare";
        public override string Version => "1.1";
        public override string Link => "";
        public override void OnEngineInit()
        {
            Config = GetConfiguration();
            Config.Save(true);
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
                volumeoverride.Default.Value = Config.GetValue(Volume);
                ____mainAudioOutput.Target.MaxDistance.Value = Config.GetValue(MaxDistance);
                ____mainAudioOutput.Target.MinDistance.Value = Config.GetValue(MinDistance);
                // Linear ensures good volume for watchers and harsh falloff
                ____mainAudioOutput.Target.RolloffMode.Value = Config.GetValue(RolloffMode) == 0 ? AudioRolloffMode.Linear : AudioRolloffMode.Logarithmic;
                ____mainAudioOutput.Target.DistanceSpace.Value = Config.GetValue(DistanceSpace) == 0 ? AudioDistanceSpace.Local : AudioDistanceSpace.Global;
                /* Fix this later
                IsPlayingDriver disableonpause = __instance.Slot.AttachComponent<IsPlayingDriver>()
                disableonpause.Targets.Add(____mainAudioOutput.Target.EnabledField);
                disableonpause.Playback.Target = (SyncPlayback)__instance.Video.TryGetField("Playback");
                */
            }
        }
    }
}