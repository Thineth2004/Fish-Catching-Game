using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    [Header("Screen Mapping Limits (Arduino)")]
    public float minX = 0f;
    public float maxX = 1023f;
    public float minY = 0f;
    public float maxY = 1023f;

    [Header("Fixed Z Distance from Camera")]
    public float fixedZ = 9.5f;

    [Header("Smooth Movement")]
    private float smoothSpeed = 5.0f;

    [Header("Audio Settings")]
    public AudioSource reelingAudio;

    [Header("Game Reference")]
    public GameManager gameManager;

    private Vector3 targetPos;
    private Vector3 lastPos;

    void Start()
    {
        Cursor.visible = false;
        targetPos = transform.position;
        lastPos = transform.position;
    }

    void Update()
    {
        if (gameManager == null || !gameManager.gameScreen.activeSelf)
        {
            if (reelingAudio.isPlaying)
                reelingAudio.Stop();
            return;
        }

        Vector3 screenPos = Input.mousePosition;
        screenPos.z = fixedZ;
        targetPos = Camera.main.ScreenToWorldPoint(screenPos);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);

        float distance = Vector3.Distance(transform.position, lastPos);
        bool isMoving = distance > 0.01f; 

        if (isMoving)
        {
            if (!reelingAudio.isPlaying)
            {
                reelingAudio.loop = true;
                reelingAudio.Play();
            }
        }
        else
        {
            if (reelingAudio.isPlaying)
                reelingAudio.Stop();
        }

        lastPos = transform.position;
    }
}
