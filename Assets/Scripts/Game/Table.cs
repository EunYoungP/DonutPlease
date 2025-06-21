using DonutPlease.Game.Character;
using System.Collections.Generic;
using UniRx;
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
    
    private List<GameObject> _trash = new List<GameObject>();

    private List<Seat> _seats = new List<Seat>();

    public Transform TrashFrontPos => _trashFrontPosition;

    private void Awake()
    {
        FluxSystem.ColliderTriggerActionStream.Subscribe(data =>
        {
            if (data is FxOnTriggerEnter fxTriggerEnter)
            {
                OnTriggerEnterAction(fxTriggerEnter.characterBase, fxTriggerEnter.colliderType);
            }

        }).AddTo(this);


        for (int i = 0; i < _trashPosition.Count; i++)
        {
            _seats.Add(new Seat
            {
                haveCustomer = false,
                seatPos = _seatPositions[i],
                trashPos = _trashPosition[i],
                donutPile = _donutPiles[i]
            });

            _trash.Add(null);
        }
    }

    private void OnTriggerEnterAction(CharacterBase character, EColliderIdentifier identifier)
    {
        if (identifier == EColliderIdentifier.GetTrash)
        {
            ClearTable(out GameObject trash);
            if (trash != null)
            {
                if (character is CharacterPlayer player)
                {
                    player.AddToTray(trash.transform);
                }
            }
        }
    }

    public void MakeTrash(int seatIndex)
    {
        var seat = _seats[seatIndex];

        GameObject go = Instantiate(_trashPrefab, seat.trashPos.position, Quaternion.identity, seat.trashPos);
        AddTrash(seatIndex, go);
    }

    public void ClearTable(out GameObject trash)
    {
        RemoveTrash(out int seatIdnex, out var popTrash);
        trash = popTrash;
    }

    public bool CheckHaveTrash()
    {
        foreach(var t in _trash)
        {
            if (t != null)
                return true;
        }
        return false;
    }

    public bool CheckHaveEmptySeat()
    {
        for (int i = 0; i < _seats.Count; i++)
        {
            var seat = _seats[i];

            bool haveCustomer = seat.haveCustomer;
            if (haveCustomer == false)
            {
                if (CheckClearTable(i))
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
                if (!CheckClearTable(i))
                    continue;

                seatPosition = seatPos.position;
                seatIndex = i;
                UpdateSeatEmptyState(i);
                return true;
            }
        }
        return false;
    }

    private bool CheckClearTable(int trashIndex)
    {
        return _trash[trashIndex] == null;
    }

    public void UpdateSeatEmptyState(int seatIndex)
    {
        var s = _seats[seatIndex];
        s.haveCustomer = !_seats[seatIndex].haveCustomer;
        _seats[seatIndex] = s;
    }

    public Seat GetSeatByIndex(int index)
    {
        return _seats[index];
    }


    private void AddTrash(int seatIndex, GameObject trash)
    {
        _trash[seatIndex] = trash;
    }

    private void RemoveTrash(out int trashIndex, out GameObject trash)
    {
        for(int i = 0; i < _trash.Count; i++)
        {
            if (_trash[i] != null)
            {
                trashIndex = i;
                trash = _trash[i];
                _trash[i] = null;
                return;
            }
        }

        trashIndex = -1;
        trash = null;
    }
}
