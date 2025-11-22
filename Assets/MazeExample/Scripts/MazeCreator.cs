using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MazeCreator : MonoBehaviour
{
    // Note that all objects must be scaled by 1 units, resizing and positioning them is based on that


    [SerializeField] private GameObject m_wallPrefab;
    [SerializeField] private GameObject m_walls;
    [SerializeField] private GameObject m_wallSpawnPoint;
    [SerializeField] private GameObject m_borderLeft;
    [SerializeField] private GameObject m_borderRight;
    [SerializeField] private GameObject m_startArea;
    [SerializeField] private GameObject m_goalArea;
    [SerializeField] private GameObject m_player;

    public void CreateMaze(GridSO mazeConfig)
    {
        float xSize = mazeConfig.columnCount;
        float zSize = mazeConfig.rowCount;

        int rowIndex = 0;
        foreach (GridSO.Row row in mazeConfig.rows)
        {

            for (int i = 0; i < row.row.Length; i++)
            {
                if (Convert.ToBoolean(row.row[i]))
                {
                    float xPos = m_wallSpawnPoint.transform.position.x - (xSize / 2) + i + 0.5f; // 0.5 shift to align to center
                    float zPos = m_wallSpawnPoint.transform.position.z - (zSize / 2) + rowIndex + 0.5f;
                    Vector3 pos = new Vector3(xPos, 0.0f, zPos);
                    Instantiate(m_wallPrefab, pos, Quaternion.identity, m_walls.transform);
                }
            }
            rowIndex++;
        }
        // place borders
        {// left
            float xPos = m_wallSpawnPoint.transform.position.x - xSize / 2 - 0.5f; // -0.5 to align to edge
            float zScale = zSize;
            m_borderLeft.transform.position = new Vector3(xPos, 0.0f, m_wallSpawnPoint.transform.position.z);
            m_borderLeft.GetComponent<BorderHelper>().Resize(new Vector3(1.0f, 1.0f, zScale));
        }
        {// right
            float xPos = m_wallSpawnPoint.transform.position.x + xSize / 2 + 0.5f; // 0.5 to align to edge; 
            float zScale = zSize;
            m_borderRight.transform.position = new Vector3(xPos, 0.0f, m_wallSpawnPoint.transform.position.z);
            m_borderRight.GetComponent<BorderHelper>().Resize(new Vector3(1.0f, 1.0f, zScale));
        }
        {// start
            float zPos = m_wallSpawnPoint.transform.position.z - zSize / 2 - 0.5f; // -0.5 to align to edge
            float xScale = zSize + 2; // +2 to cover corners
            m_startArea.transform.position = new Vector3(m_wallSpawnPoint.transform.position.x, 0.0f, zPos);
            m_startArea.GetComponent<BorderHelper>().Resize(new Vector3(xScale, 1.0f, 1.0f));
        }
        {// goal
            float zPos = m_wallSpawnPoint.transform.position.z + zSize / 2 + 0.5f; // 0.5 to align to edge
            float xScale = zSize + 2; // +2 to cover corners
            m_goalArea.transform.position = new Vector3(m_wallSpawnPoint.transform.position.x, 0.0f, zPos);
            m_goalArea.GetComponent<BorderHelper>().Resize(new Vector3(xScale, 1.0f, 1.0f));
        }
        // Set player at start
        m_player.transform.position = m_startArea.transform.position;
    }
}
