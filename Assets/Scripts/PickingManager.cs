using UnityEngine;
using VV.Utility;

public class PickingManager : MonoBehaviour
{
    public static PickingManager Instance { get; private set; }
    
    [SerializeField] private Camera pickingCamera;
    [SerializeField] public PuzzleSolution puzzleSolution;
    [SerializeField] private float pickingDistance = 10000f;
    
    public PuzzleSolution PuzzleSolution => puzzleSolution;

    private GameObject selected = null;
    private Plane selectedPlane;
    private Vector3 selectedOffset;

    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        InputManager.OnPointerDown += InputManager_OnPointerDown;
        InputManager.OnPointerPressed += InputManager_OnPointerPressed;
        InputManager.OnPointerUp += InputManager_OnPointerUp;
    }

    private void InputManager_OnPointerDown(object sender, Vector2 pointerPos)
    {
        Ray pickingRay = pickingCamera.ScreenPointToRay(pointerPos);
        if (!Physics.Raycast(pickingRay, out RaycastHit hit, pickingDistance, LayerMask.GetMask("Pieces"))) return;
        if(hit.collider.gameObject.TryGetComponent(out IPointerSelectable selectable) && selectable.CanBeSelected())
        {
            selected       = hit.collider.gameObject;
            selectedPlane  = new Plane(-pickingCamera.transform.forward.With(y:0f).normalized, hit.point);
            selectedOffset = selected.transform.position - hit.point;
        }
    }
    
    private void InputManager_OnPointerPressed(object sender, Vector2 pointerPos)
    {
        if (!selected) return;
        
        Ray pickingRay = pickingCamera.ScreenPointToRay(pointerPos);
        if (selectedPlane.Raycast(pickingRay, out float distance))
        {
            Vector3 planePoint = pickingRay.GetPoint(distance);
            DebugDraw.DrawSphere(planePoint, .01f, Color.red, .5f);
            selected.transform.position = planePoint + selectedOffset;
        }

        if (!selected.TryGetComponent(out PuzzlePieceController puzzlePieceController)) return;
        
        if (puzzleSolution.CheckPiecePosition(puzzlePieceController.ID, selected.transform.position))
        {
            selected.transform.position = puzzleSolution.GetPiecePosition(puzzlePieceController.ID);
            puzzlePieceController.enabled = false;
            puzzleSolution.SetPieceSolved(puzzlePieceController.ID);
            selected = null;
        }
    }
    
    private void InputManager_OnPointerUp(object sender, Vector2 e)
    {
        selected = null;
    }
}
