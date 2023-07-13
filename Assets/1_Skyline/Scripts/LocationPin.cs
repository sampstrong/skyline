using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class LocationPin : MonoBehaviour
{
    [SerializeField] private Animation _animation;
    
    public void MoveToPosition(Vector3 position)
    {
        Debug.Log($"Location Pin moved to new position: {position}");
        gameObject.transform.position = position;
        AnimateIn();
    }

    private void AnimateIn()
    {
        _animation.Play();
    }

    private void AnimateOut()
    {
        
    }
}
