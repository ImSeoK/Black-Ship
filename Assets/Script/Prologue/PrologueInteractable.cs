using UnityEngine;
using UnityEngine.Events;

public class PrologueInteractable : InteractableObject
{
    public UnityEvent onInteract;
    private bool triggered = false;

    public override void Interact()
    {
        if (triggered) return;
        triggered = true;
        onInteract?.Invoke();
    }
}