using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    MapManager mapManager;
    Transform myTransform;
    Camera myCamera;

    [SerializeField] float scrollSpeed;
    public float renderDistance {get; set;} = 2;
    public float moveSpeed { get; set; } = 5;

    void Start()
    {
        this.myCamera = gameObject.GetComponent<Camera>();
        this.myTransform = gameObject.GetComponent<Transform>();
        this.mapManager = GameObject.Find("World Tilemap").GetComponent<MapManager>();
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
        int tileSize = (int)this.mapManager.tileSize;
        int offset = (int)this.renderDistance * tileSize;

        // TODO: Destroy unrendered chunks
        for (float y = pos.y - offset; y < pos.y + offset; y += tileSize)
        {
            for (float x = pos.x - offset; x < pos.x + offset; x += tileSize)
            {
                Vector2 vec = new Vector2(x, y);
                var (start, end) = PositionToQuadrant(vec, tileSize);
                this.mapManager.CreateChunk(start, end);
            }
        }
    }

    (Vector2Int, Vector2Int) PositionToQuadrant(Vector2 target, int tileSize)
    {
        int sx = Mathf.FloorToInt(target.x / tileSize) * tileSize;
        int sy = Mathf.FloorToInt(target.y / tileSize) * tileSize;
        Vector2Int start = new Vector2Int(sx, sy);

        int ex = Mathf.CeilToInt(target.x / tileSize) * tileSize;
        int ey = Mathf.CeilToInt(target.y / tileSize) * tileSize;
        Vector2Int end = new Vector2Int(ex, ey);

        return (start, end);
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

        if (Input.GetKey(KeyCode.A))
            myTransform.position += Vector3.left * speedBonus * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            myTransform.position += Vector3.right * speedBonus * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
            myTransform.position += Vector3.up * speedBonus * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            myTransform.position += Vector3.down * speedBonus * Time.deltaTime;

        float newZoom = myCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

        myCamera.orthographicSize = Mathf.Clamp(newZoom, 3f, 75f);
    }
}
