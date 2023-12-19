using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject waitingCamera;
    public GameObject WaitCamvas;
    public Text text;

    public Transform spawnPoint;

    const int maxPlayer = 2;  //部屋の最大人数
    

    void Start()
    {
        Debug.Log("Connecting...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        //base.OnConnectedToMaster();
        Debug.Log("Connected to Server");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()  //現在ある部屋にランダムに入る
    {
        PhotonNetwork.JoinRandomRoom();

        Debug.Log("We're in the lobby");
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)  //部屋に入れなかったら部屋を作る
    {
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayer;
        PhotonNetwork.CreateRoom(RoomNameCreator.CreateRoomName(), roomOptions, null);
    }

    public override async void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        Debug.Log("We're connected and in a room!");

        await WaitAnotherPlayer();

        text.text = "スタート!";
        await WaitOneSec();

        SetUpGame();
    }

    void SetUpGame()
    {
        GameObject.Destroy(WaitCamvas, 0.2f);
        GameObject.Destroy(waitingCamera, 0.2f);

        GameObject _player = PhotonNetwork.Instantiate(GeneralSettings.Instance.m_Prehabs.Player.name, spawnPoint.position, Quaternion.identity);
        PlayerSetup playerSetup = _player.GetComponent<PlayerSetup>();
        playerSetup.IsLocalPlayer(playerSetup);
    }

    //部屋の人数が最大になったら待機終了
    async UniTask WaitAnotherPlayer() => await UniTask.WaitUntil(() => PhotonNetwork.PlayerList.Length == maxPlayer);
    async UniTask WaitOneSec() => await UniTask.Delay(1000);
}



public class RoomNameCreator
{
    private static int nameLength = 10;

    private static int largeA = 65;
    private static int largeZ = 90;
    private static int smallA = 97;
    private static int smallZ = 122;
    public static string CreateRoomName()
    {
        string name = "";
        for(int i = 0; i < nameLength; i++)
        {
            if(i == 0)  //1文字目
            {
                name += (char)Random.Range(largeA, largeZ + 1);
            }
            else
            {
                name  += (char)Random.Range(smallA, smallZ + 1);
            }
        }
        Debug.Log(name);
        return name;
    }
}