using System.Collections; // Importamos el namespace System.Collections para el uso de colecciones
using System.Collections.Generic; // Importamos el namespace System.Collections.Generic para el uso de colecciones genéricas
using System; // Importamos el namespace System para el manejo de excepciones y otras funcionalidades básicas
using System.IO; // Importamos el namespace System.IO para el manejo de entradas y salidas de datos
using System.Net.Sockets; // Importamos el namespace System.Net.Sockets para el manejo de conexiones de red
using UnityEngine; // Importamos la librería de Unity para el manejo de componentes y funciones de Unity
using Photon.Pun; // Importamos la librería Photon.Pun para el manejo de la red en Photon

public class Network_Manager : MonoBehaviour // Definimos la clase Network_Manager que hereda de MonoBehaviour
{
    public static Network_Manager _NETWORK_MANAGER; // Variable estática para el patrón singleton de esta clase

    private TcpClient socket; // Cliente TCP para la conexión con el servidor
    private NetworkStream stream; // Flujo de datos para la conexión de red
    private StreamWriter writer; // Escritor para enviar datos al servidor
    private StreamReader reader; // Lector para recibir datos del servidor
    private bool connected = false; // Bandera para indicar si estamos conectados al servidor

    const string host = "192.168.1.149"; // Dirección IP del servidor
    const int port = 6543; // Puerto del servidor

    // Referencias a los elementos de la UI
    [SerializeField] private GameObject registerScreen;
    [SerializeField] private GameObject loginScreen;
    [SerializeField] private GameObject matchmakingScreen;

    [SerializeField] private GameObject registerError;
    [SerializeField] private GameObject loginError;

    private void Awake()
    {
        // Implementamos el patrón singleton para asegurar que solo haya una instancia de Network_Manager
        if (_NETWORK_MANAGER != null && _NETWORK_MANAGER != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _NETWORK_MANAGER = this;
            DontDestroyOnLoad(this.gameObject); // Evitamos que este objeto se destruya al cargar una nueva escena
        }
    }

    // Revisa los datos que llegan del servidor
    private void ManageData(string data)
    {
        if (data == "Ping")
        {
            Debug.Log("Recibo ping");
            writer.WriteLine("1"); // Respondemos al ping del servidor
            writer.Flush();
        }
    }

    private void Update()
    {
        // Verificamos si estamos conectados y si hay datos disponibles en el flujo de red
        if (connected && stream.DataAvailable)
        {
            string data = reader.ReadLine(); // Leemos los datos del servidor
            if (data != null)
            {
                ManageData(data); // Gestionamos los datos recibidos
            }
        }
    }

    // Nos conectamos al servidor para poder loguearnos
    public void ConnectToServerForLogin(string nick, string password)
    {
        try
        {
            // Nos conectamos al servidor
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            connected = true;

            // Pedimos loguearnos al servidor
            writer.WriteLine("LOGIN/" + nick + "/" + password);
            writer.Flush();

            // Leemos la respuesta del servidor
            string response = reader.ReadLine();
            if (response.StartsWith("true"))
            {
                // Si se completa el login establecemos el nombre de usuario local
                PhotonNetwork.LocalPlayer.NickName = nick;
                Debug.Log("Login exitoso.");

                // Extraemos los valores adicionales de la respuesta del servidor
                string[] parts = response.Split('/');
                if (parts.Length >= 3)
                {
                    float speed = float.Parse(parts[1]);
                    float jumpForce = float.Parse(parts[2]);

                    // Almacena los valores de speed y jumpforce en PlayerPrefs
                    Character.SetPlayerSpeed(speed);
                    Character.SetPlayerJumpForce(jumpForce);
                }
                else
                {
                    Debug.Log("La respuesta del servidor no contiene los valores de speed y jumpforce.");
                }

                // Activamos la pantalla de crear unir sala
                registerScreen.SetActive(false);
                loginScreen.SetActive(false);
                matchmakingScreen.SetActive(true);
                loginError.SetActive(false);
            }
            else if (response == "false")
            {
                // Si se falla el inicio de sesion salta error
                loginError.SetActive(true);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    void OnApplicationQuit()
    {
        if (connected)
        {
            // Enviamos mensaje de desconexion al servidor al cerrar la aplicacion
            writer.WriteLine("QUIT/" + PhotonNetwork.LocalPlayer.NickName);
            writer.Flush();
        }
    }

    // Nos conectamos al servidor para poder registrarnos
    public void ConnectToServerForRegister(string nick, string password, string race)
    {
        try
        {
            // Nos conectamos al servidor
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            connected = true;

            // Enviamos solicitud de registro al servidor
            writer.WriteLine("REGISTER/" + nick + "/" + password + "/" + race);
            writer.Flush();

            // Leemos respuesta del servidor
            string response = reader.ReadLine();
            if (response.StartsWith("true"))
            {
                // Si el registro es correcto ocultamos el mensaje de error de registro
                Debug.Log("Nuevo registro");
                registerError.SetActive(false);

                // Extrae los valores de speed y jumpforce del mensaje
                string[] parts = response.Split('/');
                float speed = float.Parse(parts[1]);
                float jumpforce = float.Parse(parts[2]);

                // Almacena los valores de speed y jumpforce en PlayerPrefs
                PlayerPrefs.SetFloat("speed", speed);
                PlayerPrefs.SetFloat("jumpforce", jumpforce);
            }
            else if (response == "false")
            {
                // Cuando nos registramos con un usuario ya existente salta el mensaje de error
                Debug.Log("El usuario ya existe.");
                registerError.SetActive(true);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
}
