using System.Collections.Generic;
using UnityEngine;


public class PlayerGrowthComponent : ComponentBase
{
    private readonly Dictionary<int, int> _expMaxByLevel = new Dictionary<int, int>
    {
        { 1, 20 },
        { 2, 20 },
        { 3, 20 },
        { 4, 20 },
        { 5, 20 },
    };

    public int Level { get; private set; }
    public int Exp { get; private set; }

    public void Initialize(PlayerData playerData)
    {
        Exp = playerData.exp;
        Level = playerData.level;
    }

    public void AddExp(int exp)
    {
        if (exp < 0)
            return;

        Exp += exp;

        UpdateLevel();

        //FluxSystem.Dispatch(new OnUpdateCurrency(CurrencyType.Cash, Cash));
    }

    public void RemoveExp(int exp)
    {
        if (exp < 0 || Exp < exp)
            return;

        Exp -= exp;

        //FluxSystem.Dispatch(new OnUpdateCurrency(CurrencyType.Cash, Cash));
    }

    private void UpdateLevel()
    {
        while (Exp >= _expMaxByLevel[Level])
        {
            RemoveExp(_expMaxByLevel[Level]);
            Level++;
        }

        //FluxSystem.Dispatch(new OnUpdatePlayerLevel(Level));
    }
}
