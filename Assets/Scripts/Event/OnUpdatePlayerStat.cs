using UnityEngine;

public class OnUpdatePlayerStat
{
    public readonly int MoveSpeedGrade;
    public readonly int DonutCapacityGrade;
    public readonly int ProfitGrowthRateGrade;

    public OnUpdatePlayerStat(int moveSpeedGrade, int donutCapacityGrade, int profitGrowthRateGrade)
    {
        MoveSpeedGrade = moveSpeedGrade;
        DonutCapacityGrade = donutCapacityGrade;
        ProfitGrowthRateGrade = profitGrowthRateGrade;
    }
}
