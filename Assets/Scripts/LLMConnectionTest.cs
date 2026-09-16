using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class LLMConnectionTest : MonoBehaviour
{
    [Header("Ollama Settings")]
    public string ollamaUrl = "http://192.168.1.104:11434/api/generate";
    public string modelName = "llama3.2:3b";

    public void TestLLM()
    {
        StartCoroutine(SendTestRequest());
    }

    IEnumerator SendTestRequest()
    {
        Debug.LogError("LLM TEST: REQUEST STARTED");
        OllamaRequest requestData = new OllamaRequest
        {
            model = modelName,
            prompt =
                "You are a construction safety procedural scenario generator. " +
                "Return ONLY one JSON object and nothing else. " +
                
                "Allowed hazardType values are exactly: " +
                "Electrocution or StruckBy. " +

                "Allowed difficulty values are exactly: " +
                "Beginner or Intermediate. " +

                "Do not return Moderate, Easy, Hard, numbers, descriptions, " +
                "or any additional fields. " +

                "Rules: " +
                "If workArea is Elevated Work and overheadWork=true, " +
                "hazardType must be StruckBy. " +

                "If workArea is Electrical Maintenance or " +
                "electricalEquipment=true, hazardType must be Electrocution. " +

                "difficulty must exactly match workerLevel. " +

                "hazardOffsetX and hazardOffsetZ must be numbers " +
                "between -0.06 and 0.06. " +

                "workerOffsetX and workerOffsetZ must be numbers " +
                "between -0.04 and 0.04. " +

                "distractorCount must be an integer from 0 to 2. " +

                "Site context: " +
                "workArea=Elevated Work, " +
                "electricalEquipment=false, " +
                "elevatedPlatform=true, " +
                "overheadWork=true, " +
                "workerLevel=Intermediate. " +

                "Return exactly these seven keys: " +
                "hazardType, difficulty, hazardOffsetX, hazardOffsetZ, " +
                "workerOffsetX, workerOffsetZ, distractorCount.",

            format = "json",
            stream = false
        };

        string jsonData = JsonUtility.ToJson(requestData);
        Debug.LogError("LLM TEST: REQUEST DATA: " + jsonData);

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using UnityWebRequest request = new UnityWebRequest(ollamaUrl, UnityWebRequest.kHttpVerbPOST);
        
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Wait up to 120 seconds for the local LLM
        request.timeout = 120;

        Debug.LogError("LLM TEST: SENDING REQUEST...");

        yield return request.SendWebRequest();
       
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("LLM TEST FAILED: " + request.error);

            Debug.LogError("SERVER RESPONSE: " +request.downloadHandler.text);

            yield break;
        }
        string rawResponse = request.downloadHandler.text;

        Debug.LogError(
            "LLM STATUS = " + request.responseCode
        );

        Debug.LogError(
            "LLM RESULT = " + request.result
        );

        Debug.LogError(
            "LLM RAW LENGTH = " +
            (rawResponse == null ? -1 : rawResponse.Length)
        );

        Debug.LogError(
            "LLM RAW RESPONSE = [" + rawResponse + "]"
        );

       OllamaResponse ollamaResponse = JsonUtility.FromJson<OllamaResponse>(
        rawResponse
        );

        Debug.LogError("LLM GENERATED CONTENT = [" + ollamaResponse.response +"]");

        LLMScenarioSpec spec =
        JsonUtility.FromJson<LLMScenarioSpec>(
            ollamaResponse.response
        );

        Debug.LogError(
            "===== PARSED LLM SCENARIO =====\n" +
            "Hazard = " + spec.hazardType + "\n" +
            "Difficulty = " + spec.difficulty + "\n" +
            "Hazard X = " + spec.hazardOffsetX + "\n" +
            "Hazard Z = " + spec.hazardOffsetZ + "\n" +
            "Worker X = " + spec.workerOffsetX + "\n" +
            "Worker Z = " + spec.workerOffsetZ + "\n" +
            "Distractors = " + spec.distractorCount
        );

        bool validHazard = spec.hazardType == "Electrocution" ||
            spec.hazardType == "StruckBy";

        bool validDifficulty = spec.difficulty == "Beginner" ||
            spec.difficulty == "Intermediate";

        if (!validHazard || !validDifficulty)
        {
            Debug.LogError("LLM OUTPUT INVALID - FALLBACK REQUIRED");

            yield break;
        }

        spec.hazardOffsetX =
            Mathf.Clamp(spec.hazardOffsetX, -0.06f, 0.06f);

        spec.hazardOffsetZ =
            Mathf.Clamp(spec.hazardOffsetZ, -0.06f, 0.06f);

        spec.workerOffsetX =
            Mathf.Clamp(spec.workerOffsetX, -0.04f, 0.04f);

        spec.workerOffsetZ =
            Mathf.Clamp(spec.workerOffsetZ, -0.04f, 0.04f);

        spec.distractorCount =
            Mathf.Clamp(spec.distractorCount, 0, 2);

        Debug.LogError(
            "LLM SCENARIO VALIDATED SUCCESSFULLY"
        );
    }

}
