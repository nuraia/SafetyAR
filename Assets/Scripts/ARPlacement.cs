 using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;

using Touch =
    UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ARPlacement : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    public TrainingSessionManager
        trainingSessionManager;

    public GameObject objectToPlace;

    private GameObject placedObject;

    public bool placementReady = false;

    // LLM-generated scenario waiting
    // to be applied after placement.
    public LLMScenarioSpec pendingScenarioSpec;

    private static readonly
        List<ARRaycastHit> hits =
            new List<ARRaycastHit>();

    private int previousPlaneCount = -1;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        Debug.LogError("ARDBG: ARPLACEMENT STARTED");

        if (raycastManager == null)
            Debug.LogError("ARDBG: RAYCAST MANAGER NULL");

        if (planeManager == null)
            Debug.LogError("ARDBG: PLANE MANAGER NULL");
    }

    public void PrepareScenario(
        GameObject prefab,
        LLMScenarioSpec spec)
    {
        objectToPlace = prefab;

        pendingScenarioSpec = spec;

        placementReady = true;

        Debug.LogError("ARDBG: SCENARIO READY = " + objectToPlace.name);
    }

    void Update()
    {
        if (!placementReady)
            return;

        if (objectToPlace == null)
            return;

        if (planeManager != null)
        {
            int planeCount = planeManager.trackables.count;

            if (planeCount != previousPlaneCount)
            {
                Debug.LogError("ARDBG: PLANES DETECTED = " + planeCount);

                previousPlaneCount = planeCount;
            }
        }

        if (Touch.activeTouches.Count == 0)
            return;

        Touch touch = Touch.activeTouches[0];

        if (touch.phase != UnityEngine.InputSystem .TouchPhase.Began)
        {
            return;
        }

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(touch.touchId))
        {
            Debug.LogError("ARDBG: UI TAP - PLACEMENT IGNORED");

            return;
        }

        Vector2 touchPosition = touch.screenPosition;

        if (raycastManager.Raycast(
            touchPosition,
            hits,
            TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;

            if (placedObject == null)
            {
                placedObject = Instantiate(
                        objectToPlace,
                        hitPose.position,
                        hitPose.rotation
                    );

                // Apply LLM-generated PCG parameters
                if (pendingScenarioSpec != null)
                {
                    ScenarioRandomizer randomizer =
                        placedObject.GetComponentInChildren<ScenarioRandomizer>();

                    if (randomizer != null)
                    {
                        randomizer.ApplyGeneratedSpec(pendingScenarioSpec);
                    }
                    else
                    {
                        Debug.LogError("ARDBG: ScenarioRandomizer NOT FOUND");
                    }
                }

                placementReady = false;

                pendingScenarioSpec = null;

                if (trainingSessionManager != null)
                {
                    trainingSessionManager.StartSession();
                }

                Debug.LogError("ARDBG: CREATED = " + placedObject.name);
            }
        }
        else
        {
            Debug.LogError("ARDBG: NO PLANE HIT");
        }
    }
}