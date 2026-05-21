using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [SerializeField] private Transform Transform_Camera;
    [SerializeField] private ParallaxLayer[] _layers;

    private float _previousCameraY;

    private void Start()
    {
        _previousCameraY = Transform_Camera.position.y;
    }

    private void LateUpdate()
    {
        float deltaY = Transform_Camera.position.y - _previousCameraY;

        foreach (var layer in _layers)
        {
            layer.MoveLayer(deltaY);
        }

        _previousCameraY = Transform_Camera.position.y;
    }
}

[System.Serializable]
public class ParallaxLayer
{
    public Transform Transform_Layer;
    [Range(0f, 1f)] public float _parallaxSpeed = 0.5f;

    public void MoveLayer(float deltaY)
    {
        Transform_Layer.position += new Vector3(0f, deltaY * _parallaxSpeed, 0f);
    }
}