using UnityEngine;

namespace DonutPlease.Game.Character
{
    public class CharacterCustomer : CharacterBase
    {
        [SerializeField] private CharacterCustomerController _controller;

        public CharacterCustomerController Controller => _controller;
    }
}

