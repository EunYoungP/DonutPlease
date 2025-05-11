using UnityEngine;
using UniRx;
using NUnit.Framework;
using System.Collections.Generic;

public class PlayerStockComponent 
{
    // Donut
    public Stack<GameObject> Donuts { get; private set; } = new Stack<GameObject>();
    public int DonutCount => Donuts.Count;

    public void AddDonut(GameObject donut)
    {
        Donuts.Push(donut);
    }

    public GameObject RemoveDonut()
    {
        return Donuts.Pop();
    }
}
