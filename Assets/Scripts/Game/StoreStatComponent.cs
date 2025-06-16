using UnityEngine;
using UniRx;

public class StoreStatComponent : ComponentBase
{
    private const float MoveSpeedFactor = 0.2f;
    public ReactiveProperty<int> MoveSpeedGrade { get; private set; } = new();
    public ReactiveProperty<int> DonutCapacityGrade { get; private set; } = new();
    public ReactiveProperty<int> HiredCountGrade { get; private set; } = new();
    public float MoveSpeed { get; private set; }
    public int DonutCapacity { get; private set; }
    public int HiredCount { get; private set; }

    public void Initialize()
    {
        FluxSystem.ActionStream.Subscribe(data =>
        {
            if (data is FxOnUpdateHRStat updateData)
            {
                UpgradeWorkerData(updateData.fieldName, updateData.increase);
            }
        });

        var storeDtat = GameManager.GetGameManager.Data.SaveData.storeData;
        MoveSpeedGrade.Value = storeDtat.hrData.moveSpeedGrade;
        DonutCapacityGrade.Value = storeDtat.hrData.capacityGrade;
        HiredCountGrade.Value = storeDtat.hrData.hiredCountGrade;

        CalculateStatValue();
    }

    private void UpgradeWorkerData(string fieldName, int increment)
    {
        switch (fieldName)
        {
            case "moveSpeedGrade":
                MoveSpeedGrade.Value += increment;
                break;
            case "capacityGrade":
                DonutCapacityGrade.Value += increment;
                break;
            case "hiredCountGrade":
                HiredCountGrade.Value += increment;
                break;
            default:
                Debug.LogWarning($"[DataManager] Unknown HR field: {fieldName}");
                break;
        }

        GameManager.GetGameManager.Data.SaveHRData(
            MoveSpeedGrade.Value,
            DonutCapacityGrade.Value,
            HiredCountGrade.Value
        );

        CalculateStatValue();
    }

    private void CalculateStatValue()
    {
        MoveSpeed = 1 + (MoveSpeedFactor * MoveSpeedGrade.Value);
        DonutCapacity = 1 + DonutCapacityGrade.Value;
        HiredCount = 1 + HiredCountGrade.Value;
    }
}
