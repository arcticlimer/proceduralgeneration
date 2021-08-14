using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    Tilemap Map;
    FastNoiseLite Noise;
    string Seed = "defaultseed!";

    public float ChunkSize { get; set; } = 25;
    public Dictionary<Vector2Int, Chunk> Chunks { get; set; }

    [SerializeField] Tile Water;
    [SerializeField] Tile LightGrass;
    [SerializeField] Tile Grass;
    [SerializeField] Tile Sand;
    [SerializeField] Tile StoneFloor;
    [SerializeField] Tile StoneWall;

    void Start()
    {
        this.Map = gameObject.GetComponent<Tilemap>();
        this.Chunks = new Dictionary<Vector2Int, Chunk>();
        this.Noise = CreateNoise();
        DrawChunks();
    }

    public void SetSeed(string seed)
    {
        this.Seed = seed;
        this.Noise = CreateNoise();
        ClearChunks();
    }

    void Update()
    {
        DrawChunks();
    }

    public void ClearChunks()
    {
        List<Chunk> chunks = this.Chunks.Values.ToList();
        for (int i = 0; i < this.Chunks.Values.Count; i++)
        {
            DeleteChunk(chunks[i]);
        }
        this.Chunks.Clear();
    }

    FastNoiseLite CreateNoise()
    {
        int seed = this.Seed.GetHashCode();
        FastNoiseLite noise = new FastNoiseLite(seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        return noise;
    }

    void DrawChunks()
    {
        foreach (Chunk chunk in this.Chunks.Values)
        {
            if (!chunk.drawed)
            {
                DrawChunk(chunk);
            }
        }
    }

    int CountNeighbours(Vector2Int pos, Dictionary<Vector2Int, GameTile> tiles)
    {
        int count = 0;

        for (int y = pos.y - 1; y <= pos.y + 1; y++)
        {
            for (int x = pos.x - 1; x <= pos.x + 1; x++)
            {
                var vec = new Vector2Int(x, y);

                if (vec == pos)
                {
                    continue;
                };

                if (tiles[vec].tile == StoneWall)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public void CreateChunk(Vector2Int vertex,
            ChunkEnvironments type,
            int chunkSize)
    {
        if (this.Chunks.ContainsKey(vertex))
        {
            return;
        }

        var tiles = new Dictionary<Vector2Int, GameTile>();

        switch (type)
        {
            // TODO: Separate this into functions
            case ChunkEnvironments.Caves:
                // Cellular automata chunking
                for (int y = vertex.y; y < vertex.y + chunkSize; y++)
                {
                    for (int x = vertex.x; x < vertex.x + chunkSize; x++)
                    {
                        var vec = new Vector2Int(x, y);
                        int value = Random.Range(0, 2);
                        tiles[vec] = new GameTile(value == 0 ? StoneFloor : StoneWall, vec);
                    }
                }

                int iterations = 1;
                for (int i = 0; i < iterations; i++)
                {
                    for (int y = vertex.y + 1; y < vertex.y + chunkSize - 1; y++)
                    {
                        for (int x = vertex.x + 1; x < vertex.x + chunkSize - 1; x++)
                        {
                            var vec = new Vector2Int(x, y);
                            GameTile tile = tiles[vec];
                            int neighbours = CountNeighbours(tile.pos, tiles);

                            if (tile.tile == StoneWall)
                            {
                                if (neighbours >= 4)
                                {
                                    tile.tile = StoneWall;
                                }
                                else
                                {
                                    tile.tile = StoneFloor;
                                }
                            }
                            else
                            {
                                if (neighbours >= 5)
                                {
                                    tile.tile = StoneWall;
                                }
                                else
                                {
                                    tile.tile = StoneFloor;
                                }
                            }
                        }
                    }
                }
                break;
            case ChunkEnvironments.Terrain:
                // Noise chunking
                for (int i = vertex.x; i < vertex.x + chunkSize; i++)
                {
                    for (int j = vertex.y; j < vertex.y + chunkSize; j++)
                    {
                        float perlin = this.Noise.GetNoise(i, j);
                        Tile tile;

                        if (perlin > 0.6)
                        {
                            tile = Grass;
                        }
                        else if (perlin > 0.2)
                        {
                            tile = LightGrass;
                        }
                        else if (perlin > -0.5)
                        {
                            tile = Sand;
                        }
                        else
                        {
                            tile = Water;
                        }

                        Vector2Int pos = new Vector2Int(i, j);
                        tiles[pos] = new GameTile(tile, pos);
                    }
                }
                break;
            case ChunkEnvironments.Dungeon:
                break;
        }

        Chunk chunk = new Chunk(vertex, tiles, false);
        this.Chunks.Add(vertex, chunk);
    }

    public void DrawChunk(Chunk chunk)
    {
        foreach (GameTile tile in chunk.tiles.Values)
        {
            var vector = new Vector3Int(tile.pos.x, tile.pos.y, 0);
            this.Map.SetTile(vector, tile.tile);
        }
        chunk.drawed = true;
    }

    public void DeleteChunk(Chunk chunk)
    {
        foreach (GameTile tile in chunk.tiles.Values)
        {
            DeleteTile(tile);
        }

        this.Chunks.Remove(chunk.vertex);
    }

    void DeleteTile(GameTile tile)
    {
        Vector3Int vector = new Vector3Int(tile.pos.x, tile.pos.y, 0);
        this.Map.SetTile(vector, null);
    }
}

public enum ChunkEnvironments
{
    Caves,
    Terrain,
    Dungeon
}

public class Chunk
{
    public Dictionary<Vector2Int, GameTile> tiles;
    public Vector2Int vertex;
    public bool drawed;

    public Chunk(Vector2Int vertex, Dictionary<Vector2Int, GameTile> tiles, bool drawed)
    {
        this.vertex = vertex;
        this.tiles = tiles;
        this.drawed = drawed;
    }
}

public class GameTile
{
    public Tile tile;
    public Vector2Int pos;

    public GameTile(Tile tile, Vector2Int pos)
    {
        this.tile = tile;
        this.pos = pos;
    }
}
