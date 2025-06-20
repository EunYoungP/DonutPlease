using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace DonutPlease.System
{
    public class TutorialSystem : MonoBehaviour
    {
        private Dictionary<int, Func<IEnumerator>> _tutlrialDatas;

        public bool IsTutorial { get; private set; } = false;

        private void Awake()
        {
            _tutlrialDatas = new Dictionary<int, Func<IEnumerator>>()
            {
                { 0, () => OpenStore()},
                { 1, () => OpenHR()},
                { 2, () => OpenUpgrade()},
                { 3, () => OpenDriveThru()},
            };

            FluxSystem.ActionStream.Subscribe(data =>
            {
                if (data is FxOnCompleteUIInteraction action)
                {
                    OnCompleteUIInteraction(action.interactionId);
                }
            });
        }

        // 특정 컨텐츠 해금 되었을 경우 튜토리얼 실행
        public void StartTutorial(int id)
        {
            if (_tutlrialDatas.TryGetValue(id, out Func<IEnumerator> tutorialAction))
            {
                StartCoroutine(tutorialAction());
            }
        }

        private IEnumerator OpenStore()
        {
            IsTutorial = true;

            Transform officeHR = GameManager.GetGameManager.LocalMap.OfficeHRProps.transform;
            yield return StartCoroutine(GameManager.GetGameManager.Player.Character.Camera.MoveToTarget(officeHR));

            // 컨페티 생성

            // 아이콘 출력

            IsTutorial = false;
        }

        private IEnumerator OpenHR()
        {
            IsTutorial = true;

            Transform HROffice = GameManager.GetGameManager.LocalMap.OfficeHRProps.transform;
            yield return StartCoroutine(GameManager.GetGameManager.Player.Character.Camera.MoveToTarget(HROffice));

            IsTutorial = false;
        }

        private IEnumerator OpenUpgrade()
        {
            IsTutorial = true;

            Transform upgradeOffice = GameManager.GetGameManager.LocalMap.OfficeUpgradeProps.transform;
            yield return StartCoroutine(GameManager.GetGameManager.Player.Character.Camera.MoveToTarget(upgradeOffice));

            IsTutorial = false;
        }

        private IEnumerator OpenDriveThru()
        {
            IsTutorial = true;

            Transform driveThru = GameManager.GetGameManager.Store.GetStore(1).DriveThru.transform;
            yield return StartCoroutine(GameManager.GetGameManager.Player.Character.Camera.MoveToTarget(driveThru));

            IsTutorial = false;
        }


        private void OnCompleteUIInteraction(int id)
        {
            StartCoroutine(CoOnCompleteCameraMove(id));
        }

        private IEnumerator CoOnCompleteCameraMove(int id)
        {
            IsTutorial = true;

            var propData = GameManager.GetGameManager.LocalMap.GetInteractionPropData(id);
            foreach (int nextId in propData.NextIds)
            {
                Transform nextPorpTransform;
                var nextPropData = GameManager.GetGameManager.LocalMap.GetInteractionPropData(id);
                if (nextPropData.Type == InteractionType.OpenHR)
                {
                    nextPorpTransform = GameManager.GetGameManager.LocalMap.OfficeHRProps.transform;
                }
                else if (nextPropData.Type == InteractionType.OpenUpgrade)
                {
                    nextPorpTransform = GameManager.GetGameManager.LocalMap.OfficeUpgradeProps.transform;
                }
                else
                {
                    nextPorpTransform = GameManager.GetGameManager.LocalMap.GetProp(nextId).transform;
                }

                yield return GameManager.GetGameManager.Player.Character.Camera.MoveToTarget(nextPorpTransform);
            }

            var player = GameManager.GetGameManager.Player.Character;
            yield return GameManager.GetGameManager.Player.Character.Camera.MoveToTarget(player.gameObject.transform);

            IsTutorial = false;
        }
    }
}

