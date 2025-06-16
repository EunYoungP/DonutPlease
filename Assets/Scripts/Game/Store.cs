
using DonutPlease.Game.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EJob
{
    Cashier,
    CarryDonut,
    ClearTrash,

    // 도넛머신
}

public class Store : MonoBehaviour
{
    [SerializeField] private GameObject WorkerPrefab;
    [SerializeField] private Transform WorkerStartPos;

    public Counter MainCounter { get; private set; }
    public List<Machine> Machines { get; private set; } = new List<Machine>();
    public List<Table> Tables { get; private set; } = new List<Table>();
    public List<CharacterWorker> Workers { get; private set; } = new List<CharacterWorker>();

    private TrashCan _trashCan;

    //Jobs : 정확히 Job을 표현하는 컨테이너인지 판단해볼 필요.
    private Dictionary<EJob, List<PropBase>> _jobs = new();

    public StoreStatComponent Stat { get; private set; }

    private void OnEnable()
    {
        // 워커 생성 테스트
        //TestWorker();
    }

    public void Initialize()
    {
        Stat = new();
        Stat.Initialize();

        TrashCan localMapTrashCan = GameManager.GetGameManager.LocalMap.Map.GetComponentInChildren<TrashCan>();
        AddTrashCan(localMapTrashCan);
    }

    #region Worker

    private void TestWorker()
    {
        CreateWorker();
    }

    private void CreateWorker()
    {
        // 워커 생성
        var worker = GameManager.GetGameManager.Store.CreateWorker();
        worker.transform.SetParent(transform);

        AddWorker(worker);

        // 워커 작업 시작
        StartCoroutine(CoWorkerDo(worker));
    }

    private IEnumerator CoWorkerDo(CharacterWorker worker)
    {
        // test
        TrashCan trashCan = GameManager.GetGameManager.Store.TrashCan;

        while (true)
        {
            // 도넛 운반 가능한지 검사
            if (ShouldDoCarryDonut(out Machine targetMachine))
            {
                DonutPile machineDonutPile = targetMachine.DonutPile;
                DonutPile counterDonutPile = MainCounter.DonutPile;

                // 1. Job 등록
                AddJob(EJob.CarryDonut, targetMachine);

                // 2. 작업할 도넛 머신으로 이동
                worker.Controller.MoveTo(targetMachine.DonutPileFrontPosition);

                // 2-1. 이동 대기
                yield return new WaitUntil(() => !worker.Controller.IsMoving);

                // 3. 이동 중 도넛이 없어지면 이동 취소

                // 4. 머신 -> 도넛 가져오기
                machineDonutPile.GetDonutFromPileByCount(worker, targetMachine.DonutCount);

                // 4-1. 도넛 받기 대기
                yield return new WaitUntil(() => !machineDonutPile.IsWorking);

                // 5. 카운터로 이동
                worker.Controller.MoveTo(MainCounter.DonutPileFrontPosition);

                // 5-1. 이동 대기
                yield return new WaitUntil(() => !worker.Controller.IsMoving);

                // 6. 카운터에 도넛 놓기
                counterDonutPile.LoopMoveDonutToPile(worker);

                // 6-1. 도넛 놓기 대기
                yield return new WaitUntil(() => !counterDonutPile.IsWorking);

                //RemoveJob
                RemoveJob(EJob.CarryDonut, targetMachine);
            }

            // 캐셔 업무 존재하는지 검사
            if (ShouldDoMainCounterCashierJob(out Counter targetCounter))
            {
                // 1. Job 등록
                AddJob(EJob.Cashier, targetCounter);

                // 2. 작업할 카운터로 이동
                worker.Controller.MoveTo(targetCounter.CasherPlace);

                // 2-1. 이동 대기
                yield return new WaitUntil(() => !worker.Controller.IsMoving);

                // 3. 캐셔 동작
                targetCounter.SetCashier(worker);

                // 손님/햄버거가 없다면 작업 취소
                yield return new WaitUntil(() => !ShouldDoMainCounterCashierJob(out Counter targetCounter));

                //RemoveJob
                RemoveJob(EJob.Cashier, targetCounter);
            }

            // 테이블 청소 업무 존재하는지 검사
            if (ShouldDoClearTrash(out Table targetTable))
            {
                // 1. Job 등록
                AddJob(EJob.ClearTrash, targetTable);

                // 2. 작업할 테이블로 이동
                worker.Controller.MoveTo(targetTable.TrashFrontPos);

                // 2-1. 이동 대기
                yield return new WaitUntil(() => !worker.Controller.IsMoving);

                // 3. 쓰레기 삭제
                targetTable.ClearTable(out var trash);

                if (trash != null)
                {
                    // 3. 테이블 쓰레기 줍기
                    worker.AddToTray(trash.transform);

                    // 4. 쓰레기통으로 이동
                    worker.Controller.MoveTo(trashCan.TrashCanFrontPos);

                    // 4-1. 이동 대기
                    yield return new WaitUntil(() => !worker.Controller.IsMoving);

                    // 5. 쓰레기통에 쓰레기 버리기
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

        // 도넛이 존재하는 머신이 있는지 검사
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

        // 캐셔 작업이 존재하는지 확인
        //foreach (var job in jobs)
        //{
        //    if (job is Counter counter)
        //    {
        //        if (!counter.HaveCashier)
        //        {
        //            targetCounter = counter;
        //            return true; // 캐셔가 없는 카운터가 존재할 경우
        //        }
        //    }
        //}

        if (!MainCounter.HaveCashier)
        {
            // 도넛이 없을 경우
            if (MainCounter.DonutCount == 0)
                return false;

            // 줄 선 손님이 없을 경우
            if (MainCounter.InLineCustomerCount == 0)
                return false;

            targetCounter = MainCounter;
            return true; // 캐셔가 없는 카운터가 존재할 경우
        }
        return false;
    }

    private bool ShouldDoClearTrash(out Table targetTable)
    {
        targetTable = null;

        var jobs = GetJobs(EJob.ClearTrash);
        if (jobs == null)
            return false;

        // 트레이가 비어있거나, 쓰레기인지 확인

        // 테이블 청소 작업이 있는지 확인
        foreach (var job in jobs)
        {
            if (job is Table table)
            {
                if (table.CheckHaveTrash())
                {
                    targetTable = table;
                    return true; // 테이블 작업이 있는 경우
                }
            }
        }
        return false;
    }

    #endregion

    #region Add/Remove

    // 일 등록
    private void AddJob(EJob eJob, PropBase propBase)
    {
        if (!_jobs.TryAdd(eJob, new List<PropBase>() { propBase }))
            _jobs[eJob].Add(propBase);
    }

    //일 삭제
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
        Workers.Add(worker);
    }

    public void AddMainCouter(Counter counter)
    {
        MainCounter = counter;
        AddJob(EJob.Cashier, counter);
    }

    public void AddMachine(Machine machine)
    {
        Machines.Add(machine);
        AddJob(EJob.CarryDonut, machine);
    }

    public void AddTable(Table table)
    {
        Tables.Add(table);
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
        foreach (var table in Tables)
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

        foreach (var table in Tables)
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
}
