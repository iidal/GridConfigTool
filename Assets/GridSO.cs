using UnityEngine;

[CreateAssetMenu(fileName = "GridSO", menuName = "ScriptableObjects", order = 1)]
public class GridSO : ScriptableObject
{
    public string puzzleName;
    public string puzzleId;
    public uint difficulty;

    [System.Serializable]
    public class Row
    {
        public uint[] row;
    }
    public Row[] rows;
    public uint rowCount;
    public uint columnCount;

}