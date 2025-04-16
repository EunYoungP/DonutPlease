using UniRx;
using System;

public static class FluxSystem
{
    private static readonly Subject<object> _subject = new Subject<object>();
    public static IObservable<object> ActionStream => _subject;

    public static void Dispatch(object action)
    {
        _subject.OnNext(action);
    }
}
