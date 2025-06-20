using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealRefillAmmoEffect : MonoBehaviour
{
    [SerializeField] Color _color;
    [SerializeField] Image _effect;
    [SerializeField] float _effectDuration;

    void Start()
    {
        _effect.color = new Color(1, 0, 0, 0);
    }

    public void StartEffect()
    {
        _effect.color = _color;
        StartCoroutine(FadeHitEffect());
    }

    public IEnumerator FadeHitEffect()
    {
        float timer = 0f;

        while (timer < _effectDuration)
        {
            float alpha = Mathf.Lerp(_color.a, 0f, timer / _effectDuration);

            _effect.color = new Color(_color.r, _color.g, _color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        // Đảm bảo alpha bằng 0 hoàn toàn khi kết thúc
        _effect.color = new Color(1f, 0f, 0f, 0f);
    }

    public void ResetHitEffect()
    {
        _effect.color = new Color(1, 0, 0, 0);
    }
}
