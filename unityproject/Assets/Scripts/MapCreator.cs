using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreator : MonoBehaviour
{
    Tilemap map;
    List<Chunk> chunks;

    public Tile water;
    public Tile lightGrass;
    public Tile grass;
    public Tile sand;

    // Start is called before the first frame update
    void Start()
    {
        this.map = gameObject.GetComponent<Tilemap>();
        DrawChunk(CreateChunk(new Vector3Int(0, 0, 0), new Vector3Int(100, 100, 0)));
        DrawChunk(CreateChunk(new Vector3Int(0, 100, 0), new Vector3Int(100, 200, 0)));
        /* DrawChunk(new Vector3Int(100, 100, 0), new Vector3Int(200, 200, 0)); */
        /* DrawChunk(new Vector3Int(-100, -100, 0), new Vector3Int(0, 0, 0)); */
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Chunk CreateChunk(Vector3Int start, Vector3Int end)
    {
        var tiles = new Dictionary<Vector3Int, GameTile>();

        for (int i = start.x; i < end.x; i++)
            for (int j = start.y; j < end.y; j++)
            {
                float perlin = Mathf.PerlinNoise(i * 3.001f, j * 3.001f);
                Tile tile = perlin < 0.5 ? water : grass;
                /* this.map.SetTile(new Vector3Int(i, j, 0), perlin < 0.5 ? water : grass); */
                Vector3Int pos = new Vector3Int(i, j, 0);
                tiles[pos] = new GameTile(tile, pos);
            }

        return new Chunk(new BoundsInt(), tiles);
    }

    public void DrawChunk(Chunk chunk)
    {
        foreach (GameTile tile in chunk.tiles.Values)
        {
            this.map.SetTile(tile.pos, tile.tile);
        }
    }

    public void DeleteChunk(Chunk chunk)
    {
        foreach (GameTile tile in chunk.tiles.Values)
        {
            DeleteTile(tile);
        }
        chunks.Remove(chunk);
    }

    void DeleteTile(GameTile tile)
    {
        this.map.SetTile(tile.pos, null);
    }
}

public struct Chunk
{
    public BoundsInt bounds;
    public Dictionary<Vector3Int, GameTile> tiles;

    public Chunk(BoundsInt bounds, Dictionary<Vector3Int, GameTile> tiles)
    {
        this.bounds = bounds;
        this.tiles = tiles;
    }
}

public struct GameTile
{
    public Tile tile;
    public Vector3Int pos;

    public GameTile(Tile tile, Vector3Int pos)
    {
        this.tile = tile;
        this.pos = pos;
    }
}
