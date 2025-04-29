using DonutPlease.Game.Character;
using UnityEngine;

public class DonutPile : PileBase
{
    [SerializeField] private GameObject _prefab;

    public void AddToPile(CharacterBase target)
    {
        GameObject go = Instantiate(_prefab, transform.position, Quaternion.identity);
        base.AddToPile(go);

        if (target is CharacterCustomer customer)
        {
            //customer.SetDonut(go);
        }

        if (target is CharacterPlayer player)
        {
            player.PlayerStock.AddDonut(1);
        }
    }
}
