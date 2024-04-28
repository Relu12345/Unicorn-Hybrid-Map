using System.Collections;
using System.IO;
using HuggingFace.API;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Speech2Text : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_InputField input;

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

public static Speech2Text instance;
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


    private void Start()
    {
        startButton.onClick.AddListener(StartRecording);
    }

    private void Update()
    {
        if (recording && Microphone.GetPosition(null) >= clip.samples)
        {
            StopRecording();
        }
    }

    public void StartRecording()
    {
        //outputText.color = Color.white;
        input.text = "Recording...";
        startButton.interactable = false;
        StartCoroutine(RecordForSeconds(1f));
    }

    private IEnumerator RecordForSeconds(float duration)
    {
        //yield return new WaitForSeconds(duration + 2);
        clip = Microphone.Start(null, false, (int)duration, 44100);
        yield return new WaitForSeconds(duration);
        recording = true;
        StopRecording();
    }

    private void StopRecording()
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    public void SendRecording()
    {
        //outputText.color = Color.yellow;
        input.text = "Processing...";
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            //outputText.color = Color.white;
            input.text = response;
            GePeTo_Integration.instance.OnSubmitButtonClicked();
            startButton.interactable = true;
        }, error => {
            //outputText.color = Color.red;
            input.text = error;
            startButton.interactable = true;
        });
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}
