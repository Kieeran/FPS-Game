using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_InputWindow : MonoBehaviour
{
    private static UI_InputWindow instance;

    private Button okBtn;
    private Button cancelBtn;

    // private Action ok_Click = null;
    // private Action cancel_Click = null;

    // private TextMeshProUGUI titleText;
    private TMP_InputField inputField;

    private void Awake()
    {
        instance = this;

        // okBtn = transform.Find("okBtn").GetComponent<Button>();
        // cancelBtn = transform.Find("cancelBtn").GetComponent<Button>();
        // titleText = transform.Find("titleText").GetComponent<TextMeshProUGUI>();
        // inputField = transform.Find("inputField").GetComponent<TMP_InputField>();

        //RegisterEvents();

        //Hide();

        okBtn = transform.Find("okBtn").GetComponent<Button>();
        cancelBtn = transform.Find("cancelBtn").GetComponent<Button>();
        inputField = transform.Find("inputField").GetComponent<TMP_InputField>();

        okBtn.onClick.AddListener(() =>
        {
            Hide();
        });

        cancelBtn.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    // private void RegisterEvents()
    // {
    //     okBtn?.onClick.AddListener(() =>
    //     {
    //         ok_Click?.Invoke();
    //     });

    //     cancelBtn?.onClick.AddListener(() =>
    //     {
    //         cancel_Click?.Invoke();
    //     });
    // }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        // {
        //     //okBtn.ClickFunc();
        //     ok_Click?.Invoke();
        // }
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     //cancelBtn.ClickFunc();
        //     cancel_Click?.Invoke();
        // }
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

    //     ok_Click = () =>
    //     {
    //         Hide();
    //         onOk(inputField.text);
    //     };

    //     cancel_Click = () =>
    //     {
    //         Hide();
    //         onCancel();
    //     };
    // }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

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
