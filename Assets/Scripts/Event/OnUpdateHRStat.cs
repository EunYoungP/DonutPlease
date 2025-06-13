using UnityEngine;

public class OnUpdateHRStat
{
    public readonly int moveSpeedGrade;
    public readonly int capacityGrade;
    public readonly int hiredCountGrade;

    public OnUpdateHRStat(int moveSpeedGrade, int capacityGrade, int hiredCountGrade)
    {
        this.moveSpeedGrade = moveSpeedGrade;
        this.capacityGrade = capacityGrade;
        this.hiredCountGrade = hiredCountGrade;
    }
}
