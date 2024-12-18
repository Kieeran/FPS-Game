using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class _UI_InputWindow : MonoBehaviour
{
    public static _UI_InputWindow instance;

    private Button okBtn;
    private Button cancelBtn;
    private TextMeshProUGUI titleText;
    private TMP_InputField inputField;

    private void Awake()
    {
        instance = this;

        okBtn = transform.Find("okBtn").GetComponent<Button>();
        cancelBtn = transform.Find("cancelBtn").GetComponent<Button>();
        titleText = transform.Find("titleText").GetComponent<TextMeshProUGUI>();
        inputField = transform.Find("inputField").GetComponent<TMP_InputField>();

        RegisterEvents();

        Hide();
    }

    private void RegisterEvents()
    {
        okBtn.onClick.AddListener(() =>
        {
            Hide();
        });

        cancelBtn.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            //okBtn.ClickFunc();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //cancelBtn.ClickFunc();
        }
    }

    public void GetInputField()
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    // private void Show(string titleString, string inputString, string validCharacters, int characterLimit, Action onCancel, Action<string> onOk)
    // {
    //     gameObject.SetActive(true);
    //     transform.SetAsLastSibling();

    //     titleText.text = titleString;

    //     inputField.characterLimit = characterLimit;
    //     inputField.onValidateInput = (string text, int charIndex, char addedChar) =>
    //     {
    //         return ValidateChar(validCharacters, addedChar);
    //     };

    //     inputField.text = inputString;
    //     inputField.Select();

    //     // okBtn.ClickFunc = () =>
    //     // {
    //     //     Hide();
    //     //     onOk(inputField.text);
    //     // };

    //     // cancelBtn.ClickFunc = () =>
    //     // {
    //     //     Hide();
    //     //     onCancel();
    //     // };

    //     okBtn.onClick.AddListener(() =>
    //     {
    //         Hide();
    //         onOk(inputField.text);
    //     });

    //     cancelBtn.onClick.AddListener(() =>
    //     {
    //         Hide();
    //         onCancel();
    //     });
    // }

    // private char ValidateChar(string validCharacters, char addedChar)
    // {
    //     if (validCharacters.IndexOf(addedChar) != -1)
    //     {
    //         // Valid
    //         return addedChar;
    //     }
    //     else
    //     {
    //         // Invalid
    //         return '\0';
    //     }
    // }

    // public static void Show_Static(string titleString, string inputString, string validCharacters, int characterLimit, Action onCancel, Action<string> onOk)
    // {
    //     instance.Show(titleString, inputString, validCharacters, characterLimit, onCancel, onOk);
    // }

    // public static void Show_Static(string titleString, int defaultInt, Action onCancel, Action<int> onOk)
    // {
    //     instance.Show(titleString, defaultInt.ToString(), "0123456789-", 20, onCancel,
    //         (string inputText) =>
    //         {
    //             // Try to Parse input string
    //             if (int.TryParse(inputText, out int _i))
    //             {
    //                 onOk(_i);
    //             }
    //             else
    //             {
    //                 onOk(defaultInt);
    //             }
    //         }
    //     );
    // }
}