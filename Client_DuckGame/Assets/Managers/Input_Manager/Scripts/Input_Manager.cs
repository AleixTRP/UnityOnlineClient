using System.Collections; // Importamos el namespace System.Collections para el uso de colecciones
using System.Collections.Generic; // Importamos el namespace System.Collections.Generic para el uso de colecciones genéricas
using UnityEngine; // Importamos la librería de Unity para el manejo de componentes y funciones de Unity
using UnityEngine.InputSystem; // Importamos la librería de Unity para el manejo del sistema de entrada (Input System)
using UnityEngine.InputSystem.EnhancedTouch; // Importamos la librería de Unity para el manejo del touch mejorado (Enhanced Touch)

public class Input_Manager : MonoBehaviour // Definimos la clase Input_Manager que hereda de MonoBehaviour
{
    private Player_Input_Actions playerInputs; // Variable privada para almacenar las acciones de entrada del jugador
    public static Input_Manager _INPUT_MANAGER; // Variable estática para el patrón singleton de esta clase

    private Vector2 leftAxisValue = Vector2.zero; // Variable privada para almacenar el valor del eje izquierdo (movimiento)

    private Vector2 mouseAxisValue = Vector2.zero; // Variable privada para almacenar el valor del eje del ratón

    private float jumpButtonPressed = 0f; // Variable privada para almacenar el tiempo desde que se presionó el botón de salto

    private float shootButtonPressed = 0f; // Variable privada para almacenar el tiempo desde que se presionó el botón de disparo

    private void Awake()
    {
        // Verificamos si ya existe una instancia de Input_Manager
        if (_INPUT_MANAGER != null && _INPUT_MANAGER != this)
        {
            // Si ya existe otra instancia, destruimos esta
            Destroy(this.gameObject);
        }
        else
        {
            // Si no existe otra instancia, inicializamos las acciones de entrada del jugador
            playerInputs = new Player_Input_Actions();
            // Habilitamos las acciones del personaje
            playerInputs.Character.Enable();
            // Asignamos los métodos de callback para las acciones específicas
            playerInputs.Character.Move.performed += LeftAxisUpdate;
            playerInputs.Character.Jump.performed += JumpButton;
            playerInputs.Character.Shoot.performed += ShootButton;

            // Asignamos esta instancia a la variable estática
            _INPUT_MANAGER = this;
            // Evitamos que este objeto se destruya al cargar una nueva escena
            DontDestroyOnLoad(this);
        }
    }

    private void Update()
    {
        // Incrementamos el tiempo desde la última vez que se presionaron los botones de salto y disparo
        jumpButtonPressed += Time.deltaTime;
        shootButtonPressed += Time.deltaTime;

        // Actualizamos el sistema de entrada
        InputSystem.Update();
    }

    // Método callback para actualizar el valor del eje izquierdo
    private void LeftAxisUpdate(InputAction.CallbackContext context)
    {
        leftAxisValue = context.ReadValue<Vector2>();
    }

    // Método callback para el botón de salto
    private void JumpButton(InputAction.CallbackContext context)
    {
        jumpButtonPressed = 0f; // Reiniciamos el tiempo desde que se presionó el botón de salto
    }

    // Método callback para el botón de disparo
    private void ShootButton(InputAction.CallbackContext context)
    {
        shootButtonPressed = 0f; // Reiniciamos el tiempo desde que se presionó el botón de disparo
    }

    // Método público para obtener el valor actualizado del eje izquierdo
    public Vector2 GetLeftAxisUpdate()
    {
        return this.leftAxisValue;
    }

    // Método público para obtener el valor actualizado del eje del ratón
    public Vector2 GetMouseAxisUpdate()
    {
        return this.mouseAxisValue;
    }

    // Método público para verificar si el botón de salto ha sido presionado
    public bool GetJumpButton()
    {
        return this.jumpButtonPressed == 0f;
    }

    // Método público para verificar si el botón de disparo ha sido presionado
    public bool GetShootButton()
    {
        return this.shootButtonPressed == 0f;
    }
}
