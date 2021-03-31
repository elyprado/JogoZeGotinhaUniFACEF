
using UnityEngine;
using UnityEngine.UI;

//adiciona o componente CharacterController automaticamente
[RequireComponent(typeof(CharacterController))]

public class ControleJogador : MonoBehaviour
{
    public float speed = 7.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Transform playerCameraParent;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    public Text txtPontuacao;
    private int pontuacao = 0;

    private AudioSource somMoeda;
    private AudioSource somPulo;
    private AudioSource somChao;
    private AudioSource somMorte;
    private bool chao = true;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rotation.y = transform.eulerAngles.y;

        somMoeda = GetComponents<AudioSource>()[0];
        somPulo = GetComponents<AudioSource>()[2];
        somChao = GetComponents<AudioSource>()[3];
        somMorte = GetComponents<AudioSource>()[4];
    }
    void Update()
    {
        if (characterController.isGrounded)
        {

            //pouso do jogador após o pulo
            if (!chao)
            {
                somChao.Play();
            }
            chao = true;

            // Se o jogador estiver no chão, então pode se mover
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = speed * Input.GetAxis("Vertical");
            float curSpeedY = speed * Input.GetAxis("Horizontal");
            if (Input.GetButton("Fire3"))
            {
                curSpeedX = curSpeedX * 2;
                curSpeedY = curSpeedY * 2;
            }
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

           

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
                somPulo.Play();
                chao = false;
            }
        }

        // Aplica gravidade
        moveDirection.y -= gravity * Time.deltaTime;

        // Move o jogador
        characterController.Move(moveDirection * Time.deltaTime);

        if (Input.GetButton("Fire1")) {
            rotation.y -= 1 * lookSpeed;
        } else if (Input.GetButton("Fire2")) { 
            rotation.y += 1 * lookSpeed;
        } else {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
        }

        // Gira a Câmera

        rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
        playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
        transform.eulerAngles = new Vector2(0, rotation.y);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Moeda"))
        {
            other.gameObject.SetActive(false);
            pontuacao++;
            txtPontuacao.text = "" + pontuacao;
            somMoeda.Play();
        }
        else if (other.gameObject.CompareTag("Danger"))
        {
            somMorte.Play();
        }
    }

}
