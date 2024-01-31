using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PipImageChanger : BaseMonoBehaviour
{
    public bool IsFilled { get => isFilled; set => SetIsFilled(value); }
    public bool isFilled;
    [ReadOnly][SerializeField] private bool prevIsFilled;

    public float Transition { get => transition; set => SetTransition(value); }
    public float transition = 1;

    [SerializeField] private float cycleRate = 0.2f;
    [ReadOnly][SerializeField] private int index;

    public Sprite[] filled;
    public Sprite[] unfilled;
    public Color color = Color.white;

    [ReadOnly][SerializeField] private Sprite[] current;

    [Foldout("Events", true)]
    [ReadOnly] public Image image;
    [ReadOnly] public SpriteRenderer renderer;
    public UnityEvent<Sprite> onSpriteChange;
    [Foldout("Events")] public UnityEvent<Color> onColorChange;

    private Coroutine coroutine;
    private bool coroutineStarting = false;

    private void Start()
    {
        IsFilled = IsFilled;
    }

    private void Update()
    {
        if (isFilled != prevIsFilled)
        {
            IsFilled = IsFilled;
        }
        if (coroutine == null)
        {
            coroutineStarting = true;
            coroutine = StartCoroutine(Increment());
        }
    }

    public IEnumerator Increment()
    {
        while (coroutine != null || coroutineStarting)
        {
            coroutineStarting = false;

            yield return new WaitForSeconds(cycleRate);
            index = (int)Mathf.Repeat(index+1, current.Length - 1);
            onSpriteChange.Invoke(current[index]);
        }
    }

    public void SetImageSprite(Sprite sprite) { image.sprite = sprite; }
    public void SetImageColor(Color color) { image.color = color; }
    public void SetRendererSprite(Sprite sprite) { renderer.sprite = sprite; }
    public void SetRendererColor(Color color) { renderer.color = color; }

    public void SetIsFilled(bool isFilled)
    {
        this.isFilled = isFilled;
        current = isFilled ? filled : unfilled;
        onSpriteChange.Invoke(current[index]);
    }

    public void SetTransition(float transition)
    {
        this.transition = transition;
        onColorChange.Invoke(new(color.r, color.g, color.b, transition));
    }
}
