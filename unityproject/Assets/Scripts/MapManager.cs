using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    Tilemap map;
    Dictionary<Vector2Int, Chunk> chunks;
    FastNoiseLite noise;

    public Dictionary<Vector2Int, Chunk> Chunks => chunks;
    public float tileSize { get; set; } = 25;

    [SerializeField] Tile water;
    [SerializeField] Tile lightGrass;
    [SerializeField] Tile grass;
    [SerializeField] Tile sand;

    void Start()
    {
        this.map = gameObject.GetComponent<Tilemap>();
        this.chunks = new Dictionary<Vector2Int, Chunk>();
        this.noise = CreateNoise();
        DrawChunks();
    }

    void Update()
    {
        DrawChunks();
    }

    FastNoiseLite CreateNoise()
    {
        var noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        return noise;
    }

    void DrawChunks()
    {
        foreach (Chunk chunk in this.chunks.Values)
        {
            if (!chunk.drawed)
                DrawChunk(chunk);
        }
    }

    public void CreateChunk(Vector2Int vertex, int chunkSize)
    {
        if (this.chunks.ContainsKey(vertex))
        {
            return;
        }

        var tiles = new Dictionary<Vector3Int, GameTile>();

        for (int i = vertex.x; i < vertex.x + chunkSize; i++)
            for (int j = vertex.y; j < vertex.y + chunkSize; j++)
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

        Chunk chunk = new Chunk(vertex, tiles, false);
        this.chunks.Add(vertex, chunk);
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

        this.chunks.Remove(chunk.vertex);
    }

    void DeleteTile(GameTile tile)
    {
        this.map.SetTile(tile.pos, null);
    }
}

public class Chunk
{
    public Vector2Int vertex;
    public Dictionary<Vector3Int, GameTile> tiles;
    public bool drawed;

    public Chunk(Vector2Int vertex, Dictionary<Vector3Int, GameTile> tiles, bool drawed)
    {
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
