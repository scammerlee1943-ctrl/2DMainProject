using UnityEngine;

public class TestManager : MonoBehaviour
{
    [SerializeField] private Transform Transform_Player;
    [SerializeField] private Rigidbody2D Rigid_Player;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Transform_Player.position = new Vector3(3.42f, 15.37f, 0f);
            Rigid_Player.linearVelocity = Vector2.zero;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Transform_Player.position = new Vector3(-1.2f, 30.02f, 0f);
            Rigid_Player.linearVelocity = Vector2.zero;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Transform_Player.position = new Vector3(3.4f, 80.83f, 0f);
            Rigid_Player.linearVelocity = Vector2.zero;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Transform_Player.position = new Vector3(5f, 110.83f, 0f);
            Rigid_Player.linearVelocity = Vector2.zero;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Transform_Player.position = new Vector3(5f, 130.83f, 0f);
            Rigid_Player.linearVelocity = Vector2.zero;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Transform_Player.position = new Vector3(5f, 150.83f, 0f);
            Rigid_Player.linearVelocity = Vector2.zero;
        }
    }
}
