using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.RemoteConfig;
using UnityEngine;

public class RemoteLoader : MonoBehaviour
{
    [SerializeField] private CubeController _cubeController;

    private FirebaseRemoteConfig _config;
    private bool _isSubscribedToUpdates;
    private bool _isConfigOperationInProgress;

    private void OnEnable()
    {
        SubscribeToUpdates();
    }

    private void OnDisable()
    {
        UnsubscribeFromUpdates();
    }

    private async void Start()
    {
        try
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependencyStatus != DependencyStatus.Available)
            {
                Debug.LogError($"Firebase initialization failed: {dependencyStatus}");
                return;
            }

            await InitializeRemoteConfigAsync();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    private async Task InitializeRemoteConfigAsync()
    {
        var defaults = new Dictionary<string, object>
        {
            { "cube_force", 10f },
            { "cube_scale", 1f },
            { "texture_url", "url" }
        };

        _config = FirebaseRemoteConfig.DefaultInstance;
        await _config.SetDefaultsAsync(defaults);

        await FetchAndApplyConfigAsync("initial");
        SubscribeToUpdates();
    }

    private async Task FetchAndApplyConfigAsync(string requestSource)
    {
        if (_isConfigOperationInProgress)
        {
            Debug.LogWarning("A Remote Config operation is already in progress.");
            return;
        }

        _isConfigOperationInProgress = true;

        try
        {
            Debug.Log($"Remote Config {requestSource} fetch started.");
            await _config.FetchAsync(TimeSpan.Zero);

            var info = _config.Info;
            Debug.Log(
                $"Remote Config {requestSource} fetch finished: " +
                $"status={info.LastFetchStatus}, fetchTime={info.FetchTime:O}");

            if (info.LastFetchStatus != LastFetchStatus.Success)
            {
                Debug.LogError(
                    $"Remote Config fetch failed: {info.LastFetchStatus}, " +
                    $"reason={info.LastFetchFailureReason}, throttledUntil={info.ThrottledEndTime:O}");
                return;
            }

            var activated = await _config.ActivateAsync();
            Debug.Log($"Remote Config {requestSource} activation: changed={activated}");

            ApplySettings();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
        finally
        {
            _isConfigOperationInProgress = false;
        }
    }

    private void SubscribeToUpdates()
    {
        if (!isActiveAndEnabled || _config == null || _isSubscribedToUpdates)
        {
            return;
        }

        if (Application.platform != RuntimePlatform.Android &&
            Application.platform != RuntimePlatform.IPhonePlayer)
        {
            Debug.Log("Real-time Remote Config is unavailable in Unity Editor; use manual fetch.");
            return;
        }

        _config.OnConfigUpdateListener += OnConfigUpdated;
        _isSubscribedToUpdates = true;
        Debug.Log("Real-time Remote Config listener connected.");
    }

    private void UnsubscribeFromUpdates()
    {
        if (_config == null || !_isSubscribedToUpdates)
        {
            return;
        }

        _config.OnConfigUpdateListener -= OnConfigUpdated;
        _isSubscribedToUpdates = false;
    }

    private async void OnConfigUpdated(object sender, ConfigUpdateEventArgs args)
    {
        if (args.Error != RemoteConfigError.None)
        {
            Debug.LogError($"Real-time Remote Config error: {args.Error}");
            return;
        }

        if (_isConfigOperationInProgress)
        {
            Debug.LogWarning("A Remote Config operation is already in progress.");
            return;
        }

        _isConfigOperationInProgress = true;

        try
        {
            Debug.Log($"Real-time Remote Config updated keys: {string.Join(", ", args.UpdatedKeys)}");

            var activated = await _config.ActivateAsync();
            Debug.Log($"Real-time Remote Config activation: changed={activated}");

            ApplySettings();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
        finally
        {
            _isConfigOperationInProgress = false;
        }
    }

    private void ApplySettings()
    {
        var value = _config.GetValue("cube_force");
        var scale = _config.GetValue("cube_scale");
        var force = (float)value.DoubleValue;
        var totalScale = (float)scale.DoubleValue;

        Debug.Log($"cube_force={force}, source={value.Source}");
        _cubeController.SetRemoteForce(force);
        _cubeController.ApplyRemoteForce();
        _cubeController.ApplyScale(totalScale);
    }
}
