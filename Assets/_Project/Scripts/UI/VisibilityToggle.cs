using UnityEngine;

public class VisibilityToggle : MonoBehaviour
{
    public void ToggleVisibility(CanvasGroup canvasGroup)
    {
        if (canvasGroup.alpha == 0) canvasGroup.alpha = 1;
        else if (canvasGroup.alpha == 1) canvasGroup.alpha = 0;
    }
}
