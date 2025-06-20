using System.Collections.Generic;
using UnityEngine;
using UniRx;


public class PlayerGrowthComponent : ComponentBase
{
    private readonly Dictionary<int, int> _expMaxByLevel = new Dictionary<int, int>
    {
        { 1, 20 },
        { 2, 40 },
        { 3, 60 },
        { 4, 80 },
        { 5, 100 },
        { 6, 120 },
        { 7, 140 },
        { 8, 160 },
        { 9, 180 },
    };

    public ReactiveProperty<int> Level { get; private set; } = new();
    public ReactiveProperty<int> Exp { get; private set; } = new();

    public int GetMaxExpByLevel(int level)
    {
        if (_expMaxByLevel.TryGetValue(level, out int maxExp))
        {
            return maxExp;
        }
        return 0; // 기본값 또는 예외 처리
    }

    public void Initialize(PlayerData playerData)
    {
        Exp.Value = playerData.exp;
        Level.Value = playerData.level;
    }

    public void AddExp(int exp)
    {
        if (exp < 0)
            return;

        Exp.Value += exp;

        UpdateLevel();

        GameManager.GetGameManager.Data.UpgradePlayerGrowth(Level.Value, Exp.Value);

        FluxSystem.Dispatch(new FxOnUpdatePlayerGrowth(exp));
    }

    public void RemoveExp(int exp)
    {
        if (exp < 0 || Exp.Value < exp)
            return;

        Exp.Value -= exp;
    }

    private void UpdateLevel()
    {
        bool isLevelUp = Exp.Value >= _expMaxByLevel[Level.Value];

        while (Exp.Value >= _expMaxByLevel[Level.Value])
        {
            Level.Value++;
        }
    }
}
