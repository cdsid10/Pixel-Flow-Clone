using System;
using Scripts.Enums;
using Scripts.Grid;
using UnityEngine;

public class GridCubeView : MonoBehaviour
{
    public GridCube gridCubeData;
    private Material cubeMaterial;

    void Awake()
    {
        cubeMaterial = GetComponent<Renderer>().material;
    }

    public void SetGridCubeData(GridCube gridCubeData)
    {
        this.gridCubeData = gridCubeData;
    }

    public void SetMaterialColor(Color color)
    {
        cubeMaterial.color = color;
    }

    public Color GetMaterialColor()
    {
        return cubeMaterial.color;
    }

    public void DisableCubeView()
    {
        gameObject.SetActive(false);
    }
}
