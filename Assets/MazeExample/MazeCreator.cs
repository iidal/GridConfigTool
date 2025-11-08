using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCreator : MonoBehaviour
{
    public GameObject m_wallPrefab;
    void Start()
    {
        
    }

    // Update is called once per frame
    void CreateMaze()
    {
        // these from config
        int xSize = 6;
        int zSize = 5;
        GameObject[,] m_elements = new GameObject[xSize, zSize];
        for (int i = 0; i < m_xWidth; i++)
        {
            for (int j = 0; j < m_zWidth; j++)
            {
                GameObject tile = Instantiate(m_tilePrefab, new Vector3(i+(m_tileOffset * i), 0, j + (m_tileOffset* j)), m_tilePrefab.transform.rotation, this.transform);
                tile.name = $"tile{i}{j}";
                TileControl controller = tile.GetComponent<TileControl>();
                controller.InitTile(new Vector2(i,j), this);
                controller.m_onTileSelect += TileClicked;
                m_tiles[i, j] = tile.GetComponent<TileControl>();
            }
        }

    }
}
