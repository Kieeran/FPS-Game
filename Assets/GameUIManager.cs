using System.Collections;
using System.Collections.Generic;
using QFSW.QC.Actions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] public GameObject exitGamePopUp;
    [SerializeField] public Button graphicsButton;
    [SerializeField] public GameObject graphicsSettings;
    [SerializeField] public Button audioButton;
    [SerializeField] public GameObject audioSettings;

    // Start is called before the first frame update
    void Start()
    {
        exitGamePopUp.SetActive(false);
        dropdown.value = QualitySettings.GetQualityLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitGamePopUp.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            graphicsButton.onClick.AddListener(() =>
            {
                graphicsSettings.SetActive(true);
                audioSettings.SetActive(false);
            });

            audioButton.onClick.AddListener(() =>
            {
                graphicsSettings.SetActive(false);
                audioSettings.SetActive(true);
            });
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            exitGamePopUp.SetActive(false);
        }
    }

    public RenderPipelineAsset[] qualityLevels;
    public TMP_Dropdown dropdown;

    public void ChangeLevel(int value)
    {
        QualitySettings.SetQualityLevel(value, true);
        QualitySettings.renderPipeline = qualityLevels[value];
    }
}
