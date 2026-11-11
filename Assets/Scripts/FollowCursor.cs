
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float fixedZ = 9.5f;
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource reelingAudio;

    [Header("Game Reference")]
    [SerializeField] private GameManager gameManager;

    private Camera mainCamera;
    private Vector3 targetPosition;
    private Vector3 lastPosition;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }

    private void Start()
    {
        targetPosition = transform.position;
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (gameManager == null ||
            gameManager.gameScreen == null ||
            !gameManager.gameScreen.activeSelf)
        {
            StopReelingAudio();
            return;
        }

        if (mainCamera == null)
        {
            StopReelingAudio();
            return;
        }

        FollowMouse();
        HandleReelingAudio();
    }

    private void FollowMouse()
    {
        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = fixedZ;

        targetPosition = mainCamera.ScreenToWorldPoint(
            screenPosition
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * smoothSpeed
        );
    }

    private void HandleReelingAudio()
    {
        float movementDistance = Vector3.Distance(
            transform.position,
            lastPosition
        );

        bool isMoving = movementDistance > 0.01f;

        if (reelingAudio != null)
        {
            if (isMoving && !reelingAudio.isPlaying)
            {
                reelingAudio.loop = true;
                reelingAudio.Play();
            }
            else if (!isMoving && reelingAudio.isPlaying)
            {
                reelingAudio.Stop();
            }
        }

        lastPosition = transform.position;
    }

    private void StopReelingAudio()
    {
        if (reelingAudio != null && reelingAudio.isPlaying)
        {
            reelingAudio.Stop();
        }

        lastPosition = transform.position;
    }

    private void OnDisable()
    {
        StopReelingAudio();
    }
}
