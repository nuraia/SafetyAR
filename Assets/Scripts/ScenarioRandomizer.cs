 using UnityEngine;

public class ScenarioRandomizer : MonoBehaviour
{
    [Header("Objects to vary")]
    public Transform hazardObject;
    public Transform worker;

    [Header("Optional Distractors")]
    public GameObject[] distractors;

    [Header("Fallback Random Variation")]
    public float hazardRange = 0.08f;
    public float workerRange = 0.05f;

    private Vector3 baseHazardPosition;
    private Vector3 baseWorkerPosition;

    private bool useLLMScenario = false;

    void Awake()
    {
        if (hazardObject != null)
        {
            baseHazardPosition =
                hazardObject.localPosition;
        }

        if (worker != null)
        {
            baseWorkerPosition =
                worker.localPosition;
        }
    }

    void Start()
    {
        // Only use old random PCG when
        // no LLM scenario was supplied.
        if (!useLLMScenario)
        {
            Debug.LogError(
                "PCG FALLBACK: USING RANDOM SCENARIO"
            );

            RandomizeScenario();
        }
    }

    public void ApplyGeneratedSpec(
        LLMScenarioSpec spec)
    {
        if (spec == null)
            return;

        useLLMScenario = true;

        if (hazardObject != null)
        {
            hazardObject.localPosition = baseHazardPosition + new Vector3(
                    spec.hazardOffsetX,
                    0f,
                    spec.hazardOffsetZ
                );
        }

        if (worker != null)
        {
            worker.localPosition = baseWorkerPosition + new Vector3(
                spec.workerOffsetX, 
                0f, 
                spec.workerOffsetZ
                );
        }

        ApplyDistractors(spec.distractorCount);

        Debug.LogError(
            "===== LLM PCG APPLIED =====\n" +
            "Hazard X = " + spec.hazardOffsetX + "\n" +

            "Hazard Z = " + spec.hazardOffsetZ + "\n" +

            "Worker X = " + spec.workerOffsetX + "\n" +

            "Worker Z = " + spec.workerOffsetZ + "\n" +

            "Distractors = " + spec.distractorCount
        );
    }

    void ApplyDistractors(int count)
    {
        if (distractors == null)
            return;

        for (int i = 0; i < distractors.Length; i++)
        {
            if (distractors[i] != null)
            {
                distractors[i].SetActive(i < count);
            }
        }
    }

    public void RandomizeScenario()
    {
        if (hazardObject != null)
        {
            Vector3 position = baseHazardPosition;

            position.x += Random.Range(-hazardRange, hazardRange);

            position.z += Random.Range(-hazardRange, hazardRange);

            hazardObject.localPosition = position;
        }

        if (worker != null)
        {
            Vector3 position = baseWorkerPosition;

            position.x += Random.Range(-workerRange, workerRange);

            position.z += Random.Range(-workerRange, workerRange);

            worker.localPosition = position;
        }
    }
}