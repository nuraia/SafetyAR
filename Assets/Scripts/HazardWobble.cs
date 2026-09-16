using UnityEngine;

public class HazardWobble : MonoBehaviour
{
    public float wobbleSpeed = 2f; // Speed of the wobble
    public float wobbleAmount = 0.02f; // Amount of wobble
    private Vector3 initialPosition;
    void Start()
    {
        initialPosition = transform.localPosition; // Store the initial position of the hazard
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount; 
        
        transform.localPosition = initialPosition + new Vector3(0, offset, 0); // Apply the wobble effect
    }
}
