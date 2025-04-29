using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileBase : MonoBehaviour
{
    [SerializeField]
    private int _row;

    [SerializeField]
    private int _col;

    [SerializeField]
    private Vector3 _size = new Vector3(0.5f, 0.1f, 0.5f);

    [SerializeField]
    private int _maxCount = 12;

    protected Stack<GameObject> _objects = new Stack<GameObject>();

    public bool IsEmpty => _objects.Count == 0;
    public bool IsFull => _objects.Count >= _maxCount;
    public int ObjectCount => _objects.Count;

    public void Initialize(int maxCount)
    {
        _maxCount = maxCount;
    }

    public void AddToPile(GameObject go)
    {
        go.transform.position = GetPositionAt(_objects.Count - 1);

        _objects.Push(go);
    }

    public GameObject RemoveFromPile()
    {
        if (IsEmpty)
            return null;

        return _objects.Pop();
    }

    public Vector3 GetPositionAt(int pileIndex)
    {
        Vector3 offset = new Vector3((_row - 1) * _size.x / 2, 0, (_col - 1) * _size.z / 2);
        Vector3 startPos = transform.position - offset;

        int row = (pileIndex / _row) % _col;
        int col = pileIndex %  _row;
        int height = pileIndex / (_row * _col);

        float x = startPos.x + col * _size.x;
        float y = startPos.y + height * _size.y;
        float z = startPos.z + row * _size.z;

        return new Vector3(x, y, z);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 offset = new Vector3((_row - 1) * _size.x / 2, 0, (_col - 1) * _size.z / 2);
        Vector3 startPos = transform.position - offset;

        Gizmos.color = Color.yellow;

        for (int r = 0; r < _row; r++)
        {
            for (int c = 0; c < _col; c++)
            {
                Vector3 center = startPos + new Vector3(r * _size.x, _size.y / 2, c * _size.z);
                Gizmos.DrawWireCube(center, _size);
            }
        }
    }

#endif
}
