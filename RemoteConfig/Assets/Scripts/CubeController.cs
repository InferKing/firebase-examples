using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField] private CubeView _cubeView;
    [SerializeField] private float _inspectorForce;

    private float _remoteForce;

    public void SetRemoteForce(float force)
    {
        _remoteForce = force;
        Debug.Log($"New remote force: {_remoteForce}");
    }

    public void ApplyRemoteForce()
    {
        _cubeView.UpdateForce(_remoteForce);
    }

    public void ApplyInspectorForce()
    {
        _cubeView.UpdateForce(_inspectorForce);
    }

    public void ApplyScale(float scale)
    {
        _cubeView.transform.localScale = Vector3.one * scale;
    }
}
 