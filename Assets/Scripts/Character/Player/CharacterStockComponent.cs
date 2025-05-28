using UnityEngine;
using UniRx;
using NUnit.Framework;
using System.Collections.Generic;

public class CharacterStockComponent 
{
    // Donut
    public Stack<GameObject> Donuts { get; private set; } = new Stack<GameObject>();
    public int DonutCount => Donuts.Count;

    // Trash
    public Stack<GameObject> Trash { get; private set; } = new Stack<GameObject>();

    public void AddDonut(GameObject donut)
    {
        Donuts.Push(donut);
    }

    public GameObject RemoveDonut()
    {
        if (Donuts.Count == 0)
        {
            Debug.LogWarning("No donuts to remove.");
            return null;
        }

        return Donuts.Pop();
    }

    public void AddTrash(GameObject trash)
    {
        Trash.Push(trash);
    }

    public GameObject RemoveTrash()
    {
        if (Trash.Count == 0)
        {
            Debug.LogWarning("No trash to remove.");
            return null;
        }

        return Trash.Pop();
    }
}
