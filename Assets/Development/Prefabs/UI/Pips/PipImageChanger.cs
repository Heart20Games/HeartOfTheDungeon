using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PipImageChanger : BaseMonoBehaviour
{
    public bool IsFilled { get => isFilled; set => SetIsFilled(value); }
    public bool isFilled;
    [ReadOnly][SerializeField] private bool prevIsFilled;

    [SerializeField] private float cycleRate = 0.2f;
    [ReadOnly][SerializeField] private int index;

    public Sprite[] filled;
    public Sprite[] unfilled;

    [ReadOnly][SerializeField] private Sprite[] current;
    
    public UnityEvent<Sprite> onSpriteChange;

    Coroutine coroutine;

    private void Update()
    {
        if (isFilled != prevIsFilled)
        {
            IsFilled = IsFilled;
        }
        if (coroutine == null)
        {
            coroutine = StartCoroutine(Increment());
        }
    }

    public IEnumerator Increment()
    {
        while (coroutine != null)
        {
            yield return new WaitForSeconds(cycleRate);
            index = (int)Mathf.Repeat(index, current.Length - 1);
            onSpriteChange.Invoke(current[index]);
        }
    }

    public void SetIsFilled(bool isFilled)
    {
        this.isFilled = isFilled;
        current = isFilled ? filled : unfilled;
        onSpriteChange.Invoke(current[index]);
    }
}
