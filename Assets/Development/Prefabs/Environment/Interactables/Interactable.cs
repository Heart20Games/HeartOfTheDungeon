using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : BaseMonoBehaviour
{
    [Foldout("Events", true)]
    public UnityEvent onInteract;
    [Foldout("Events")] public UnityEvent<Interactor> onInteractSendInteractor;

    public bool oneOff = false;
    [ReadOnly][SerializeField] private bool hasInteracted = false;
    public bool releasable = false;
    private bool canRelease = false;

    public List<Interactor> interactors;

    public void Interact()
    {
        if (!oneOff || !hasInteracted)
        {
            if (releasable && canRelease)
            {
                onInteract.Invoke();
                onInteractSendInteractor.Invoke(null);
            }
            for (int i = 0; i < interactors.Count; i++)
            {
                Interactor interactor = interactors[i];
                if (interactor.canInteract)
                {
                    onInteract.Invoke();
                    onInteractSendInteractor.Invoke(interactor);
                    hasInteracted = true;
                }
            }
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
