using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCreator : MonoBehaviour
{
    // Note that all objects must be scaled to 1 units, resizing and positioning them is based on that


    public GameObject m_wallPrefab;
    public GameObject m_walls;
    public GameObject m_wallSpawnPoint;
    public GameObject m_borderLeft;
    public GameObject m_borderRight;
    public GameObject m_startArea;
    public GameObject m_goalArea;
    void Start()
    {
        CreateMaze();
    }

    void CreateMaze()
    {
        // these from config,
        // float for coordinates, see if have to be int
        float xSize = 6;
        float zSize = 5;

        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < zSize; j++)
            {
                float xPos = m_wallSpawnPoint.transform.position.x - (xSize / 2) + i + 0.5f; // 0.5 shift to align to center
                float zPos = m_wallSpawnPoint.transform.position.z - (zSize / 2) + j + 0.5f;
                Vector3 pos = new Vector3(xPos, 0.0f, zPos);
                Instantiate(m_wallPrefab, pos, Quaternion.identity, m_walls.transform);
            }
        }
        // place borders
        {// left
            float xPos = m_wallSpawnPoint.transform.position.x - xSize / 2 -0.5f; // -0.5 to align to edge
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
            m_goalArea.transform.position = new Vector3(m_wallSpawnPoint.transform.position.x, 0.0f , zPos);
            m_goalArea.GetComponent<BorderHelper>().Resize(new Vector3(xScale, 1.0f, 1.0f));
        }
    }
}
