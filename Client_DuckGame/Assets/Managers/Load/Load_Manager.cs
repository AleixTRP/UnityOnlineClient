using System.Collections; // Importamos el namespace System.Collections para el uso de colecciones
using System.Collections.Generic; // Importamos el namespace System.Collections.Generic para el uso de colecciones genéricas
using UnityEngine; // Importamos la librería de Unity para el manejo de componentes y funciones de Unity

public class Load_Manager : MonoBehaviour // Definimos la clase Load_Manager que hereda de MonoBehaviour
{
    // Referencia al objeto GameManager que se encargará de gestionar el juego
    [SerializeField] GameObject GameManagerObject;

    // Tiempo que se muestra la pantalla de carga
    private float timeToLoadingScreen = 3f;

    private void Update()
    {
        // Disminuimos el tiempo restante en cada frame
        timeToLoadingScreen -= Time.deltaTime;

        // Revisamos si el tiempo ha terminado y si este objeto sigue existiendo
        if (timeToLoadingScreen < 0 && this.gameObject != null)
        {
            // Activamos el GameManager que se encarga de spawnear a los jugadores
            GameManagerObject.SetActive(true);
            // Destruimos este objeto para finalizar la pantalla de carga
            Destroy(this.gameObject);
        }
    }
}
