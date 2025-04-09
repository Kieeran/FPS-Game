using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadEffect : MonoBehaviour
{
    Image _reloadUI;

    [SerializeField] float _fillAmountOffset;
    [SerializeField] float _alphaOffset;
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

    public void StartReloadEffect(System.Action onDone)
    {
        gameObject.SetActive(true);

        StartCoroutine(FillReloadUI(onDone));
    }

    IEnumerator FillReloadUI(System.Action onDone)
    {
        while (_reloadUI.fillAmount < 1f)
        {
            _reloadUI.fillAmount += _fillAmountOffset;
            yield return null;
        }

        _reloadUI.fillAmount = 1f;
        StartCoroutine(FadeOutReloadUI(onDone));
    }

    IEnumerator FadeOutReloadUI(System.Action onDone)
    {
        Color color = _reloadUI.color;

        while (color.a > 0f)
        {
            color.a -= _alphaOffset;
            _reloadUI.color = color;

            yield return null;
        }

        color.a = 0f;
        _reloadUI.color = color;

        ResetReloadUI();
        onDone?.Invoke();
    }
}
