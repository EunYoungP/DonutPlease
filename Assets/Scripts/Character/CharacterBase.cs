using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace DonutPlease.Game.Character
{
    public class CharacterBase : MonoBehaviour
    {
        [SerializeField] private int _id;
        [SerializeField] protected UIFollowCharacterBase _uiFollowCharacter;

        protected GameManager GameMng { get; private set; }

        private HashSet<int> _idSet = new HashSet<int>();

        private void Awake()
        {
            GameMng = GameManager.GetGameManager;

            while (true)
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

            FluxSystem.ActionStream
            .Subscribe(data =>
            {
                if (data is FxOnGetItem getDonut)
                {
                    getDonut.character.AddToTray(getDonut.item.transform);
                }
            })
            .AddTo(this);

            FluxSystem.ActionStream
            .Subscribe(data =>
            {
                if (data is FxOnPutDownItemToPile putDownDonut)
                {
                    putDownDonut.character.RemoveFromTray(EItemType.Donut, putDownDonut.pile);
                }
            })      
            .AddTo(this);
        }

        public virtual void AddToTray(Transform child)
        {
        }

        public virtual void RemoveFromTray(EItemType itemType, PileBase pile)
        {
        }

        public virtual void RemoveFromTray(EItemType itemType, Transform trashDropPos)
        {
        }
    }
}


