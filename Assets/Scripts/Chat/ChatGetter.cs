using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Common.DesignPatterns;
using UnityEngine.Events;

/// <summary>
/// This class collects the chats 
/// </summary>
public class ChatGetter : Singleton<ChatGetter>
{
    // All the nodes to be displayed
    private List<ChatNode> _currentNodes;
    // Current index for the chat (order)
    private int _currentIndex;
    // To display the chat UI
    [SerializeField] private ChatSectionUIDisplay _chatDisplayUI;
    // The current (or latest) speaker
    private Speaker _speaker;
    // The container containing all the nodes
    [SerializeField] private ChatList _chatListContainer;
    // The container containing all the speakers
    [SerializeField] private SpeakerList _speakerListContainer;
    [Tooltip("Next Button")][SerializeField] private GameObject _button;
    private UnityEvent _postSpeakingAction;

    // Start the chatting system
    public void StartChat(string ID) {
        // Get the nodes, the questions, and the speakers
        _currentNodes = GetChatList(ID);
        _currentIndex = 0;
        _speaker = GetSpeaker(_currentNodes[_currentIndex].Speaker);
        _button.SetActive(true);

        // Start the chat UI 
        // Display the first node
        if (MainGameManager.Instance.GameState.GetValue() == MainGameState.CHAT)
        {
        }

        _chatDisplayUI.DisplayChatText(_currentNodes[_currentIndex], _speaker);



    }

    public void StartChat(string ID, UnityEvent afterSpeakingAction) {
        StartChat(ID);
        _postSpeakingAction = afterSpeakingAction;
    }

    // Get the next chat node
    public void GetNext() {
        _currentIndex++;
        if (_currentIndex == _currentNodes.Count) {
            // current index is the same as the end of the current node..
            // Stop talking
            _currentIndex = 0;
            

            if (MainGameManager.Instance.GameState.GetValue() == MainGameState.CHAT)
            {
                _button.SetActive(false);
                _chatDisplayUI.CloseAllChatDisplay();
                MainGameManager.Instance.StartGameAnimation();
            }

        }
        else {
            // Get the node speaker
            // Display the chat node text
            _speaker = GetSpeaker(_currentNodes[_currentIndex].Speaker);
            _chatDisplayUI.DisplayChatText(_currentNodes[_currentIndex], _speaker);

        }
    }

    public List<ChatNode> GetChatList(string check_ID) 
    {
        // Count how many chat nodes with the ID exist
        int count = GetNumChats(check_ID);
        if (count == 0) {
            // if there are no results; display a warning 
            Debug.LogWarning("Requested ID of "+ check_ID + " returned no results. Are you sure the ID is correct?");
            return null;
        }
        
        // get a list of chatnodes, ordered by their order
        var result = _chatListContainer.ChatNodeList.Where(s => s.ID == check_ID).ToList().OrderBy(s => s.Order);
        return result.ToList();
    }

    public Speaker GetSpeaker(string speakerId) 
    {
        // Count how many speakers exist with the speakerID
        int count = _speakerListContainer.speakerList.Where(s => s.SpeakerId.ToString() == speakerId).Count();
        if (count == 0) 
        {
            // if there are no results; display a warning
            Debug.LogWarning("Requested ID of "+ speakerId + " returned no results. Are you sure the Speaker name is correct?");
            return null;
        }

        // Get the first instance of the speaker (if there exists more than 1)
        Speaker result = _speakerListContainer.speakerList.Where(s => s.SpeakerId.ToString() == speakerId).First();
        return result;
    }

    

    public string getChatID()
    {
        return _currentNodes[_currentIndex].ID;
    }
    
    private int GetNumChats(string ID)
    {
        return _chatListContainer.ChatNodeList.Count(p => p.ID == ID);
    }

}
