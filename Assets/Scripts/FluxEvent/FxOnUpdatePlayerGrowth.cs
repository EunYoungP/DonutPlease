using UnityEngine;

public class FxOnUpdatePlayerGrowth : IFluxAction
{
    public readonly int addExp;

    public FxOnUpdatePlayerGrowth(int addExp)
    {
        this.addExp = addExp;
    }
}
