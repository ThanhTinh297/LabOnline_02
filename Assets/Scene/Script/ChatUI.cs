using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI chatContentText;
    [SerializeField] private Button sendButton;
    [SerializeField] private ScrollRect chatScrollRect;

    public Action<string> OnSendMessage;
    public bool IsChatInputActive => inputField != null && inputField.isFocused;
    void Start()
    {
        sendButton.onClick.AddListener(TrySendMessage);

        if (inputField != null)
        {
            inputField.onSubmit.AddListener((Text) =>
            {
                if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
                {
                    TrySendMessage();
                }
            });
        }
        if (chatContentText != null)
        {
            chatContentText.text = "";
        }
    }

    private void TrySendMessage()
    {
        if (inputField == null) return;

        string message = inputField.text;
        if (!string.IsNullOrWhiteSpace(message))
        {
            OnSendMessage?.Invoke(message);
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    public void AddMessage(string sender, string message)
    {
        if (chatContentText == null) return;
        chatContentText.text += $"<b>{sender}: <b>{message}\n";

        if (chatScrollRect != null)
        {
            StartCoroutine(ScrollToButton());
        }
    }

    private System.Collections.IEnumerator ScrollToButton()
    {
        yield return new WaitForEndOfFrame();
        if (sendButton != null)
        {
            chatScrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private void OnDestroy()
    {
        if (sendButton != null)
        {
            sendButton.onClick.RemoveListener(TrySendMessage);
        }

        if (inputField != null)
        {
            inputField.onSubmit.RemoveAllListeners();
        }
    }
}
