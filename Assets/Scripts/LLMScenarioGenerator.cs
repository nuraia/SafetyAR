using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LLMScenarioGenerator : MonoBehaviour
{
    [Header("Ollama Settings")]
    public string ollamaUrl =
        "http://192.168.1.104:11434/api/generate";

    public string modelName = "llama3.2:3b";

    public void GenerateScenario(
        BIMContextData context,
        Action<LLMScenarioSpec> onSuccess,
        Action onFailure)
    {
        StartCoroutine(
            GenerateScenarioCoroutine(
                context,
                onSuccess,
                onFailure
            )
        );
    }

    IEnumerator GenerateScenarioCoroutine(
        BIMContextData context,
        Action<LLMScenarioSpec> onSuccess,
        Action onFailure)
    {
        Debug.LogError("===== LLM SCENARIO GENERATION STARTED =====");

        string prompt =
            "You are a construction safety procedural scenario generator. " +

            "Return ONLY one JSON object with exactly these keys: " +
            "hazardType, difficulty, hazardOffsetX, hazardOffsetZ, " +
            "workerOffsetX, workerOffsetZ, distractorCount. " +

            "Allowed hazardType values: Electrocution or StruckBy. " +
            "Allowed difficulty values: Beginner or Intermediate. " +

            "Rules: " +
            "If workArea is Elevated Work and overheadWork=true, " +
            "hazardType must be StruckBy. " +

            "Otherwise, if workArea is Electrical Maintenance or " +
            "electricalEquipment=true, hazardType must be Electrocution. " +

            "Otherwise, if elevatedPlatform=true and overheadWork=true, " +
            "hazardType must be StruckBy. " +

            "difficulty must exactly equal workerLevel. " +

            "hazardOffsetX and hazardOffsetZ must be between -0.06 and 0.06. " +
            "workerOffsetX and workerOffsetZ must be between -0.04 and 0.04. " +
            "distractorCount must be an integer from 0 to 2. " +

            "Use non-zero offsets when possible to create procedural variation. " +
            "Do not add descriptions or extra fields. " +

            "SITE CONTEXT: " +
            "workArea=" + context.workArea + ", " +
            "electricalEquipment=" +
            context.electricalEquipmentPresent + ", " +
            "elevatedPlatform=" +
            context.elevatedPlatformPresent + ", " +
            "overheadWork=" +
            context.overheadWorkPresent + ", " +
            "workerLevel=" +
            context.workerLevel + ".";

        OllamaRequest requestData =
            new OllamaRequest
            {
                model = modelName,
                prompt = prompt,
                format = "json",
                stream = false
            };

        string jsonData = JsonUtility.ToJson(requestData);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using UnityWebRequest request =
            new UnityWebRequest(
                ollamaUrl,
                UnityWebRequest.kHttpVerbPOST
            );

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        request.timeout = 120;

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("LLM REQUEST FAILED: " + request.error);

            onFailure?.Invoke();
            yield break;
        }

        OllamaResponse ollamaResponse;

        try
        {
            ollamaResponse = JsonUtility.FromJson<OllamaResponse>(
                    request.downloadHandler.text
                );
        }
        catch (Exception e)
        {
            Debug.LogError("FAILED TO PARSE OLLAMA RESPONSE: " + e.Message);

            onFailure?.Invoke();
            yield break;
        }

        if (ollamaResponse == null || string.IsNullOrEmpty(ollamaResponse.response))
        {
            Debug.LogError("OLLAMA RETURNED EMPTY RESPONSE");

            onFailure?.Invoke();
            yield break;
        }

        Debug.LogError("LLM GENERATED JSON = [" + ollamaResponse.response + "]");

        LLMScenarioSpec spec;

        try
        {
            spec = JsonUtility.FromJson<LLMScenarioSpec>(ollamaResponse.response);
        }
        catch (Exception e)
        {
            Debug.LogError("FAILED TO PARSE SCENARIO: " + e.Message);

            onFailure?.Invoke();
            yield break;
        }

        if (spec == null)
        {
            onFailure?.Invoke();
            yield break;
        }

        // ---------------------------------
        // SAFETY / CONSTRAINT VALIDATION
        // ---------------------------------

        string expectedHazard = GetExpectedHazard(context);

        if (!string.IsNullOrEmpty(expectedHazard) && spec.hazardType != expectedHazard)
        {
            Debug.LogError("LLM HAZARD REPAIRED: " + spec.hazardType + " -> " + expectedHazard);

            spec.hazardType = expectedHazard;
        }

        // Difficulty must follow worker profile
        spec.difficulty = context.workerLevel;

        // Clamp generated spatial values
        spec.hazardOffsetX = Mathf.Clamp(spec.hazardOffsetX, -0.06f, 0.06f);

        spec.hazardOffsetZ = Mathf.Clamp(spec.hazardOffsetZ, -0.06f, 0.06f);

        spec.workerOffsetX = Mathf.Clamp(spec.workerOffsetX, -0.04f, 0.04f);

        spec.workerOffsetZ = Mathf.Clamp(spec.workerOffsetZ, -0.04f, 0.04f);

        spec.distractorCount =
            Mathf.Clamp(spec.distractorCount, 0, 2);

        Debug.LogError(
            "===== VALIDATED LLM SCENARIO =====\n" +
            "Hazard = " + spec.hazardType + "\n" +
            "Difficulty = " + spec.difficulty + "\n" +
            "Hazard Offset = (" + spec.hazardOffsetX + ", " + spec.hazardOffsetZ + ")\n" +
            "Worker Offset = (" + spec.workerOffsetX + ", " + spec.workerOffsetZ + ")\n" +
            "Distractors = " + spec.distractorCount
        );

        onSuccess?.Invoke(spec);
    }

    string GetExpectedHazard(
        BIMContextData context)
    {
        if (context.workArea == "Elevated Work" && context.overheadWorkPresent)
        {
            return "StruckBy";
        }

        if (context.workArea == "Electrical Maintenance" || context.electricalEquipmentPresent)
        {
            return "Electrocution";
        }

        if (context.elevatedPlatformPresent && context.overheadWorkPresent)
        {
            return "StruckBy";
        }

        return "";
    }
}