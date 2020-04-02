using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

[RequireComponent(typeof(SocketIOComponent))]
public class ConnectionManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerIDGroup
    {
        public List<string> playerIDList = new List<string>();
    }

    public class PlayerData
    {
        public string uid;
        public Player playerObj;
        public Vector3 correctPos;
    }

    public Player playerObjPref;

    public string ownerID;

    public PlayerIDGroup playerIDGroup;

    public PlayerIDGroup cachePlayerIDGroup;

    private List<PlayerData> characterList = new List<PlayerData>();

    private SocketIOComponent socket;

    private void OnGUI()
    {
        GUILayout.TextField("OwnerID : " + ownerID);
    }

    // Start is called before the first frame update
    void Start()
    {
        socket = GetComponent<SocketIOComponent>();

        socket.On("OnOwnerClientConnect", OnOwnerClientConnect);
        socket.On("OnClientConnect", OnClientConnect);
        socket.On("OnClientFetchPlayerList", OnClientFetchPlayerList);
        socket.On("OnClientDisconnect", OnClientDisconnect);

        cachePlayerIDGroup = new PlayerIDGroup();
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayerConnect();
    }

    private void DetectPlayerConnect()
    {
        if(cachePlayerIDGroup.playerIDList.Count != playerIDGroup.playerIDList.Count)
        {
            bool checkConnect;
            List<string> firstList;
            List<string> secondList;

            if(playerIDGroup.playerIDList.Count > cachePlayerIDGroup.playerIDList.Count)
            {
                firstList = playerIDGroup.playerIDList;
                secondList = cachePlayerIDGroup.playerIDList;
                checkConnect = true;
            }
            else
            {
                firstList = cachePlayerIDGroup.playerIDList;
                secondList = playerIDGroup.playerIDList;
                checkConnect = false;
            }

            foreach(var fID in firstList)
            {
                bool isFound = false;
                foreach(var sID in secondList)
                {
                    if(fID == sID)
                    {
                        isFound = true;
                        break;
                    }
                }

                if(!isFound)
                {
                    if(checkConnect)//Check player connect
                    {
                        Debug.Log("Player connected : " + fID);
                        CreateCharacter(fID);
                    }
                    else//Check player disconnect
                    {
                        Debug.Log("Player disconnected : " + fID);
                        DestroyCharacter(fID);
                    }
                }
            }
        }

        cachePlayerIDGroup.playerIDList = playerIDGroup.playerIDList;
    }

    private void CreateCharacter(string uid)
    {
        PlayerData newPlayerData = new PlayerData();

        newPlayerData.uid = uid;
        newPlayerData.playerObj = Instantiate(playerObjPref, Vector3.zero, Quaternion.identity);

        newPlayerData.playerObj.name = "Player : " + uid;

        if (uid == ownerID)
        {
            newPlayerData.playerObj.canControl = true;
        }

        characterList.Add(newPlayerData);
    }

    private void DestroyCharacter(string uid)
    {
        for(int i = 0; i < characterList.Count; i++)
        {
            if(characterList[i].uid == uid)
            {
                Destroy(characterList[i].playerObj.gameObject);
                characterList.RemoveRange(i, 1);
                break;
            }
        }
    }

    #region Callback Group
    void OnClientConnect(SocketIOEvent evt)
    {
        Debug.Log("OnClientConnect : "+ evt.data.ToString());
        socket.Emit("OnClientFetchPlayerList");
    }

    void OnClientDisconnect(SocketIOEvent evt)
    {
        Debug.Log("OnClientDisconnect : " + evt.data.ToString());
        socket.Emit("OnClientFetchPlayerList");
    }

    void OnOwnerClientConnect(SocketIOEvent evt)
    {
        Debug.Log("OnOwnerClientConnect : " + evt.data.ToString());

        var dictData = evt.data.ToDictionary();

        ownerID = dictData["uid"];
    }

    void OnClientFetchPlayerList(SocketIOEvent evt)
    {
        Debug.Log("OnClientFetchPlayerList : "+ evt.data.ToString());

        playerIDGroup = JsonUtility.FromJson <PlayerIDGroup> (evt.data.ToString());
    }
    #endregion
}
