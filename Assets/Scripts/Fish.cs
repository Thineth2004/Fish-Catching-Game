using UnityEngine;

public class Fish : MonoBehaviour
{
    public float moveSpeed;
    private float xDestroy = 50.0f;
    private float zPos = -3.5f;
    private float xPos;
    public int pointValue;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        transform.position = RandomSpawnPos();

        if (Random.value < 0.4f)
        {
            moveSpeed *= 1.8f;
        }

        if (transform.position.x > 0.0f)
        {
            transform.rotation = Quaternion.Euler(0, 270, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }

    void Update()
    {
        transform.Translate(0, 0, moveSpeed * Time.deltaTime, Space.Self);

        if (transform.position.x > xDestroy || transform.position.x < -xDestroy)
        {
            Destroy(gameObject);
        }
    }

    Vector3 RandomSpawnPos()
    {
        float yPos = Random.Range(-2.3f, 5.0f);

        if (Random.value > 0.5f)
        {
            xPos = -20.0f;
        }
        else
        {
            xPos = 20.0f;
        }

        return new Vector3(xPos, yPos, zPos);
    }
}
