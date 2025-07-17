using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class FrameManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject dialogPanel;
    public GameObject textPanel, choicePanel, finalPanel;
    public GameObject clickCatcher;

    [Header("Dialog")]
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI characterName;
    public Image leftCharacterImage;
    public Image rightCharacterImage;

    [Header("Text")]
    public TextMeshProUGUI text;

    [Header("Choice")]
    public TextMeshProUGUI choiceTitle;
    public Transform optionsContainer;
    public GameObject optionButtonPrefab;

    [Header("Final")]
    public TextMeshProUGUI finalText;

    private Dictionary<string, Frame> frameDict;
    private Frame currentFrame;

    private void Start()
    {
        LoadFrames();
        ShowFrame("start");
    }

    private void LoadFrames()
    {
        TextAsset json = Resources.Load<TextAsset>("frames");
        FrameList list = JsonUtility.FromJson<FrameList>(json.text);

        frameDict = new Dictionary<string, Frame>();

        foreach (var f in list.frames)
        {
            if (Enum.TryParse<FrameType>(f.type, out FrameType parsedType))
            {
                f.frameType = parsedType;
            }
            else
            {
                Debug.LogWarning($"Unknown frame type '{f.type}' in frame {f.id}, defaulting to Dialog");
                f.frameType = FrameType.Dialog;
            }
            frameDict[f.id] = f;
        }
    }
    private void SetAllPanelsInactive()
    {
        dialogPanel.SetActive(false);
        textPanel.SetActive(false);
        choicePanel.SetActive(false);
        finalPanel.SetActive(false);
    }

    public void OnGlobalClick()
    {
        if (currentFrame.frameType == FrameType.Dialog || currentFrame.frameType == FrameType.Text || currentFrame.frameType == FrameType.Final)
        {
            if (!string.IsNullOrEmpty(currentFrame.nextId))
                ShowFrame(currentFrame.nextId);
            else
                Debug.LogWarning("No nextId specified for this frame.");
        }
    }

    public void ShowFrame(string id)
    {
        currentFrame = frameDict[id];
        clickCatcher.SetActive(currentFrame.frameType != FrameType.Choice);

        SetAllPanelsInactive();

        switch (currentFrame.frameType)
        {
            case FrameType.Dialog:
                dialogPanel.SetActive(true);
                dialogText.text = currentFrame.text;
                characterName.text = currentFrame.characterName;

                leftCharacterImage.gameObject.SetActive(false);
                rightCharacterImage.gameObject.SetActive(false);

                Sprite sprite = Resources.Load<Sprite>(currentFrame.spritePath);

                if (currentFrame.characterPosition == "left")
                {
                    leftCharacterImage.gameObject.SetActive(true);
                    leftCharacterImage.sprite = sprite;
                }
                else if (currentFrame.characterPosition == "right")
                {
                    rightCharacterImage.gameObject.SetActive(true);
                    rightCharacterImage.sprite = sprite;
                }
                break;

            case FrameType.Text:
                textPanel.SetActive(true);
                text.text = currentFrame.text;
                break;

            case FrameType.Choice:
                choicePanel.SetActive(true);
                choiceTitle.text = currentFrame.text;

                foreach (Transform child in optionsContainer)
                    Destroy(child.gameObject);

                foreach (var option in currentFrame.options)
                {
                    GameObject button = Instantiate(optionButtonPrefab, optionsContainer);
                    button.GetComponentInChildren<TextMeshProUGUI>().text = option.title;
                    string capturedId = option.nextFrameId;
                    button.GetComponent<Button>().onClick.AddListener(() => ShowFrame(capturedId));
                }
                break;

            case FrameType.Final:
                finalPanel.SetActive(true);
                finalText.text = currentFrame.text;
                break;
        }
    }
}