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

    public Image ScopeAim;
    public Image CrossHair;

    public TMP_Text timerNum;

    public Transform VictoryDefeatPopUp;
    public TMP_Text VictoryDefeatText;

    void Awake()
    {
        VictoryDefeatPopUp.gameObject.SetActive(false);
        VictoryDefeatText.text = "AAAAA";
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
}