using UnityEngine;

public interface IPointerSelectable
{
    public bool CanBeSelected();
    void OnPointerUp();
    
    void OnPointerPress();
}
