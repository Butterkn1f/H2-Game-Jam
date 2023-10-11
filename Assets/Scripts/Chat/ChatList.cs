using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom data containers/Chat List")]
public class ChatList : ScriptableObject
{
    // Custom container to store all the chat nodes
    public List<ChatNode> ChatNodeList;
}
