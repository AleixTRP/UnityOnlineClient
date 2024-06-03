using System.Collections; // Importamos el namespace System.Collections para el uso de colecciones
using System.Collections.Generic; // Importamos el namespace System.Collections.Generic para el uso de colecciones genéricas
using UnityEngine; // Importamos la librería de Unity para el manejo de componentes y funciones de Unity
using UnityEngine.UI; // Importamos la librería de Unity para el manejo de la interfaz de usuario (UI)

public class Register_Screen : MonoBehaviour // Definimos la clase Register_Screen que hereda de MonoBehaviour
{
    // Referencias a los elementos de la UI: botón de registro, textos de usuario y contraseña, y dropdown de razas
    [SerializeField] private Button registerButton;
    [SerializeField] private Text registerText;
    [SerializeField] private Text passwordText;
    [SerializeField] private Dropdown raceDropdown;

    private void Awake()
    {
        // Añadimos un listener al botón de registro para ejecutar el método Clicked cuando se haga clic
        registerButton.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        // Obtenemos la raza seleccionada del dropdown
        string race = raceDropdown.options[raceDropdown.value].text;
        // Enviamos la información al Network Manager para registrarnos
        Network_Manager._NETWORK_MANAGER.ConnectToServerForRegister(registerText.text.ToString(), passwordText.text.ToString(), race);
    }
}
