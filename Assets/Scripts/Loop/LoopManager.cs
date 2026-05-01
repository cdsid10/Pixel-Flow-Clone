using System.Collections.Generic;
using Scripts.Core;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance { get; private set; }

    public List<Transform> loopPositions = new();

    public List<Character> charactersInLoopList = new();

    [SerializeField]
    private int maxLoopSize = 5;
    public int MaxLoopSize => maxLoopSize;

    public float characterMoveSpeed = 5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (charactersInLoopList.Count == 0)
        {
            return;
        }

        CheckForExit();

        if (GameManager.Instance.IsGameWon || GameManager.Instance.IsGameWon)
        {
            return;
        }

        MoveCharacters();
        ProcessAllCharactersShoot();
    }

    public void MoveCharacters()
    {
        foreach (var character in charactersInLoopList)
        {
            var distance = Vector3.Distance(character.targetTransform.position, character.characterView.transform.position);
            if (distance < 0.1f)
            {
                if (character.currentPathIndex < loopPositions.Count - 1)
                {
                    character.currentPathIndex++;
                    character.targetTransform = loopPositions[character.currentPathIndex];
                }
            }

            var view = character.characterView;
            view.transform.position = Vector3.MoveTowards(view.transform.position, character.targetTransform.position, characterMoveSpeed * Time.deltaTime);

            //look
            Vector3 moveDir = character.targetTransform.position - view.transform.position;

            if (moveDir.sqrMagnitude > 0.001f)
            {
                Vector3 crossDir = Vector3.Cross(Vector3.up, moveDir);
                view.transform.forward = -crossDir.normalized;
            }

            //Debug.Log($"Character with color {character.colorType} moved to path index {character.currentPathIndex}");
        }
    }

    private void ProcessAllCharactersShoot()
    {
        foreach (var character in charactersInLoopList)
        {
            character.timeSinceLastShot += Time.deltaTime;

            if (character.timeSinceLastShot >= ShootingSystem.Instance.TimeBetweenShots)
            {
                ProcessCharactersShoot(character);
            }
        }
    }

    private void ProcessCharactersShoot(Character character)
    {
        ShootingSystem.Instance.ProcessCharacterShoot(character);
    }

    private void CheckForExit()
    {
        if (charactersInLoopList.Count == 0)
        {
            return;
        }

        var lastNode = loopPositions[^1];

        for (int i = charactersInLoopList.Count - 1; i >= 0; i--)
        {
            var character = charactersInLoopList[i];
            var characterNearLastNode = character.targetTransform == lastNode &&
            (character.characterView.transform.position - character.targetTransform.position).sqrMagnitude < 0.001f;


            //No ammo left, remove from loop and destroy character
            if (character.shotsLeft <= 0)
            {
                SpawnManager.Instance.RemoveCharacterFromCurrentlyLeftList(character);
                character.DestroyCharacter();
                RemoveCharacterFromLoop(character);
                continue;
            }
            else if (SpawnManager.Instance.CurrentlyLeftCharactersList.Count <= maxLoopSize &&
            characterNearLastNode)
            {
                character.currentPathIndex = 0;
                character.targetTransform = loopPositions[0];
                continue;
            }
            else if (character.currentState == Character.CharacterState.InLoop && characterNearLastNode)
            {
                RestingAreaManager.Instance.AddCharacterToRestingArea(character);
                RemoveCharacterFromLoop(character);
            }
        }
    }

    // public void FillLoopFromQueue(Queue<Character> characterQueue)
    // {
    //     while (charactersInLoopList.Count < maxLoopSize && characterQueue.Count > 0)
    //     {
    //         var character = characterQueue.Dequeue();
    //         AddCharacterToLoop(character);
    //     }
    // }

    public void AddCharacterToLoop(Character character)
    {
        if (charactersInLoopList.Count < maxLoopSize)
        {
            character.currentPathIndex = 0; // Start at the beginning of the loop
            charactersInLoopList.Add(character);
            character.currentState = Character.CharacterState.InLoop;
            character.targetTransform = loopPositions[character.currentPathIndex];
            character.characterView.transform.position = loopPositions[0].position;

            //Debug.Log($"Character with color {character.colorType} added to loop. Total characters in loop: {charactersInLoopList.Count}");
        }
        else
        {
            Debug.Log("Loop is full! Cannot add more characters.");
        }
    }

    public void RemoveCharacterFromLoop(Character character)
    {
        if (charactersInLoopList.Contains(character))
        {
            charactersInLoopList.Remove(character);
            //Debug.Log($"Character with color {character.colorType} removed from loop. Total characters in loop: {charactersInLoopList.Count}");
        }
        else
        {
            Debug.Log("Character not found in loop!");
        }
    }

    public bool IsLoopFull()
    {
        return charactersInLoopList.Count >= maxLoopSize;
    }
}
