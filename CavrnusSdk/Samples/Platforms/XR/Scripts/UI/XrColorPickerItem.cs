using System;
using UnityEngine;
using UnityEngine.UI;

namespace CavrnusSdk.XR.UI
{
    public class XrColorPickerItem : MonoBehaviour
    {
        [SerializeField] private Image image;

        private Color color;
        private Action<Color> onSelected;

        public void Setup(Color color, Action<Color> onSelected)
        {
            this.color = color;
            this.onSelected = onSelected;

            image.color = color;
        }

        public void Select()
        {
            onSelected?.Invoke(color);
        }
    }
}