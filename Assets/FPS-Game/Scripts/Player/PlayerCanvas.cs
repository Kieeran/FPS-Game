using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    public HealthBar HealthBar;
    public HitEffect HitEffect;
    public EscapeUI EscapeUI;
    public Scoreboard Scoreboard;
    public BulletHud BulletHud;
    public WeaponHud WeaponHud;
    public HealRefillAmmoEffect HealRefillAmmoEffect;
    public Image ScopeAim;
    public Image CrossHair;
    [SerializeField] TMP_Text timerNum;
    [SerializeField] TMP_Text locationText;
    public Transform VictoryDefeatPopUp;
    public TMP_Text VictoryDefeatText;

    public Image FadeToBlackEffectImage;
    public float WaitForFadeToBlackEffect;
    public float FadeToBlackDuration;

    void Awake()
    {
        VictoryDefeatPopUp.gameObject.SetActive(false);
        VictoryDefeatText.text = "";
    }

    public void PopUpVictoryDefeat(string text)
    {
        VictoryDefeatPopUp.gameObject.SetActive(true);
        VictoryDefeatText.text = text;
    }

    public void ToggleCrossHair(bool b)
    {
        CrossHair.gameObject.SetActive(b);
    }

    public void ToggleEscapeUI()
    {
        EscapeUI.gameObject.SetActive(!EscapeUI.gameObject.activeSelf);
        Cursor.lockState = !EscapeUI.gameObject.activeSelf ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void ToggleScoreBoard()
    {
        Scoreboard.gameObject.SetActive(!Scoreboard.gameObject.activeSelf);
    }

    public void UpdateTimerNum(int mins, int secs)
    {
        timerNum.text = $"{mins}:{secs:D2}";
    }

    public void UpdateLocationText(string text)
    {
        locationText.text = text.ToUpper();
    }

    public void PlayEndGameFadeOut(Action OnEndEffect)
    {
        StartCoroutine(FadeToBlackRoutine(OnEndEffect));
    }

    private IEnumerator FadeToBlackRoutine(Action OnEndEffect)
    {
        yield return new WaitForSecondsRealtime(WaitForFadeToBlackEffect);

        float elapsed = 0f;
        Color startColor = new(0, 0, 0, 0);
        Color endColor = new(0, 0, 0, 1); // Full đen, không trong suốt

        FadeToBlackEffectImage.gameObject.SetActive(true);
        while (elapsed < FadeToBlackDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Dùng unscaled để không bị ảnh hưởng bởi slow-motion
            float t = Mathf.Clamp01(elapsed / FadeToBlackDuration);
            FadeToBlackEffectImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        FadeToBlackEffectImage.color = endColor; // Đảm bảo kết thúc là đen hẳn

        OnEndEffect?.Invoke();
    }
}