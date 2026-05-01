using System.Collections.Generic;
using Scripts.Core;
using Scripts.Enums;
using Scripts.Grid;
using UnityEngine;
using static Character;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    public Dictionary<ColorTypeEnum, Queue<CharacterView>> colorCharactersQueue = new();

    private Dictionary<ColorTypeEnum, Transform> queueTransformMap = new();

    [SerializeField]
    private List<Character> currentlyLeftCharactersList = new();
    public List<Character> CurrentlyLeftCharactersList => currentlyLeftCharactersList;


    [SerializeField]
    private CharacterView characterViewPrefab;

    [SerializeField]
    private Transform firstQueueSpawnTransform;

    [SerializeField]
    private Transform secondQueueSpawnTransform;

    [SerializeField]
    private Transform thirdQueueSpawnTransform;

    [SerializeField] private float spawnOffset = 1f;

    [SerializeField] private int lowestAmmoCount = 4;

    [SerializeField] private int highestAmmoCount = 12;

    void Awake()
    {
        // if (Instance == null)
        // {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }

        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        foreach (var colorSO in GridManager.Instance.colorDataList)
        {
            colorCharactersQueue[colorSO.colorType] = new Queue<CharacterView>();
        }

        queueTransformMap[ColorTypeEnum.Red] = firstQueueSpawnTransform;
        queueTransformMap[ColorTypeEnum.Green] = secondQueueSpawnTransform;
        queueTransformMap[ColorTypeEnum.Blue] = thirdQueueSpawnTransform;

        CreateQueueCharacters();
    }

    private void CreateQueueCharacters()
    {
        var colorIndex = 0;

        foreach (var colorSO in GridManager.Instance.colorDataList)
        {
            if (colorCharactersQueue[colorSO.colorType].Count > 0)
            {
                continue;
            }

            var queueOffsetIndex = 0;
            var spawnTransform = queueTransformMap[colorSO.colorType];
            var totalShotsForThisColor = GridManager.Instance.colorTypeCountDictionary[colorSO.colorType];

            while (totalShotsForThisColor > 0)
            {
                var shotsChunk = Random.Range(lowestAmmoCount, highestAmmoCount);
                shotsChunk = Mathf.Min(shotsChunk, totalShotsForThisColor);

                var newCharacter = CreateCharacter(colorSO, shotsChunk, spawnTransform.position + new Vector3(0, 0, spawnOffset * queueOffsetIndex));
                currentlyLeftCharactersList.Add(newCharacter);

                totalShotsForThisColor -= shotsChunk;
                queueOffsetIndex++;
            }

            colorIndex++;
        }
    }

    public void RemoveCharacterFromCurrentlyLeftList(Character character)
    {
        currentlyLeftCharactersList.Remove(character);
    }

    public CharacterView GetFrontCharacter(ColorTypeEnum color)
    {
        var queue = colorCharactersQueue[color];

        if (queue.Count == 0)
            return null;

        return queue.Peek();
    }

    public CharacterView DequeueCharacter(ColorTypeEnum color)
    {
        var queue = colorCharactersQueue[color];

        if (queue.Count == 0)
            return null;

        return queue.Dequeue();
    }

    public void RepositionQueue(ColorTypeEnum color)
    {
        var queue = colorCharactersQueue[color];

        int index = 0;
        var baseTransform = queueTransformMap[color];

        foreach (var characterView in queue)
        {
            Vector3 newPos = baseTransform.position + new Vector3(0, 0, spawnOffset * index);
            characterView.transform.position = newPos;

            index++;
        }
    }

    private Character CreateCharacter(ColorTypeSO colorSO, int shotsToAssign, Vector3 spawnPosition)
    {

        var character = CreateCharacterData(colorSO, shotsToAssign);
        var characterView = CreateCharacterView(colorSO, character, spawnPosition);
        character.characterView = characterView;
        characterView.character = character;
        colorCharactersQueue[colorSO.colorType].Enqueue(characterView);
        character.currentState = CharacterState.InQueue;

        return character;
    }

    private Character CreateCharacterData(ColorTypeSO colorSO, int shotsToAssign)
    {
        var colorToAssign = colorSO.colorType;
        var character = new Character() { colorType = colorToAssign, shotsLeft = shotsToAssign };
        //Debug.Log($"Spawned character with color {character.colorType} and {character.shotsLeft} shots left.");
        return character;
    }

    private CharacterView CreateCharacterView(ColorTypeSO colorSO, Character character, Vector3 spawnPosition)
    {
        var characterView = Instantiate(characterViewPrefab, spawnPosition, Quaternion.identity);
        characterView.SetMaterialColor(colorSO.color);
        characterView.SetAmmoText(character.shotsLeft);
        characterView.name = $"Character_{colorSO.colorType}_{character.shotsLeft}";
        return characterView;
    }

    public void HandleQueueClick(CharacterView characterView)
    {
        var color = characterView.character.colorType;
        var frontCharacter = GetFrontCharacter(color);

        if (frontCharacter != characterView)
        {
            Debug.Log("Clicked character is not at the front of the queue. Ignoring click.");
            return;
        }

        if (LoopManager.Instance.IsLoopFull())
        {
            Debug.Log("Loop is full. Cannot add more characters.");
            return;
        }

        var dequeuedCharacter = DequeueCharacter(color);

        LoopManager.Instance.AddCharacterToLoop(dequeuedCharacter.character);

        RepositionQueue(color);
    }
}