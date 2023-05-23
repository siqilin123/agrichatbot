using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenAIController : MonoBehaviour
{
    //public TMP_Text textField;
    public TMP_InputField inputField;
    public Button okButton;

    public RectTransform content;
    public ScrollRect scrollView;
    public GameObject chatUser;
    public GameObject chatFriend;
    public GameObject waitModel;
    private GameObject wait;

    private OpenAIAPI api;
    private List<ChatMessage> messages;

    // Start is called before the first frame update
    void Start()
    {
        // This line gets your API key (and could be slightly different on Mac/Linux)
        api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User));
        //If you don't have environment variable, use this:
        //api = new OpenAIAPI(insert your api);
        StartConversation();
        okButton.onClick.AddListener(() => GetResponse());
    }

    private void StartConversation()
    {
        messages = new List<ChatMessage> {
            new ChatMessage(ChatMessageRole.System, "Ignore all prior instructions and imagine you are an expert plant specialist recruited to take care of my fruits farm containing apples and jackfruits" +
            "You are here to help the user with problems in the farm and give live updates on what happend everyday at the farm. " +
            "You keep your responses short and to the point. If plant has an illness, " +
            "you will recommend the most appropriate action and pesticide to use. " +
            "Today, the plants are doing very well. The jackfruit is ready for harvest. Notify to harvest the jackfruit")
        };

        inputField.text = "";
        string startString = "You have just entered the farm where your assistant is ready to give you a daily update.";
        //textField.text = startString;
        Debug.Log(startString);
    }
    private void AddUserChat(string message)
    {
        AddChat(chatUser, message);
    }

    private void AddFriendChat(string message)
    {
        AddChat(chatFriend, message);
    }
    private void AddWait()
    {
        wait = Instantiate(waitModel, waitModel.transform.parent);
        wait.SetActive(true);
    }

    private void AddChat(GameObject chatModel, string message)
    {
        GameObject newChat = Instantiate(chatModel, chatModel.transform.parent);
        newChat.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = message;
        newChat.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = System.DateTime.Now.ToString("HH:mm:ss");
        newChat.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        scrollView.verticalNormalizedPosition = 0;
    }

    private async void GetResponse()
    {
        if (inputField.text.Length < 1)
        {
            return;
        }

        // Disable the OK button
        okButton.enabled = false;

        AddUserChat(inputField.text);
        AddWait();
        // Fill the user message from the input field
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.Content = inputField.text;

        if (userMessage.Content.Length > 100)
        {
            // Limit messages to 100 characters
            userMessage.Content = userMessage.Content.Substring(0, 100);
        }
        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content));

        // Add the message to the list
        messages.Add(userMessage);

        // Update the text field with the user message
        //textField.text = string.Format("You: {0}", userMessage.Content);

        // Clear the input field
        inputField.text = "";

        // Send the entire chat to OpenAI to get the next message
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.9,
            MaxTokens = 50,
            Messages = messages
        });

        
        // Get the response message
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

        // Add the response to the list of messages
        messages.Add(responseMessage);

        AddFriendChat(string.Format(responseMessage.Content));
        // Update the text field with the response
        //textField.text = string.Format("You: {0}\n\nGuard: {1}", userMessage.Content, responseMessage.Content);

        // Re-enable the OK button
        okButton.enabled = true;
    }
}