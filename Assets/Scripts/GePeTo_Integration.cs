using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using com.studios.taprobana;

[System.Serializable]
public class GePeTo_Integration : MonoBehaviour
{
    [SerializeField] private Button Button;
    [SerializeField] private TMP_InputField input;
    [SerializeField] private TextMeshProUGUI output;
    [SerializeField] private ApiConfig apiConfig;

    private ChatCompletionsApi chatCompletionsApi;

    public static GePeTo_Integration instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initialise();
        }
        else
            Destroy(gameObject);
    }
    bool _initialised = false;
    private void Initialise()
    {
        if (_initialised)
            return;
    }

    // Start is called before the first frame update
    void Start()
    {
        Button.onClick.AddListener(OnSubmitButtonClicked);
    }

    public void OnSubmitButtonClicked()
    {
        string userPrompt = $"Write a small paragraph of information about the following location: {input.text}";
        SendRequestToAPI(userPrompt);

        // DEBUG
        Debug.Log($"The GePeTo prompt is:\n{userPrompt}");
    }

    private async void SendRequestToAPI(string userPrompt)
    {
        try
        {
            chatCompletionsApi = new(apiConfig.get());
            chatCompletionsApi.ConversationHistoryMemory = 5;
            chatCompletionsApi.SetSystemMessage("You are a tour guide");

            ChatCompletionsRequest chatCompletionsRequest = new ChatCompletionsRequest();
            Message message = new(Roles.USER, userPrompt);
            chatCompletionsRequest.Messages.Clear();

            chatCompletionsRequest.Model = "gpt-3.5-turbo-0125";
            chatCompletionsRequest.AddMessage(message);

            ChatCompletionsResponse res = await chatCompletionsApi.CreateChatCompletionsRequest(chatCompletionsRequest);
            string response = res.GetResponseMessage();

            output.text = response;
            Debug.Log($"GePeTo's Response: {response}");
        }
        catch (OpenAiRequestException exception)
        {
            // exception.Code
            // exception.Param
            // exception.Type
            // exception.Message
            Debug.LogError(exception);
        }
    }
}


