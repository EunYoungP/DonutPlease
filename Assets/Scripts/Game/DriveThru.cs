using UnityEngine;

public class DriveThru : MonoBehaviour
{
    [SerializeField] private DriveThruCounter Counter;

    private void OnEnable()
    {
        Counter.Initialize();
    }
}
