using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Core;
using Scripts.Misc;
using UnityEngine;
using static Character;

public class RestingAreaManager : MonoBehaviour
{
    public static RestingAreaManager Instance { get; private set; }

    public Character[] restingAreaCharactersArray = new Character[5];

    public List<Transform> restAreaPositions = new();

    private bool isRestingAreaFull = false;
    public bool IsRestingAreaFull => isRestingAreaFull;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void AddCharacterToRestingArea(Character character)
    {
        for (int i = 0; i < restingAreaCharactersArray.Length; i++)
        {
            if (restingAreaCharactersArray[i] == null)
            {
                restingAreaCharactersArray[i] = character;
                var currentCharacter = restingAreaCharactersArray[i];

                currentCharacter.currentState = CharacterState.InRestingArea;
                character.targetTransform = restAreaPositions[i];
                character.characterView.transform.position = restAreaPositions[i].position;
                character.characterView.transform.forward = Vector3.forward;

                SoundManager.Instance.PlayInRestingAreaSound();

                return;
            }
        }

        isRestingAreaFull = true;
        Debug.Log("Resting area is full! Cannot add more characters. GAME OVER!");
        GameManager.Instance.GameOver(false); //lose game
    }

    public void SendBackToLoop(Character character)
    {
        if (LoopManager.Instance.charactersInLoopList.Count < LoopManager.Instance.MaxLoopSize)
        {
            character.currentPathIndex = 0; // Start at the beginning of the loop

            for (int i = 0; i < restingAreaCharactersArray.Length; i++)
            {
                if (restingAreaCharactersArray[i] == character)
                {
                    restingAreaCharactersArray[i] = null;

                    if (isRestingAreaFull)
                    {
                        isRestingAreaFull = false;
                    }

                    LoopManager.Instance.AddCharacterToLoop(character);
                    // Debug.Log($"Character with color {character.colorType} sent back to loop. Total characters in resting area: {restingAreaCharactersArray.Length}");
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Loop is full! Cannot send character back to loop.");
        }
    }

    public void HandleRestingClick(Character character)
    {
        if (character.currentState != CharacterState.InRestingArea)
        {
            Debug.Log("Clicked character is not in the resting area. Ignoring click.");
            return;
        }

        SendBackToLoop(character);
    }
}
