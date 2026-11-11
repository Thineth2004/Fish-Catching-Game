
using UnityEngine;

public class FishingRodController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource ouchAudio;
    [SerializeField] private AudioSource catchAudio;

    [Header("Fishing Settings")]
    [SerializeField] private float catchRadius = 1f;

    [Header("Game Settings")]
    public bool canCatch = true;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if (!canCatch || gameManager == null ||
            !gameManager.isGameActive)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            CatchNearestFish();
        }
    }

    private void CatchNearestFish()
    {
        if (catchRadius <= 0f)
            return;

        Fish nearestFish = null;
        float nearestDistance = catchRadius * catchRadius;

        Fish[] fishes = FindObjectsByType<Fish>(
            FindObjectsSortMode.None
        );

        foreach (Fish fish in fishes)
        {
            float distance = (
                fish.transform.position - transform.position
            ).sqrMagnitude;

            if (distance <= nearestDistance)
            {
                nearestDistance = distance;
                nearestFish = fish;
            }
        }

        if (nearestFish == null)
            return;

        // Award points for the caught fish.
        gameManager.UpdateScore(nearestFish.pointValue);

        if (nearestFish.isDangerous)
        {
            gameManager.UpdateHearts();

            if (ouchAudio != null)
                ouchAudio.Play();
        }
        else
        {
            if (catchAudio != null)
                catchAudio.Play();
        }

        Destroy(nearestFish.gameObject);
    }
}
