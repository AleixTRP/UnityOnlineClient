using System.Collections; // Importamos el namespace System.Collections para el uso de colecciones
using System.Collections.Generic; // Importamos el namespace System.Collections.Generic para el uso de colecciones gen�ricas
using UnityEngine; // Importamos la librer�a de Unity para el manejo de componentes y funciones de Unity
using UnityEngine.UI; // Importamos la librer�a de Unity para el manejo de la UI

public class Room_UI_Manager : MonoBehaviour
{
    [SerializeField]
    private Button createButton; // Referencia al bot�n para crear una sala

    [SerializeField]
    private Button joinButton; // Referencia al bot�n para unirse a una sala

    [SerializeField]
    private Text createText; // Referencia al campo de texto para el nombre de la sala a crear

    [SerializeField]
    private Text joinText; // Referencia al campo de texto para el nombre de la sala a unirse

    [SerializeField]
    private GameObject matchmakingUI; // Referencia al objeto de la UI de matchmaking

    [SerializeField]
    private GameObject loadingScreen; // Referencia al objeto de la pantalla de carga

    private void Awake()
    {
        // Asignamos los listeners para los botones
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);
    }

    // Funci�n que crea salas
    private void CreateRoom()
    {
        // Llamamos a la funci�n CreateRoom del Photon_Manager pas�ndole el texto del campo de creaci�n
        Photon_Manager._PHOTON_MANAGER.CreateRoom(createText.text.ToString());

        // Ocultamos la UI de matchmaking y mostramos la pantalla de carga
        matchmakingUI.SetActive(false);
        loadingScreen.SetActive(true);
    }

    // Funci�n que une a salas
    private void JoinRoom()
    {
        // Llamamos a la funci�n JoinRoom del Photon_Manager pas�ndole el texto del campo de uni�n
        Photon_Manager._PHOTON_MANAGER.JoinRoom(joinText.text.ToString());
    }
}
