using System.Collections; // Importamos el namespace System.Collections para el uso de colecciones
using System.Collections.Generic; // Importamos el namespace System.Collections.Generic para el uso de colecciones genéricas
using UnityEngine; // Importamos la librería de Unity para el manejo de componentes y funciones de Unity
using Photon.Pun; // Importamos la librería Photon.Pun para el manejo de la red en Photon

public class Character : MonoBehaviourPun, IPunObservable
{
    private float speed; // Velocidad del personaje
    private float jumpForce; // Fuerza de salto del personaje
    private Rigidbody2D rb; // Referencia al componente Rigidbody2D del personaje
    public SpriteRenderer sr; // Referencia al componente SpriteRenderer del personaje
    public Animator anim; // Referencia al componente Animator del personaje
    private PhotonView pv; // Componente PhotonView para la sincronización de la red
    private Vector3 enemyPosition = Vector3.zero; // Posición del enemigo para la sincronización

    private static float PlayerSpeed; // Velocidad del jugador
    private static float PlayerJumpForce; // Fuerza de salto del jugador

    private float timeToShoot = 1f; // Tiempo de recarga del disparo
    private float lastTimeShoot; // Último tiempo de disparo
    private bool cooldownReady = false; // Indica si el cooldown está listo

    private int initHp = 3; // Puntos de vida iniciales
    private int playerHp = 3; // Puntos de vida actuales

    private int playerLifes = 3; // Vidas del jugador

    [SerializeField] private TextMesh playerNameText; // Texto para el nombre del jugador

    [SerializeField] private TextMesh playerHpText; // Texto para los puntos de vida del jugador
    [SerializeField] private TextMesh playerLifesText; // Texto para las vidas del jugador

    [SerializeField] private GameObject bulletSpawner; // Objeto para spawnear las balas

    [SerializeField] private AudioSource shootCollisionSound; // Sonido de colisión de disparo
    [SerializeField] private AudioSource shootFinalCollisionSound; // Sonido de colisión final de disparo

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Obtenemos el componente Rigidbody2D
        pv = rb.GetComponent<PhotonView>(); // Obtenemos el componente PhotonView
        sr = GetComponent<SpriteRenderer>(); // Obtenemos el componente SpriteRenderer
        anim = GetComponent<Animator>(); // Obtenemos el componente Animator

        // Tasa de envío para la sincronización de red
        PhotonNetwork.SendRate = 20;
        // Tasa de serialización para la sincronización de red
        PhotonNetwork.SerializationRate = 20;

        // Obtiene los valores de speed y jumpforce
        speed = PlayerPrefs.GetFloat("speed");
        jumpForce = PlayerPrefs.GetFloat("jumpforce");

        // Determinamos el color del jugador dependiendo del número de jugador local
        int localPlayerNum = PhotonNetwork.LocalPlayer.ActorNumber;

        if (localPlayerNum == 1)
        {
            if (pv.IsMine)
            {
                sr.color = Color.blue;
            }
            else
            {
                sr.color = Color.red;
            }
        }
        else
        {
            if (pv.IsMine)
            {
                sr.color = Color.black;
            }
            else
            {
                sr.color = Color.magenta;
            }
        }
    }

    private void Start()
    {
        // Asigna los valores de speed y jumpforce a las variables de instancia
        speed = PlayerSpeed;
        jumpForce = PlayerJumpForce;

        // Mostramos el nombre del jugador
        playerNameText.text = PhotonNetwork.CurrentRoom.Players[photonView.OwnerActorNr].NickName;
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            CheckInputs(); // Revisamos los inputs
            // Manejo de cooldown del disparo de bala
            if (cooldownReady)
            {
                lastTimeShoot -= Time.deltaTime;
            }
            if (lastTimeShoot < 0)
            {
                cooldownReady = false;
                lastTimeShoot = 0;
            }
        }
        else
        {
            // Replicamos la posición del jugador enemigo de forma suave
            SmoothReplicate();
        }

        // UI de vida de los jugadores
        playerLifesText.text = "Lifes: " + playerLifes.ToString();
        playerHpText.text = "Health: " + playerHp.ToString();
    }

    private void FixedUpdate()
    {
        if (pv.IsMine)
        {
            // Obtenemos el input horizontal del jugador
            float horizontalInput = Input_Manager._INPUT_MANAGER.GetLeftAxisUpdate().x;

            // Se calcula la velocidad horizontal del jugador mediante los inputs y velocidad del jugador
            float horizontalVelocity = horizontalInput * speed * Time.fixedDeltaTime;

            // Aplicamos la velocidad horizontal al rigidbody para poder movernos
            rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);

            // Flipeamos el sprite del jugador y el spawn de la bala dependiendo de la dirección en la que mire
            if (horizontalVelocity > 0)
            {
                sr.flipX = false;
                bulletSpawner.transform.localPosition = new Vector3(1f, 0f, 0f);
            }
            else if (horizontalVelocity < 0)
            {
                sr.flipX = true;
                bulletSpawner.transform.localPosition = new Vector3(-1f, 0f, 0f);
            }

            // Animación de movimiento del jugador
            anim.SetBool("IsMoving", horizontalInput != 0);
        }
    }

    // Verificamos los inputs del jugador
    private void CheckInputs()
    {
        // Verificamos si el personaje pulsa el botón de salto y está en el suelo
        if (Input_Manager._INPUT_MANAGER.GetJumpButton() && Mathf.Approximately(rb.velocity.y, 0f))
        {
            // Saltamos aplicando fuerza y hacemos animación de salto
            rb.AddForce(new Vector2(0f, jumpForce));
            anim.SetBool("IsJumping", true);
        }

        // Verificamos si el jugador está cayendo
        if (rb.velocity.y < 0)
        {
            // Desactivamos la animación de salto y activamos la de caída
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", true);
        }
        else
        {
            // Si deja de caer desactivamos la animación
            anim.SetBool("IsFalling", false);
        }

        // Revisamos si pulsamos el botón de disparar y si el cooldown ha terminado
        if (Input_Manager._INPUT_MANAGER.GetShootButton() && lastTimeShoot <= 0 && !cooldownReady)
        {
            // Disparamos y manejamos el cooldown
            Shoot();
            lastTimeShoot = timeToShoot;
            cooldownReady = true;
        }
    }

    // Función de disparo
    private void Shoot()
    {
        // Instanciamos la bala en una posición
        GameObject bullet = PhotonNetwork.Instantiate("Bullet", bulletSpawner.transform.position, Quaternion.identity);
        // Obtenemos referencia al script de la bala
        Bullet bulletRef = bullet.GetComponent<Bullet>();

        // Flipeamos la dirección de la bala dependiendo de la dirección en la que mire
        if (sr.flipX)
        {
            bulletRef.SetDirection(false);
        }
        else
        {
            bulletRef.SetDirection(true);
        }
    }

    // Función para recibir daño
    public void Damage()
    {
        // Si no eres el jugador local hacemos daño en la red
        if (!pv.IsMine)
        {
            pv.RPC("NetworkDamage", RpcTarget.All);
        }
    }

    // Función RPC para recibir daño en la red
    [PunRPC]
    private void NetworkDamage()
    {
        // Reducimos puntos de vida
        playerHp--;

        // Revisamos si la vida ha bajado a cero
        if (playerHp <= 0)
        {
            // Si los corazones se han terminado el juego termina y vuelves al matchmaking, si no se resta un corazón y reinicia la vida, el jugador derribado vuelve a aparecer
            // en el centro del mapa, también aparecen sonidos dependiendo de si pierdes vida o corazones
            if (playerLifes <= 0)
            {
                PhotonNetwork.LoadLevel("Matchmaking");
            }
            else
            {
                playerLifes--;
                playerHp = initHp;
                shootFinalCollisionSound.Play();
                transform.position = Vector3.zero;
            }
        }
        else
        {
            shootCollisionSound.Play();
        }
    }

    // Función para revisar la posición del jugador enemigo de manera suave
    private void SmoothReplicate()
    {
        transform.position = Vector3.Lerp(transform.position, enemyPosition, Time.fixedDeltaTime * 20);
    }

    // Función para la sincronización de la red
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Enviamos la posición del jugador a través de la red
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            // Recibimos la posición del enemigo a través de la red
            enemyPosition = (Vector3)stream.ReceiveNext();
        }
    }

     //Sets de las variables de speed y jumpforce para poder setearlas en otros scripts
        public static void SetPlayerSpeed(float speed)
    {
        PlayerSpeed = speed;
    }

    public static void SetPlayerJumpForce(float jumpForce)
    {
        PlayerJumpForce = jumpForce;
    }
}
