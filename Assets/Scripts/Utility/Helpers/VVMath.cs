using UnityEngine;

namespace VV.Utility
{
    public static class VVMath
    {
        public const float Epsilon = 0.0001f;
        
        /// <summary>Standard residual</summary>
        private const float kLogNegligibleResidual = -4.605170186f;
        
        /// <summary>Get a damped version of a quantity.  This is the portion of the
        /// quantity that will take effect over the given time.</summary>
        /// <param name="initial">The amount that will be damped</param>
        /// <param name="dampTime">The rate of damping.  This is the time it would
        /// take to reduce the original amount to a negligible percentage</param>
        /// <param name="deltaTime">The time over which to damp</param>
        /// <returns>The damped amount.  This will be the original amount scaled by
        /// a value between 0 and 1.</returns>
        public static float Damp(float initial, float dampTime, float deltaTime)
        {
            return StandardDamp(initial, dampTime, deltaTime);
        }

        /// <summary>Get a damped version of a quantity.  This is the portion of the
        /// quantity that will take effect over the given time.</summary>
        /// <param name="initial">The amount that will be damped</param>
        /// <param name="dampTime">The rate of damping.  This is the time it would
        /// take to reduce the original amount to a negligible percentage</param>
        /// <param name="deltaTime">The time over which to damp</param>
        /// <returns>The damped amount.  This will be the original amount scaled by
        /// a value between 0 and 1.</returns>
        public static Vector3 Damp(Vector3 initial, Vector3 dampTime, float deltaTime)
        {
            for (int i = 0; i < 3; ++i)
                initial[i] = Damp(initial[i], dampTime[i], deltaTime);
            return initial;
        }
        
        public static Vector2 Damp(Vector2 initial, Vector2 dampTime, float deltaTime)
        {
            for (int i = 0; i < 2; ++i)
                initial[i] = Damp(initial[i], dampTime[i], deltaTime);
            return initial;
        }
        
        /// <summary>Get a damped version of a quantity.  This is the portion of the
        /// quantity that will take effect over the given time.</summary>
        /// <param name="initial">The amount that will be damped</param>
        /// <param name="dampTime">The rate of damping.  This is the time it would
        /// take to reduce the original amount to a negligible percentage</param>
        /// <param name="deltaTime">The time over which to damp</param>
        /// <returns>The damped amount.  This will be the original amount scaled by
        /// a value between 0 and 1.</returns>
        public static Vector3 Damp(Vector3 initial, float dampTime, float deltaTime)
        {
            for (int i = 0; i < 3; ++i)
                initial[i] = Damp(initial[i], dampTime, deltaTime);
            return initial;
        }
        
        public static Vector2 Damp(Vector2 initial, float dampTime, float deltaTime)
        {
            for (int i = 0; i < 2; ++i)
                initial[i] = Damp(initial[i], dampTime, deltaTime);
            return initial;
        }
        
        /// <summary>Get a damped version of a quantity.  This is the portion of the
        /// quantity that will take effect over the given time.</summary>
        /// <param name="initial">The amount that will be damped</param>
        /// <param name="dampTime">The rate of damping.  This is the time it would
        /// take to reduce the original amount to a negligible percentage</param>
        /// <param name="deltaTime">The time over which to damp</param>
        /// <returns>The damped amount.  This will be the original amount scaled by
        /// a value between 0 and 1.</returns>
        // Internal for testing
        internal static float StandardDamp(float initial, float dampTime, float deltaTime)
        {
            if (dampTime < Epsilon || Mathf.Abs(initial) < Epsilon)
                return initial;
            if (deltaTime < Epsilon)
                return 0;
            return initial * (1 - Mathf.Exp(kLogNegligibleResidual * deltaTime / dampTime));
        }

        public static bool Approximately(float num, float comp)
        {
            return NearlyZero(num - comp);
        }
        
        public static bool NearlyZero(this float num)
        {
            return num is < Epsilon and > -Epsilon;
        }
        
        public static int Wrap(int x, int min, int max)
        {
            int size = (max - min) + 1;
            int endVal = x;
            while (endVal < min)
            {
                endVal += size;
            }

            while (endVal > max)
            {
                endVal -= size;
            }
            return endVal;
        }
    }
}