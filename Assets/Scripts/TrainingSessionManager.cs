using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TrainingSessionManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text scoreText;

    [Header("Session Settings")]
    public float timeLimit = 45f;

    private float remainingTime;
    private int score;

    [Header("Completion UI")]
    public GameObject completionPanel;
    public TMP_Text performanceText;
    public TMP_Text finalScoreText;

    [Header("Difficulty")]
    public string currentDifficultyLevel = "Beginner"; 

    private bool sessionRunning = false;
    private bool hazardPointGiven = false;
    private bool controlPointGiven = false;

    public void StartSession()
    {
        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }

        remainingTime = timeLimit;
        score = 0;
        sessionRunning = true;
        hazardPointGiven = false;
        controlPointGiven = false;

        UpdateUI();
        Debug.Log("Training: session started. Time limit: " + timeLimit + " seconds.");
    }

    void Update()
    {
        if (!sessionRunning) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            sessionRunning = false;
            Debug.Log("Training: session ended. Final score: " + score);
        }

        UpdateUI();
    }

    public void HazardCorrect()
    {
        if (hazardPointGiven) return;

        score += 50;
        hazardPointGiven = true;
        Debug.Log("Training: hazard correctly identified. Score: " + score);
        UpdateUI();

    }

    public void ControlCorrect()
    {
        if (controlPointGiven) return;

        score += 30;
        controlPointGiven = true;

        if(remainingTime > 0f)
        {
            score += 20;
            Debug.Log("Training: control correctly identified. Time bonus: 20 seconds. New remaining time: " + remainingTime + " seconds.");
        }
        sessionRunning = false;
        Debug.Log("Training: control correctly identified. Score: " + score);
        UpdateUI();

        ShowCompletionPanel();
    }

    void ShowCompletionPanel()
    {
        if (completionPanel == null) return;

        completionPanel.SetActive(true);
        performanceText.text = "Training Completed!";
        finalScoreText.text = "Final Score: " + score + " / 100";

        if (score >= 80)
        {
            performanceText.text += "\nExcellent performance!";
        }
        else if (score >= 50)
        {
            performanceText.text += "\nGood job! Review the material for improvement.";
        }
        else
        {
            performanceText.text += "\nNeeds improvement. Please review the training material.";
        }
    }

    public int GetScore()
    {
        return score;
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

    void UpdateUI()
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(remainingTime).ToString() + "s";

        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }

    public void ConfigureDifficulty(string workLevel)
    {
        currentDifficultyLevel = workLevel;

        if ( workLevel == "Beginner" )
        {
            timeLimit = 45f;
        }
        else if (workLevel == "Intermediate")
        {
            timeLimit = 25f;
        }
    }

    public void RestartTraining()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
