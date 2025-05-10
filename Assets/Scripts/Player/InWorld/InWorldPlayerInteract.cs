using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InWorldPlayerInteract : MonoBehaviour
{
    [SerializeField] private UIInWorld inWorldUIManager;
    [SerializeField] private float interactRange = 2f;
    private IInteractable currentInteractable;
    private MiniGameEntryInteractable miniGameEntryInteractable;

    private void Awake(){
        inWorldUIManager.interactButton.GetComponent<Button>().onClick.AddListener(delegate{InteractButtonOnClick(currentInteractable); inWorldUIManager.interactButton.GetComponent<Button>().interactable = false;});
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        
        currentInteractable = GetClosestInteractable();
        if(currentInteractable != null)
        {
            miniGameEntryInteractable = currentInteractable.GetTransform().GetComponent<MiniGameEntryInteractable>();
            if(miniGameEntryInteractable != null)
            {
                //Debug.Log("Found mini game interactable: " + miniGameEntryInteractable.gameIndex);
                //Debug.Log(GameManager.Instance.finishedMiniCount);
            }
        }
        UpdateInWorldUI();
    }

    private void UpdateInWorldUI()
    {
        if (currentInteractable != null
        && (
            (GameManager.Instance.finishedMiniCount >= 3) 
            ||(miniGameEntryInteractable != null
            && miniGameEntryInteractable.gameIndex == (GameManager.Instance.finishedMiniCount + 1))) 
        // && inWorldUIManager.mainMenuButton.gameObject.activeSelf
        )
        {
            inWorldUIManager.interactButton.SetActive(true);
            inWorldUIManager.interactText.gameObject.SetActive(true);
            inWorldUIManager.interactText.text = currentInteractable.GetInteractPrompt();
        }
        else
        {
            inWorldUIManager.interactButton.SetActive(false);
            inWorldUIManager.interactText.gameObject.SetActive(false);
        }
    }

    private IInteractable GetClosestInteractable()
    {
        return Physics.OverlapSphere(transform.position, interactRange)
            .Select(collider => collider.GetComponent<IInteractable>())
            .Where(interactable => interactable != null)
            .OrderBy(interactable => Vector3.Distance(transform.position, interactable.GetTransform().position))
            .FirstOrDefault();
    }

    private void InteractButtonOnClick(IInteractable currentInteractable){
        currentInteractable.Interact(transform);
        inWorldUIManager.ClearInWorldUI();
    }
}
