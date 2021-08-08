using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    MapCreator mapCreator;
    Transform myTransform;
    Camera myCamera;

    public float moveSpeed;
    public float scrollSpeed;

    void Start()
    {
        myCamera = gameObject.GetComponent<Camera>();
        myTransform = gameObject.GetComponent<Transform>();
        mapCreator = GameObject.Find("World Tilemap").GetComponent<MapCreator>();
    }

    void Update()
    {
        Movement();
        DrawChunks();
        DeleteChunks();
    }

    void DrawChunks()
    {
        Debug.Log((myTransform.position.x, myTransform.position.y));
        /* if (myTransform.position.x ) */
    }

    void DeleteChunks()
    {

    }

    void Movement()
    {
        bool holdingShift = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        float speedBonus = holdingShift ? moveSpeed * 2 : moveSpeed;

        if (Input.GetKey(KeyCode.A))
            myTransform.position += Vector3.left * speedBonus * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            myTransform.position += Vector3.right * speedBonus * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
            myTransform.position += Vector3.up * speedBonus * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            myTransform.position += Vector3.down * speedBonus * Time.deltaTime;

        float newZoom = myCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

        if (newZoom > 3f && newZoom < 25f)
        {
            myCamera.orthographicSize = newZoom;
        }
    }
}
