/*
The easing equations are from the lovely Rovert Penner website: http://robertpenner.com/easing/
License: http://robertpenner.com/easing_terms_of_use.html

Terms of Use: Easing Functions(Equations)

Open source under the MIT License and the 3-Clause BSD License.
MIT License

Copyright © 2001 Robert Penner

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.



THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
BSD License

Copyright © 2001 Robert Penner

Redistribution and use in source and binary forms, with or without modification, are permitted provided 
that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and
the following disclaimer.

Redistributions in binary form must reproduce the above copyright notice, this list of conditions 
and the following disclaimer in the documentation and/or other materials provided with the distribution.

Neither the name of the author nor the names of contributors may be used to endorse or promote 
products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR 
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF 
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.UIToolkitScrollViewPro
{
    public enum Easing
    {
        Linear = 0,

        CustomCurve = 1,

        Ease,

        QuadIn,
        QuadOut,
        QuadInOut,

        CubicIn,
        CubicOut,
        CubicInOut,

        QuartIn,
        QuartOut,
        QuartInOut,

        QuintIn,
        QuintOut,
        QuintInOut,

        SineIn,
        SineOut,
        SineInOut,

        ExpoIn,
        ExpoOut,
        ExpoInOut,

        CircIn,
        CircOut,
        CircInOut,

        ElasticIn,
        ElasticOut,
        ElasticInOut,

        BackIn,
        BackOut,
        BackInOut,

        BounceIn,
        BounceOut,
        BounceInOut
    }

    public static class EasingUtils
    {
        static Easing EasingModeToEasing(UnityEngine.UIElements.EasingMode easingMode)
        {
            switch (easingMode)
            {
                case UnityEngine.UIElements.EasingMode.Ease:
                    return Easing.Ease;
                case UnityEngine.UIElements.EasingMode.EaseIn:
                    return Easing.Linear; // Not supported
                case UnityEngine.UIElements.EasingMode.EaseOut:
                    return Easing.Linear; // Not supported
                case UnityEngine.UIElements.EasingMode.EaseInOut:
                    return Easing.Linear; // Not supported
                case UnityEngine.UIElements.EasingMode.Linear:
                    return Easing.Linear;
                case UnityEngine.UIElements.EasingMode.EaseInSine:
                    return Easing.SineIn;
                case UnityEngine.UIElements.EasingMode.EaseOutSine:
                    return Easing.SineOut;
                case UnityEngine.UIElements.EasingMode.EaseInOutSine:
                    return Easing.SineInOut;
                case UnityEngine.UIElements.EasingMode.EaseInCubic:
                    return Easing.CubicIn;
                case UnityEngine.UIElements.EasingMode.EaseOutCubic:
                    return Easing.CubicOut;
                case UnityEngine.UIElements.EasingMode.EaseInOutCubic:
                    return Easing.CubicInOut;
                case UnityEngine.UIElements.EasingMode.EaseInCirc:
                    return Easing.CircIn;
                case UnityEngine.UIElements.EasingMode.EaseOutCirc:
                    return Easing.CircOut;
                case UnityEngine.UIElements.EasingMode.EaseInOutCirc:
                    return Easing.CircInOut;
                case UnityEngine.UIElements.EasingMode.EaseInElastic:
                    return Easing.ElasticIn;
                case UnityEngine.UIElements.EasingMode.EaseOutElastic:
                    return Easing.ElasticOut;
                case UnityEngine.UIElements.EasingMode.EaseInOutElastic:
                    return Easing.ElasticInOut;
                case UnityEngine.UIElements.EasingMode.EaseInBack:
                    return Easing.BackIn;
                case UnityEngine.UIElements.EasingMode.EaseOutBack:
                    return Easing.BackOut;
                case UnityEngine.UIElements.EasingMode.EaseInOutBack:
                    return Easing.BackInOut;
                case UnityEngine.UIElements.EasingMode.EaseInBounce:
                    return Easing.BounceIn;
                case UnityEngine.UIElements.EasingMode.EaseOutBounce:
                    return Easing.BounceOut;
                case UnityEngine.UIElements.EasingMode.EaseInOutBounce:
                    return Easing.BounceInOut;
                default:
                    return Easing.Linear;
            }
        }

        static Dictionary<Easing, System.Func<float, float, float, float, float>> _functionTable = new Dictionary<Easing, System.Func<float, float, float, float, float>>()
        {
            { Easing.Linear, EaseLinear },

            { Easing.Ease, EaseDefault },

            { Easing.QuadIn, EaseInQuad },
            { Easing.QuadOut, EaseOutQuad },
            { Easing.QuadInOut, EaseInOutQuad },

            { Easing.CubicIn, EaseInCubic },
            { Easing.CubicOut, EaseOutCubic },
            { Easing.CubicInOut, EaseInOutCubic },

            { Easing.QuartIn, EaseInQuart },
            { Easing.QuartOut, EaseOutQuart },
            { Easing.QuartInOut, EaseInOutQuart },

            { Easing.QuintIn, EaseInQuint },
            { Easing.QuintOut, EaseOutQuint },
            { Easing.QuintInOut, EaseInOutQuint },

            { Easing.SineIn, EaseInSine },
            { Easing.SineOut, EaseOutSine },
            { Easing.SineInOut, EaseInOutSine },

            { Easing.ExpoIn, EaseInExpo },
            { Easing.ExpoOut, EaseOutExpo },
            { Easing.ExpoInOut, EaseInOutExpo },

            { Easing.CircIn, EaseInCirc },
            { Easing.CircOut, EaseOutCirc },
            { Easing.CircInOut, EaseInOutCirc },

            { Easing.ElasticIn, EaseInElastic },
            { Easing.ElasticOut, EaseOutElastic },
            { Easing.ElasticInOut, EaseInOutElastic },

            { Easing.BackIn, EaseInBack },
            { Easing.BackOut, EaseOutBack },
            { Easing.BackInOut, EaseInOutBack },

            { Easing.BounceIn, EaseInBounce },
            { Easing.BounceOut, EaseOutBounce },
            { Easing.BounceInOut, EaseInOutBounce }
        };

        /// <summary>
        /// t is the time progress normalized from 0 to 1.
        /// </summary>
        /// <param name="easing"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Ease(Easing easing, float t)
        {
            return _functionTable[easing].Invoke(t, 0f, 1f, 1f);
        }

        /// <summary>
        /// Example: t = 0, b = 0, c = 1, d = 1 (animates from 0 to 1 in 1 sec)
        /// </summary>
        /// <param name="easing"></param>
        /// <param name="t">current time in sec</param>
        /// <param name="b">start value</param>
        /// <param name="c">change in value (delta to add to b)</param>
        /// <param name="d">duration in sec</param>
        /// <returns></returns>
        public static float Ease(Easing easing, float t, float b, float c, float d)
        {
            return _functionTable[easing].Invoke(t, b, c, d);
        }

        // Unity's default ease (starts fast, ends slow)
        public static float EaseDefault(float t, float b, float c, float d)
        {
            if (d <= 0f) return b + c;
            t = t / d;
            t = -0.2f * t * t * t + -0.6f * t * t + 1.8f * t;
            return b + t * c;
        }

        public static float EaseLinear(float t, float b, float c, float d)
        {
            if (d <= 0f) return b + c;
            return b + (t/d) * c;
        }

        public static float EaseInQuad(float t, float b, float c, float d)
        {
            return c * (t /= d) * t + b;
        }

	    public static float EaseOutQuad(float t, float b, float c, float d)
        {
            return -c * (t /= d) * (t - 2) + b;
        }

	    public static float EaseInOutQuad(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t + b;
            return -c / 2 * ((--t) * (t - 2) - 1) + b;
        }

	    public static float EaseInCubic(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t + b;
        }

	    public static float EaseOutCubic(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t + 1) + b;
        }

	    public static float EaseInOutCubic(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t + 2) + b;
        }

	    public static float EaseInQuart(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t + b;
        }

	    public static float EaseOutQuart(float t, float b, float c, float d)
        {
            return -c * ((t = t / d - 1) * t * t * t - 1) + b;
        }

	    public static float EaseInOutQuart(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
            return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
        }

	    public static float EaseInQuint(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t * t + b;
        }

	    public static float EaseOutQuint(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
        }

	    public static float EaseInOutQuint(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
        }

	    public static float EaseInSine(float t, float b, float c, float d)
        {
            return -c * Mathf.Cos(t / d * (Mathf.PI / 2)) + c + b;
        }

	    public static float EaseOutSine(float t, float b, float c, float d)
        {
            return c * Mathf.Sin(t / d * (Mathf.PI / 2)) + b;
        }

	    public static float EaseInOutSine(float t, float b, float c, float d)
        {
            return -c / 2 * (Mathf.Cos(Mathf.PI * t / d) - 1) + b;
        }

	    public static float EaseInExpo(float t, float b, float c, float d)
        {
            return (t == 0) ? b : c * Mathf.Pow(2, 10 * (t / d - 1)) + b;
        }

	    public static float EaseOutExpo(float t, float b, float c, float d)
        {
            return (t == d) ? b + c : c * (-Mathf.Pow(2, -10 * t / d) + 1) + b;
        }

	    public static float EaseInOutExpo(float t, float b, float c, float d)
        {
            if (t == 0) return b;
            if (t == d) return b + c;
            if ((t /= d / 2) < 1) return c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b;
            return c / 2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
        }

	    public static float EaseInCirc(float t, float b, float c, float d)
        {
            return -c * (Mathf.Sqrt(1 - (t /= d) * t) - 1) + b;
        }

	    public static float EaseOutCirc(float t, float b, float c, float d)
        {
            return c * Mathf.Sqrt(1 - (t = t / d - 1) * t) + b;
        }

	    public static float EaseInOutCirc(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return -c / 2 * (Mathf.Sqrt(1 - t * t) - 1) + b;
            return c / 2 * (Mathf.Sqrt(1 - (t -= 2) * t) + 1) + b;
        }

	    public static float EaseInElastic(float t, float b, float c, float d)
        {
            float s;
            var p = 0f;
            var a = c;
            if (t == 0) return b;
            if ((t /= d) == 1f) return b + c;
            if (p == 0f) p = d * 0.3f;
            if (a < Mathf.Abs(c))
            {
                a = c;
                s = p / 4f;
            }
            else
            {
                s = p / (2f * Mathf.PI) * Mathf.Asin(c / a);
            }
            return -(a * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
        }

	    public static float EaseOutElastic(float t, float b, float c, float d)
        {
            float s;
            var p = 0f;
            var a = c;
            if (t == 0) return b;
            if ((t /= d) == 1) return b + c;
            if (p == 0f) p = d * 0.3f;
            if (a < Mathf.Abs(c)) { a = c; s = p / 4f; }
            else s = p / (2f * Mathf.PI) * Mathf.Asin(c / a);
            return a * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) + c + b;
        }

	    public static float EaseInOutElastic(float t, float b, float c, float d)
        {
            var s = 1.70158f; var p = 0f; var a = c;
            if (t == 0) return b; if ((t /= d / 2f) == 2f) return b + c; if (p == 0f) p = d * (0.3f * 1.5f);
            if (a < Mathf.Abs(c)) { a = c; s = p / 4f; }
            else s = p / (2f * Mathf.PI) * Mathf.Asin(c / a);
            if (t < 1f) return -0.5f * (a * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t * d - s) * (2f * Mathf.PI) / p)) + b;
            return a * Mathf.Pow(2f, -10f * (t -= 1)) * Mathf.Sin((t * d - s) * (2f * Mathf.PI) / p) * 0.5f + c + b;
        }

        public static float EaseInBack(float t, float b, float c, float d)
        {
            return EaseInBack(t, b, c, d, 1.70158f);
        }

        public static float EaseInBack(float t, float b, float c, float d, float s)
        {
            return c * (t /= d) * t * ((s + 1) * t - s) + b;
        }

        public static float EaseOutBack(float t, float b, float c, float d)
        {
            return EaseOutBack(t, b, c, d, 1.70158f);
        }

        public static float EaseOutBack(float t, float b, float c, float d, float s)
        {
            return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
        }

        public static float EaseInOutBack(float t, float b, float c, float d)
        {
            return EaseInOutBack(t, b, c, d, 1.70158f);
        }

        public static float EaseInOutBack(float t, float b, float c, float d, float s)
        {
            if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + b;
            return c / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + b;
        }

	    public static float EaseInBounce(float t, float b, float c, float d)
        {
            return c - EaseOutBounce(d - t, 0f, c, d) + b;
        }

	    public static float EaseOutBounce(float t, float b, float c, float d)
        {
            if ((t /= d) < (1f / 2.75f))
            {
                return c * (7.5625f * t * t) + b;
            }
            else if (t < (2f / 2.75f))
            {
                return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f) + b;
            }
            else if (t < (2.5f / 2.75f))
            {
                return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f) + b;
            }
            else
            {
                return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + 0.984375f) + b;
            }
        }

	    public static float EaseInOutBounce(float t, float b, float c, float d)
        {
            if (t < d / 2f) return EaseInBounce(t * 2f, 0f, c, d) * 0.5f + b;
            return EaseOutBounce(t * 2f - d, 0f, c, d) * 0.5f + c * 0.5f + b;
        }
    }
}