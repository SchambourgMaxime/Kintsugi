using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePieceDispenser : MonoBehaviour
{
    [SerializeField] private List<PuzzlePieceController> puzzlesPieces;
    [SerializeField] private Transform startPos;
    
    private PuzzlePieceController selectedPiece = null;
    private int currentIndex = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (PuzzlePieceController puzzlePieceController in puzzlesPieces)
            puzzlePieceController.gameObject.SetActive(false);
        
        ActivatePiece(currentIndex++);
        PickingManager.Instance.PuzzleSolution.OnPieceSolved += _ => ActivatePiece(currentIndex++);
    }

    private void ActivatePiece(int index)
    {
        //if (selectedPiece) selectedPiece.gameObject.SetActive(false);
        selectedPiece = puzzlesPieces[index];
        selectedPiece.transform.position = startPos.position;
        selectedPiece.gameObject.SetActive(true);
    }

    public void ForEachPuzzlePiece(Action<int, PuzzlePieceController> action)
    {
        for (int i = 0; i < puzzlesPieces.Count; i++)
        {
            PuzzlePieceController puzzlePieceController = puzzlesPieces[i]; 
            action(i, puzzlePieceController);
        }
    }
}
