using UnityEngine;

public class PlayerStockComponent 
{
    // Donut
    public int DonutCount { get; private set; }

    public void AddDonut(int count)
    {
        DonutCount += count;
    }

    public void RemoveDonut(int count)
    {
        DonutCount -= count;
    }
}
