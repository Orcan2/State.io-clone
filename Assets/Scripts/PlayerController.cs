using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    private State selectedState;
    private bool isDragging = false;
    private LineRenderer lineRenderer;

    

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0; 
    }

    void Update()
    {
        PlayerHuman.Instance.HandleInput(ref selectedState, ref isDragging);

      
        if (isDragging && selectedState != null)
        {
            Vector3 startPos = selectedState.transform.position;
            Vector3 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endPos.z = 0f;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
        else
        {
            lineRenderer.positionCount = 0; 
        }
    }
   
}
