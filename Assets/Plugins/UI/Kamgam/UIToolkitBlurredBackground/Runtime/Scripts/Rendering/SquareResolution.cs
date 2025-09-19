using UnityEngine;

namespace Kamgam.UIToolkitBlurredBackground
{
    public enum SquareResolution
    {
        _32, 
        _64, 
        _128, 
        _256, 
        _512, 
        _1024, 
        _2048,
        _4096
    };

    public static class SquareResolutionsUtils
    {
        public static Vector2Int ToResolution(this SquareResolution res)
        {
            switch (res)
            {
                case SquareResolution._32:
                    return new Vector2Int(32, 32);

                case SquareResolution._64:
                    return new Vector2Int(64, 64);
                    
                case SquareResolution._128:
                    return new Vector2Int(128, 128);
                    
                case SquareResolution._256:
                    return new Vector2Int(256, 256);
                    
                case SquareResolution._512:
                    return new Vector2Int(512, 512);
                    
                case SquareResolution._1024:
                    return new Vector2Int(1024, 1024);
                    
                case SquareResolution._2048:
                    return new Vector2Int(2048, 2048);

                case SquareResolution._4096:
                    return new Vector2Int(4096, 4096);

                default:
                    return new Vector2Int(512, 512);
            }
        }

        public static SquareResolution FromResolution(this Vector2Int res)
        {
            if (res.x >= 4096 && res.y >= 4096)
            {
                return SquareResolution._4096;
            }
            else if (res.x >= 2048 && res.y >= 2048)
            {
                return SquareResolution._2048;
            }
            else if (res.x >= 1024 && res.y >= 1024)
            {
                return SquareResolution._1024;
            }
            else if (res.x >= 512 && res.y >= 512)
            {
                return SquareResolution._512;
            }
            else if (res.x >= 256 && res.y >= 256)
            {
                return SquareResolution._256;
            }
            else if (res.x >= 128 && res.y >= 128)
            {
                return SquareResolution._128;
            }
            else if (res.x >= 64 && res.y >= 64)
            {
                return SquareResolution._64;
            }
            else
            {
                return SquareResolution._32;
            }
        }
    }
}