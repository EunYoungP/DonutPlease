using UnityEngine;
using UniRx;

public class StoreStatComponent : ComponentBase
{
    private const float MoveSpeedFactor = 0.2f;
    public int MoveSpeedGrade { get; private set; }
    public int DonutCapacityGrade { get; private set; }
    public int HiredCountGrade { get; private set; }
    public ReactiveProperty<float> MoveSpeed { get; private set; } = new();
    public ReactiveProperty<int> DonutCapacity { get; private set; } = new();
    public ReactiveProperty<int> HiredCount { get; private set; } = new();

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
        MoveSpeedGrade = storeDtat.hrData.moveSpeedGrade;
        DonutCapacityGrade = storeDtat.hrData.capacityGrade;
        HiredCountGrade = storeDtat.hrData.hiredCountGrade;

        CalculateStatValue();
    }

    private void UpgradeWorkerData(string fieldName, int increment)
    {
        switch (fieldName)
        {
            case "moveSpeedGrade":
                MoveSpeedGrade += increment;
                break;
            case "capacityGrade":
                DonutCapacityGrade += increment;
                break;
            case "hiredCountGrade":
                HiredCountGrade += increment;
                break;
            default:
                Debug.LogWarning($"[DataManager] Unknown HR field: {fieldName}");
                break;
        }

        GameManager.GetGameManager.Data.SaveHRData(
            MoveSpeedGrade,
            DonutCapacityGrade,
            HiredCountGrade
        );

        CalculateStatValue();
    }

    private void CalculateStatValue()
    {
        MoveSpeed.Value = 1 + (MoveSpeedFactor * MoveSpeedGrade);
        DonutCapacity.Value = 1 + DonutCapacityGrade;
        HiredCount.Value = HiredCountGrade;
    }
}
