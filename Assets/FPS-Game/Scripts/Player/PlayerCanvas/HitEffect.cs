using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HitEffect : MonoBehaviour
{
    public Image Effect { get; private set; }
    [Header("Hit effect alpha")]
    [SerializeField] float _minorHitEffectAlpha;
    [SerializeField] float _moderateHitEffectAlpha;
    [SerializeField] float _criticalHitEffectAlpha;

    [Header("Damage threshold")]
    [SerializeField] float _minorDamageThreshold;
    [SerializeField] float _moderateDamageThreshold;

    [SerializeField] float _hitEffectDuration;

    void Awake()
    {
        Effect = GetComponent<Image>();
        Effect.color = new Color(1, 0, 0, 0);
    }

    public void StartFadeHitEffect(float damage)
    {
        float hitAlpha;

        if (damage > 0 && damage <= _minorDamageThreshold)
        {
            hitAlpha = _minorHitEffectAlpha;
        }

        else if (damage > _minorDamageThreshold && damage <= _moderateDamageThreshold)
        {
            hitAlpha = _moderateHitEffectAlpha;
        }

        else if (damage > _moderateDamageThreshold)
        {
            hitAlpha = _criticalHitEffectAlpha;
        }

        else return;

        StartCoroutine(FadeHitEffect(Effect, hitAlpha / 255));
    }

    public IEnumerator FadeHitEffect(Image hitEffect, float targetAlpha)
    {
        float timer = 0f;

        while (timer < _hitEffectDuration)
        {
            float alpha = Mathf.Lerp(targetAlpha, 0f, timer / _hitEffectDuration);
            hitEffect.color = new Color(1f, 0f, 0f, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        // Đảm bảo alpha bằng 0 hoàn toàn khi kết thúc
        hitEffect.color = new Color(1f, 0f, 0f, 0f);
    }

    public void ResetHitEffect()
    {
        Effect.color = new Color(1, 0, 0, 0);
    }
}
