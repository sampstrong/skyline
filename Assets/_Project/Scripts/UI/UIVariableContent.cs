using _Project.Scripts.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class UIVariableContent : MonoBehaviour
    {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public GameObject imageObject;

        private Image _currentImage;

        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            _currentImage = imageObject.GetComponent<Image>();
            SetContent();
        }
        
        private void SetContent()
        {
            titleText.text = ModelManager.Instance.currentModel.modelName;
            descriptionText.text = ModelManager.Instance.currentModel.modelDescription;
            _currentImage.sprite = ModelManager.Instance.currentModel.modelImage;
        }
    }
}
