using UnityEngine;
using UnityEngine.Events;

public class Interactor : BaseMonoBehaviour
{
    public Transform pickupTarget;
    public bool canInteract = true;

    public UnityEvent<Interactor> onSubscribe;

    public void Subscribe(UnityAction<Interactor> stopListening)
    {
        onSubscribe.Invoke(this);
        onSubscribe.AddListener(stopListening);
    }

    public void UnSubscribe(UnityAction<Interactor> stopListening)
    {
        onSubscribe.RemoveListener(stopListening);
    }

    private void OnDisable()
    {
        onSubscribe.Invoke(this);
    }
}
