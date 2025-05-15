using DonutPlease.Game.Character;
using UnityEngine;

namespace DonutPlease.Game.Character
{
    public class CharacterWorker : CharacterBase
    {
        [SerializeField] private CharacterWorkerController _controller;

        public CharacterWorkerController Controller => _controller;
    }
}

