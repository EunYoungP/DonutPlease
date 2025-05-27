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

    // 도넛머신
}

public struct EJobData
{
    // 도넛 옮기기 -> 머신 개수
    // 계산 하기 -> 카운터 개수
    // 테이블 청소하기 -> 테이블 개수
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
        // 워커 생성
        GameObject workerObj = Instantiate(WorkerPrefab, WorkerStartPos.position, Quaternion.identity);
        worker = workerObj.GetComponent<CharacterWorker>();
        worker.transform.SetParent(transform);

        AddWorker(worker);

        // 워커 작업 시작
        StartCoroutine(CoWorkerDo(worker));
    }

    private IEnumerator CoWorkerDo(CharacterWorker worker)
    {
        // 도넛 운반 가능한지 검사
        if (ShouldDoCarryDonut(out Machine targetMachine))
        {
            DonutPile donutPile = targetMachine.DonutPile;

            // 1. Job 등록
            AddJob(EJob.CarryDonut, null);

            // 2. 작업할 도넛 머신으로 이동
            worker.Controller.MoveTo(targetMachine.transform);

            // 2-1. 이동 대기
            yield return new WaitUntil(() => !worker.Controller.IsMoving);

            // 3. 이동 중 도넛이 없어지면 이동 취소


            // 4. 머신 -> 도넛 가져오기
            donutPile.GetDonutFromPileByCount(worker, targetMachine.DonutCount);

            // 4-1. 도넛 받기 대기
            yield return new WaitUntil(() => !donutPile.IsWorkingAI);

            // 5. 카운터로 이동
            worker.Controller.MoveTo(MainCounter.DonutPileFrontPosition);

            // 5-1. 이동 대기
            yield return new WaitUntil(() => !worker.Controller.IsMoving);

            // 6. 카운터에 도넛 놓기
            donutPile.LoopMoveDonutToPile(worker);

            //RemoveJob
        }

        // 캐셔 업무 존재하는지 검사
        if (ShouldDoMainCounterCashierJob(out Counter targetCounter))
        {
            // 1. Job 등록
            AddJob(EJob.Cashier, null);

            // 2. 작업할 카운터로 이동
            // 3. 캐셔 동작

            // 손님/햄버거가 없다면 작업 취소
            //RemoveJob
        }

        // 테이블 청소 업무 존재하는지 검사
        if (ShouldDoClearTrash(out Table targetTable))
        {
            // 1. Job 등록
            AddJob(EJob.ClearTrash, null);

            // 2. 작업할 테이블로 이동
            // 3. 테이블 쓰레기 줍기
            // 4. 쓰레기통으로 이동
            // 5. 쓰레기통에 쓰레기 버리기

            //RemoveJob
        }

        yield return null;
    }

    private bool ShouldDoCarryDonut(out Machine targetMachine)
    {
        targetMachine = null;

        var jobs = GetJobs(EJob.CarryDonut);

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
        var jobs = GetJobs(EJob.CarryDonut);

        // 캐셔 작업이 존재하는지 확인
        foreach (var job in jobs)
        {
            if (job is Counter counter)
            {
                if (!counter.HaveCashier)
                {
                    targetCounter = counter;
                    return true; // 캐셔가 없는 카운터가 존재할 경우
                }
            }
        }
        targetCounter = null;
        return false;
    }

    private bool ShouldDoClearTrash(out Table targetTable)
    {
        var jobs = GetJobs(EJob.ClearTrash);

        // 테이블 청소 작업이 있는지 확인
        foreach (var job in jobs)
        {
            if (job is Table table)
            {
                if (!table.CheckHaveTrash())
                {
                    targetTable = table;
                    return true; // 테이블 작업이 있는 경우
                }
            }
        }
        targetTable = null;
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
