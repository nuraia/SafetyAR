using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;


public class HazardTapDetector : MonoBehaviour
{
    public Camera arCamera;
    public TrainingUIManager trainingUIManager;
    public TrainingSessionManager trainingSessionManager;

    void Start()
    {
        Debug.LogError("HAZARD TAP DETECTOR STARTED");
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
        if (arCamera == null)
        {
            Debug.LogError("HAZARD: AR Camera is not assigned and no main camera found.");
        } else
        {
            Debug.Log("HAZARD TAP DETECTOR INITIALIZED.");
        }
    }

    void Update()
    {
        if (Touch.activeTouches.Count == 0) return;
         
        Touch touch = Touch.activeTouches[0];
        
        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began) return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(touch.touchId))
        {
            Debug.Log("UI TAP - AR RAYCAST IGNORED");
            return;
        }

        Vector2 touchPosition = touch.screenPosition;

        Ray ray = arCamera.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.LogError($"HAZARD Tap Detected: {hit.collider.gameObject.name}");

            HazardTarget hazardTarget = hit.collider.GetComponentInParent<HazardTarget>();

            if (hazardTarget != null)
            {
                hazardTarget.IdentifyHazard();

                trainingSessionManager.HazardCorrect();

                trainingUIManager.ShowCorrect(hazardTarget);
            } else
            {
                trainingUIManager.ShowIncorrect();
            }
        } else
        {
            Debug.LogError("No hazard detected at tap position.");
        }
        
    }
}
