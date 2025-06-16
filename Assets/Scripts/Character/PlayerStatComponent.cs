using NUnit.Framework;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerStatComponent : ComponentBase
{
    private const float MoveSpeedFactor = 0.2f;

    public ReactiveProperty<int> MoveSpeedGrade { get; private set; } = new();
    public ReactiveProperty<int> DonutCapacityGrade { get; private set; } = new();
    public ReactiveProperty<int> ProfitGrowthRateGrade { get; private set; } = new();

    public float MoveSpeed { get; private set; }
    public int DonutCapacity { get; private set; }
    public float ProfitGrowthRate { get; private set; }

    public PlayerStatComponent()
    {
        FluxSystem.ActionStream.Subscribe(data =>
        {
            if (data is FxOnUpdatePlayerStat updateData)
            {
                UpgradePlayerData(updateData.fieldName, updateData.increase);
            }
        });

        Initialize();
    }

    private void Initialize()
    {
        var playerData = GameManager.GetGameManager.Data.SaveData.playerData;

        MoveSpeedGrade.Value = playerData.moveSpeedGrade;
        DonutCapacityGrade.Value = playerData.capacityGrade;
        ProfitGrowthRateGrade.Value = playerData.profitGrowthGrade;

        CalculateStatValue();
    }

    private void UpgradePlayerData(string fieldName, int increment)
    {
        switch (fieldName)
        {
            case "moveSpeedGrade":
                MoveSpeedGrade.Value += increment;
                break;
            case "capacityGrade":
                DonutCapacityGrade.Value += increment;
                break;
            case "profitGrowthGrade":
                ProfitGrowthRateGrade.Value += increment;
                break;
            default:
                Debug.LogWarning($"[DataManager] Unknown HR field: {fieldName}");
                break;
        }

        GameManager.GetGameManager.Data.SavePlayerData(
            MoveSpeedGrade.Value, 
            DonutCapacityGrade.Value, 
            ProfitGrowthRateGrade.Value);   

        CalculateStatValue();
    }

    private void CalculateStatValue()
    {
        MoveSpeed = 1 + (MoveSpeedFactor * MoveSpeedGrade.Value);
        DonutCapacity = 1 + DonutCapacityGrade.Value;
        ProfitGrowthRate = 1 + (0.1f * ProfitGrowthRateGrade.Value);
    }
}
