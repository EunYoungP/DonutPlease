using UniRx;
using System;
using DonutPlease.Game.Character;

public static class FluxSystem
{
    private static readonly Subject<object> _subject = new Subject<object>();
    public static IObservable<object> ActionStream => _subject;


    private static readonly Subject<(CharacterBase, EColliderIdentifier)> _colliderSubject = new Subject<(CharacterBase, EColliderIdentifier)>();
    public static IObservable<(CharacterBase, EColliderIdentifier)> ColliderActionStream => _colliderSubject;


    public static void Dispatch(object action)
    {
        _subject.OnNext(action);
    }

    public static void DispatchColliderEvent(CharacterBase target, EColliderIdentifier identifier)
    {
        _colliderSubject.OnNext((target, identifier));
    }
}
