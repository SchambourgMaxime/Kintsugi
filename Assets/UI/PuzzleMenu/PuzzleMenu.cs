using UnityEngine;
using UnityEngine.UIElements;

public class PuzzleMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TryGetComponent(out UIDocument doc))
        {
            VisualElement puzzleMenu = doc.rootVisualElement.Q<VisualElement>("PuzzleMenu");
            Button openBtn = doc.rootVisualElement.Q<Button>("PuzzleMenuButton");
            openBtn.clicked += () => puzzleMenu.RemoveFromClassList("PuzzleMenuClosed");
            Button closeBtn = doc.rootVisualElement.Q<Button>("closeMenu");
            closeBtn.clicked += () => puzzleMenu.AddToClassList("PuzzleMenuClosed");
        }
    }
}
