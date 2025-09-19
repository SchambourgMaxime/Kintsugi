using UnityEngine;
namespace Kamgam.UIToolkitBlurredBackground
{
    public interface IBlurRenderer
    {
        /// <summary>
        /// Defines how often the blur will be applied. Use with caution."
        /// </summary>
        int Iterations { get; set; }

        /// <summary>
        /// Defines how far out the sampling foes and thus the blur strength for each pass.
        /// </summary>
        float Offset { get; set; }

        /// <summary>
        /// The square texture resolution of the blurred image. Default is 512 x 512. Please use 2^n values like 256, 512, 1024, 2048. Reducing this will increase performance but decrease quality. Every frame your rendered image will be copied, resized and then blurred [BlurStrength] times.
        /// </summary>
        Vector2Int Resolution { get; set; }

        /// <summary>
        /// Defines how may samples are taken per pass. The higher the quality the more texels will be sampled and the lower the performance will be.
        /// </summary>
        ShaderQuality Quality { get; set; }

        bool Active { get; set; }

        Texture GetBlurredTexture();
        bool Update();
    }
}