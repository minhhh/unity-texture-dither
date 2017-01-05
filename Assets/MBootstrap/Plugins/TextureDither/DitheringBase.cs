using UnityEngine;
using System.Collections;

namespace MBootStrap
{
    // This Delegate is used to find best suitable color from limited set of colors
    public delegate Color FindColor (Color original);

    public abstract class DitheringBase
    {
        protected FindColor colorFunction = null;

        protected int width;
        protected int height;
        private Color[] pixels = null;

        public DitheringBase (FindColor colorfunc)
        {
            this.colorFunction = colorfunc;
        }

        //
        public Color[] DoDithering (Color[] pixels, int texw, int texh)
        {
            this.pixels = pixels;
            width = texw;
            height = texh;
            Color originalPixel = Color.white; // Default value isn't used
            Color newPixel = Color.white; // Default value isn't used
            float[] quantError = null; // Default values aren't used

            for (int y = 0; y < texh; y++) {
                for (int x = 0; x < texw; x++) {
                    originalPixel = this.pixels [GetIndexWith (x, y)];
                    newPixel = this.colorFunction (originalPixel);

                    this.pixels [GetIndexWith (x, y)] = newPixel;

                    quantError = GetQuantError (originalPixel, newPixel);
                    this.PushError (x, y, quantError);
                }
            }

            return this.pixels;
        }

        // Implement this for every dithering method
        protected abstract void PushError (int x, int y, float[] quantError);

        protected bool IsValidCoordinate (int x, int y)
        {
            return (0 <= x && x < this.width && 0 <= y && y < this.height);
        }

        protected int GetIndexWith (int x, int y)
        {
            return y * this.width + x;
        }

        protected float[] GetQuantError (Color originalPixel, Color newPixel)
        {
            float[] returnValue = new float[4];

            returnValue [0] = (originalPixel.r - newPixel.r);
            returnValue [1] = (originalPixel.g - newPixel.g);
            returnValue [2] = (originalPixel.b - newPixel.b);
            returnValue [3] = (originalPixel.a - newPixel.a);

            return returnValue;
        }

        public void ModifyImageWithErrorAndMultiplier (int x, int y, float[] quantError, float multiplier)
        {
            Color oldColor = this.pixels [GetIndexWith (x, y)];

            Color newColor = new Color (
                             GetLimitedValue (oldColor.r, (quantError [0] * multiplier)),
                             GetLimitedValue (oldColor.g, (quantError [1] * multiplier)),
                             GetLimitedValue (oldColor.b, (quantError [2] * multiplier)),
                             GetLimitedValue (oldColor.a, (quantError [3] * multiplier)));

            this.pixels [GetIndexWith (x, y)] = newColor;
        }

        private static float GetLimitedValue (float original, float error)
        {
            float newValue = original + error;   
            return Mathf.Clamp01 (newValue);
        }
    }
}