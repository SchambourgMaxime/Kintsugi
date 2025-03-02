using System;
using UnityEngine;

public class PuzzlePieceController : MonoBehaviour, IPointerSelectable
{
    [SerializeField] private int id;
    public int ID => id;
    public void OnPointerDown()
    {
        Debug.Log("Clicked");
    }

    bool IPointerSelectable.CanBeSelected()
    {
        return enabled;
    }

    public void OnPointerUp()
    {
        throw new NotImplementedException();
    }

    public void OnPointerPress()
    {
        throw new NotImplementedException();
    }
    
    #if UNITY_EDITOR
    public void SetID(int newID)
    {
        id = newID;
    }
    #endif
}
