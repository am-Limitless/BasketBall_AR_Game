using System;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [Header("Ball Settings")]
    public Rigidbody rbBall;
    public Transform ballSpawnPoint;
    public float throwForceMultiplier = 0.1f;
    public Vector3 ballOffset;

    [Header("AR Settings")]
    public Camera arCamera;

    [Header("Win Explosion Effects")]
    private ParticleSystem[] particleSystems;

    // Private variables for handling throw mechanics
    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;
    private bool isThrown = false;
    private float timeCounter = 0f;
    private const float throwTimeout = 2f;

    // For drawing debug lines
    private bool shouldDrawLine = false;
    private Vector3 startLineWorldPosition;
    private Vector3 endLineWorldPosition;

    // Reference to original parent (AR camera)
    private Transform originalParent;

    // Observer Event
    public static event Action onBallThrown;

    private void Start()
    {
        arCamera = Camera.main;
    }

    private void OnEnable()
    {
        RespawnBall();
        originalParent = transform.parent;
    }

    private void Update()
    {
        if (isThrown)
        {
            HandleThrownState();
        }
        else
        {
            HandleInput();
        }

        if (shouldDrawLine)
        {
            Debug.DrawLine(startLineWorldPosition, endLineWorldPosition, Color.red);
        }
    }

    private void HandleThrownState()
    {
        timeCounter += Time.deltaTime;

        if (rbBall.linearVelocity.magnitude < 0.1f || timeCounter >= throwTimeout)
        {
            RespawnBall();
            isThrown = false;
        }
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPosition = touch.position;
                shouldDrawLine = false;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchEndPosition = touch.position;
                ProcessThrow();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            touchStartPosition = Input.mousePosition;
            shouldDrawLine = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchEndPosition = Input.mousePosition;
            ProcessThrow();
        }
    }

    private void ProcessThrow()
    {
        Vector2 direction = touchEndPosition - touchStartPosition;
        float magnitude = direction.magnitude;

        ThrowBall(direction, magnitude);
        DrawThrowLine();
    }

    private void ThrowBall(Vector2 direction, float magnitude)
    {
        // Detach ball from parent (AR camera) before throwing
        transform.SetParent(null);

        // Normalize the screen direction and calculate the 3D throw direction
        Vector3 screenDirection = new Vector3(direction.x, direction.y, 0).normalized;
        Vector3 throwDirection = CalculateThrowDirection(screenDirection);

        // Apply force to the ball
        Vector3 force = throwDirection * (magnitude / 100f) * throwForceMultiplier;
        rbBall.isKinematic = false;
        rbBall.useGravity = true;
        rbBall.AddForce(force, ForceMode.Impulse);

        isThrown = true;
        Debug.Log($"Force applied: {force}");
    }

    private Vector3 CalculateThrowDirection(Vector3 screenDirection)
    {
        Vector3 forward = arCamera.transform.forward;
        Vector3 right = arCamera.transform.right;
        Vector3 up = arCamera.transform.up;

        return (forward * screenDirection.y + right * screenDirection.x + up * 0.7f).normalized;
    }

    private void DrawThrowLine()
    {
        Vector3 startScreenPosition = new Vector3(touchStartPosition.x, touchStartPosition.y, arCamera.nearClipPlane + 1f);
        Vector3 endScreenPosition = new Vector3(touchEndPosition.x, touchEndPosition.y, arCamera.farClipPlane + 1f);

        startLineWorldPosition = arCamera.ScreenToWorldPoint(startScreenPosition);
        endLineWorldPosition = arCamera.ScreenToWorldPoint(endScreenPosition);

        shouldDrawLine = true;
        Debug.Log($"Start Line: {startLineWorldPosition}, End Line: {endLineWorldPosition}");
    }

    private void RespawnBall()
    {
        // Reset variables
        timeCounter = 0f;
        rbBall.linearVelocity = Vector3.zero;
        rbBall.angularVelocity = Vector3.zero;
        rbBall.useGravity = false;
        rbBall.isKinematic = true;

        // Reattach ball to original parent and reposition
        transform.SetParent(arCamera.transform);
        transform.position = ballSpawnPoint.position + arCamera.transform.TransformDirection(ballOffset);

        onBallThrown?.Invoke();
        Debug.Log($"Ball respawned at: {ballSpawnPoint.position}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hoop"))
        {
            RespawnBall();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("HoopPanel"))
        {
            AudioManager.PlayAudio(global::AudioClipType.BallHoopHit);
            Debug.Log("Playing Hoop Hit Sound");
        }
        else
        {
            AudioManager.PlayAudio(global::AudioClipType.BallGroundPitch);
        }
    }
}
