using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Store
{
    private Counter _mainCounter;

    private List<Machine> _machines = new List<Machine>();

    private List<Table> _tables = new List<Table>();

    public Counter MainCounter => _mainCounter;
    public List<Machine> Machines => _machines;
    public List<Table> Tables => _tables;

    public void AddMainCouter(Counter counter)
    {
        _mainCounter = counter;
    }

    public void AddMachine(Machine machine)
    {
        _machines.Add(machine);
    }

    public void AddTable(Table table)
    {
        _tables.Add(table);
    }

    public bool CheckHaveEmptySeat()
    {
        foreach (var table in _tables)
        {
            if (table.GetEmptySeatPos(out var seatPos))
            {
                if (table.CheckHaveEmptySeat())
                    return true;
            }
        }
        return false;
    }

    public Transform GetEmptyTableSeat(out Table targetTable)
    {
        targetTable = null;

        foreach (var table in _tables)
        {
            if (table.GetEmptySeatPos(out var seatPos))
            {
                targetTable = table;
                return seatPos;
            }
        }
        return null;
    }
}
