using UniRx;
using System;
using DonutPlease.Game.Character;

public static class FluxSystem
{
    private static readonly Subject<(IntercationData, object)> _subject = new Subject<(IntercationData,  object)>();
    public static IObservable<(IntercationData,  object)> ActionStream => _subject;


    private static readonly Subject<(CharacterBase, EColliderIdentifier)> _colliderSubject = new Subject<(CharacterBase, EColliderIdentifier)>();
    public static IObservable<(CharacterBase, EColliderIdentifier)> ColliderActionStream => _colliderSubject;

    public static void Dispatch(IntercationData d, object action)
    {
        _subject.OnNext((d, action));
    }

    public static void DispatchColliderEvent(CharacterBase target, EColliderIdentifier identifier)
    {
        _colliderSubject.OnNext((target, identifier));
    }
}
