using UnityEngine;
using UnityEngine.Serialization;

namespace FirebaseExamples.RemoteConfigAddressables
{
    public sealed class AppearanceDemoPanel : MonoBehaviour
    {
        [FormerlySerializedAs("remoteConfig")]
        [SerializeField] private RemoteConfigAppearance _remoteConfig;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(16, 16, 320, 100), GUI.skin.box);
            GUILayout.Label(_remoteConfig.Status);

            bool wasEnabled = GUI.enabled;
            GUI.enabled = wasEnabled && !_remoteConfig.IsBusy;
            if (GUILayout.Button("Update from Firebase", GUILayout.Height(36)))
                _remoteConfig.Refresh();
            GUI.enabled = wasEnabled;

            GUILayout.EndArea();
        }
    }
}

