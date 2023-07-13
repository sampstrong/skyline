using System.Collections;
using System.Collections.Generic;
using Buck;
using UnityEngine;

public class ScanningPlane : MonoBehaviour
{
    [SerializeField] private InteractionManager _interactionManager;
    [SerializeField] private GeospatialManager _geospatialManager;

    [SerializeField] private Renderer _rend;

    private void Start()
    {
        _rend.enabled = false;
    }
    void Update()
    {
        if (!_geospatialManager.IsTracking) return;
        if (!_interactionManager.CurrentARPlane) return;

        _rend.enabled = true;
        transform.position = new Vector3(transform.position.x, _interactionManager.CurrentARPlane.transform.position.y, transform.position.z);
    }
}
