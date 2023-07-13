using System;
using UnityEngine;
using UnityEngine.UI;

public class Timeline : MonoBehaviour
{
    public int TimelineDate => _timelineDate;
    public event Action<int> onDateChanged; 

    [SerializeField] private Slider _timelineSlider;

    private int _timelineDate;
    private int _baseDate = 2023;
    private int _lastDate;

    private void Start()
    {
        SetDate(_timelineSlider.value);
        _timelineSlider.onValueChanged.AddListener(SetDate);
    }

    private void Update()
    {
        if (_timelineDate != _lastDate) onDateChanged?.Invoke(_timelineDate);
        _lastDate = _timelineDate;
    }

    private void SetDate(float sliderValue)
    {
        _timelineDate = Mathf.FloorToInt(_baseDate + (sliderValue * 9));
    }

    
    
    
}
