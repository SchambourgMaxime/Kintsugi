using System;
using System.Collections.Generic;
using System.Linq;
using ForIndustrie.Utility;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Action = Unity.Android.Gradle.Manifest.Action;

[Serializable]
public struct PuzzlePieceSolution
{
    public int id;
    public Vector3 position;
    public Mesh mesh;
}

[CreateAssetMenu(fileName = "PuzzleSolution", menuName = "Scriptable Objects/PuzzleSolution")]
public class PuzzleSolution : ScriptableObject
{
    public event Action<int> OnPieceSolved;
    public event Action<int> OnPuzzleSolved;
    
    public List<PuzzlePieceSolution> puzzlePieceSolutions;
    public Dictionary<int, bool> solvedPuzzlePieceSolutions = new();

    public float distanceToSolve = 5f;

    public bool CheckPiecePosition(int pieceID, Vector3 pos)
    {
        foreach (var puzzlePieceSolution in puzzlePieceSolutions.
                     Where(puzzlePieceSolution => puzzlePieceSolution.id == pieceID))
            return (pos - puzzlePieceSolution.position).ShorterThan(distanceToSolve);

        return false;
    }

    public Vector3 GetPiecePosition(int pieceID)
    {
        foreach (var puzzlePieceSolution in puzzlePieceSolutions.Where(puzzlePieceSolution =>
                     puzzlePieceSolution.id == pieceID))
            return puzzlePieceSolution.position;

        return Vector3.negativeInfinity;
    }

    public void SetPieceSolved(int pieceID)
    {
        solvedPuzzlePieceSolutions[pieceID] = true;
        
        OnPieceSolved?.Invoke(pieceID);

        if (solvedPuzzlePieceSolutions.Values.Any(value => !value)) return;
        
        OnPuzzleSolved?.Invoke(pieceID);
    }
}
