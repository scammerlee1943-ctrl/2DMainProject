using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform Transform_Player;

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
    }
    private void OnDrawGizmos()
    {
        float startY = transform.position.y;
        float screenH = Camera.main.orthographicSize * 2f;
        float screenW = screenH * Camera.main.aspect;

        int previewCount = 10;

        for (int i = 0; i < previewCount; i++)
        {
            float sectionY = startY + (screenH * i);
            Gizmos.color = (i % 2 == 0) ? new Color(1f, 1f, 0f, 0.3f) : new Color(0f, 1f, 1f, 0.3f);
            Gizmos.DrawWireCube(
                new Vector3(transform.position.x, sectionY, 0f),
                new Vector3(screenW, screenH, 0f)
            );
        }
    }
}
