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
                if (data is OnGetDonut getDonut)
                {
                    if (getDonut.character is CharacterPlayer player)
                    {
                        player.AddToTray(getDonut.donut.transform);
                    }
                    else if (getDonut.character is CharacterCustomer customer)
                    {
                        customer.AddToTray(getDonut.donut.transform);
                    }
                    else if (getDonut.character is CharacterWorker worker)
                    {
                        worker.AddToTray(getDonut.donut.transform);
                    }
                }
            })
            .AddTo(this);

            FluxSystem.ActionStream
            .Subscribe(data =>
            {
                if (data is OnPutDownDonut putDownDonut)
                {
                    RemoveFromTray(putDownDonut.character, putDownDonut.pile);
                }
            })
            .AddTo(this);
        }

        protected virtual void AddToTray(Transform child)
        {
        }

        protected virtual void RemoveFromTray(CharacterBase character, PileBase pile)
        {
        }
    }
}


