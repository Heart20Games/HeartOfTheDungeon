using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : BaseMonoBehaviour
{
    public UnityEvent onInteract;

    public List<Interactor> interactors;

    public void Interact()
    {
        if (interactors.Count > 0)
        {
            onInteract.Invoke();
        }
    }

    public void AddInteractor(Impact impact)
    {
        if (impact.other.TryGetComponent<Interactor>(out var interactor))
            AddInteractor(interactor);
    }

    public void RemoveInteractor(Impact impact)
    {
        if (impact.other.TryGetComponent<Interactor>(out var interactor))
            RemoveInteractor(interactor);
    }

    public void AddInteractor(Interactor interactor)
    {
        if (!interactors.Contains(interactor))
        {
            interactors.Add(interactor);
            interactor.Subscribe(RemoveInteractor);
        }
    }

    public void RemoveInteractor(Interactor interactor)
    {
        if (interactors.Remove(interactor))
            interactor.UnSubscribe(RemoveInteractor);
    }
}
