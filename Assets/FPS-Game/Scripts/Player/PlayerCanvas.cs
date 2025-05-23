using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    public HealthBar HealthBar;
    public HitEffect HitEffect;
    public EscapeUI EscapeUI;
    public Scoreboard Scoreboard;
    public Image ScopeAim;

    public void ToggleEscapeUI()
    {
        EscapeUI.gameObject.SetActive(!EscapeUI.gameObject.activeSelf);
        Cursor.lockState = !EscapeUI.gameObject.activeSelf ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void ToggleScoreBoard()
    {

    }
}
