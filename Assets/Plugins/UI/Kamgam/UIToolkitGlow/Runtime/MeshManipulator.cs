using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.UIR;

namespace Kamgam.UIToolkitGlow
{
    public abstract class MeshManipulator<T> : ManipulatorBase<T>
        where T : ManipulatorBase<T>
    {
        protected override void RegisterCallbacksOnTarget()
        {
            target.generateVisualContent += generateVisualContent;
        }

        protected abstract void generateVisualContent(MeshGenerationContext mgc);

        protected override void UnregisterCallbacksFromTarget()
        {
            target.generateVisualContent -= generateVisualContent;
        }
    }
}