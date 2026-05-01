using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private CanvasGroup gameEndStatusCanvasGroup;

    [SerializeField] private TextMeshProUGUI gameEndStatusTMP;

    [SerializeField] private Button reloadLevelButton;

    [SerializeField] private Button nextLevelButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        reloadLevelButton.onClick.AddListener(LevelManager.Instance.ReloadLevel);
        nextLevelButton.onClick.AddListener(LevelManager.Instance.NextLevel);
    }

    public void ShowGameEndStatus(bool hasWon)
    {
        gameEndStatusCanvasGroup.alpha = 1;
        gameEndStatusCanvasGroup.interactable = true;
        gameEndStatusCanvasGroup.blocksRaycasts = true;
        SetGameEndStatus(hasWon);
    }

    private void SetGameEndStatus(bool hasWon)
    {
        if (hasWon)
        {
            gameEndStatusTMP.text = "You Won! :)";
        }
        else
        {
            gameEndStatusTMP.text = "You Lost! :(";
        }
    }
}
