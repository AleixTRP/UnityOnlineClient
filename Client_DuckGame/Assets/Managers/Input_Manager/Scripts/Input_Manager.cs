using System.Collections; // Importamos el namespace System.Collections para el uso de colecciones
using System.Collections.Generic; // Importamos el namespace System.Collections.Generic para el uso de colecciones gen�ricas
using UnityEngine; // Importamos la librer�a de Unity para el manejo de componentes y funciones de Unity
using UnityEngine.InputSystem; // Importamos la librer�a de Unity para el manejo del sistema de entrada (Input System)
using UnityEngine.InputSystem.EnhancedTouch; // Importamos la librer�a de Unity para el manejo del touch mejorado (Enhanced Touch)

public class Input_Manager : MonoBehaviour // Definimos la clase Input_Manager que hereda de MonoBehaviour
{
    private Player_Input_Actions playerInputs; // Variable privada para almacenar las acciones de entrada del jugador
    public static Input_Manager _INPUT_MANAGER; // Variable est�tica para el patr�n singleton de esta clase

    private Vector2 leftAxisValue = Vector2.zero; // Variable privada para almacenar el valor del eje izquierdo (movimiento)

    private Vector2 mouseAxisValue = Vector2.zero; // Variable privada para almacenar el valor del eje del rat�n

    private float jumpButtonPressed = 0f; // Variable privada para almacenar el tiempo desde que se presion� el bot�n de salto

    private float shootButtonPressed = 0f; // Variable privada para almacenar el tiempo desde que se presion� el bot�n de disparo

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
            // Asignamos los m�todos de callback para las acciones espec�ficas
            playerInputs.Character.Move.performed += LeftAxisUpdate;
            playerInputs.Character.Jump.performed += JumpButton;
            playerInputs.Character.Shoot.performed += ShootButton;

            // Asignamos esta instancia a la variable est�tica
            _INPUT_MANAGER = this;
            // Evitamos que este objeto se destruya al cargar una nueva escena
            DontDestroyOnLoad(this);
        }
    }

    private void Update()
    {
        // Incrementamos el tiempo desde la �ltima vez que se presionaron los botones de salto y disparo
        jumpButtonPressed += Time.deltaTime;
        shootButtonPressed += Time.deltaTime;

        // Actualizamos el sistema de entrada
        InputSystem.Update();
    }

    // M�todo callback para actualizar el valor del eje izquierdo
    private void LeftAxisUpdate(InputAction.CallbackContext context)
    {
        leftAxisValue = context.ReadValue<Vector2>();
    }

    // M�todo callback para el bot�n de salto
    private void JumpButton(InputAction.CallbackContext context)
    {
        jumpButtonPressed = 0f; // Reiniciamos el tiempo desde que se presion� el bot�n de salto
    }

    // M�todo callback para el bot�n de disparo
    private void ShootButton(InputAction.CallbackContext context)
    {
        shootButtonPressed = 0f; // Reiniciamos el tiempo desde que se presion� el bot�n de disparo
    }

    // M�todo p�blico para obtener el valor actualizado del eje izquierdo
    public Vector2 GetLeftAxisUpdate()
    {
        return this.leftAxisValue;
    }

    // M�todo p�blico para obtener el valor actualizado del eje del rat�n
    public Vector2 GetMouseAxisUpdate()
    {
        return this.mouseAxisValue;
    }

    // M�todo p�blico para verificar si el bot�n de salto ha sido presionado
    public bool GetJumpButton()
    {
        return this.jumpButtonPressed == 0f;
    }

    // M�todo p�blico para verificar si el bot�n de disparo ha sido presionado
    public bool GetShootButton()
    {
        return this.shootButtonPressed == 0f;
    }
}
