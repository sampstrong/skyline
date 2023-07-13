using PlanetX.Map;
using UnityEngine;

public class RenderTextureRaycast : MonoBehaviour
{
    [SerializeField] protected Camera UICamera;
    [SerializeField] protected RectTransform RawImageRectTrans;
    [SerializeField] protected Camera RenderToTextureCamera;


    private void Update()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                RegisterTouch(touch);
            }
        }
    }
    
    public void RegisterTouch(Touch touch)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(RawImageRectTrans, touch.position, UICamera, out localPoint);
        Vector2 normalizedPoint = Rect.PointToNormalized(RawImageRectTrans.rect, localPoint);
        var renderRay = RenderToTextureCamera.ViewportPointToRay(normalizedPoint);
        if (Physics.Raycast(renderRay, out var raycastHit))
        {
            Debug.Log("Hit: " + raycastHit.collider.gameObject.name);
            if (!raycastHit.transform.gameObject.TryGetComponent(out FutureBuildingMapObject mapObject)) return;
            Debug.Log($"Map Object Found: {mapObject.Building.Name}");
            mapObject.ToggleSelected();
        }
        else
        {
            Debug.Log("No Object Hit");
        }
    }
}
