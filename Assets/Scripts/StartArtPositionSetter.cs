using UnityEngine;

namespace Youregone.Utils
{
    public class StartArtPositionSetter : MonoBehaviour
    {
        private void Awake()
        {
            float cameraHeight = Camera.main.orthographicSize;
            float cameraWidth = cameraHeight * Camera.main.aspect;

            float leftEdge = Camera.main.transform.position.x - cameraWidth;
            transform.position = new Vector2(leftEdge, transform.position.y);
        }
    }
}