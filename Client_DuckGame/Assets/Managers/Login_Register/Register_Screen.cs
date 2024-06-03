using System.Collections; // Importamos el namespace System.Collections para el uso de colecciones
using System.Collections.Generic; // Importamos el namespace System.Collections.Generic para el uso de colecciones gen�ricas
using UnityEngine; // Importamos la librer�a de Unity para el manejo de componentes y funciones de Unity
using UnityEngine.UI; // Importamos la librer�a de Unity para el manejo de la interfaz de usuario (UI)

public class Register_Screen : MonoBehaviour // Definimos la clase Register_Screen que hereda de MonoBehaviour
{
    // Referencias a los elementos de la UI: bot�n de registro, textos de usuario y contrase�a, y dropdown de razas
    [SerializeField] private Button registerButton;
    [SerializeField] private Text registerText;
    [SerializeField] private Text passwordText;
    [SerializeField] private Dropdown raceDropdown;

    private void Awake()
    {
        // A�adimos un listener al bot�n de registro para ejecutar el m�todo Clicked cuando se haga clic
        registerButton.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        // Obtenemos la raza seleccionada del dropdown
        string race = raceDropdown.options[raceDropdown.value].text;
        // Enviamos la informaci�n al Network Manager para registrarnos
        Network_Manager._NETWORK_MANAGER.ConnectToServerForRegister(registerText.text.ToString(), passwordText.text.ToString(), race);
    }
}
