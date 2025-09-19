using UnityEngine.UIElements;
using VV.UI;

public class PuzzleMenu : UIBaseElement
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisualElement puzzleMenu = Get("PuzzleMenu");
        Button openBtn = GetButton("PuzzleMenuButton");
        openBtn.clicked += () => puzzleMenu.RemoveFromClassList("PuzzleMenuClosed");
        Button closeBtn = GetButton("closeMenu");
        closeBtn.clicked += () => puzzleMenu.AddToClassList("PuzzleMenuClosed");
    }
    
    
}
