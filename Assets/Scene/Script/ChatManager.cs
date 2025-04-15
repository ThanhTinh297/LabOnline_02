using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Fusion;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    [SerializeField] private ChatUI chatUI;
    [SerializeField] private string chatAppId;
    [SerializeField] private string generalChannel = "General";

    private ChatClient chatClient;
    private string username;

    void Start()
    {
        chatUI.OnSendMessage += SendChatMessageToChannel;
    }

    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }

    public void InitializeAndConnect(NetworkRunner runner)
    {
        if (runner == null || runner.LocalPlayer == PlayerRef.None)
        {
            Debug.LogError(
                "ChatManager: InitializeAndConnect được gọi với Runner không hợp lệ hoặc LocalPlayer chưa sẵn sàng.");
            return;
        }

        if (chatClient != null && chatClient.State != ChatState.Disconnected)
        {
            Debug.LogWarning("ChatManager: Đã có kết nối hoặc đang kết nối tới chat.");
            return;
        }

        Debug.Log("ChatManager: Đang kết nối tới Photon Chat thông qua InitializeAndConnect...");
        chatClient = new ChatClient(this);

        username = $"User_{runner.LocalPlayer.PlayerId}";
        chatClient.Connect(chatAppId, "1.0", new AuthenticationValues(username));
    }

    private void SendChatMessageToChannel(string message)
    {
        if (chatClient == null)
        {
            Debug.LogError("ChatManager: Không thể gửi tin nhắn, chatClient là null.");
            return;
        }

        if (!chatClient.CanChat)
        {
            Debug.LogError(
                $"ChatManager: Không thể gửi tin nhắn, chưa kết nối hoặc chưa sẵn sàng. Trạng thái: {chatClient.State}");
            return;
        }

        chatClient.PublishMessage(generalChannel, message);
    }

    public void SendPrivateMessage(string targetUsername, string message)
    {
        if (chatClient == null || !chatClient.CanChat) return;
        chatClient.SendPrivateMessage(targetUsername, message);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnConnected()
    {
        Debug.Log("ChatManager: Kết nối thành công tới Photon Chat!");
        chatClient.Subscribe(new string[] { generalChannel });
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnDisconnected()
    {
        Debug.LogWarning("ChatManager: Đã ngắt kết nối khỏi Photon Chat.");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"ChatManager: Trạng thái Chat thay đổi - {state}");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (chatUI == null)
        {
            Debug.LogError("OnGetMessages: chatUI reference is NULL!");
            return;
        }
        for (int i = 0; i < senders.Length; i++)
        {
            if (messages[i] is string messageContent)
            {
                Debug.Log($"Processing message from {senders[i]}: '{messageContent}'. Calling chatUI.AddMessage.");
                chatUI.AddMessage(senders[i], messageContent);
            }
            else
            {
                Debug.LogWarning(
                    $"ChatManager: Nhận được kiểu tin nhắn không mong đợi từ {senders[i]} trên kênh {channelName}: {messages[i]?.GetType()}");
            }
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("OnSubscribed callback received.");
        for (int i = 0; i < channels.Length; i++)
        {
            if (results[i])
            {
                Debug.Log($"ChatManager: Đã tham gia kênh '{channels[i]}' thành công.");
            }
            else
            {
                Debug.LogError($"ChatManager: KHÔNG THỂ tham gia kênh '{channels[i]}'.");
            }
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }

    private void OnApplicationQuit()
    {
        if (chatClient != null && chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            chatClient.Disconnect();
        }
    }

    private void OnDestroy()
    {
        if (chatClient != null)
        {
            chatClient.Disconnect();
        }

        if (chatUI != null)
        {
            chatUI.OnSendMessage -= SendChatMessageToChannel;
        }
    }
}