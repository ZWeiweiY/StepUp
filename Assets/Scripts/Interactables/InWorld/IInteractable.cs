using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IInteractable
{
    void Interact(Transform interactorTransform);
    string GetInteractPrompt();
    Transform GetTransform();
}
