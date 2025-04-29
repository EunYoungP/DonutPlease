using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StoreSystem
{
    private static List<Machine> _donutMachines = new();

    public void Initialize()
    {
        
    }

    public void AddMachine(Machine machine)
    {
        if (machine == null)
        {
            Debug.LogError("Machine is null");
            return;
        }
        _donutMachines.Add(machine);
    }
}
