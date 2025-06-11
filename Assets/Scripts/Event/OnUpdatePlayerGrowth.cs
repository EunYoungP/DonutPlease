using UnityEngine;

public class OnUpdatePlayerGrowth
{
    public readonly int level;
    public readonly int exp;
    public readonly int maxExp;

    public OnUpdatePlayerGrowth(int level, int exp, int maxExp)
    {
        this.level = level;
        this.exp = exp;
        this.maxExp = maxExp;
    }
}
