using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreator : MonoBehaviour
{
    Tilemap map;
    List<Chunk> chunks;
    FastNoiseLite noise;

    public List<Chunk> Chunks => chunks;
    [SerializeField] int tileSize;
    public int TileSize => tileSize;

    [SerializeField] Tile water;
    [SerializeField] Tile lightGrass;
    [SerializeField] Tile grass;
    [SerializeField] Tile sand;

    [SerializeField] float renderDelay;
    float timer;

    void Start()
    {
        this.map = gameObject.GetComponent<Tilemap>();
        this.chunks = new List<Chunk>();
        this.noise = CreateNoise();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > renderDelay)
        {
            timer = 0f;
            DrawChunks();
        }
    }

    FastNoiseLite CreateNoise()
    {
        var noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        return noise;
    }

    void DrawChunks()
    {
        foreach (Chunk chunk in this.chunks)
        {
            Debug.Log(chunk.drawed);
            if (!chunk.drawed)
                DrawChunk(chunk);
        }
    }

    public void CreateChunk(Vector3Int start, Vector3Int end)
    {
        var tiles = new Dictionary<Vector3Int, GameTile>();

        for (int i = start.x; i < end.x; i++)
            for (int j = start.y; j < end.y; j++)
            {
                float perlin = this.noise.GetNoise(i, j);
                Tile tile;

                if (perlin > 0.6)
                {
                    tile = grass;
                }
                else if (perlin > 0.2)
                {
                    tile = lightGrass;
                }
                else if (perlin > -0.5)
                {
                    tile = sand;
                }
                else
                {
                    tile = water;
                }
                Vector3Int pos = new Vector3Int(i, j, 0);
                tiles[pos] = new GameTile(tile, pos);
            }

        for (int i = 0; i < this.chunks.Count; i++)
        {
            DeleteChunk(this.chunks[i]);
        }

        this.chunks.Add(new Chunk(new BoundsInt(start, end), tiles, false));
    }

    public void DrawChunk(Chunk chunk)
    {
        foreach (GameTile tile in chunk.tiles.Values)
        {
            this.map.SetTile(tile.pos, tile.tile);
        }
        chunk.drawed = true;
    }

    public void DeleteChunk(Chunk chunk)
    {
        foreach (GameTile tile in chunk.tiles.Values)
        {
            DeleteTile(tile);
        }

        this.chunks.Remove(chunk);
    }

    public bool InsideChunk(Chunk chunk, Vector3Int position)
    {
        return chunk.bounds.Contains(position);
    }

    void DeleteTile(GameTile tile)
    {
        this.map.SetTile(tile.pos, null);
    }
}

public class Chunk
{
    public BoundsInt bounds;
    public Dictionary<Vector3Int, GameTile> tiles;
    public bool drawed;

    public Chunk(BoundsInt bounds, Dictionary<Vector3Int, GameTile> tiles, bool drawed)
    {
        this.bounds = bounds;
        this.tiles = tiles;
        this.drawed = drawed;
    }
}

public class GameTile
{
    public Tile tile;
    public Vector3Int pos;

    public GameTile(Tile tile, Vector3Int pos)
    {
        this.tile = tile;
        this.pos = pos;
    }
}
