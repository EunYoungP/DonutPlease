using System.Collections.Generic;
using UnityEngine;

namespace DonutPlease.Game.Character
{
    public class CharacterBase : MonoBehaviour
    {
        [SerializeField] private int _id;

        private HashSet<int> _idSet = new HashSet<int>();

        private void Awake()
        {
            while(true)
            {
                _id = Random.Range(0, 10000);

                if (_idSet.Contains(_id))
                {
                    continue;
                }
                else
                {
                    _idSet.Add(_id);
                    break;
                }
            }
        }
    }
}


