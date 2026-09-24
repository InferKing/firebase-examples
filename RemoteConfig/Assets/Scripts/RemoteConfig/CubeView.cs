using UnityEngine;

public class CubeView : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

    private float _currentForce;

    private void Update()
    {
        transform.Rotate(Vector3.up * (_currentForce * Time.deltaTime));
    }

    public void UpdateForce(float force)
    {
        _currentForce = force;
    }
    
    public void UpdateView()
    {
        
    }
}
