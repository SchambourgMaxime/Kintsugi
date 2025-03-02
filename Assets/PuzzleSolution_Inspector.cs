using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PuzzleSolution))]
public class PuzzleSolution_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        PuzzleSolution puzzleSolution = (PuzzleSolution)target;
        if (GUILayout.Button("Generate Puzzle Solution"))
        {
            puzzleSolution.puzzlePieceSolutions = new List<PuzzlePieceSolution>(Selection.count);
            for (int i = 0; i < Selection.count; i++)
            {
                if (Selection.gameObjects[i].TryGetComponent(out PuzzlePieceController puzzlePieceController))
                {
                    puzzlePieceController.SetID(i);
                    puzzleSolution.puzzlePieceSolutions.Add(
                        new PuzzlePieceSolution()
                        {
                            id = i,
                            position = Selection.objects[i].GetComponent<Transform>().position,
                            mesh = Selection.objects[i].GetComponent<MeshFilter>().sharedMesh,
                        });
                }
            }
        }
    }
}
