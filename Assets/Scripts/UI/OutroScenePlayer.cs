using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Youregone.SL;

namespace Youregone.UI
{
    public class OutroScenePlayer : MonoBehaviour
    {
        [CustomHeader("Settings")]
        [SerializeField] private float _outroTransitionDuration;
        [SerializeField] private List<OutroScene> _outroScenes;

        private Transition _transition;
        private void Start()
        {
            _transition = ServiceLocator.Get<Transition>();
        }

        public IEnumerator PlayOutroCoroutine()
        {
            yield return _transition.StartCoroutine(_transition.PlayTransitionStart());

            foreach (OutroScene outroScene in _outroScenes)
            {
                OutroScene currentOutroScene = Instantiate(outroScene);
                currentOutroScene.transform.SetParent(transform);

                RectTransform rectTransform = currentOutroScene.GetComponent<RectTransform>();

                if (rectTransform != null)
                {
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                }

                yield return _transition.StartCoroutine(_transition.PlayTransitionEnd());

                yield return StartCoroutine(currentOutroScene.ShowTextCoroutine());

                yield return new WaitUntil(() => Input.anyKeyDown);

                yield return _transition.StartCoroutine(_transition.PlayTransitionStart());

                Destroy(currentOutroScene.gameObject);
            }

            yield return _transition.StartCoroutine(_transition.PlayTransitionEnd());
        }
    }
}