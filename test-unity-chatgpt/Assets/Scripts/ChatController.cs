using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;


public class ChatController : MonoBehaviour
{
    public GameObject splash;

    public RectTransform content;
    public ScrollRect scrollView;
    public GameObject chatUser;
    public GameObject chatFriend;
    public GameObject waitModel;
    public TMP_InputField inputField;

    public AudioSource audioSource;

    private TouchScreenKeyboard keyboard;
    private bool blinkStatus;
    private string idToken;
    private GameObject wait;


    private void Start()
    {
        //StartCoroutine(Login());
        InvokeRepeating("Blink", 0, 0.5f);
    }

    private void Update()
    {
        inputField.text = blinkStatus ? "|" : "";

        if (keyboard != null) 
		{
            if (keyboard.status == TouchScreenKeyboard.Status.Done)
			{
                SendMessageToGPT(keyboard.text.Trim());
			}
		}
    }

    private void SendMessageToGPT(string message)
    {       
        if (message != "")
        {            
            AddUserChat(message);
            AddWait();
            //StartCoroutine(PostData(message));
        }            

        HideKeyboard();
    }

    private void AddWait()
    {
        wait = Instantiate(waitModel, waitModel.transform.parent);
        wait.SetActive(true);
    }


 
    private void AddUserChat(string message)
    {
        AddChat(chatUser, message);
    }

    private void AddFriendChat(string message)
    {
        AddChat(chatFriend, message);
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

    public void ShowKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, true, false, false, false);
    }

    public void HideKeyboard()
    {
	    if(keyboard != null)
        {
            keyboard.text = "";
            keyboard.active = false;
            keyboard = null;
	    }
    }

    private void Blink()
    {
        blinkStatus = !blinkStatus;
    }
}
