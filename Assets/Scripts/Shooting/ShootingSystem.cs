using System;
using Scripts.Enums;
using Scripts.Grid;
using Scripts.Misc;
using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    public static ShootingSystem Instance { get; private set; }

    [SerializeField]
    private float shootingRange = 5.5f;

    [SerializeField]
    private float sphereCastRadius = 2f;

    [SerializeField]
    private float timeBetweenShots;
    public float TimeBetweenShots => timeBetweenShots;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private bool CheckShooting(Character character)
    {
        var view = character.characterView;
        var gridManager = GridManager.Instance;

        //Debug.DrawRay(view.transform.position, view.transform.forward * shootingRange, Color.blue);

        //if (Physics.Raycast(view.transform.position, view.transform.forward, out hit, shootingRange))
        if (Physics.SphereCast(view.transform.position, sphereCastRadius, view.transform.forward,
        out RaycastHit hit, shootingRange))
        {
            if (hit.collider.TryGetComponent(out GridCubeView gridCubeView))
            {
                if (gridCubeView.gridCubeData.colorType != character.colorType)
                {
                    return false;
                }
                else
                {
                    SoundManager.Instance.PlayShootSound();
                    gridManager.DestroyCubeView(gridCubeView);
                    character.shotsLeft--;
                    view.SetAmmoText(character.shotsLeft);
                    character.timeSinceLastShot = 0f;
                    return true;
                }
            }
        }
        return false;
    }

    public bool ProcessCharacterShoot(Character character)
    {
        if (character.shotsLeft <= 0 || character.currentPathIndex >= LoopManager.Instance.loopPositions.Count)
        {
            // Debug.Log("No more shots left!");
            return false;
        }

        return CheckShooting(character);
    }
}
