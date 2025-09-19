using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Kamgam.UIToolkitScrollViewPro;

namespace Kamgam.UIToolkitScrollViewPro
{
    public class TargetFrameRate : MonoBehaviour
    {
        public int FrameRate = 60;

        private void Awake()
        {
            Application.targetFrameRate = FrameRate;
        }
    }
}
