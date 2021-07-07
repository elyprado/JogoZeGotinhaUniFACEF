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
    
    
    private AudioSource somPegaNPC;
    private AudioSource somPulo;
    private AudioSource somHospital;
    private AudioSource somVacinar;
    private bool chao = true;
    private bool dancando = false;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;
    private Animator animator;

    private GameObject npc = null;

    private InformacoesHUD hud;

    public static bool pausado = false;

    void Start()
    {
        hud = UnityEngine.Object.FindObjectOfType<InformacoesHUD>() ;
        


        characterController = GetComponent<CharacterController>();
        rotation.y = transform.eulerAngles.y;

        somPegaNPC = GetComponents<AudioSource>()[0];
        somPulo = GetComponents<AudioSource>()[2];
        somHospital = GetComponents<AudioSource>()[3];
        somVacinar = GetComponents<AudioSource>()[4];
        animator = gameObject.transform.GetChild (1).gameObject.GetComponent<Animator> ();
    }


    void Update()
    {

        if (ControleJogador.pausado) {
            //pausa todos NPCs enquanto estiver vacinando
            return;
        }


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

    void OnTriggerEnter(Collider other) {
        if (ControleJogador.pausado) {
            //pausa todos NPCs enquanto estiver vacinando
            return;
        }

        if (other.gameObject.CompareTag("NPC") && npc == null) {
            //pegar NPC
            if (other.gameObject.GetComponent<ControleNPC>().hospitalizado) {
                return;
            }
            if (other.gameObject.GetComponent<ControleNPC>().doente == false) {
                if (other.gameObject.GetComponent<ControleNPC>().vacinado == false) {
                    //gira o jogador em direção ao NPC
                    Vector3 targetDir = other.gameObject.transform.position;
                    targetDir.y = transform.position.y;
                    transform.LookAt(targetDir);

                    pausado = true;
                    animator.SetInteger("lado", 0);
                    animator.SetInteger("frente", 0);
                    animator.SetBool("pulando", false);
                    animator.SetBool("dancando", false);
                    animator.SetBool("vacinando", true);
                    
                    Invoke("despausar", 1.0f);

                    somVacinar.Play();
                    other.gameObject.GetComponent<ControleNPC>().aplicaMaterialVacinado();
                    hud.vacinados++;
                    hud.saudaveis--;
                    hud.atualizarTextos();
                }
                return;
            }
            //pontuacao++;
            //txtPontuacao.text = "" + pontuacao;

            somPegaNPC.Play();

            //pegar NPC
            other.transform.parent = transform;
            other.transform.localPosition = new Vector3(0.7f,-0.2f,0.5f);
            other.transform.rotation =  transform.rotation;
            other.transform.Rotate(-90.0f, 0.0f, 90.0f, Space.Self);

            animator.SetInteger("lado", 0);
            animator.SetInteger("frente", 0);
            animator.SetBool("pulando", false);
            animator.SetBool("carregando", true);

            

            npc = other.gameObject;
            npc.GetComponent<ControleNPC>().carregado = true;
            npc.GetComponent<Rigidbody>().isKinematic = true;
            npc.GetComponent<ControleNPC>().pegaNPC();
        } else if (other.gameObject.CompareTag("Hospital") && npc != null) {
            somHospital.Play();
            //Destroy(npc);
            Renderer[] renders = npc.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renders) {
                r.enabled = false;
            }
            npc.GetComponent<ControleNPC>().carregado = false;
            npc.GetComponent<ControleNPC>().hospitalizado = true;
            npc.GetComponent<ControleNPC>().tempoHospital = 0F;
            
            npc.GetComponent<ControleNPC>().desligaColisao();
            npc.transform.parent = null;

            npc = null;

            animator.SetInteger("lado", 0);
            animator.SetInteger("frente", 0);
            animator.SetBool("pulando", false);
            animator.SetBool("carregando", false);
            hud.doentes--;
            hud.hospitalizados++;
            hud.atualizarTextos();
            if (hud.doentes == 0) {
                iniciarDanca();

            }
        }
    }

    void iniciarDanca() {
        animator.SetInteger("lado", 0);
        animator.SetInteger("frente", 0);
        animator.SetBool("pulando", false);
        animator.SetBool("dancando", true);
        dancando = true;
        Invoke("pararDanca", 10.0f);
        moveDirection = Vector3.zero;
    }

    void pararDanca() {
        animator.SetBool("dancando", false);
        dancando = false;
    }

    void despausar() {
        pausado = false;
        animator.SetBool("vacinando", false);
    }

}
