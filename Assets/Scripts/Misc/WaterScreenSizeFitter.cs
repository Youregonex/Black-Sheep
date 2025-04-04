using UnityEngine;

namespace Youregone.Utils
{
    public class WaterScreenSizeFitter : MonoBehaviour
    {
        [CustomHeader("Settings")]
        [SerializeField] private Camera _waterReflctionCamera;

        private void Awake()
        {
            float worldScreenWidth = Camera.main.orthographicSize * 2 * Screen.width / Screen.height;
            float parameter = 100f;
            transform.localScale = new Vector3(worldScreenWidth * parameter, transform.localScale.y, transform.localScale.z);
        }
    }
}