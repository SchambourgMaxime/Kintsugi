using System;
using Kamgam.UIToolkitVisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using VV.UI;

public class PuzzleMenu : UIBaseElement
{
    [SerializeField] private VisualTreeAsset puzzlePieceAsset;
    [SerializeField] private PuzzlePieceDispenser puzzlePieceDispenser;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisualElement puzzleMenu = Get("PuzzleMenu");
        Button openBtn = GetButton("PuzzleMenuButton");
        openBtn.clicked += () => puzzleMenu.RemoveFromClassList("PuzzleMenuClosed");
        Button closeBtn = GetButton("closeMenu");
        closeBtn.clicked += () => puzzleMenu.AddToClassList("PuzzleMenuClosed");

        InitPuzzlePieces();
    }

    private void InitPuzzlePieces()
    {
        ScrollView scrollView = Get<ScrollView>("puzzle-piece-list");
        scrollView.Clear();
        puzzlePieceDispenser.ForEachPuzzlePiece((idx, controller) =>
        {
            VisualElement puzzlePiece = puzzlePieceAsset.Instantiate();
            scrollView.Add(puzzlePiece);
            GameObject ppgo = new GameObject("PuzzlePiece" + idx);
            controller.transform.SetParent(ppgo.transform, false);
            Camera cam = ppgo.AddComponent<Camera>();
            cam.cullingMask = LayerMask.GetMask("Pieces");
            puzzlePiece.SetBackgroundImage(cam.activeTexture);
        });
    }
}
