using System.Collections;
using System.Collections.Generic;
using Niantic.Platform.Debugging;
using UnityEngine;

public class OcclusionModel : MonoBehaviour
{
    public List<Renderer> OcclusionRenderers => _occlusionRenderers;
    public Material CurrentMaterial => _currentMaterial;
    
    [SerializeField] private List<Renderer> _occlusionRenderers;

    private Material _currentMaterial;
    [HideInInspector] public Material startingMat;

    private void Start()
    {
        Assert.IsNotNull(_occlusionRenderers[0], "The list of renderers for the Occlusion Model is empty. " +
                                                         "Please assign renderers in the inspector");
        _currentMaterial = _occlusionRenderers[0].material;
        startingMat = _currentMaterial;
    }
    
    public void SetOcclusionMaterial(Material mat)
    {
        Debug.Log("Set Occlusion Material Triggered");
        
        foreach (var rend in _occlusionRenderers)
        {
            rend.material = mat;
        }

        _currentMaterial = mat;
        Debug.Log($"Occlusion Model material changed to: {mat.name}");
    }
}
