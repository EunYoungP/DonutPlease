using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace DonutPlease.Game.Character
{
    public class CharacterBase : MonoBehaviour
    {
        [SerializeField] private int _id;

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
                if (data is OnGetItem getDonut)
                {
                    if (getDonut.character is CharacterPlayer player)
                    {
                        player.AddToTray(getDonut.item.transform);
                    }
                    else if (getDonut.character is CharacterCustomer customer)
                    {
                        customer.AddToTray(getDonut.item.transform);
                    }
                    else if (getDonut.character is CharacterWorker worker)
                    {
                        worker.AddToTray(getDonut.item.transform);
                    }
                }
            })
            .AddTo(this);

            FluxSystem.ActionStream
            .Subscribe(data =>
            {
                if (data is OnPutDownItemToPile putDownDonut)
                {
                    if (putDownDonut.character is CharacterPlayer player)
                    {
                        player.RemoveFromTray(EItemType.Donut,  putDownDonut.pile);
                    }
                    else if (putDownDonut.character is CharacterCustomer customer)
                    {
                        customer.RemoveFromTray(EItemType.Donut, putDownDonut.pile);
                    }
                    else if (putDownDonut.character is CharacterWorker worker)
                    {
                        worker.RemoveFromTray(EItemType.Donut, putDownDonut.pile);
                    }
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


