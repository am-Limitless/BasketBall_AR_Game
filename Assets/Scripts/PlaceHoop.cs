using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceHoop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject hoopPrefab;
    [SerializeField] private GameObject basketBall;
    [SerializeField] private Camera arCamera;

    [Header("AR Components")]
    [SerializeField] private ARRaycastManager arRaycastManager;

    [Header("Settings")]
    public Vector3 hoopOffset;

    // Public properties
    public GameObject SpawnedHoop { get; private set; }
    public GameObject PlacedPrefab
    {
        get => hoopPrefab;
        set => hoopPrefab = value;
    }

    // Events
    public static event Action OnPlacedObject;

    // Private variables
    private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();
    private bool isHoopPlaced = false;

    private void Awake()
    {
        arRaycastManager = GetComponentInParent<ARRaycastManager>();
    }

    private void Update()
    {
        basketBall.SetActive(isHoopPlaced);

        if (!isHoopPlaced && Input.touchCount > 0)
        {
            PlaceHoopOnTouch();
        }
    }

    private void PlaceHoopOnTouch()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        if (arRaycastManager.Raycast(touch.position, Hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = Hits[0].pose;
            PlaceHoopAtPosition(hitPose.position + hoopOffset);
        }
    }

    private void PlaceHoopAtPosition(Vector3 position)
    {
        SpawnedHoop = Instantiate(hoopPrefab, position, Quaternion.identity);
        SpawnedHoop.transform.parent = transform.parent;

        AlignHoopToCamera();

        isHoopPlaced = true;
        OnPlacedObject?.Invoke();
    }

    private void AlignHoopToCamera()
    {
        if (arCamera == null || SpawnedHoop == null) return;

        Vector3 directionToCamera = arCamera.transform.position - SpawnedHoop.transform.position;
        directionToCamera.y = 0; // Keep the rotation on the horizontal plane
        Quaternion rotationToCamera = Quaternion.LookRotation(directionToCamera);
        SpawnedHoop.transform.rotation = rotationToCamera;
    }

    public void RemoveHoop()
    {
        if (SpawnedHoop != null)
        {
            Destroy(SpawnedHoop);
        }
        isHoopPlaced = false;
    }

}
