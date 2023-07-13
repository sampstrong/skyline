using System.Collections;
using System.Collections.Generic;
using Buck;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private void Awake()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("OrthographicMap"))
        {
            DestroyOnLoad();
        }
    }

    private void DestroyOnLoad()
    {
        Debug.Log("DestroyOnLoad Invoked");
        
        if (PlaceablesManager.Instance != null) Destroy(PlaceablesManager.Instance);
        if (GeospatialManager.Instance != null) Destroy(GeospatialManager.Instance);
        if (InteractionManager.Instance != null) Destroy(InteractionManager.Instance);
            
        Debug.Log("AR Core Managers Destroyed");
        
        
    }

    public void LoadSceneByIndex(int index)
    {
        var operation = SceneManager.LoadSceneAsync(index);
    }
    
    
}
