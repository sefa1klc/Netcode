using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPopUpManager : MonoBehaviour
{
    [Header("YOU DIED pop up")]
    [SerializeField] GameObject _youDiedPopUpGameObject;
    [SerializeField] TextMeshProUGUI _youDiedPopUpBackgroundText;
    [SerializeField] TextMeshProUGUI _youDiedPopUpText;
    [SerializeField] CanvasGroup _youDiedPopUpCanvasGroup; // allow to set alpha to fade over time

    public void SenYouDiedPopUp()
    {
        _youDiedPopUpGameObject.SetActive(true);
        _youDiedPopUpBackgroundText.characterSpacing = 0;

        StartCoroutine(StretchPopUpTextOverTime(_youDiedPopUpBackgroundText,8, 19f));
        StartCoroutine(FadeInPopUpOverTime(_youDiedPopUpCanvasGroup,5));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(_youDiedPopUpCanvasGroup,2,5));
    }

    private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
    {
        if (duration > 0F)
        {
            text.characterSpacing = 0;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer = timer + Time.deltaTime;
                text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                yield return null;
            }
        }
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvasGroup, float duration)
    {
        if(duration > 0)
        {
            canvasGroup.alpha = 0;
            float timer = 0;

            yield return null;

            while(timer < duration)
            {
                timer = timer + Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha,1,duration*Time.deltaTime);
                yield return null;
            }
        }

        canvasGroup.alpha = 1;

        yield return null;
    }

    private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvasGroup,float duration, float delay)
    {
        if (duration > 0)
        {

            while (delay > 0)
            {
                delay -= Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 1;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer = timer + Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, duration * Time.deltaTime);
                yield return null;
            }
        }

        canvasGroup.alpha = 0;

        yield return null;
    }

}
