/*
   This file implements error pushing of dithering via (Robert) Floyd and (Louis) Steinberg kernel.
   This is free and unencumbered software released into the public domain.
*/

namespace MBootStrap
{
    public class NoDithering : DitheringBase
    {
        public NoDithering (FindColor colorfunc) : base (colorfunc)
        {
        }

        override protected void PushError (int x, int y, float[] quantError)
        {
        }
    }
}