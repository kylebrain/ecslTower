using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum PlayerType {None, Defender, Attacker};

public class LobbyPlayer : NetworkLobbyPlayer {

    public static LobbyPlayer localPlayer;

    static List<PlayerType> usedPlayerTypes = new List<PlayerType>();

    public float increasedAlpha = 200f;

    [SyncVar(hook = "UpdateText")]
    public PlayerType playerType = PlayerType.None;
    public Text typeText;

    public Text playerText;

    public ReadyButton readyButton;
    public Button typeButton;

    /*
    private void OnEnable()
    {
        readyButton.SetReadyAppearance(readyButton);
    }
    */

    public override void OnStartLocalPlayer()
    {
        localPlayer = this;
        Image panelImage = transform.Find("Panel").GetComponent<Image>();
        Color panelColor = Color.blue;
        panelColor.a = panelImage.color.a + increasedAlpha / 255f;
        panelImage.color = panelColor;

        // ultimately change to sync function of all players
        playerText.text += " - " + (isServer ? "hosting" : "client");
        typeButton.onClick.AddListener(UpdateType);
    }

    private void Start()
    {
        if(!isLocalPlayer)
        {
            readyButton.Button.interactable = false;
            typeButton.interactable = false;
            UpdateText(playerType);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isLocalPlayer)
        {
            CmdUpdateType();
        }
    }

    // can't add a command as a listener
    void UpdateType()
    {
        CmdUpdateType();
    }

    void UpdateText(PlayerType _playerType)
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

    public override void OnClientReady(bool readyState)
    {
        if(readyState)
        {
            //Debug.Log("Client is ready!");
        }
        readyButton.SetReadyAppearance(readyState);
    }

    /*public void Disconnect()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if(isServer)
        {
            FindObjectOfType<Lobby>().StopHost();
        } else
        {
            FindObjectOfType<Lobby>().StopClient();
        }
    }*/

}
