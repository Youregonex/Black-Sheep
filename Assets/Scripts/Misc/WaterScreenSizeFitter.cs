using UnityEngine;

namespace Youregone.Utils
{
    public class WaterScreenSizeFitter : MonoBehaviour
    {
        [CustomHeader("Settings")]
        [SerializeField] private Camera _waterReflctionCamera;
        [SerializeField] private Transform _waterVisualTransform;

        private void Start()
        {
            float waterVisualDefaultAspectRatio = _waterVisualTransform.localScale.x / _waterVisualTransform.localScale.y;
            float cameraHeight = Camera.main.orthographicSize * 2;
            float worldScreenWidth = cameraHeight * Screen.width / Screen.height;
            float newHeight = worldScreenWidth / waterVisualDefaultAspectRatio;
            float scaleCoef = 100f;

            StretchSpriteDownOnly(_waterVisualTransform, newHeight * scaleCoef);

            _waterVisualTransform.localScale = new Vector2(
                worldScreenWidth * scaleCoef,
                newHeight * scaleCoef);


            float waterReflectionCameraBottomEdge = _waterReflctionCamera.transform.position.y - _waterReflctionCamera.orthographicSize;
            transform.position = new Vector2(_waterReflctionCamera.transform.position.x, waterReflectionCameraBottomEdge);
        }

        void StretchSpriteDownOnly(Transform stretchObj, float newScaleY)
        {
            float originalScaleY = stretchObj.localScale.y;
            float deltaScaleY = newScaleY - originalScaleY;

            SpriteRenderer sr = stretchObj.GetComponent<SpriteRenderer>();
            float spriteHeight = sr.bounds.size.y / originalScaleY;

            float spriteHeightCoef = -.5f;
            stretchObj.position += new Vector3(0f, spriteHeightCoef * spriteHeight * deltaScaleY, 0f);
        }
    }
}