using UnityEngine;

public class Fish : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float destroyDistance = 50f;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnHeightMin = -2.3f;
    [SerializeField] private float spawnHeightMax = 5f;
    [SerializeField] private float spawnDistance = 20f;
    [SerializeField] private float zPosition = -3.5f;

    [Header("Fish Properties")]
    public int pointValue = 10;
    public bool isDangerous = false;

    private void Start()
    {
        SpawnFish();
    }

    private void Update()
    {
        MoveFish();
        CheckDestroyDistance();
    }

    private void SpawnFish()
    {
        float xPosition = Random.value > 0.5f
            ? -spawnDistance
            : spawnDistance;

        float yPosition = Random.Range(
            spawnHeightMin,
            spawnHeightMax
        );

        transform.position = new Vector3(
            xPosition,
            yPosition,
            zPosition
        );

        transform.rotation = Quaternion.Euler(
            0f,
            xPosition > 0f ? 270f : 90f,
            0f
        );

        // 40% chance of increased movement speed.
        if (Random.value < 0.4f)
        {
            moveSpeed *= 1.8f;
        }
    }

    private void MoveFish()
    {
        transform.Translate(
            Vector3.forward * moveSpeed * Time.deltaTime,
            Space.Self
        );
    }

    private void CheckDestroyDistance()
    {
        if (Mathf.Abs(transform.position.x) > destroyDistance)
        {
            Destroy(gameObject);
        }
    }

}
