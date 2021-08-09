using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    MapCreator mapCreator;
    Transform myTransform;
    Camera myCamera;
    BoundsInt cameraBounds;

    [SerializeField] float moveSpeed;
    [SerializeField] float scrollSpeed;

    /* TODO: Every frame get Vector3Int for each corner of the screen, and then
     * chuck if each one of them would be inside a chunk, if yes, render them */
    void Start()
    {
        this.myCamera = gameObject.GetComponent<Camera>();
        this.myTransform = gameObject.GetComponent<Transform>();
        this.mapCreator = GameObject.Find("World Tilemap").GetComponent<MapCreator>();
        /* Debug.Log(mapCreator); */
        /* Debug.Log(mapCreator.Chunks); */
    }

    void Update()
    {
        Movement();
        UpdateCameraBounds();
        DrawChunks();
        DeleteChunks();
    }

    void DrawChunks()
    {
        /* Vector3Int cameraPosition = new Vector3Int( */
        /*     Mathf.RoundToInt(myTransform.position.x), */
        /*     Mathf.RoundToInt(myTransform.position.y), */
        /*     0); */

        int tileSize = mapCreator.TileSize;

        int sx = Mathf.FloorToInt(myTransform.position.x / tileSize) * tileSize;
        int sy = Mathf.FloorToInt(myTransform.position.y / tileSize) * tileSize;

        Vector3Int start = new Vector3Int(sx, sy, 0);

        int ex = Mathf.CeilToInt(myTransform.position.x / tileSize) * tileSize;
        int ey = Mathf.CeilToInt(myTransform.position.y / tileSize) * tileSize;

        Vector3Int end = new Vector3Int(ex, ey, 0);

        this.mapCreator.CreateChunk(start, end);

        /* Debug.Log((myTransform.position.x, myTransform.position.y)); */
        /* if (myTransform.position.x ) */
    }

    public void UpdateCameraBounds()
    {
        Vector3Int start = new Vector3Int(
            Mathf.RoundToInt(myCamera.transform.position.x - (Screen.width / 2)),
            Mathf.RoundToInt(myCamera.transform.position.y - (Screen.height / 2)),
            0);
        Vector3Int end = new Vector3Int(
            Mathf.RoundToInt(myCamera.transform.position.x + (Screen.width / 2)),
            Mathf.RoundToInt(myCamera.transform.position.y + (Screen.height / 2)),
            0);

        cameraBounds = new BoundsInt(start, end);
        /* Debug.Log(cameraBounds); */
    }

    void DeleteChunks()
    {
        Vector3Int cameraPosition = new Vector3Int(
            Mathf.RoundToInt(myTransform.position.x),
            Mathf.RoundToInt(myTransform.position.y),
            0);

        for (int i = 0; i < this.mapCreator.Chunks.Count; i++)
        {
            Chunk chunk = this.mapCreator.Chunks[i];
            /* Debug.Log(this.mapCreator.InsideChunk(chunk, cameraPosition)); */
            /* if (!this.mapCreator.InsideChunk(chunk, cameraPosition)) */
            /* { */
            /*     this.mapCreator.DeleteChunk(chunk); */
            /* } */
        }
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

        myCamera.orthographicSize = Mathf.Clamp(newZoom, 3f, 25f);
    }
}
