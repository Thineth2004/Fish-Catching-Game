using UnityEngine;

public class FishingRodController : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource ouchAudio;
    public AudioSource catchAudio; 

    private GameManager gameManager;

    private float catchRadius = 1.0f;
    public bool canCatch = true;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CatchNearestFish();
        }      
    }

    void CatchNearestFish()
    {
        if (!canCatch || catchRadius <= 0f) return;

        Fish nearestFish = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Fish fish in FindObjectsByType<Fish>(FindObjectsSortMode.None))
        {
            float distance = Vector3.Distance(transform.position, fish.transform.position);
            if (distance <= catchRadius && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestFish = fish;
            }
        }

        if (nearestFish == null) return;

        if (nearestFish.gameObject.name.Contains("Shark(Clone)") ||
            nearestFish.gameObject.name.Contains("Whale(Clone)") ||
            nearestFish.gameObject.name.Contains("Dolphin(Clone)"))
        {
            gameManager.UpdateHearts();

            if (ouchAudio != null)
            {
                ouchAudio.Play();
            }
        }

        else
        {
            if (catchAudio != null)
            {
                catchAudio.Play();
            }
        }

        gameManager.UpdateScore(nearestFish.pointValue);
        Destroy(nearestFish.gameObject);
    }
}
