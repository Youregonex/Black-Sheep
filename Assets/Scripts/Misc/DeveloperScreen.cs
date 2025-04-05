using UnityEngine;
using UnityEngine.UI;
using Youregone.SaveSystem;
using System.Collections;

namespace Youregone.DeveloperTools
{
    public class DeveloperScreen : MonoBehaviour
    {
        [CustomHeader("Settings")]
        [SerializeField] private RectTransform _developerScreen;
        [SerializeField] private Button _deleteSaveFilesButton;

        private bool _screenOpened = false;

        private void Start()
        {
            _deleteSaveFilesButton.onClick.AddListener(() =>
            {
                JsonSaverLoader.DeleteScoreFileJson();
            });
        }

        private void Update()
        {
            if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Escape))
            {
                if (_screenOpened)
                    CloseDeveloperScreen();
                else
                    OpenDeveloperScreen();
            }
        }

        private void OpenDeveloperScreen()
        {
            _developerScreen.gameObject.SetActive(true);
            _screenOpened = true;
        }

        private void CloseDeveloperScreen()
        {
            _developerScreen.gameObject.SetActive(false);
            _screenOpened = false;
        }
    }
}