 using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class TrainingUIManager : MonoBehaviour
{
    [Header("Feedback")]
    public GameObject feedbackPanel;
    public TMP_Text feedbackText;

    [Header("Control Question")]
    public GameObject controlQuestionPanel;
    public TMP_Text controlQuestionText;

    public Button optionAButton;
    public Button optionBButton;
    public Button optionCButton;

    public TMP_Text optionAText;
    public TMP_Text optionBText;
    public TMP_Text optionCText;

    [Header("Training Session")]
    public TrainingSessionManager trainingSessionManager;

    private int activeCorrectIndex = -1;
    private HazardTarget currentHazardTarget;

    void Start()
    {
        Debug.LogError(
            "HAZARD BUTTON MAPPING:\n" +
            "A / 0 = " + optionAButton.name + "\n" +
            "B / 1 = " + optionBButton.name + "\n" +
            "C / 2 = " + optionCButton.name
        );
        feedbackPanel.SetActive(false);
        controlQuestionPanel.SetActive(false);

        optionAButton.onClick.RemoveAllListeners();
        optionBButton.onClick.RemoveAllListeners();
        optionCButton.onClick.RemoveAllListeners();

        optionAButton.onClick.AddListener(() => CheckControl(0));
        optionBButton.onClick.AddListener(() => CheckControl(1));
        optionCButton.onClick.AddListener(() => CheckControl(2));
    }

    public void ShowCorrect(HazardTarget hazardTarget)
    {
        currentHazardTarget = hazardTarget;

        activeCorrectIndex =
            currentHazardTarget.correctOptionIndex;

        Debug.LogError(
            "HAZARD RECEIVED = " +
            currentHazardTarget.hazardName +
            " | RUNTIME CORRECT INDEX = " +
            activeCorrectIndex
        );

        feedbackText.text =
            "✓ Correct! You identified the hazard: " +
            hazardTarget.hazardName;

        feedbackPanel.SetActive(true);
        controlQuestionPanel.SetActive(false);

        StartCoroutine(ShowControlQuestion());
    }

    public void ShowIncorrect()
    {
        feedbackPanel.SetActive(true);
        controlQuestionPanel.SetActive(false);

        feedbackText.text =
            "✗ NOT THE PRIMARY HAZARD\n" +
            "Look carefully and try again.";

        StartCoroutine(HideFeedbackAfterDelay());
    }

    IEnumerator ShowControlQuestion()
    {
        if (currentHazardTarget == null)
        {
            Debug.LogError(
                "HAZARD ERROR: currentHazardTarget is NULL"
            );

            yield break;
        }

        yield return new WaitForSeconds(2f);

        feedbackPanel.SetActive(false);

        // Load question from the SAME hazard object
        controlQuestionText.text =
            currentHazardTarget.safetyControlQuestion;

        optionAText.text =
            currentHazardTarget.optionA;

        optionBText.text =
            currentHazardTarget.optionB;

        optionCText.text =
            currentHazardTarget.optionC;

        controlQuestionPanel.SetActive(true);

        Debug.LogError(
            "HAZARD ===== CONTROL QUESTION =====\n" +
            "Hazard = " +
            currentHazardTarget.hazardName + "\n" +
            "Correct Index = " +
            currentHazardTarget.correctOptionIndex + "\n" +
            "A = " + currentHazardTarget.optionA + "\n" +
            "B = " + currentHazardTarget.optionB + "\n" +
            "C = " + currentHazardTarget.optionC
        );
    }

    public void CheckControl(int selectedIndex)
    {
        if (currentHazardTarget == null)
        {
            Debug.LogError("HAZARD NO CURRENT HAZARD");
            return;
        }

        int correctIndex =
            currentHazardTarget.correctOptionIndex;

        Debug.LogError(
            "HAZARD CHECKING ANSWER -> Clicked = " +
            selectedIndex +
            " | Correct = " +
            correctIndex
        );

        if (selectedIndex == correctIndex)
        {
            Debug.LogError("HAZARD ### CORRECT BRANCH ENTERED ###");

            feedbackPanel.SetActive(true);
            controlQuestionPanel.SetActive(false);

            feedbackText.text =
                "✓ CORRECT SAFETY CONTROL";

            Debug.LogError(
                "HAZARD TEXT SET TO = " +
                feedbackText.text
            );

            if (trainingSessionManager != null)
            {
                trainingSessionManager.ControlCorrect();
            }
        }
        else
        {
            Debug.LogError("HAZARD ### INCORRECT BRANCH ENTERED ###");

            feedbackPanel.SetActive(true);
            controlQuestionPanel.SetActive(false);

            feedbackText.text = "✗ INCORRECT SAFETY CONTROL";
            
            StartCoroutine(ReturnToControls());
        }
    }
    IEnumerator ReturnToControls()
    {
        yield return new WaitForSeconds(2f);

        feedbackPanel.SetActive(false);

        if (currentHazardTarget != null)
        {
            controlQuestionPanel.SetActive(true);
        }
    }

    IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        feedbackPanel.SetActive(false);
    }
}