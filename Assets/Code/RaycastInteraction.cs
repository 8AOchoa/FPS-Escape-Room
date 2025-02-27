using UnityEngine;
using UnityEngine.UI;

public class RaycastInteraction : MonoBehaviour
{
    public float range = 3f;  // Distance to detect objects
    public LayerMask interactableLayer;
    public Image reticle;  // UI Reticle image (Assign in Inspector)
    private Color defaultColor = Color.white;
    private Color highlightColor = Color.red;

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, range, interactableLayer))
        {
            reticle.color = highlightColor;  // Change color when looking at an interactable object
        }
        else
        {
            reticle.color = defaultColor;
        }
    }
}
