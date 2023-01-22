using System.Collections;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class AnimationManager : MonoBehaviour
    {
        [SerializeField] private GameObject _baseUI;
        [SerializeField] private GameObject _reminderUI;
        [SerializeField] private GameObject _tutorialAnimation;

        private float _timer = 0;
        private float _videoLength = 30f;
        private bool _videoOver;
        private bool _videoPlayed;
    
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= _videoLength)
            {
                _videoOver = true;
            }

            if (_videoOver && !_videoPlayed)
            {
                StartCoroutine(FadeIn(_baseUI.GetComponent<CanvasGroup>(), _baseUI));
                StartCoroutine(FadeIn(_reminderUI.GetComponent<CanvasGroup>(), _reminderUI));
                StartCoroutine(FadeOut(_tutorialAnimation.GetComponent<CanvasGroup>(), _tutorialAnimation));
            
                _videoPlayed = true;
            }
        }

        private IEnumerator FadeIn(CanvasGroup canvas, GameObject obj)
        {
            obj.SetActive(true);
        
            float duration = 1;
        
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                canvas.alpha = Mathf.Lerp(0, 1, t / duration);
            
                yield return null;
            }
        }

        private IEnumerator FadeOut(CanvasGroup canvas, GameObject obj)
        {
            float duration = 1;
        
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                canvas.alpha = Mathf.Lerp(1, 0, t / duration);
            
                yield return null;
            }
        
            obj.SetActive(false);
        }
    }
}
