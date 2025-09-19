using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Kamgam.UIToolkitBlurredBackground
{
    public static class RenderUtils
    {
        static Camera _cachedGameViewCam;

        static Camera[] _tmpAllCameras = new Camera[10];

        public static Camera GetGameViewCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                // Fetch cameras
                int allCamerasCount = Camera.allCamerasCount;
                // Alloc new array only if needed
                if (allCamerasCount > _tmpAllCameras.Length)
                {
                    _tmpAllCameras = new Camera[allCamerasCount + 5];
                }
                Camera.GetAllCameras(_tmpAllCameras);

                // We sort by depth and start from the back because we assume
                // that among cameras with equal depth the last takes precedence.
                float maxDepth = float.MinValue;
                for (int i = _tmpAllCameras.Length - 1; i >= 0; i--)
                {
                    // Null out old references
                    if (i >= allCamerasCount)
                    {
                        _tmpAllCameras[i] = null;
                        continue;
                    }

                    var cCam = _tmpAllCameras[i];

                    if (!cCam.isActiveAndEnabled)
                        continue;

                    // Only take full screen cameras that are not rendering into render textures
                    if (cCam.depth > maxDepth && cCam.targetTexture == null && cCam.rect.width >= 1f && cCam.rect.height >= 1f)
                    {
                        maxDepth = cCam.depth;
                        cam = cCam;
                    }
                }
            }

            // cache game view camera
            if (cam != null && cam.cameraType == CameraType.Game)
                _cachedGameViewCam = cam;

            if (cam == null)
                return _cachedGameViewCam;

            return cam;
        }
    }
}