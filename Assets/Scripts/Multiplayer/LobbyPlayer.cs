using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum PlayerType {None, Defender, Attacker};

public class LobbyPlayer : NetworkLobbyPlayer {

    // statics
    public static LobbyPlayer localPlayer;
    public static List<PlayerType> usedPlayerTypes = new List<PlayerType>();

    // inspector values
    public float increasedAlpha = 200f;

    // syncVars
    [SyncVar(hook = "UpdateTypeText")]
    public PlayerType playerType = PlayerType.None;

    [SyncVar]
    public bool isHost = false;

    // UI/gameObject inspector references
    public Text typeText;
    public Text playerText;
    public ReadyButton readyButton;
    public Button typeButton;

    private delegate void TypeButtonDelegate();
    private TypeButtonDelegate typeButtonDelegate;

    // networkBehaviour override functions

    public override void OnStartLocalPlayer()
    {
        localPlayer = this;

        Image panelImage = transform.Find("Panel").GetComponent<Image>();
        Color panelColor = Color.blue;
        panelColor.a = panelImage.color.a + increasedAlpha / 255f;
        panelImage.color = panelColor;

        // this will be only true for the host player on the server
            // which will then be synced to each client
        isHost = isServer;
        //typeButton.onClick.AddListener(UpdateType);
        typeButtonDelegate = UpdateType;
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        playerText.text = "Player " + (slot + 1);
        Transform parent = FindObjectOfType<Lobby>().playerList.transform;
        transform.SetParent(parent, false);
        readyButton.SetReadyAppearance(readyToBegin);
        //GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
    }

    public override void OnClientReady(bool readyState)
    {
        if (readyState)
        {
            //Debug.Log("Client is ready!");
        }
        readyButton.SetReadyAppearance(readyState);
    }


    // start/update functions, general monoBehaviour functions

    private void Start()
    {
        playerText.text += " - " + (isHost ? "hosting" : "client");

        if (!isLocalPlayer)
        {
            readyButton.Button.interactable = false;
            typeButton.interactable = false;
            UpdateTypeText(playerType);
        }

        if(isServer)
        {
            RpcSetLevel(SelectedLevel.instance.SelectedLevelName);
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isLocalPlayer)
        {
            CmdUpdateType();
        }
    }

    // general functions

    // needs to be set on enable, but on enable breaks the code
    public void SetLobbyValues()
    {
        typeButtonDelegate = UpdateType;
        playerText.text += " - " + (isHost ? "hosting" : "client");
    }

    // playerType functions

    public void TypeButtonFunction()
    {
        typeButtonDelegate();
    }

    // can't add a command as a listener
    void UpdateType()
    {
        CmdUpdateType();
    }

    void UpdateTypeText(PlayerType _playerType)
    {
        playerType = _playerType;
        if (playerType == PlayerType.None && readyToBegin && isLocalPlayer)
        {
            Debug.Log("Not ready!");
            SendNotReadyToBeginMessage();
        }
        typeText.text = _playerType.ToString();
    }

    [Command]
    void CmdUpdateType()
    {
        int typeIndex = (int)playerType;
        PlayerType previousType = playerType;

        do
        {
            typeIndex++;
            typeIndex %= System.Enum.GetNames(typeof(PlayerType)).Length;
        } while (usedPlayerTypes.Contains((PlayerType)typeIndex) && (PlayerType)typeIndex != PlayerType.None);

        playerType = (PlayerType)typeIndex;

        usedPlayerTypes.Remove(previousType);

        if(!usedPlayerTypes.Contains(playerType))
        {
            usedPlayerTypes.Add(playerType);
        }

        RpcUpdateType(playerType);
    }

    [ClientRpc]
    void RpcUpdateType(PlayerType _playerType)
    {
        UpdateTypeText(_playerType);
    }

    // selectedLevel functions

    [Command]
    public void CmdSetLevel(string _levelName)
    {
        SelectedLevel.instance.UpdateSelectedLevel(_levelName);
        RpcSetLevel(_levelName);
    }

    [ClientRpc]
    void RpcSetLevel(string _levelName)
    {
        SelectedLevel.instance.UpdateSelectedLevel(_levelName);
    }

    

}
