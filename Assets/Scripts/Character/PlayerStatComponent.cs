using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerStatData
{
    public float moveSpeedGrade;
    public int donutCapacityGrade;
    public float profitGrowthRateGrade;

    public PlayerStatData(float moveSpeedGrade, int donutCapacityGrade, float profitGrowthRateGrade)
    {
        this.moveSpeedGrade = moveSpeedGrade;
        this.donutCapacityGrade = donutCapacityGrade;
        this.profitGrowthRateGrade = profitGrowthRateGrade;
    }
}

public class PlayerStatComponent : ComponentBase
{
    private const float MoveSpeedFactor = 0.2f;

    public int MoveSpeedGrade { get; private set; }
    public int DonutCapacityGrade { get; private set; }
    public int ProfitGrowthRateGrade { get; private set; }

    public float MoveSpeed { get; private set; }
    public int DonutCapacity { get; private set; }
    public float ProfitGrowthRate { get; private set; }

    public PlayerStatComponent()
    {
        Initialize();
    }

    private void Initialize()
    {
        var playerData = GameManager.GetGameManager.Data.SaveData.playerData;

        MoveSpeedGrade = playerData.moveSpeedGrade;
        DonutCapacityGrade = playerData.capacityGrade;
        ProfitGrowthRateGrade = playerData.profitGrowthGrade;

        CalculateStatValue();
    }

    public void UpgradePlayerData(string fieldName, int increment)
    {
        switch (fieldName)
        {
            case "moveSpeedGrade":
                MoveSpeedGrade += increment;
                break;
            case "capacityGrade":
                DonutCapacityGrade += increment;
                break;
            case "profitGrowthGrade":
                ProfitGrowthRateGrade += increment;
                break;
            default:
                Debug.LogWarning($"[DataManager] Unknown HR field: {fieldName}");
                break;
        }

        CalculateStatValue();

        FluxSystem.Dispatch(new OnUpdatePlayerStat(MoveSpeedGrade, DonutCapacityGrade, ProfitGrowthRateGrade));
    }

    private void CalculateStatValue()
    {
        MoveSpeed = 1 + (MoveSpeedFactor * MoveSpeedGrade);
        DonutCapacity = 1 + DonutCapacityGrade;
        ProfitGrowthRate = 1 + (0.1f * ProfitGrowthRateGrade);
    }
}
