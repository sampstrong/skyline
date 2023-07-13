using System.Collections;
using System.Collections.Generic;
using Buck;
using UnityEngine;

public class BaseFacingObject : MonoBehaviour
{
    [SerializeField] private bool _whilePlacingOnly;
    private bool _rotationDisabled;
    
    protected Camera _mainCamera;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _mainCamera = FindObjectOfType<Camera>();
        PlaceablesManager.Instance.ObjectPlaced.AddListener(DisableRotation);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_rotationDisabled) return;
        RotateTowardsCamera();
    }
    
    protected virtual void RotateTowardsCamera()
    {
        //if (!_mainCamera.gameObject.activeSelf) return; 
        
        Vector3 direction = (_mainCamera.transform.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
    
    /// <summary>
    /// Allows Object to face user while placing, but remain at the orientation it's placed
    /// once placement is finalized
    /// </summary>
    /// <param name="arg0"></param>
    private void DisableRotation(GameObject arg0)
    {
        if (_whilePlacingOnly) _rotationDisabled = true;
        PlaceablesManager.Instance.ObjectPlaced.RemoveListener(DisableRotation);
    }
    
    
}
