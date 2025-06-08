using UniRx;
using UnityEngine;

public class ComponentBase
{
    protected readonly CompositeDisposable Disposables = new CompositeDisposable();

    private void OnDestroy()
    {
        Disposables.Dispose();
    }
}
