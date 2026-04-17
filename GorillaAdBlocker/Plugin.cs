using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaAdBlocker;

[BepInPlugin(Constants.GUID, Constants.NAME, Constants.VERS)]
class Plugin : BaseUnityPlugin
{
    private ConfigEntry<bool> isActive, isMuted, isDeleted;

    private void Awake()
    {
        isActive = Config.Bind("General", "Activity", true, "Enables/Disables modification to the Ads");
        isMuted = Config.Bind("General", "Mute TVs", true, "Mutes TV Audio");
        isDeleted = Config.Bind("General", "Hide TVs", false, "Hides TV Screens");

        SceneManager.sceneLoaded += RemoveTVs;
    }

    private void OnDestroy()
        => SceneManager.sceneLoaded -= RemoveTVs;

    private void RemoveTVs(Scene s, LoadSceneMode l)
    {
        if (!isActive.Value) return;

        if (s.name == "City")
        {
            var vTargets = FindObjectsByType<VODTarget>(FindObjectsSortMode.None);

            foreach (var vod in vTargets)
            {
                if (isMuted.Value && vod.AudioSettings != null)
                    vod.AudioSettings.volume = 0f;

                if (isDeleted.Value)
                {
                    Transform screen = vod.transform.Find("Screen");

                    var renderer = screen.GetComponentsInChildren<MeshRenderer>(true);

                    foreach (var r in renderer)
                        r.enabled = false;
                }
            }
        }
    }
}
class Constants
{
    public const string GUID = "kinomonke.gorillaadblocker",
        NAME = "GorillaAdBlocker",
        VERS = "1.00";
}