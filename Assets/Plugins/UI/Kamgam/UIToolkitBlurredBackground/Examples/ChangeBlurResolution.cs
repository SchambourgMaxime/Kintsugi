using UnityEngine;

namespace Kamgam.UIToolkitBlurredBackground
{
    [ExecuteInEditMode]
    public class ChangeBlurResolution : MonoBehaviour
    {
        public bool ChangeResolution = false;
        public Vector2Int Resolution = new Vector2Int(512, 256);

        void Update()
        {
            var mgr = BlurManager.Instance;
            if (mgr != null)
                mgr.Resolution = Resolution;
        }
    }
}
