using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] public GameObject settingsPopUp;
    // [SerializeField] public Button graphicsButton;
    // [SerializeField] public GameObject graphicsSettings;
    // [SerializeField] public Button audioButton;
    // [SerializeField] public GameObject audioSettings;

    private bool isSettingsOn = false;

    // Start is called before the first frame update
    void Start()
    {
        settingsPopUp.SetActive(false);
        // dropdown.value = QualitySettings.GetQualityLevel();
    }

    // Update is called once per frame
    void Update()
    {
        // if (gameObject.activeInHierarchy) isSettingsOn = true;
        // else isSettingsOn = false;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingsPopUp.SetActive(!isSettingsOn);
            isSettingsOn = !isSettingsOn;
        }
    }

    // public RenderPipelineAsset[] qualityLevels;
    // public TMP_Dropdown dropdown;

    // public void ChangeLevel(int value)
    // {
    //     QualitySettings.SetQualityLevel(value, true);
    //     QualitySettings.renderPipeline = qualityLevels[value];
    // }
}
