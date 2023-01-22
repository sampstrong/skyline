using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class UIScreen : MonoBehaviour
    {
        public enum UIType
        {
            Main = 0,
            Overlay = 1,
            Other = 2,
            Debug = 3
        }

        public UIType uiType;
        public List<UIElement> UIElements;
        public Animation animation; // do we need separate clips for animation in vs out?

        public void SetToDefault()
        {
            if (UIElements.Count <= 0) return;
            
            foreach (var element in UIElements)
            {
                if (element.isDefault) element.gameObject.SetActive(true);
                else element.gameObject.SetActive(false);
            }
        }
    }
}
