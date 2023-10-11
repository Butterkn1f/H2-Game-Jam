using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace ChatSys
{
    // This class reads different CSV files
    public class CSVReader : MonoBehaviour
    {
        [SerializeField] private string _chatFileName;
        [SerializeField] private ChatList _chatListContainer;

        // Start is called before the first frame update
        void Awake()
        {
            _chatListContainer.ChatNodeList.Clear(); 
            ReadCSV();
        }

        // Clear log after runtime because then its gna be fuckeddddddd 
        void OnApplicationQuit() {
            _chatListContainer.ChatNodeList.Clear(); 
        }

        // Function to read CSV files
        void ReadCSV() 
        {
            // using textAsset implementation
            TextAsset CSVFile = Resources.Load<TextAsset>(_chatFileName);
            if(CSVFile != null)
            {
                StreamReader reader = new StreamReader(new MemoryStream(CSVFile.bytes)); 
                bool EOF = false;

                while (!EOF) {
                    string data_string = reader.ReadLine();
                    if (data_string == null) {
                        EOF = true;
                        break;
                    }

                    // store in a chat node
                    ChatNode temp_chatNode = new ChatNode();
                    string[] dataValues = data_string.Split(",");

                    if (dataValues[0] == null || dataValues[1] == null || dataValues[2] == null) {
                        Debug.LogWarning("Incorrect formatting of CSV files, certain fields missing. Check your formatting of the file.");
                        continue;
                    }


                    if (dataValues[0] != null) {
                        // Get ID
                        // Check if there is a proper ID (starting with pound sign)
                        if (dataValues[0][0] == '#') {
                            temp_chatNode.ID = dataValues[0];
                        }
                    }
                    else
                    {
                        // invalid ID, therefore there is no chat here return 
                        return;
                    }

                    if (dataValues[1] != null)
                    {
                        // Get order
                        temp_chatNode.Order = uint.Parse(dataValues[1]);
                    }

                    if (dataValues[2] != null)
                    {
                        // Get speaker name
                        temp_chatNode.Speaker = dataValues[2];
                    }

                    if (dataValues[3] != null)
                    {
                        // Get sprite mood
                        temp_chatNode.Mood = (int.Parse(dataValues[3]));
                    }

                    if (dataValues[4] != null)
                    {
                        // Get main body
                        dataValues[4] = dataValues[4].Replace("//", ",");
                        temp_chatNode.BodyText = dataValues[4];
                    }
                    
                    if (temp_chatNode != null) {
                        _chatListContainer.ChatNodeList.Add(temp_chatNode);
                    }
                }
            }
            else 
            {
                Debug.LogWarning("Text file given not found");
            }
        }
    }

}
