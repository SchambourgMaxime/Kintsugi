using System;
using UnityEngine;

public class PuzzlePieceController : MonoBehaviour, IPointerSelectable
{
    [SerializeField] private int id;
    public int ID => id;

    private void Start()
    {
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.gameObject.SetActive(false);
            ps.Stop(true);
        }
    }
    
    private void OnEnable()
    {
        GetComponent<Outline>().enabled = true;
    }
    
    private void OnDisable()
    {

    }

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
    }

    public void OnPointerPress()
    {
    }
    
    #if UNITY_EDITOR
    public void SetID(int newID)
    {
        id = newID;
    }
    #endif
    public void OnPiecePlaced()
    {
        GetComponent<Outline>().enabled = false;

        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.gameObject.SetActive(true);
            ps.Play(true);
        }
    }
}
