using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    public Character character;

    [SerializeField]
    private TextMeshProUGUI ammoTMP;

    [SerializeField]
    private List<Renderer> meshRendererList = new();

    public void SetMaterialColor(Color color)
    {
        foreach (var mesh in meshRendererList)
        {
            mesh.material.color = color;
        }
    }

    public void SetAmmoText(int ammoCount)
    {
        ammoTMP.text = ammoCount.ToString();
    }

    void OnMouseDown()
    {
        InputManager.Instance.OnCharacterClicked(this);
    }
}
