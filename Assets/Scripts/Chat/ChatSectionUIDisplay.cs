using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatSectionUIDisplay : MonoBehaviour
{
    #region UI element variables
    [Tooltip("Overall panel of the chat UI")] [SerializeField] private GameObject _panel;
    [Tooltip("Overall container for self chat")] [SerializeField] private GameObject _selfChatItem;
    [Tooltip("Overall container for other people chat")] [SerializeField] private GameObject _otherChatItem;
    [Tooltip("Overall container for other people chat")] [SerializeField] private TextMeshProUGUI _selfChatSpeakerName;
    [Tooltip("Overall container for other people chat")] [SerializeField] private TextMeshProUGUI _otherChatSpeakerName;
    [Tooltip("Overall container for other people chat")] [SerializeField] private TextMeshProUGUI _selfMainBody;
    [Tooltip("Overall container for other people chat")] [SerializeField] private TextMeshProUGUI _otherMainBody;

    // TODO: make this spine 
    [Tooltip("Character mood sprite")] [SerializeField] private Image _selfImage;
    [Tooltip("Character mood sprite")] [SerializeField] private Image _otherImage;
    #endregion

    // This functions displays a chat node, given a chat node and speaker
    public void DisplayChatText(ChatNode chatNode, Speaker speaker) {



        if (speaker.SpeakerId == SpeakerId.TOAST)
        {
            // Set some gameobjects true and false
            // Fuck this project
            if (_selfChatItem.activeInHierarchy == false)
            {
                _selfChatItem.SetActive(true);
            }

            _otherChatItem.SetActive(false);

            // retrieve the mood sprite from the speaker class based on the mood indicated in the CSV file
            if ((int)chatNode.Mood <= speaker.moodImages.Count)
            {
                _selfImage.sprite = speaker.moodImages[(int)chatNode.Mood];
            }
            else
            {
                _selfImage.sprite = speaker.moodImages[0];
            }

            // TODO: Animate the text 

            _selfChatSpeakerName.text = speaker.name;
            _selfMainBody.text = chatNode.BodyText;
        }
        else
        {
            _otherChatItem.SetActive(true);
            _selfChatItem.SetActive(false);

            // retrieve the mood sprite from the speaker class based on the mood indicated in the CSV file
            if ((int)chatNode.Mood <= speaker.moodImages.Count)
            {
                _otherImage.sprite = speaker.moodImages[(int)chatNode.Mood];
            }
            else 
            {
                _otherImage.sprite = speaker.moodImages[0];  
            }
            _otherMainBody.text = chatNode.BodyText;
            _otherChatSpeakerName.text = speaker.name;
        }
    }

    // Close all the chat displays (for chat end)
    public void CloseAllChatDisplay()
    {
        // Animate the chat display closing
        _otherChatItem.SetActive(false);
        _selfChatItem.SetActive(false);

    }

    // Open all the chat displays (for chat end)
    public void OpenAllChatDisplay()
    {
        // Animate the chat display opening   
        _panel.SetActive(true);

    }
}
