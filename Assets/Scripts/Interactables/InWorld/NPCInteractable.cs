using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractable : MonoBehaviour, IInteractable
{

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void Interact(Transform interactorTransform)
    {
        //Debug.Log("NPCInteractable: Interacting with NPCInteractable");
    }

    public string GetInteractPrompt()
    {
        return "Talk";
    }

    public Transform GetTransform()
    {
        return transform;
    }
    private void NPCInteractOnClick(){
        //Debug.Log("NPCInteractable: NPC Interact!");
    }
}
