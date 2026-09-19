using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.UI
{
    public static class UIToolkitExtensions
    {
        /// <summary>
        /// Invokes the callback once this element's layout has been resolved.
        /// Fires immediately if layout is already computed (e.g. runtime-spawned
        /// elements attached after their first pass), otherwise waits for the
        /// next GeometryChangedEvent.
        /// </summary>
        public static void WaitUntilLayoutReady(this VisualElement element, Action onReady)
        {
            if (!float.IsNaN(element.layout.width))
            {
                onReady();
            }
            else
            {
                // RegisterCallbackOnce handles auto unregister once event is fired
                element.RegisterCallbackOnce<GeometryChangedEvent>(_ => onReady());
            }
        }
            
        public static Color32 GetAverageColor(this Color32[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                return new Color32(0, 0, 0, 0);
            }
    
            long r = 0, g = 0, b = 0, a = 0; // Use long to prevent integer overflow
            int total = colors.Length;
    
            for (int i = 0; i < total; i++)
            {
                r += colors[i].r;
                g += colors[i].g;
                b += colors[i].b;
                a += colors[i].a;
            }
    
            return new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), (byte)(a / total));
        }
    
        public static Color32 GetBrightVersion(this Color32 color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            Color brightColor = Color.HSVToRGB(h, s, 1f); // v forced to max
    
            return brightColor;
        }
    }
}