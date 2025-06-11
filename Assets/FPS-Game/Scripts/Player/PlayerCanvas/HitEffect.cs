using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HitEffect : MonoBehaviour
{
    public Image Effect { get; private set; }
    [SerializeField] float _hitBodyAlpha;
    [SerializeField] float _hitHeadAlpha;

    void Awake()
    {
        Effect = GetComponent<Image>();
    }

    public void StartFadeHitEffect(string shotType)
    {
        switch (shotType)
        {
            case "Headshot":
                StartCoroutine(FadeHitEffect(Effect, _hitHeadAlpha / 255));
                break;
            case "Torsoshot":
            case "Legshot":
                StartCoroutine(FadeHitEffect(Effect, _hitBodyAlpha / 255));
                break;
            default:
                Debug.Log("Unvalid shotType ");
                break;
        }
    }

    public IEnumerator FadeHitEffect(Image hitEffect, float targetAlpha)
    {
        float currentAlpha = targetAlpha;

        while (currentAlpha > 0)
        {
            hitEffect.color = new Color(1, 0, 0, currentAlpha);
            currentAlpha -= Time.deltaTime;
            yield return null;
        }
    }

    public void ResetHitEffect()
    {
        Effect.color = new Color(1, 0, 0, 0);
    }
}
