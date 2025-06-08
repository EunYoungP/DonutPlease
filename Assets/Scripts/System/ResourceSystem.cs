using UnityEngine;

namespace DonutPlease.System
{
    public class ResourceSystem : MonoBehaviour
    {
        private GameObject Counter;
        private GameObject Machine;
        private GameObject Table;
        private GameObject TrashCan;
        private GameObject FrontDoor;
        private GameObject InteractionUI;

        public void Initialize()
        {
            Counter = Resources.Load<GameObject>("Prefabs/Counter");
            Machine = Resources.Load<GameObject>("Prefabs/DonutMachine");
            Table = Resources.Load<GameObject>("Prefabs/TableSet");
            TrashCan = Resources.Load<GameObject>("Prefabs/TrashCan");
            FrontDoor = Resources.Load<GameObject>("Prefabs/FrontDoor");
            InteractionUI = Resources.Load<GameObject>("Prefabs/UI/InteractionUI");
        }

        public GameObject GetPropByType(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.CreateCounter:
                    return Counter;
                case InteractionType.CreateMachine:
                    return Machine;
                case InteractionType.CreateTable:
                    return Table;
                case InteractionType.OpenFrontDoor:
                    return FrontDoor;
                case InteractionType.TrashCan:
                    return TrashCan;
                case InteractionType.CreateInteractionUI:
                    return InteractionUI;
                default:
                    Debug.LogWarning("Unknown interaction type: " + type);
                    return null;
            }
        }
    }
}
