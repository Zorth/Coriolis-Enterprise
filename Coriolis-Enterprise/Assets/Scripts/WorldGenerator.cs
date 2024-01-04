using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public List<Object> modules;
    public const float tileRadius = 1;
    // 1.73205080757 = sqrt(3)
    public const float tileHeight = 1.73205080757f * tileRadius;
    public const float tileWidthSpacing = tileRadius * 3 / 2;
    public Dictionary<string, Vector3> offsets = new Dictionary<string, Vector3>();

    private GameObject center, NE, E, SE, SW, W, NW;

    public int worldSize = 10;
    private Tile[][] tiles;

    void Start()
    {
        CalculateOffsets();

        InitTileArray();

        SetupTiles();
    }

    private void CalculateOffsets()
    {
        offsets.Add("NE", new Vector3(tileWidthSpacing * worldSize, 0, tileHeight * (1.5f * worldSize - 1)));
        offsets.Add("E", new Vector3(tileWidthSpacing * (1.5f * worldSize + 1), 0, -tileHeight / 2f));
        offsets.Add("SE", new Vector3(tileWidthSpacing * (worldSize - 1), 0, -tileHeight * (1.5f * worldSize - .5f)));
        offsets.Add("SW", new Vector3(tileWidthSpacing * -worldSize, 0, -tileHeight * (1.5f * worldSize - 1f)));
        offsets.Add("W", new Vector3(tileWidthSpacing * -(worldSize * 1.5f + 1), 0, tileHeight / 2f));
        offsets.Add("NW", new Vector3(tileWidthSpacing * -(worldSize - 1), 0, tileHeight * (1.5f * worldSize - .5f)));
    }

    private void InitTileArray()
    {
        tiles = new Tile[worldSize][];

        tiles[0] = new Tile[1];
        for (int i = 1; i < worldSize; i++)
        {
            tiles[i] = new Tile[i * 6];
        }
    }

    private void SetupTiles()
    {
        //center = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, this.transform);
        center = this.gameObject.transform.GetChild(0).gameObject;

        InstantiateTiles(center.transform);

        NE = Instantiate(center, offsets["NE"], Quaternion.identity, this.transform);
        E = Instantiate(center, offsets["E"], Quaternion.identity, this.transform);
        SE = Instantiate(center, offsets["SE"], Quaternion.identity, this.transform);
        SW = Instantiate(center, offsets["SW"], Quaternion.identity, this.transform);
        W = Instantiate(center, offsets["W"], Quaternion.identity, this.transform);
        NW = Instantiate(center, offsets["NW"], Quaternion.identity, this.transform);
    }

    private void InstantiateTiles(Transform parent)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                Vector3 pos;
                if (i == 0)
                    pos = Vector3.zero;
                else if (j < i)
                    pos = new Vector3((j % i) * tileWidthSpacing, 0, i * tileHeight - (j % i) * tileHeight / 2);
                else if (j < i * 2)
                    pos = new Vector3(i * tileWidthSpacing, 0, i * tileHeight / 2 - (j % i) * tileHeight);
                else if (j < i * 3)
                    pos = new Vector3(i * tileWidthSpacing - (j % i) * tileWidthSpacing, 0, -i * tileHeight / 2 - (j % i) * tileHeight / 2);
                else if (j < i * 4)
                    pos = new Vector3(-(j % i) * tileWidthSpacing, 0, -i * tileHeight + (j % i) * tileHeight / 2);
                else if (j < i * 5)
                    pos = new Vector3(-i * tileWidthSpacing, 0, -i * tileHeight / 2 + (j % i) * tileHeight);
                else
                    pos = new Vector3(-i * tileWidthSpacing + (j % i) * tileWidthSpacing, 0, i * tileHeight / 2 + (j % i) * tileHeight / 2);

                tiles[i][j] = new Tile(parent, modules[0], pos);
                //Instantiate(modules[0], pos, Quaternion.identity, this.transform);
            }
        }
    }

    void Update()
    {

    }
}

public class Tile
{
    private Object mesh;
    private Vector3 position;
    private MeshFilter meshFilter;

    public Tile(Transform parent, Object tileMesh, Vector3 position)
    {
        mesh = tileMesh;
        this.position = position;

        Object.Instantiate(tileMesh, position, Quaternion.identity, parent.transform);
    }
}