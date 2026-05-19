using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    private float _screenLeft;
    private float _screenRight;

    [SerializeField] private float _wrapOffsetLeft = 0.1f;
    [SerializeField] private float _wrapOffsetRight = 0.9f;
    private void Awake()
    {
        float z = Mathf.Abs(Camera.main.transform.position.z);
        _screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(_wrapOffsetLeft, 0, z)).x;
        _screenRight = Camera.main.ViewportToWorldPoint(new Vector3(_wrapOffsetRight, 0, z)).x;
    }

    private void Update()
    {
        Vector3 pos = transform.position;

        if(pos.x > _screenRight)
        {
            pos.x = _screenLeft;
            transform.position = pos;
        }
        else if(pos.x < _screenLeft)
        {
            pos.x = _screenRight;
            transform.position = pos;
        }
    }

}
