using MyBox;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class BossHealthMeterController : MonoBehaviour
{
    
    [SerializeField] private int currentHealth;
    private int previousHealth;
    [SerializeField] private float healthMax;
    private float previousHealthMax;
    [SerializeField] private VisualEffect bossHealthMeter;
    [SerializeField] private VisualEffect bloodRainVFX;
    [SerializeField] private float pipResizeSpeed = 1f;
    [SerializeField] private float bannerMoveSpeed = 1f;
    private float bannerPosition = 0f;
    [SerializeField] bool bannerMove = false;
    [SerializeField][ReadOnly] bool bannerMoveFired = false;
    [SerializeField] bool canMoveBannerBeforeRevealed = false;
    [SerializeField] bool unpauseSceneTimelineOnBannerMove = false;
    private bool bannerSmall = false;
    [SerializeField] bool revealHealthBar = false;
    private bool healthBarRevealed = false;
    private bool healthBarTransition;
    [SerializeField] private float bannerFadeInSpeed = 1f;
    [SerializeField] private float textFadeInSpeed = 1f;
    [SerializeField] private float pipRevealSpeed = 1f;
    [SerializeField] private float pipInitialRevealTime = .01f;
    private float bannerFadePosition = 0f;
    private float textFadePosition = 0f;
    private float portraitFadePosition = 0f;
    [SerializeField] private float pauseBetweenBannerAndText = 1f;
    [SerializeField] private float pauseBetweenTextAndPips = 1f;
    [SerializeField] private bool bannerFadeOut = false;

    private void OnEnable()
    {
        revealHealthBar = true;
    }

    private void OnDisable()
    {
        bannerMoveFired = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        bossHealthMeter.SetFloat("Banner Wipe Position", 0f);
        bloodRainVFX.SetFloat("Banner Wipe Position", 0f);
        bossHealthMeter.SetFloat("Text Wipe Position", 0f);
        bossHealthMeter.SetFloat("Number of Health Pips", 0f);
        bossHealthMeter.SetFloat("Portrait Wipe Position", 0f);
        
        previousHealth = currentHealth;
        previousHealthMax = healthMax;
        bossHealthMeter.SetFloat("Current Health", currentHealth);
        bossHealthMeter.SetFloat("Banner X Pos", bannerPosition);
        bloodRainVFX.SetFloat("Banner X Pos", bannerPosition);
        bossHealthMeter.SetFloat("Text X Pos", bannerPosition);
        bossHealthMeter.SetFloat("Text Y Pos", bannerPosition);
        bossHealthMeter.SetFloat("Text Size", bannerPosition);
        bossHealthMeter.SetFloat("Text Angle", bannerPosition);
        bossHealthMeter.SetBool("No Subtext", false);
        bossHealthMeter.SetBool("No Description", false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(revealHealthBar && !healthBarRevealed && !healthBarTransition)
        {
            healthBarTransition = true;
            HealthBarReveal();
        }
        
        if(healthBarRevealed)
        {
            bossHealthMeter.SetFloat("Number of Health Pips", healthMax);
            

            if (currentHealth != previousHealth)
            {
                StartCoroutine(PipChange(previousHealth, currentHealth));
            }

            if (bannerMove)
            {
                StartCoroutine(BannerMove());
            }

            if (healthMax != previousHealthMax)
            {
                ResetPips();
            }
            if (bannerFadeOut && healthBarRevealed && !healthBarTransition)
            {
                StartCoroutine(BannerHide());
            }        
        }
    }

    IEnumerator PipChange(int old, int current)
    {
        previousHealth = currentHealth;
        float resizePosition = 0f;
        bool healthUpdated = false;
        bossHealthMeter.SetFloat("Old Health", old);
        bossHealthMeter.SetFloat("New Health", current);
        while (resizePosition < 1f && old > current)
        {                           
            bossHealthMeter.SetFloat("Pip Resize Position", resizePosition);
            if (resizePosition > .50f && !healthUpdated)
            {
                bossHealthMeter.SetFloat("Current Health", current);
                healthUpdated = true;
            }               
                                       
            resizePosition += .05f;
            yield return new WaitForSeconds(pipResizeSpeed*Time.deltaTime);
        }

        while (resizePosition < 1f && current > old)
        {                                               
            bossHealthMeter.SetFloat("Pip Resize Position", resizePosition);
            if (resizePosition > .50f && !healthUpdated)
            {
                bossHealthMeter.SetFloat("Current Health", current);
                healthUpdated = true;
            }               
                                       
            resizePosition += .05f;
            yield return new WaitForSeconds(pipResizeSpeed*Time.deltaTime);        
        }
    }

    public void BannerBegin()
    {
        revealHealthBar = true;
    }

    public void BannerSlide()
    {
        bannerMove = true;
    }

    public void BannerRemove()
    {
        bannerFadeOut = true;
    }

    [ButtonMethod]
    public void TriggerBannerMove()
    {
        if (isActiveAndEnabled && (canMoveBannerBeforeRevealed || !healthBarRevealed))
        {
            if (!bannerMoveFired)
            {
                print("Triggered Banner Move!");
                bannerMove = true;
                bannerMoveFired = true;
                if (unpauseSceneTimelineOnBannerMove && SceneTimeline.main)
                {
                    SceneTimeline.main.UnPause();
                }
            }
            else
            {
                print("Banner Move not Triggered-- already fired!");
            }
        }
        else print("Ignoring, because not active/revealed.");
    }

    IEnumerator BannerMove()
    {
        bannerMove = false;
        if (!bannerSmall)
        {        
            bossHealthMeter.SetBool("No Subtext", true);
            bossHealthMeter.SetBool("No Description", true);
            
            while (bannerPosition < 1f)
            {
                bossHealthMeter.SetFloat("Banner X Pos", bannerPosition);
                bloodRainVFX.SetFloat("Banner X Pos", bannerPosition);
                bossHealthMeter.SetFloat("Text X Pos", bannerPosition);
                bossHealthMeter.SetFloat("Text Y Pos", bannerPosition);
                bossHealthMeter.SetFloat("Text Size", bannerPosition);
                bossHealthMeter.SetFloat("Text Angle", bannerPosition);
                bannerPosition += .05f;
                if (portraitFadePosition > 0f)
                {
                    portraitFadePosition -= .05f;
                    bossHealthMeter.SetFloat("Portrait Wipe Position", portraitFadePosition);
                }
                yield return new WaitForSeconds(bannerMoveSpeed*Time.deltaTime);
            }
            bannerSmall = true;
            GetComponent<SortingGroup>().sortingOrder = -2;
        }
        else
        {
            GetComponent<SortingGroup>().sortingOrder = 2;
            while (bannerPosition > 0f)
            {
                bossHealthMeter.SetFloat("Banner X Pos", bannerPosition);
                bloodRainVFX.SetFloat("Banner X Pos", bannerPosition);
                bossHealthMeter.SetFloat("Text X Pos", bannerPosition);
                bossHealthMeter.SetFloat("Text Y Pos", bannerPosition);
                bossHealthMeter.SetFloat("Text Size", bannerPosition);
                bossHealthMeter.SetFloat("Text Angle", bannerPosition);
                bannerPosition -= .05f;
                if (portraitFadePosition < 1f)
                {
                    portraitFadePosition += .05f;
                    bossHealthMeter.SetFloat("Portrait Wipe Position", portraitFadePosition);
                }
                yield return new WaitForSeconds(bannerMoveSpeed*Time.deltaTime);
            }
            bannerSmall = false;
            bossHealthMeter.SetBool("No Subtext", false);
            bossHealthMeter.SetBool("No Description", false);
        }
    }

    private void ResetPips()
    {
        bossHealthMeter.Reinit();
        previousHealthMax = healthMax;
    }

    private void HealthBarReveal()
    {       
        bannerFadePosition = 0f;
        textFadePosition = 0f;
        bossHealthMeter.SetFloat("Banner Wipe Position", bannerFadePosition);
        bloodRainVFX.SetFloat("Banner Wipe Position", bannerFadePosition);
        bossHealthMeter.SetFloat("Text Wipe Position", textFadePosition);
        bossHealthMeter.SetFloat("Number of Health Pips", 0f);

        StartCoroutine(BannerFadeIn());
    }

    IEnumerator BannerFadeIn()
    {
        while (bannerFadePosition < 1f)
        {
            bannerFadePosition += .05f;
            bossHealthMeter.SetFloat("Banner Wipe Position", bannerFadePosition);
            bloodRainVFX.SetFloat("Banner Wipe Position", bannerFadePosition);
            yield return new WaitForSeconds(bannerFadeInSpeed*Time.deltaTime);
        }
        bannerFadePosition = 1f;
        bossHealthMeter.SetFloat("Banner Wipe Position", bannerFadePosition);
        bloodRainVFX.SetFloat("Banner Wipe Position", bannerFadePosition);
        yield return new WaitForSeconds(pauseBetweenBannerAndText);
        StartCoroutine(TextFadeIn());
    }

    IEnumerator TextFadeIn()
    {
        while (textFadePosition < 1f && portraitFadePosition < 1f)
        {
            textFadePosition += .05f;
            portraitFadePosition += .05f;
            bossHealthMeter.SetFloat("Text Wipe Position", textFadePosition);
            bossHealthMeter.SetFloat("Portrait Wipe Position", portraitFadePosition);
            yield return new WaitForSeconds(textFadeInSpeed*Time.deltaTime);
        }
        textFadePosition = 1f;
        portraitFadePosition = 1f;
        bossHealthMeter.SetFloat("Text Wipe Position", textFadePosition);
        bossHealthMeter.SetFloat("Portrait Wipe Position", portraitFadePosition);
        yield return new WaitForSeconds(pauseBetweenTextAndPips);
        StartCoroutine(PipFadeIn());
    }

    IEnumerator PipFadeIn()
    {
        Debug.Log("Pips Section");
        bossHealthMeter.SetBool("Initial Reveal", true);

        for (float i = 1f; i < healthMax + 1; i++)
        {
            float resizePosition = 0f;
            bossHealthMeter.SetFloat("Number of Health Pips", i);
            bossHealthMeter.SetFloat("Old Health", i - 1);
            bossHealthMeter.SetFloat("New Health", i);
            bossHealthMeter.Reinit();

            float startTime = Time.time;
            while (resizePosition < 1f)
            {
                resizePosition = Mathf.Lerp(0f, 1f, (Time.time - startTime) / pipInitialRevealTime);
                bossHealthMeter.SetFloat("Pip Resize Position", resizePosition);
                yield return null;
            }

            yield return new WaitForSeconds(pipRevealSpeed);
        }
        bossHealthMeter.SetFloat("Number of Health Pips", healthMax);
        healthBarRevealed = true;
        healthBarTransition = false;
        bossHealthMeter.SetBool("Initial Reveal", true);
    }

    IEnumerator BannerHide()
    {
        healthBarTransition = true;
        bossHealthMeter.SetFloat("Number of Health Pips", 0f);
        ResetPips();
        while (bannerFadePosition > 0f || textFadePosition > 0f)
        {
            Debug.Log("Fading Slowly...");
            bannerFadePosition -= .05f;
            textFadePosition -= .05f;
            bossHealthMeter.SetFloat("Banner Wipe Position", bannerFadePosition);
            bossHealthMeter.SetFloat("Text Wipe Position", textFadePosition);
            if (portraitFadePosition > 0f)
                {
                    portraitFadePosition -= .05f;
                    bossHealthMeter.SetFloat("Portrait Wipe Position", portraitFadePosition);
                }
            yield return new WaitForSeconds(textFadeInSpeed*Time.deltaTime);
        }
        bannerFadePosition = 0f;
        textFadePosition = 0f;
        portraitFadePosition = 0f;
        bossHealthMeter.SetFloat("Banner Wipe Position", bannerFadePosition);
        bloodRainVFX.SetFloat("Banner Wipe Position", bannerFadePosition);
        bossHealthMeter.SetFloat("Text Wipe Position", textFadePosition);
        bossHealthMeter.SetFloat("Portrait Wipe Position", portraitFadePosition);
        
        healthBarRevealed = false;
        
        GetComponent<SortingGroup>().sortingOrder = 2;
        bannerPosition = 0f;  
        bossHealthMeter.SetFloat("Banner X Pos", bannerPosition);
        bloodRainVFX.SetFloat("Banner X Pos", bannerPosition);
        bossHealthMeter.SetFloat("Text X Pos", bannerPosition);
        bossHealthMeter.SetFloat("Text Y Pos", bannerPosition);
        bossHealthMeter.SetFloat("Text Size", bannerPosition);
        bossHealthMeter.SetFloat("Text Angle", bannerPosition);
        bannerSmall = false;
        bossHealthMeter.SetBool("No Subtext", false);
        bossHealthMeter.SetBool("No Description", false);
        bannerFadeOut = false;
        revealHealthBar = false;
        healthBarTransition = false;
    }

}
