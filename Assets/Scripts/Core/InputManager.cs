using Scripts.Core;
using UnityEngine;
using static Character;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnCharacterClicked(CharacterView characterView)
    {
        if (GameManager.Instance.IsGameWon || GameManager.Instance.IsGameWon)
        {
            return;
        }

        var character = characterView.character;

        if (character.currentState == CharacterState.InQueue)
        {
            SpawnManager.Instance.HandleQueueClick(characterView);
        }
        else if (character.currentState == CharacterState.InRestingArea)
        {
            RestingAreaManager.Instance.HandleRestingClick(character);
        }
        else
        {
            Debug.Log("Character click ignored (not in interactable area)");
        }
    }
}
