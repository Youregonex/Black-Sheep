using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Youregone.GameSystems;
using Youregone.SL;

namespace Youregone.UI
{
    public class OutroScenePlayer : MonoBehaviour
    {
        [CustomHeader("Settings")]
        [SerializeField] private float _outroTransitionDuration;
        [SerializeField] private List<OutroScene> _outroScenesList;
        [SerializeField] private WaterSplash _waterSplashPrefab;
        [SerializeField] private RectTransform _waterSplashSpawnPositionRectTransform;

        private Transition _transition;
        private SoundManager _soundManager;

        private void Start()
        {
            _transition = ServiceLocator.Get<Transition>();
            _soundManager = ServiceLocator.Get<SoundManager>();
        }

        public IEnumerator PlayOutroCoroutine()
        {
            yield return _transition.StartCoroutine(_transition.PlayTransitionStart());

            int transitionIndex = 0;

            foreach (OutroScene outroScene in _outroScenesList)
            {
                OutroScene currentOutroScene = Instantiate(outroScene, transform.position, Quaternion.identity, transform);
                currentOutroScene.transform.localScale = Vector3.one;

                //RectTransform rectTransform = currentOutroScene.GetComponent<RectTransform>();

                //if (rectTransform != null)
                //{
                //    rectTransform.anchorMin = Vector2.zero;
                //    rectTransform.anchorMax = Vector2.one;
                //    rectTransform.offsetMin = Vector2.zero;
                //    rectTransform.offsetMax = Vector2.zero;
                //}

                yield return _transition.StartCoroutine(_transition.PlayTransitionEnd());

                if(transitionIndex == _outroScenesList.Count - 1)
                {
                    Debug.Log("Splash");
                    _soundManager.PlayPlayerDrownClip(Vector3.zero);
                    Instantiate(_waterSplashPrefab, _waterSplashSpawnPositionRectTransform.position, Quaternion.identity, transform);
                }

                yield return StartCoroutine(currentOutroScene.ShowTextCoroutine());

                yield return new WaitUntil(() => Input.anyKeyDown);

                yield return _transition.StartCoroutine(_transition.PlayTransitionStart());

                transitionIndex++;
                Destroy(currentOutroScene.gameObject);
            }

            yield return _transition.StartCoroutine(_transition.PlayTransitionEnd());
        }
    }
}