using UniRx;
using System;
using DonutPlease.Game.Character;

public static class FluxSystem
{
    private static readonly Subject<object> _subject = new Subject<object>();
    public static IObservable<object> ActionStream => _subject;


    private static readonly Subject<(CharacterBase, EColliderIdentifier)> _colliderEnterSubject = new Subject<(CharacterBase, EColliderIdentifier)>();
    public static IObservable<(CharacterBase, EColliderIdentifier)> ColliderEnterActionStream => _colliderEnterSubject;


    private static readonly Subject<(CharacterBase, EColliderIdentifier)> _colliderExitSubject = new Subject<(CharacterBase, EColliderIdentifier)>();
    public static IObservable<(CharacterBase, EColliderIdentifier)> ColliderExitActionStream => _colliderExitSubject;

    public static void Dispatch(object action)
    {
        _subject.OnNext(action);
    }

    public static void DispatchColliderEnterEvent(CharacterBase target, EColliderIdentifier identifier)
    {
        _colliderEnterSubject.OnNext((target, identifier));
    }

    public static void DispatchColliderExitEvent(CharacterBase target, EColliderIdentifier identifier)
    {
        _colliderExitSubject.OnNext((target, identifier));
    }
}
