 using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BIMContextManager : MonoBehaviour
{
    [Header("BIM / SITE Context UI")]

    public TMP_Dropdown workAreaDropdown;
    public Toggle elevatedPlatformToggle;
    public Toggle electricalEquipmentToggle;
    public Toggle overheadWorkToggle;
    public TMP_Dropdown workerLevelDropdown;

    [Header("Scenario Prefabs")]
    public GameObject electrocutionPrefab;
    public GameObject struckByPrefab;

    [Header("AR Placement")]
    public ARPlacement aRPlacement;

    [Header("LLM")]
    public LLMScenarioGenerator
        llmScenarioGenerator;

    [Header("UI")]
    public GameObject bimContextPanel;

    public BIMContextData currentContext;

    public TrainingSessionManager
        trainingSessionManager;

    private bool generationInProgress = false;

    public void ReadBIMContext()
    {
        if (generationInProgress)
        {
            Debug.LogError("AI GENERATION ALREADY RUNNING");

            return;
        }

        currentContext = new BIMContextData();

        currentContext.workArea =  workAreaDropdown.options[workAreaDropdown.value].text;

        currentContext.elevatedPlatformPresent = elevatedPlatformToggle.isOn;

        currentContext.electricalEquipmentPresent = electricalEquipmentToggle.isOn;

        currentContext.overheadWorkPresent = overheadWorkToggle.isOn;

        currentContext.workerLevel =workerLevelDropdown.options[workerLevelDropdown.value].text;

        if (trainingSessionManager != null)
        {
            trainingSessionManager.ConfigureDifficulty(currentContext.workerLevel);
        }

        Debug.LogError("===== BIM CONTEXT =====\n" +
            "Work Area = " + currentContext.workArea + "\n" + 
            "Elevated = " + currentContext.elevatedPlatformPresent + "\n" +
            "Electrical = " + currentContext.electricalEquipmentPresent + "\n" +
            "Overhead = " + currentContext.overheadWorkPresent + "\n" +
            "Worker Level = " + currentContext.workerLevel
        );

        // Try LLM first
        if (llmScenarioGenerator != null)
        {
            generationInProgress = true;

            Debug.LogError("BIM -> LLM GENERATION STARTED");

            llmScenarioGenerator
                .GenerateScenario(
                    currentContext,
                    OnLLMScenarioReady,
                    OnLLMGenerationFailed
                );
        }
        else
        {
            Debug.LogError(
                "LLM GENERATOR NOT ASSIGNED - USING FALLBACK"
            );

            SelectFallbackScenario();
        }
    }

    void OnLLMScenarioReady(
        LLMScenarioSpec spec)
    {
        generationInProgress = false;

        GameObject selectedPrefab;

        if (spec.hazardType == "StruckBy")
        {
            selectedPrefab = struckByPrefab;
        }
        else if (spec.hazardType == "Electrocution")
        {
            selectedPrefab = electrocutionPrefab;
        }
        else
        {
            Debug.LogError( "UNKNOWN LLM HAZARD - USING FALLBACK" );

            SelectFallbackScenario();
            return;
        }

        Debug.LogError(
            "===== LLM SELECTED SCENARIO =====\n" +
            "Hazard = " + spec.hazardType + "\n" +
            "Difficulty = " + spec.difficulty + "\n" +
            "Prefab = " + selectedPrefab.name
        );

        aRPlacement.PrepareScenario(selectedPrefab, spec );

        if (bimContextPanel != null)
        {
            bimContextPanel.SetActive(false);
        }

        Debug.LogError( "AI SCENARIO READY - TAP TABLE TO PLACE");
    }

    void OnLLMGenerationFailed()
    {
        generationInProgress = false;

        Debug.LogError( "LLM FAILED - USING RULE-BASED FALLBACK" );

        SelectFallbackScenario();
    }

    void SelectFallbackScenario()
    {
        int workAreaIndex = workAreaDropdown.value;

        GameObject selectedPrefab = null;

        // 0 = General Construction
        // 1 = Electrical Maintenance
        // 2 = Elevated Work

        if (workAreaIndex == 2 && currentContext.overheadWorkPresent)
        {
            selectedPrefab = struckByPrefab;
        }
        else if ( workAreaIndex == 1 || currentContext.electricalEquipmentPresent)
        {
            selectedPrefab = electrocutionPrefab;
        }
        else if ( currentContext.elevatedPlatformPresent && currentContext.overheadWorkPresent)
        {
            selectedPrefab = struckByPrefab;
        }

        if (selectedPrefab == null)
        {
            Debug.LogError( "NO MATCHING FALLBACK SCENARIO" );

            return;
        }

        Debug.LogError( "FALLBACK SCENARIO = " + selectedPrefab.name );

        // null spec means old procedural
        // randomizer is used.
        aRPlacement.PrepareScenario(selectedPrefab, null);

        if (bimContextPanel != null)
        {
            bimContextPanel.SetActive(false);
        }
    }
}