using System.Collections.Generic;
using Scripts.Enums;
using Scripts.Grid;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Scriptable Objects/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    public int width;
    public int height;
    public List<LevelRowData> cubeRows = new();
}

[System.Serializable]
public class LevelRowData
{
    public ColorTypeEnum colorTypeEnum;
    public List<ColorTypeEnum> cubeColumns = new();
}
