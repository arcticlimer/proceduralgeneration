using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    MapManager mapManager;
    Transform myTransform;
    Camera myCamera;
    Canvas myCanvas;

    [SerializeField] float scrollSpeed;
    public float renderDistance { get; set; } = 2;
    public float moveSpeed { get; set; } = 5;

    void Start()
    {
        this.myCamera = gameObject.GetComponent<Camera>();
        this.myTransform = gameObject.GetComponent<Transform>();
        this.mapManager = GameObject.Find("World Tilemap").GetComponent<MapManager>();
        this.myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    void Update()
    {
        Movement();
        DrawChunks();
        DeleteChunks();
    }

    void DrawChunks()
    {
        var pos = new Vector2(myTransform.position.x, myTransform.position.y);
        int chunkSize = (int)this.mapManager.chunkSize;
        int offset = (int)this.renderDistance * chunkSize;

        // TODO: Destroy unrendered chunks
        for (float y = pos.y - offset; y < pos.y + offset; y += chunkSize)
        {
            for (float x = pos.x - offset; x < pos.x + offset; x += chunkSize)
            {
                Vector2 vec = new Vector2(x, y);
                Vector2Int vertex = PositionToQuadrant(vec, chunkSize);
                this.mapManager.CreateChunk(vertex, chunkSize);
            }
        }
    }

    Vector2Int PositionToQuadrant(Vector2 target, int chunkSize)
    {
        int sx = Mathf.FloorToInt(target.x / chunkSize) * chunkSize;
        int sy = Mathf.FloorToInt(target.y / chunkSize) * chunkSize;
        return new Vector2Int(sx, sy);
    }

    void DeleteChunks()
    {
        /* Vector2Int cameraPosition = new Vector2Int( */
        /*     Mathf.RoundToInt(myTransform.position.x), */
        /*     Mathf.RoundToInt(myTransform.position.y)); */
    }

    void Movement()
    {
        bool holdingShift = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        float speedBonus = holdingShift ? moveSpeed * 2 : moveSpeed;

        // Movement
        if (Input.GetKey(KeyCode.A))
            myTransform.position += Vector3.left * speedBonus * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            myTransform.position += Vector3.right * speedBonus * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
            myTransform.position += Vector3.up * speedBonus * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            myTransform.position += Vector3.down * speedBonus * Time.deltaTime;

        // Toggle UI
        if (Input.GetKeyDown(KeyCode.G))
            myCanvas.enabled = !myCanvas.enabled;

        float newZoom = myCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

        myCamera.orthographicSize = Mathf.Clamp(newZoom, 3f, 75f);
    }
}
