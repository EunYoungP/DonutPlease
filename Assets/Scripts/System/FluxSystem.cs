using System;
using UniRx;

public static class FluxSystem
{
    private static readonly Subject<object> _subject = new Subject<object>();
    public static IObservable<object> ActionStream => _subject;


    private static readonly Subject<IFluxAction> _colliderTriggerActionSubject = new Subject<IFluxAction>();
    public static IObservable<IFluxAction> ColliderTriggerActionStream => _colliderTriggerActionSubject;


    public static void Dispatch(object action)
    {
        _subject.OnNext(action);
    }

    public static void DispatchColliderTriggerEvent(IFluxAction fluxAction)
    {
        _colliderTriggerActionSubject.OnNext(fluxAction);
    }
}
