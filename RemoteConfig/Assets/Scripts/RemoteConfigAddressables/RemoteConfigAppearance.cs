using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.RemoteConfig;
using UnityEngine;
using UnityEngine.Serialization;

namespace FirebaseExamples.RemoteConfigAddressables
{
    public sealed class RemoteConfigAppearance : MonoBehaviour
    {
        [FormerlySerializedAs("cube")]
        [SerializeField] private AddressableCubeView _cube;

        private FirebaseRemoteConfig _config;

        public bool IsBusy { get; private set; }
        public string Status { get; private set; } = "Local default";

        private void Start()
        {
            Refresh();
        }

        public async void Refresh()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Status = "Loading...";

            try
            {
                if (_config == null)
                    await InitializeAsync();

                if (this == null)
                    return;

                // Zero interval is for the demo's manual refresh button.
                await _config.FetchAsync(TimeSpan.Zero);
                if (this == null)
                    return;

                if (_config.Info.LastFetchStatus != LastFetchStatus.Success)
                    throw new Exception("Firebase fetch failed: " + _config.Info.LastFetchStatus);

                await _config.ActivateAsync();
                if (this == null)
                    return;

                await ApplyAsync();
            }
            catch (Exception exception)
            {
                if (this != null)
                {
                    Status = "Could not update. Previous visual kept.";
                    Debug.LogWarning(exception.Message, this);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task InitializeAsync()
        {
            var dependencies = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (this == null)
                return;

            if (dependencies != DependencyStatus.Available)
                throw new Exception("Firebase initialization failed: " + dependencies);

            var config = FirebaseRemoteConfig.DefaultInstance;
            await config.SetDefaultsAsync(new Dictionary<string, object>
            {
                { "cube_visual_key", "local/default" }
            });

            if (this == null)
                return;

            _config = config;
        }

        private async Task ApplyAsync()
        {
            string key = _config.GetValue("cube_visual_key").StringValue.Trim();
            if (string.IsNullOrEmpty(key))
                throw new Exception("cube_visual_key is empty.");

            await _cube.ShowAsync(key);
            if (this != null)
                Status = "Visual: " + _cube.CurrentKey;
        }
    }
}


