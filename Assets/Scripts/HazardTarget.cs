using UnityEngine;

public class HazardTarget : MonoBehaviour
{
    public string hazardName;
    public string hazardCategory;

    [Header("Safety Control Question")]
    public string safetyControlQuestion;

    [TextArea]
    public string optionA;

    [TextArea]
    public string optionB;

    [TextArea]
    public string optionC;

    [Range(0, 2)]
    public int correctOptionIndex;

    public void IdentifyHazard()
    {
        Debug.Log($"Correct Hazard Identified: Hazard Name: {hazardName}, Hazard Category: {hazardCategory}");
    }
}
