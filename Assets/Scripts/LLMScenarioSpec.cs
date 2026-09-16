using System;

[Serializable]
public class LLMScenarioSpec 
{
    public string hazardType;
    public string difficulty;

    public float hazardOffsetX;
    public float hazardOffsetZ;

    public float workerOffsetX;
    public float workerOffsetZ;

    public int distractorCount;
}
