
using UnityEngine;
using UnityEngine.UI;

//adiciona o componente CharacterController automaticamente
[RequireComponent(typeof(CharacterController))]

public class ControleJogador : MonoBehaviour
{
    public float speed = 0.05f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Transform playerCameraParent;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    public Text txtPontuacao;
    private int pontuacao = 0;

    private AudioSource somMoeda;
    private AudioSource somPulo;
    //private AudioSource somChao;
    //private AudioSource somMorte;
    private bool chao = true;
    private bool dancando = false;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;
    private Animator animator;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rotation.y = transform.eulerAngles.y;

        somMoeda = GetComponents<AudioSource>()[0];
        somPulo = GetComponents<AudioSource>()[2];
        //somChao = GetComponents<AudioSource>()[3];
        //somMorte = GetComponents<AudioSource>()[4];
        animator = gameObject.transform.GetChild (1).gameObject.GetComponent<Animator> ();
    }
    void Update()
    {
        if (characterController.isGrounded && dancando == false )
        {

            //pouso do jogador após o pulo
            if (!chao)
            {
                //somChao.Play();
            }
            chao = true;

            // Se o jogador estiver no chão, então pode se mover
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = speed * Input.GetAxis("Vertical");
            float curSpeedY = speed * Input.GetAxis("Horizontal");
            if (Input.GetButton("Fire3") && curSpeedX>0) {
                //correndo
                curSpeedX = curSpeedX * 3;
            }
            
            //andando para o lado
            animator.SetInteger("lado", (int) curSpeedY);
            animator.SetInteger("frente", (int) curSpeedX);

            //movimenta personagem
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

           
           
            animator.SetBool("pulando", false);
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
                somPulo.Play();
                chao = false;
                animator.SetBool("pulando", true);
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
    
        if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            Application.Quit();
        }
    
    }


}
