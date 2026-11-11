using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DifficultyButton : MonoBehaviour
{
    [Header("Difficulty Settings")]
    [Tooltip("Time between fish spawns in seconds.")]
    [Min(0.1f)]
    public float difficulty = 2f;

    private Button button;
    private GameManager gameManager;

    private void Awake()
    {
        button = GetComponent<Button>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(SetDifficulty);
    }

    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(SetDifficulty);
    }

    private void SetDifficulty()
    {
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        gameManager.StartGame(difficulty);
    }
}
