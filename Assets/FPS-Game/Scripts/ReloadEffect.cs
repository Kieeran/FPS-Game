using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReloadEffect : MonoBehaviour
{
    Image _reloadUI;

    [SerializeField] float _fadeOutDuration = 0.3f;
    float _startAlpha;

    void Start()
    {
        _reloadUI = GetComponent<Image>();
        _startAlpha = _reloadUI.color.a;
        ResetReloadUI();
    }

    void ResetReloadUI()
    {
        gameObject.SetActive(false);
        _reloadUI.color = new Color(1f, 1f, 1f, _startAlpha);
        _reloadUI.fillAmount = 0;
    }

    public void StartReloadEffect(float reloadDuration, System.Action onDone)
    {
        gameObject.SetActive(true);
        StartCoroutine(FillReloadUI(reloadDuration, onDone));
    }

    IEnumerator FillReloadUI(float duration, System.Action onDone)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _reloadUI.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        _reloadUI.fillAmount = 1f;
        onDone?.Invoke();

        StartCoroutine(FadeOutReloadUI());
    }

    IEnumerator FadeOutReloadUI()
    {
        Color color = _reloadUI.color;
        float elapsed = 0f;

        while (elapsed < _fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(_startAlpha, 0f, elapsed / _fadeOutDuration);
            _reloadUI.color = color;
            yield return null;
        }

        color.a = 0f;
        _reloadUI.color = color;

        ResetReloadUI();
    }
}