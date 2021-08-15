using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    ChunkEnvironments lastEnvironment = ChunkEnvironments.Terrain;
    MapManager mapManager;
    Transform myTransform;
    Camera myCamera;
    Canvas myCanvas;
    int mode = 1;

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
        int chunkSize = (int)this.mapManager.ChunkSize;
        int offset = (int)this.renderDistance * chunkSize;
        ChunkEnvironments environment;

        if (this.mode == 1)
        {
            environment = ChunkEnvironments.Terrain;
        }
        else if (this.mode == 0)
        {
            environment = ChunkEnvironments.Caves;
        }
        else if (this.mode == -1)
        {
            environment = ChunkEnvironments.Dungeon;
        }
        else
        {
            environment = ChunkEnvironments.Terrain;
        }

        if (environment != lastEnvironment) {
          this.mapManager.ClearChunks();
        }

        // TODO: Destroy unrendered chunks
        for (float y = pos.y - offset; y < pos.y + offset; y += chunkSize)
        {
            for (float x = pos.x - offset; x < pos.x + offset; x += chunkSize)
            {
                Vector2 vec = new Vector2(x, y);
                Vector2Int vertex = PositionToQuadrant(vec, chunkSize);
                this.mapManager.CreateChunk(vertex, environment, chunkSize);
            }
        }

        this.lastEnvironment = environment;
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
        {
            myTransform.position += Vector3.left * speedBonus * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            myTransform.position += Vector3.right * speedBonus * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            myTransform.position += Vector3.up * speedBonus * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            myTransform.position += Vector3.down * speedBonus * Time.deltaTime;
        }

        // Toggle UI
        if (Input.GetKeyDown(KeyCode.G))
        {
            myCanvas.enabled = !myCanvas.enabled;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (this.mode > -1)
            {
                this.mode--;
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (this.mode < 1)
            {
                this.mode++;
            }
        }

        float newZoom = myCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        myCamera.orthographicSize = Mathf.Clamp(newZoom, 3f, 75f);
    }
}
