using DG.Tweening.Core.Easing;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : PropBase
{
    public struct Seat
    {
        public bool haveCustomer;
        public Transform seatPos;
        public Transform trashPos;
        public DonutPile donutPile;
    }

    [SerializeField] private GameObject _trashPrefab;
    [SerializeField] List<Transform> _trashPosition;
    [SerializeField] private List<Transform> _seatPositions;
    [SerializeField] private List<DonutPile> _donutPiles;

    [SerializeField] private Transform _trashFrontPosition;
    
    private Stack<GameObject> _trash = new();

    private List<Seat> _seats = new List<Seat>();

    public Transform TrashFrontPos => _trashFrontPosition;

    private void Awake()
    {
        for(int i = 0; i < _trashPosition.Count; i++)
        {
            _seats.Add(new Seat
            {
                haveCustomer = false,
                seatPos = _seatPositions[i],
                trashPos = _trashPosition[i],
                donutPile = _donutPiles[i]
            });
        }
    }

    public void MakeTrash(int seatIndex)
    {
        var seat = _seats[seatIndex];

        GameObject go = Instantiate(_trashPrefab, seat.trashPos.position, Quaternion.identity, seat.trashPos);
        AddTrash(go);
    }

    public void ClearTable(out GameObject trash)
    {
        RemoveTrash(out var popTrash);
        trash = popTrash;
    }

    public bool CheckHaveTrash()
    {
        return _trash.Count > 0;
    }

    public bool CheckHaveEmptySeat()
    {
        for (int i = 0; i < _seats.Count; i++)
        {
            var seat = _seats[i];

            bool haveCustomer = seat.haveCustomer;
            if (haveCustomer == false)
            {
                return true;
            }
        }
        return false;
    }

    public bool GetEmptySeatPos(out Vector3 seatPosition, out int seatIndex)
    {
        seatPosition = Vector3.zero;
        seatIndex = -1;

        for (int i = 0; i < _seats.Count; i++)
        {
            var seat = _seats[i];
            bool haveCustomer = seat.haveCustomer;
            Transform seatPos = seat.seatPos;

            if (haveCustomer == false)
            {
                seatPosition = seatPos.position;
                seatIndex = i;
                UpdateSeatEmptyState(i);
                return true;
            }
        }
        return false;
    }

    public void UpdateSeatEmptyState(int seatIndex)
    {
        var s = _seats[seatIndex];
        s.haveCustomer = !_seats[seatIndex].haveCustomer;
        _seats[seatIndex] = s;
    }


    private void AddTrash(GameObject trash)
    {
        _trash.Push(trash);
    }

    private void RemoveTrash(out GameObject trash)
    {
        _trash.TryPop(out GameObject popTrash);
        trash = popTrash;
    }
}
