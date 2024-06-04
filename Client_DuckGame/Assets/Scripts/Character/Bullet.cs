using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using Photon.Pun; 
public class Bullet : MonoBehaviour
{
    private float speed = 10f; // Velocidad de la bala
    private Rigidbody2D rb; // Referencia al componente Rigidbody2D de la bala
    private bool characterLooksRight = true; // Indica si el personaje mira hacia la derecha

    private PhotonView pv; // Componente PhotonView para la sincronización de la red

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Obtenemos el componente Rigidbody2D
        pv = GetComponent<PhotonView>(); // Obtenemos el componente PhotonView
    }

    private void Start()
    {
        if (!characterLooksRight)
        {
            // Si el personaje no mira a la derecha, invertimos la velocidad de la bala
            speed = -speed;
            // Invertimos la escala en el eje x para voltear la bala
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // Asignamos la velocidad al Rigidbody2D para mover la bala
        rb.velocity = new Vector2(speed, 0f);
    }

    // Método para establecer la dirección de la bala
    public void SetDirection(bool looksRight)
    {
        characterLooksRight = looksRight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si la bala instanciada pertenece al jugador local
        if (!pv.IsMine)
        {
            return; // Salimos de la función si no es del jugador local
        }

        // Verificamos si la bala ha colisionado con un objeto que tiene la etiqueta "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Llamamos al método Damage() del componente Character del objeto colisionado
            collision.gameObject.GetComponent<Character>().Damage();
            // Destruimos la bala en la red
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
