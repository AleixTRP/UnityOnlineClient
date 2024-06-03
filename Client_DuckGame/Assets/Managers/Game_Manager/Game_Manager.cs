using Photon.Pun; // Importamos la librer�a Photon.Pun para el manejo de la red en Photon
using UnityEngine; // Importamos la librer�a de Unity para el manejo de componentes y funciones de Unity

public class GameManager : MonoBehaviour // Definimos la clase GameManager que hereda de MonoBehaviour
{
    // Definimos dos variables privadas para los puntos de spawn de los jugadores y las inicializamos en el Inspector de Unity
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    private void Awake()
    {
        // Verificamos si el cliente es el master client, es decir, el cliente que ha creado la sala
        if (PhotonNetwork.IsMasterClient)
        {
            // Si es el master client, spawneamos al jugador 1 en el primer punto de spawn
            SpawnPlayer("Player1Race", spawnPoint1.position);
        }
        else
        {
            // Si no es el master client, spawneamos al jugador 2 en el segundo punto de spawn
            SpawnPlayer("Player2Race", spawnPoint2.position);
        }
    }

    // M�todo para instanciar un jugador en un punto de spawn espec�fico
    public GameObject SpawnPlayer(string race, Vector3 spawnPoint)
    {
        // Instanciamos el objeto del jugador en la red utilizando PhotonNetwork.Instantiate
        // El primer par�metro es el nombre del prefab del jugador, el segundo es la posici�n de spawn, y el tercero es la rotaci�n (Quaternion.identity para rotaci�n por defecto)
        return PhotonNetwork.Instantiate("Player", spawnPoint, Quaternion.identity);
    }
}
