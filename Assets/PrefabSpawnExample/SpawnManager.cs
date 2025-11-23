using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
[SerializeField] private GameObject m_spawnCenter;
[SerializeField] private GridSO m_config;

   void Start()
    {
        SpawnPrefabs();
    }
    void SpawnPrefabs()
    {
        Debug.Log("Spawn");
        float xSize = m_config.columnCount;
        float zSize = m_config.rowCount;

        int rowIndex = 0;
        foreach (GridSO.Row row in m_config.rows)
        {
        Debug.Log("1");

            for (int i = 0; i < row.rowObjects.Length; i++)
            {
        Debug.Log("2");

                if (row.rowObjects[i] != null)
                {
                    float xPos = m_spawnCenter.transform.position.x - (xSize / 2) + i - 0.5f; // 0.5 shift to align to center
                    float zPos = m_spawnCenter.transform.position.z + (zSize / 2) - rowIndex - 0.5f;
                    Vector3 pos = new Vector3(xPos, 0.0f, zPos);
                    Instantiate(row.rowObjects[i], pos, Quaternion.identity, this.transform);
                }
            }
            rowIndex++;
        }
    }
}
