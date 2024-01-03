using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public List<Object> modules;
    static float tileRadius = 1;
    static float tileHeight = Mathf.Sqrt(3) * tileRadius;
    static float tileWidthSpacing = tileRadius * 3/2;

    public int worldSize = 10;
    private Tile[][] tiles;

    // Start is called before the first frame update
    void Start()
    {
        InitTileArray();
        InstantiateTiles();
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

    private void InstantiateTiles()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                Vector3 pos;
                if (i == 0)
                    pos = Vector3.zero;
                else if (j < i)
                    pos = new Vector3((j % i) * tileWidthSpacing, 0, i * tileHeight - (j%i)*tileHeight/2);
                else if (j < i * 2)
                    pos = new Vector3(i * tileWidthSpacing, 0, i * tileHeight/2 - (j%i)*tileHeight);
                else if (j < i * 3)
                    pos = new Vector3(i * tileWidthSpacing - (j % i) * tileWidthSpacing, 0, -i * tileHeight/2 - (j%i)*tileHeight/2);
                else if (j < i * 4)
                    pos = new Vector3(- (j % i) * tileWidthSpacing, 0, -i * tileHeight + (j%i)*tileHeight/2);
                else if (j < i * 5)
                    pos = new Vector3(-i * tileWidthSpacing, 0, -i * tileHeight/2 + (j%i)*tileHeight);
                else
                    pos = new Vector3(-i * tileWidthSpacing + (j%i) * tileWidthSpacing, 0, i * tileHeight/2 + (j%i)*tileHeight/2);

                tiles[i][j] = new Tile(this.transform, modules[0], pos);
        //Instantiate(modules[0], pos, Quaternion.identity, this.transform);
    }
        }
    }

    // Update is called once per frame
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