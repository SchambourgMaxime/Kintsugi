#if KAMGAM_VISUAL_SCRIPTING
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.UIToolkitVisualScripting.Nodes
{
    public static partial class EventNames
    {
        public static string OnTextFieldEndEdit = "UITK_OnTextFieldEndEdit";
    }

    [IncludeInSettings(include: false)]
    [UnitCategory("Events\\UI Toolkit")] // Events have to be under the Events category: https://forum.unity.com/threads/c-custom-unitcategory-for-events.1178791/#post-7549207
    [UnitTitle("On Text Field End Edit (UITK)")]
    [UnitShortTitle("On Text Field End Edit (UITK)")]
    [TypeIcon(typeof(UnityEngine.UI.InputField))]
    public class OnTextFieldEndEdit : UIToolkitChangeEventBase<string>
    {
        protected override string getHookName()
        {
            return EventNames.OnTextFieldEndEdit;
        }

        protected override void registerCallbacks(VisualElement ve)
        {
            var textfield = ve as TextField;
            if (textfield != null)
            {
                // If delayed is on then, quote:
                // "This will make make the ChangeEvent<string> be delayed until the user
                //  presses enter or gives away focus, but will also won't notify you the
                //  user cancels with Escape."
                // Source: https://forum.unity.com/threads/how-do-you-detect-if-someone-clicks-enter-return-on-a-textfield.688579/#post-4609261
                if (textfield.isDelayed)
                {
                    textfield.RegisterValueChangedCallback(handleChangedIfDelayed);
                    textfield.RegisterCallback<KeyUpEvent>(handleKeysIfDelayed);
                }
                else
                {
                    textfield.RegisterValueChangedCallback(handleChangedIfNotDelayed);
                    textfield.RegisterCallback<KeyUpEvent>(handleKeysIfNotDelayed);
                    textfield.Q(name: "unity-text-input")?.RegisterCallback<FocusEvent>(handleGainedFocusIfNotDelayed);
                    textfield.Q(name: "unity-text-input")?.RegisterCallback<BlurEvent>(handleLostFocusIfNotDelayed);
                }
            }
        }

        protected void handleChangedIfDelayed(ChangeEvent<string> evt)
        {
            triggerEvent(evt);
        }

        protected void handleKeysIfDelayed(KeyUpEvent evt)
        {
            var textfield = _cachedVisualElement as TextField;
            if (textfield != null)
            {
                if (evt.keyCode == UnityEngine.KeyCode.Escape)
                {
                    var e = ChangeEvent<string>.GetPooled(previousValue: textfield.text, newValue: textfield.text);
                    triggerEvent(e);
                }
            }
        }

        protected string _tmpPreviousText;

        protected void handleChangedIfNotDelayed(ChangeEvent<string> evt)
        {
            if (_tmpPreviousText == null && _hasFocus)
            {
                _tmpPreviousText = evt.previousValue;
            }
        }

        protected void handleKeysIfNotDelayed(KeyUpEvent evt)
        {
            var textfield = _cachedVisualElement as TextField;
            if (textfield != null && _hasFocus)
            {
                // If the textfield is single line then interpret ENTER as the confirm key.
                if (!textfield.multiline)
                {
                    if (evt.keyCode == KeyCode.Return)
                    {
                        var e = ChangeEvent<string>.GetPooled(previousValue: _tmpPreviousText, newValue: textfield.text);
                        triggerEvent(e);
                    }
                }
            }
        }

        protected bool _hasFocus;

        protected void handleGainedFocusIfNotDelayed(FocusEvent evt)
        {
            _hasFocus = true;
        }

        protected void handleLostFocusIfNotDelayed(BlurEvent evt)
        {
            _hasFocus = false;

            try
            {
                var textfield = _cachedVisualElement as TextField;
                if (textfield != null)
                {
                    var e = ChangeEvent<string>.GetPooled(previousValue: _tmpPreviousText, newValue: textfield.text);
                    triggerEvent(e);
                }
            }
            finally
            {
                _tmpPreviousText = null;
            }
        }

        protected override void unregisterCallbacks(VisualElement ve)
        {
            var textfield = ve as TextField;
            if (textfield != null)
            {
                textfield.UnregisterValueChangedCallback(handleChangedIfDelayed);
                textfield.UnregisterCallback<KeyUpEvent>(handleKeysIfDelayed);

                textfield.UnregisterValueChangedCallback(handleChangedIfNotDelayed);
                textfield.UnregisterCallback<KeyUpEvent>(handleKeysIfNotDelayed);
                textfield.Q(name: "unity-text-input")?.UnregisterCallback<FocusEvent>(handleGainedFocusIfNotDelayed);
                textfield.Q(name: "unity-text-input")?.UnregisterCallback<BlurEvent>(handleLostFocusIfNotDelayed);
            }
        }
    }
}
#endif