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
                { 1, () => OpenStore()}, // 예시: 튜토리얼 ID 1 실행

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

            // 캐릭터 움직임 제한

            // 문으로 카메라 이동
            Transform door = GameObject.Find("FrontDoor(Clone)").gameObject.transform.parent;
            yield return StartCoroutine(GameManager.GetGameManager.Player.Character.Camera.MoveToTarget(door));

            // 컨페티 생성

            // 아이콘 출력

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
                var connectProp = GameManager.GetGameManager.LocalMap.GetProp(nextId);
                yield return GameManager.GetGameManager.Player.Character.Camera.MoveToTarget(connectProp?.transform);
            }

            var player = GameManager.GetGameManager.Player.Character;
            yield return GameManager.GetGameManager.Player.Character.Camera.MoveToTarget(player.gameObject.transform);

            IsTutorial = false;
        }
    }
}

