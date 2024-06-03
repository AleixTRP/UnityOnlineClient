using System.Collections; // Importamos el namespace System.Collections para el uso de colecciones
using System.Collections.Generic; // Importamos el namespace System.Collections.Generic para el uso de colecciones genéricas
using UnityEngine; // Importamos la librería de Unity para el manejo de componentes y funciones de Unity
using UnityEngine.UI; // Importamos la librería de Unity para el manejo de la interfaz de usuario (UI)

public class Login_Screen : MonoBehaviour // Definimos la clase Login_Screen que hereda de MonoBehaviour
{
    // Referencias a los elementos de la UI: botón de login, texto de usuario y texto de contraseña
    [SerializeField] private Button loginButton;
    [SerializeField] private Text loginText;
    [SerializeField] private Text passwordText;

    private void Awake()
    {
        // Añadimos un listener al botón de login para ejecutar el método Clicked cuando se haga clic
        loginButton.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        // Mandamos la información de usuario y contraseña al Network Manager para conectar al servidor
        Network_Manager._NETWORK_MANAGER.ConnectToServerForLogin(loginText.text.ToString(), passwordText.text.ToString());
    }
}
