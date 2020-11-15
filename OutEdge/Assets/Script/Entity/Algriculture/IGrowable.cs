
using Unity.Entities;

public struct IGrowable : IComponentData
{
    public float startTime;
    public float matureTime;
    public int wetness;

    public int curve;

    public int ResultType;
    public bool seperated;

    public int grownTime;
    public int randomBiasRange;

    public override string ToString()
    {
        return startTime + ":" + matureTime + ":" + wetness;
    }
}