using DG.Tweening.Core.Easing;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField] private List<Transform> _seatPositions;

    [SerializeField] private GameObject _trash;

    private List<KeyValuePair<int, Transform>> _seatList = new(); //0:empty seat, 1:not empty seat

    private void Awake()
    {
        foreach (var seatPos in _seatPositions)
        {
            _seatList.Add(new KeyValuePair<int, Transform>(0, seatPos));
        }
    }

    public void MakeTrash()
    {
        _trash.SetActive(true);
    }

    public void ClearTable()
    {
        _trash.SetActive(false);
    }

    public bool CheckHaveEmptySeat()
    {
        for (int i = 0; i < _seatList.Count; i++)
        {
            var seat = _seatList[i];
            int haveCustomer = seat.Key;
            Transform seatPos = seat.Value;

            if (haveCustomer == 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool GetEmptySeatPos(out Vector3 seatPosition)
    {
        seatPosition = Vector3.zero;
        for(int i = 0; i < _seatList.Count; i++)
        {
            var seat = _seatList[i];
            int haveCustomer = seat.Key;
            Vector3 seatPos = seat.Value.position;

            if (haveCustomer == 0)
            {
                seatPosition = seatPos;
                UpdateSeatEmptyState(i, 1);
                return true;
            }
        }
        return false;
    }

    private void UpdateSeatEmptyState(int seatIndex, int value)
    {
        _seatList[seatIndex] = new KeyValuePair<int, Transform>(value, _seatList[seatIndex].Value);
    }
}
