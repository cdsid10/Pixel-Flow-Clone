using Scripts.Enums;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "Scriptable Objects/ColorData")]
public class ColorTypeSO : ScriptableObject
{
    public ColorTypeEnum colorType;
    public Color color;
}
