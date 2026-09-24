using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace FirebaseExamples.RemoteConfigAddressables
{
    public sealed class AddressableCubeView : MonoBehaviour
    {
        [FormerlySerializedAs("visualRoot")]
        [SerializeField] private Transform _visualRoot;
        [FormerlySerializedAs("fallbackVisual")]
        [SerializeField] private GameObject _fallbackVisual;

        private AsyncOperationHandle<GameObject> _currentInstance;
        private bool _isLoading;

        public string CurrentKey { get; private set; } = "local/default";

        private void Update()
        {
            transform.Rotate(Vector3.up, 30f * Time.deltaTime);
        }

        public async Task ShowAsync(string key)
        {
            if (_isLoading || key == CurrentKey)
                return;

            if (key == "local/default")
            {
                ReleaseCurrent();
                _fallbackVisual.SetActive(true);
                CurrentKey = key;
                return;
            }

            _isLoading = true;
            var nextInstance = default(AsyncOperationHandle<GameObject>);

            try
            {
                nextInstance = Addressables.InstantiateAsync(key, _visualRoot);
                await nextInstance.Task;

                if (this == null)
                    return;

                if (nextInstance.Status != AsyncOperationStatus.Succeeded)
                    throw nextInstance.OperationException ?? new Exception("Could not load: " + key);

                // Keep the previous visual until the replacement is ready.
                ReleaseCurrent();
                _currentInstance = nextInstance;
                nextInstance = default;
                _fallbackVisual.SetActive(false);
                CurrentKey = key;
            }
            finally
            {
                if (nextInstance.IsValid())
                    Addressables.ReleaseInstance(nextInstance);

                _isLoading = false;
            }
        }

        private void ReleaseCurrent()
        {
            if (!_currentInstance.IsValid())
                return;

            if (_currentInstance.Result != null)
                _currentInstance.Result.SetActive(false);
            Addressables.ReleaseInstance(_currentInstance);
            _currentInstance = default;
        }

        private void OnDestroy()
        {
            ReleaseCurrent();
        }
    }
}


