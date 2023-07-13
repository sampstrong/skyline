using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;

public class AirMessage : BaseFacingObject
{
    [SerializeField] private TextMeshProUGUI _textField;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private bool _popInOn = true;
    
    private float _distanceThreshold = 3.5f;
    private Renderer[] _renderers;
    private Canvas _canvas;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        _renderers = GetComponentsInChildren<Renderer>();
        _canvas = GetComponentInChildren<Canvas>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
        ControlVisibility();
    }

    
    private void ControlVisibility()
    {
        float distance = Vector3.Distance(_mainCamera.transform.position, transform.position);

        if (_popInOn)
        {
            if (distance < _distanceThreshold)
            {
                ToggleVisibility(true);
            }
            else
            {
                ToggleVisibility(false);
                // play pop in animation
            }
        }
        else
        {
            ToggleVisibility(true);
        }
    }
    

    public void ToggleVisibility(bool visibility)
    {
        foreach (var rend in _renderers)
        {
            rend.enabled = visibility;
        }

        _canvas.enabled = visibility;
    }

    public void UpdateText(string text)
    {
        StartCoroutine(FadeOutFadeIn(text));
    }

    private IEnumerator FadeOutFadeIn(string text)
    {
        float duration = 1f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / duration);

            _canvasGroup.alpha = alpha;
            
            yield return null;
        }

        _canvasGroup.alpha = 0;
        _textField.text = text;
        
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0 ,1, t / duration);

            _canvasGroup.alpha = alpha;
            
            yield return null;
        }
        
        _canvasGroup.alpha = 1;

    }
}
