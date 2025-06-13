using DonutPlease.Game.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private TrashCan _trashCan;

    public Counter MainCounter => _mainCounter;
    public List<Machine> Machines => _machines;
    public List<Table> Tables => _tables;

    // Worker Stat
    private const float MoveSpeedFactor = 0.2f;
    public int MoveSpeedGrade { get; private set; }
    public int DonutCapacityGrade { get; private set; }
    public int HiredCountGrade { get; private set; }
    public float MoveSpeed { get; private set; }
    public int DonutCapacity { get; private set; }
    public int HiredCount { get; private set; }

    //Jobs : ��Ȯ�� Job�� ǥ���ϴ� �����̳����� �Ǵ��غ� �ʿ�.
    private Dictionary<EJob, List<PropBase>> _jobs = new();

    private void OnEnable()
    {
        // ��Ŀ ���� �׽�Ʈ
        TestWorker();
    }

    public void Initialize()
    {
        TrashCan localMapTrashCan = GameManager.GetGameManager.LocalMap.Map.GetComponentInChildren<TrashCan>();
        AddTrashCan(localMapTrashCan);

        var storeDtat = GameManager.GetGameManager.Data.SaveData.storeData;

        MoveSpeedGrade = storeDtat.hrData.moveSpeedGrade;
        DonutCapacityGrade = storeDtat.hrData.capacityGrade;
        HiredCountGrade = storeDtat.hrData.hiredCountGrade;

        CalculateStatValue();
    }

    public void UpgradeWorkerData(string fieldName, int increment)
    {
        switch (fieldName)
        {
            case "moveSpeedGrade":
                MoveSpeedGrade += increment;
                break;
            case "capacityGrade":
                DonutCapacityGrade += increment;
                break;
            case "hiredCountGrade":
                HiredCountGrade += increment;
                break;
            default:
                Debug.LogWarning($"[DataManager] Unknown HR field: {fieldName}");
                break;
        }

        CalculateStatValue();

        FluxSystem.Dispatch(new OnUpdateHRStat(MoveSpeedGrade, DonutCapacityGrade, HiredCountGrade));
    }

    private void CalculateStatValue()
    {
        MoveSpeed = 1 + (MoveSpeedFactor * MoveSpeedGrade);
        DonutCapacity = 1 + DonutCapacityGrade;
        HiredCount = 1 + HiredCountGrade;
    }

    #region Worker

    private void TestWorker()
    {
        CreateWorker();
    }

    private void CreateWorker()
    {
        // ��Ŀ ����
        var worker = GameManager.GetGameManager.Store.CreateWorker();
        worker.transform.SetParent(transform);

        AddWorker(worker);

        // ��Ŀ �۾� ����
        StartCoroutine(CoWorkerDo(worker));
    }

    private IEnumerator CoWorkerDo(CharacterWorker worker)
    {
        // test
        TrashCan trashCan = GameManager.GetGameManager.Store.TrashCan;

        while (true)
        {
            // ���� ��� �������� �˻�
            if (ShouldDoCarryDonut(out Machine targetMachine))
            {
                DonutPile machineDonutPile = targetMachine.DonutPile;
                DonutPile counterDonutPile = _mainCounter.DonutPile;

                // 1. Job ���
                AddJob(EJob.CarryDonut, targetMachine);

                // 2. �۾��� ���� �ӽ����� �̵�
                worker.Controller.MoveTo(targetMachine.DonutPileFrontPosition);

                // 2-1. �̵� ���
                yield return new WaitUntil(() => !worker.Controller.IsMoving);

                // 3. �̵� �� ������ �������� �̵� ���

                // 4. �ӽ� -> ���� ��������
                machineDonutPile.GetDonutFromPileByCount(worker, targetMachine.DonutCount);

                // 4-1. ���� �ޱ� ���
                yield return new WaitUntil(() => !machineDonutPile.IsWorking);

                // 5. ī���ͷ� �̵�
                worker.Controller.MoveTo(_mainCounter.DonutPileFrontPosition);

                // 5-1. �̵� ���
                yield return new WaitUntil(() => !worker.Controller.IsMoving);

                // 6. ī���Ϳ� ���� ����
                counterDonutPile.LoopMoveDonutToPile(worker);

                // 6-1. ���� ���� ���
                yield return new WaitUntil(() => !counterDonutPile.IsWorking);

                //RemoveJob
                RemoveJob(EJob.CarryDonut, targetMachine);
            }

            // ĳ�� ���� �����ϴ��� �˻�
            if (ShouldDoMainCounterCashierJob(out Counter targetCounter))
            {
                // 1. Job ���
                AddJob(EJob.Cashier, targetCounter);

                // 2. �۾��� ī���ͷ� �̵�
                worker.Controller.MoveTo(targetCounter.CasherPlace);

                // 2-1. �̵� ���
                yield return new WaitUntil(() => !worker.Controller.IsMoving);

                // 3. ĳ�� ����
                targetCounter.SetCashier(worker);

                // �մ�/�ܹ��Ű� ���ٸ� �۾� ���
                yield return new WaitUntil(() => !ShouldDoMainCounterCashierJob(out Counter targetCounter));

                //RemoveJob
                RemoveJob(EJob.Cashier, targetCounter);
            }

            // ���̺� û�� ���� �����ϴ��� �˻�
            if (ShouldDoClearTrash(out Table targetTable))
            {
                // 1. Job ���
                AddJob(EJob.ClearTrash, targetTable);

                // 2. �۾��� ���̺�� �̵�
                worker.Controller.MoveTo(targetTable.TrashFrontPos);

                // 2-1. �̵� ���
                yield return new WaitUntil(() => !worker.Controller.IsMoving);

                // 3. ������ ����
                targetTable.ClearTable(out var trash);

                if (trash != null)
                {
                    // 3. ���̺� ������ �ݱ�
                    worker.AddToTray(trash.transform);

                    // 4. ������������ �̵�
                    worker.Controller.MoveTo(trashCan.TrashCanFrontPos);

                    // 4-1. �̵� ���
                    yield return new WaitUntil(() => !worker.Controller.IsMoving);

                    // 5. �������뿡 ������ ������
                    worker.RemoveFromTray(EItemType.Trash, trashCan.TrashDropPos);
                }

                //RemoveJob
            }
            yield return null;
        }
    }

    private bool ShouldDoCarryDonut(out Machine targetMachine)
    {
        targetMachine = null;

        var jobs = GetJobs(EJob.CarryDonut);
        if (jobs == null)
            return false;

        if (MainCounter == null)
            return false;

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
        targetCounter = null;

        if (MainCounter == null)
            return false;

        //var jobs = GetJobs(EJob.Cashier);
        //if (jobs == null)
        //    return false;

        // ĳ�� �۾��� �����ϴ��� Ȯ��
        //foreach (var job in jobs)
        //{
        //    if (job is Counter counter)
        //    {
        //        if (!counter.HaveCashier)
        //        {
        //            targetCounter = counter;
        //            return true; // ĳ�Ű� ���� ī���Ͱ� ������ ���
        //        }
        //    }
        //}

        if (!_mainCounter.HaveCashier)
        {
            // ������ ���� ���
            if (_mainCounter.DonutCount == 0)
                return false;

            // �� �� �մ��� ���� ���
            if (_mainCounter.InLineCustomerCount == 0)
                return false;

            targetCounter = _mainCounter;
            return true; // ĳ�Ű� ���� ī���Ͱ� ������ ���
        }
        return false;
    }

    private bool ShouldDoClearTrash(out Table targetTable)
    {
        targetTable = null;

        var jobs = GetJobs(EJob.ClearTrash);
        if (jobs == null)
            return false;

        // Ʈ���̰� ����ְų�, ���������� Ȯ��

        // ���̺� û�� �۾��� �ִ��� Ȯ��
        foreach (var job in jobs)
        {
            if (job is Table table)
            {
                if (table.CheckHaveTrash())
                {
                    targetTable = table;
                    return true; // ���̺� �۾��� �ִ� ���
                }
            }
        }
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
        _jobs.TryGetValue(eJob, out var jobList);
        return jobList;
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

    public void AddTrashCan(TrashCan trashCan)
    {
        if (_trashCan == null)
        {
            _trashCan = trashCan;
        }
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

    public Vector3 GetEmptyTableSeat(out Table targetTable, out int seatIndex)
    {
        targetTable = null;
        seatIndex = -1;

        foreach (var table in _tables)
        {
            if (table.GetEmptySeatPos(out var seatPos, out var index))
            {
                targetTable = table;
                seatIndex = index;
                return seatPos;
            }
        }
        return Vector3.zero;
    }

    #endregion

    #region HR

    public void UpdateHRData()
    {
        var hrData = GameManager.GetGameManager.Data.SaveData.storeData.hrData;

        var moveSpeedGrade = hrData.moveSpeedGrade;
        var burgerCapacityGrade = hrData.capacityGrade;
        var hiredCountGrade = hrData.hiredCountGrade;

        MoveSpeed = 1 + (MoveSpeedFactor * moveSpeedGrade);
        DonutCapacity = 1 + burgerCapacityGrade;
        HiredCount = 1 + hiredCountGrade;


    }

    #endregion
}
