using System.Collections; // Importamos el namespace System.Collections para el uso de colecciones
using System.Collections.Generic; // Importamos el namespace System.Collections.Generic para el uso de colecciones gen�ricas
using UnityEngine; // Importamos la librer�a de Unity para el manejo de componentes y funciones de Unity
using Photon.Pun; // Importamos la librer�a Photon.Pun para el manejo de la red en Photon
using Photon.Realtime; // Importamos la librer�a Photon.Realtime para el manejo de funcionalidades en tiempo real de Photon

public class Photon_Manager : MonoBehaviourPunCallbacks // Definimos la clase Photon_Manager que hereda de MonoBehaviourPunCallbacks
{
    public static Photon_Manager _PHOTON_MANAGER; // Variable est�tica para el patr�n singleton de esta clase

    private void Awake()
    {
        // Implementamos el patr�n singleton para asegurar que solo haya una instancia de Photon_Manager
        if (_PHOTON_MANAGER != null && _PHOTON_MANAGER != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _PHOTON_MANAGER = this;
            DontDestroyOnLoad(this.gameObject); // Evitamos que este objeto se destruya al cargar una nueva escena

            // Conecto con el servidor
            PhotonConnect();
        }
    }

    private void PhotonConnect()
    {
        // Configurar el inicio de partida
        PhotonNetwork.AutomaticallySyncScene = true;

        // Me conecto al servidor
        PhotonNetwork.ConnectUsingSettings();
    }

    // M�todo llamado cuando se conecta al servidor maestro
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conexion al servidor realizada correctamente");
        PhotonNetwork.JoinLobby(TypedLobby.Default); // Unirse al lobby por defecto
    }

    // M�todo llamado cuando se pierde la conexi�n con el servidor
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("He implosionado because: " + cause);
        Application.Quit(); // Cerrar la aplicaci�n
    }

    // M�todo llamado cuando se une a un lobby
    public override void OnJoinedLobby()
    {
        Debug.Log("He accedido al lobby");
    }

    // M�todo para crear una sala
    public void CreateRoom(string nameRoom)
    {
        PhotonNetwork.CreateRoom(nameRoom, new RoomOptions { MaxPlayers = 2 }); // Crear una sala con un m�ximo de 2 jugadores
    }

    // M�todo para unirse a una sala
    public void JoinRoom(string nameRoom)
    {
        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinRoom(nameRoom); // Unirse a la sala especificada
        }
    }

    // M�todo llamado cuando se une a una sala
    public override void OnJoinedRoom()
    {
        Debug.Log("Me he unido a una sala: " + PhotonNetwork.CurrentRoom.Name + " conn: " + PhotonNetwork.CurrentRoom.PlayerCount + " jugadores");
    }

    // M�todo llamado cuando falla al intentar unirse a una sala
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Mecachis, no se me ha podido conectar a la sala " + returnCode + " TU " + message);
    }

    // M�todo llamado cuando un nuevo jugador entra en la sala
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Esta funci�n puede coger el nick del pj

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("InGame"); // Cargar el nivel del juego cuando la sala est� llena y el cliente sea el master
        }
    }
}
