using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _boxObject;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _verticalDisplacement = 0.5f;
    [SerializeField] private float _displacementMultiplier = 0.01f;
    
    private float _startPosY;


    private void Start()
    {
        _startPosY = _boxObject.transform.position.y;
    }
    
    void FixedUpdate()
    {
        _boxObject.transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime);

        /*
        _boxObject.transform.position = new Vector3(_boxObject.transform.position.x, 
                                                  _startPosY + Mathf.Sin(Time.time * _displacementMultiplier) * _verticalDisplacement, 
                                                    _boxObject.transform.position.z);
        */
    }
}
