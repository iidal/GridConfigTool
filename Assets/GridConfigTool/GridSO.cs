using UnityEngine;

[CreateAssetMenu(fileName = "GridSO", menuName = "GridSO", order = 1)]
public class GridSO : ScriptableObject
{
    public string puzzleName;
    public string puzzleId;
    public uint difficulty;

    [System.Serializable]
    public class Row
    {
        public uint[] row;
        public GameObject[] rowObjects;
    }
    public Row[] rows;
    public uint rowCount;
    public uint columnCount;

    
    [System.Serializable]
    public class ConfigurableField
    {
        public string fieldName;
        public string fieldType;
        public string fieldValue;
    }
    public ConfigurableField[] customFields;
}