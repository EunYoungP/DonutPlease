using DonutPlease.Game.Character;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public enum EJob
{
    Cashier,
    CarryDonut,
    ClearTrash,

    // ���Ӹӽ�
}

public struct EJobData
{
    // ���� �ű�� -> �ӽ� ����
    // ��� �ϱ� -> ī���� ����
    // ���̺� û���ϱ� -> ���̺� ����
}

public class Store : MonoBehaviour
{
    [SerializeField] private GameObject WorkerPrefab;
    [SerializeField] private Transform WorkerStartPos;

    private Counter _mainCounter;

    private List<Machine> _machines = new List<Machine>();

    private List<Table> _tables = new List<Table>();

    private List<CharacterWorker> _workers = new List<CharacterWorker>();

    public Counter MainCounter => _mainCounter;
    public List<Machine> Machines => _machines;
    public List<Table> Tables => _tables;

    //Jobs
    private Dictionary<EJob, List<PropBase>> _jobs = new();

    #region Worker

    private void TestWorker()
    {

    }

    private void CreateWorker(CharacterWorker worker)
    {
        // ��Ŀ ����
        GameObject workerObj = Instantiate(WorkerPrefab, WorkerStartPos.position, Quaternion.identity);
        worker = workerObj.GetComponent<CharacterWorker>();
        worker.transform.SetParent(transform);

        AddWorker(worker);

        // ��Ŀ �۾� ����
        StartCoroutine(CoWorkerDo(worker));
    }

    private IEnumerator CoWorkerDo(CharacterWorker worker)
    {
        // ���� ��� �������� �˻�
        if (ShouldDoCarryDonut(out Machine targetMachine))
        {
            DonutPile donutPile = targetMachine.DonutPile;

            // 1. Job ���
            AddJob(EJob.CarryDonut, null);

            // 2. �۾��� ���� �ӽ����� �̵�
            worker.Controller.MoveTo(targetMachine.transform);

            // 2-1. �̵� ���
            yield return new WaitUntil(() => !worker.Controller.IsMoving);

            // 3. �̵� �� ������ �������� �̵� ���


            // 4. �ӽ� -> ���� ��������
            donutPile.GetDonutFromPileByCount(worker, targetMachine.DonutCount);

            // 4-1. ���� �ޱ� ���
            yield return new WaitUntil(() => !donutPile.IsWorkingAI);

            // 5. ī���ͷ� �̵�
            worker.Controller.MoveTo(MainCounter.DonutPileFrontPosition);

            // 5-1. �̵� ���
            yield return new WaitUntil(() => !worker.Controller.IsMoving);

            // 6. ī���Ϳ� ���� ����
            donutPile.LoopMoveDonutToPile(worker);

            //RemoveJob
        }

        // ĳ�� ���� �����ϴ��� �˻�
        if (ShouldDoMainCounterCashierJob(out Counter targetCounter))
        {
            // 1. Job ���
            AddJob(EJob.Cashier, null);

            // 2. �۾��� ī���ͷ� �̵�
            // 3. ĳ�� ����

            // �մ�/�ܹ��Ű� ���ٸ� �۾� ���
            //RemoveJob
        }

        // ���̺� û�� ���� �����ϴ��� �˻�
        if (ShouldDoClearTrash(out Table targetTable))
        {
            // 1. Job ���
            AddJob(EJob.ClearTrash, null);

            // 2. �۾��� ���̺�� �̵�
            // 3. ���̺� ������ �ݱ�
            // 4. ������������ �̵�
            // 5. �������뿡 ������ ������

            //RemoveJob
        }

        yield return null;
    }

    private bool ShouldDoCarryDonut(out Machine targetMachine)
    {
        targetMachine = null;

        var jobs = GetJobs(EJob.CarryDonut);

        // ������ �����ϴ� �ӽ��� �ִ��� �˻�
        foreach (var job in jobs)
        {
            if (job is Machine machine)
            {
                if (machine.DonutCount > 0)
                {
                    targetMachine = machine;
                    return true;
                }
            }
        }
        return false;
    }

    private bool ShouldDoMainCounterCashierJob(out Counter targetCounter)
    {
        var jobs = GetJobs(EJob.CarryDonut);

        // ĳ�� �۾��� �����ϴ��� Ȯ��
        foreach (var job in jobs)
        {
            if (job is Counter counter)
            {
                if (!counter.HaveCashier)
                {
                    targetCounter = counter;
                    return true; // ĳ�Ű� ���� ī���Ͱ� ������ ���
                }
            }
        }
        targetCounter = null;
        return false;
    }

    private bool ShouldDoClearTrash(out Table targetTable)
    {
        var jobs = GetJobs(EJob.ClearTrash);

        // ���̺� û�� �۾��� �ִ��� Ȯ��
        foreach (var job in jobs)
        {
            if (job is Table table)
            {
                if (!table.CheckHaveTrash())
                {
                    targetTable = table;
                    return true; // ���̺� �۾��� �ִ� ���
                }
            }
        }
        targetTable = null;
        return false;
    }

    #endregion

    #region Add/Remove

    // �� ���
    private void AddJob(EJob eJob, PropBase propBase)
    {
        if (!_jobs.TryAdd(eJob, new List<PropBase>() { propBase }))
            _jobs[eJob].Add(propBase);
    }

    //�� ����
    private void RemoveJob(EJob eJob, PropBase propBase)
    {
        GetJobs(eJob).Remove(propBase);
    }

    private List<PropBase> GetJobs(EJob eJob)
    {
        return _jobs[eJob];
    }

    private void AddWorker(CharacterWorker worker)
    {
        _workers.Add(worker);
    }

    public void AddMainCouter(Counter counter)
    {
        _mainCounter = counter;
        AddJob(EJob.Cashier, counter);
    }

    public void AddMachine(Machine machine)
    {
        _machines.Add(machine);
        AddJob(EJob.CarryDonut, machine);
    }

    public void AddTable(Table table)
    {
        _tables.Add(table);
        AddJob(EJob.ClearTrash, table);
    }

    public bool CheckHaveEmptySeat()
    {
        foreach (var table in _tables)
        {
            if (table.CheckHaveEmptySeat())
                return true;
        }
        return false;
    }

    public Vector3 GetEmptyTableSeat(out Table targetTable)
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
        return Vector3.zero;
    }

    #endregion
}
