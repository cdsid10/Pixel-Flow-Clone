using Scripts.Enums;
using Scripts.Misc;
using UnityEngine;

public class Character
{
    public CharacterState currentState;
    public CharacterView characterView;
    public ColorTypeEnum colorType;
    public int shotsLeft;
    public int currentPathIndex;
    public Transform targetTransform;     
    public float timeSinceLastShot = 0f;

    public void DestroyCharacter()
    {
        currentState = CharacterState.Destroyed;
        if (characterView != null)
        {
            Object.Destroy(characterView.gameObject);
        }
        SoundManager.Instance.PlayWarpSound();
    }

    public enum CharacterState
    {
        InQueue,
        InLoop,
        InRestingArea,
        Destroyed
    }
}
