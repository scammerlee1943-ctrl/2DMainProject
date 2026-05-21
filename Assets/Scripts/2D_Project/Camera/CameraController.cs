using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform Transform_Player;
    [SerializeField] private Player Player_Script;

    private float _screenHeight;
    private float _cameraY;

    private void Awake()
    {
        _screenHeight = Camera.main.orthographicSize * 2f;
        _cameraY = transform.position.y;
    }

    private void Update()
    {
        float playerY = Transform_Player.position.y;

        if (playerY > _cameraY + Camera.main.orthographicSize)
        {
            _cameraY += _screenHeight;
            transform.position = new Vector3(transform.position.x, _cameraY, transform.position.z);
        }


        // 플레이어가 화면 위쪽 끝을 넘으면 한 화면 올라감
        if (playerY < _cameraY - Camera.main.orthographicSize)
        {
            _cameraY -= _screenHeight;
            transform.position = new Vector3(transform.position.x, _cameraY, transform.position.z);
        }
        // 아래로 떨어질 때 따라 내려옴
        else if (playerY < _cameraY - Camera.main.orthographicSize)
        {
            _cameraY -= _screenHeight;
            transform.position = new Vector3(transform.position.x, _cameraY, transform.position.z);
        }
    }
}
