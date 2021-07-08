using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class ControleNPC : MonoBehaviour {
    public bool doente = false;
    public bool carregado = false;
    public bool hospitalizado = false;
    public bool vacinado = false;
    public float tempoHospital = 0F;

    private Material materialCovid;
    private Material materialSaudavel;
    private Material materialVacinado;

    //ponto de destino
    private GameObject target;
    //velocidade
    private float speed = 1.0F;

    private CapsuleCollider cc2;
    private Animator animator;

    private InformacoesHUD hud;

    public bool objPausado = false;

    void Start()
    {

        /*materialCovid = Resources.Load("ToonyTinyPeople/TT_Citizens/model/Materials/TT_Citizens_Zumbi.mat", typeof(Material)) as Material;
        materialSaudavel = Resources.Load("ToonyTinyPeople/TT_Citizens/model/Materials/TT_Citizens.mat", typeof(Material)) as Material;
        materialVacinado = Resources.Load("ToonyTinyPeople/TT_Citizens/model/Materials/TT_Citizens_Vacinados.mat", typeof(Material)) as Material;
        */
        materialCovid =  Resources.Load<Material>("MateriaisNPC/TT_Citizens_Zumbi");
        materialSaudavel =  Resources.Load<Material>("MateriaisNPC/TT_Citizens");
        materialVacinado =  Resources.Load<Material>("MateriaisNPC/TT_Citizens_Vacinados");


        hud = UnityEngine.Object.FindObjectOfType<InformacoesHUD>() ;

         //edita configurações do rigidbody
         Rigidbody rb = GetComponent<Rigidbody>();
         rb.constraints = RigidbodyConstraints.FreezeRotation;

         //1o ajusta capsule colider de fora
         CapsuleCollider cc = GetComponent<CapsuleCollider>();
         cc.center = new Vector3(0, 0.9f, 0);
         cc.radius = 0.51f;
         cc.height = 1.8f;
         cc.isTrigger = true;

         //cria um capsule colider por dentro
         cc2 = gameObject.AddComponent<CapsuleCollider>();
         cc2.center = new Vector3(0, 0.9f, 0);
         cc2.radius = 0.30f;
         cc2.height = 1.8f;
         cc2.isTrigger = false;

        target = new GameObject();
        Debug.Log("Inicio");
        //inicia para nova direção
        novaDirecao();


        animator = gameObject.GetComponent<Animator>();
        if (doente) {
            aplicaMaterialCovid();
            animator.SetBool("doente", true);
        } else {
            aplicaMaterialSaudavel();
        }

        
    }
    
    public void marcaNPCDoente() {
        doente = true;
        aplicaMaterialCovid();
        animator.SetBool("doente", true);
    }

    void novaDirecao() {
        float x = UnityEngine.Random.Range(-6, 6);
        float z =  UnityEngine.Random.Range(-6, 6);

        if (x>=0 && x <2) {
            x+= 2;
        } else if (x<=0 && x >-2) {
            x-= 2;
        }
        if (z>=0 && z<2) {
            z+= 2;
        } else if (z<=0 && z>-2) {
            z-= 2;
        }

        Vector3 p = new Vector3(x, 0, z);
        target.transform.position = p + transform.position;
    }

    void Update() {

        if (ControleJogador.pausado) {
            //pausa todos NPCs enquanto estiver vacinando
            if (! objPausado) {
                animator.SetBool("pausado", true);
                objPausado = true;
            }
            
            return;
        } else {
            if (objPausado) {
                animator.SetBool("pausado", false);
                objPausado = false;
            }
        }

        //sorteia um novo ponto de destino quando o NPC chegou ao target sorteado
        if (transform.position.x == target.transform.position.x && transform.position.z == target.transform.position.z) {
            novaDirecao();
        }

        //move NPC
        if (carregado == false && hospitalizado == false) {
            float step =  speed * Time.deltaTime;
            if (doente) {
                step = step * 2; 
            }
            
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);

            //rotacionar   
            Vector3 targetDir = target.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);

            float referencia = Mathf.Abs(newDir.x) + Mathf.Abs(newDir.z);

            if (referencia> 0.5f) {
                transform.rotation = Quaternion.LookRotation(newDir);
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            } else {
                novaDirecao();
            }
        
        }

        if (hospitalizado==true) {
            tempoHospital += Time.deltaTime;
            if (tempoHospital > 60) { //tempo para ficar curado em segundos
                //ativa todos os renders do npc (tornar visivel)
                Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renders) {
                    r.enabled = true;
                    r.material = materialSaudavel;
                }
                ligaColisao();

                //tira o NPC do hospital
                hospitalizado = false;
                doente = false;
                animator.SetBool("doente", false);
                animator.SetBool("carregando", false);
                
                GetComponent<Rigidbody>().isKinematic = false;
                novaDirecao();

                hud.saudaveis++;
                hud.hospitalizados--;
                hud.atualizarTextos();
            }
        }
    }
    void OnCollisionEnter(Collision collision) {
        if (ControleJogador.pausado) {
            //pausa todos NPCs enquanto estiver vacinando
            return;
        }

        //vai para nova direção caso haja colisão
        //importante que o objeto tenha um RigidBody 
        //desabilite a rotação XYZ
        if (carregado == false && hospitalizado == false) {
            //NovaDirecao();
            //target.transform.RotateAround(transform.position, Vector3.up, 180);
            target.transform.RotateAround(transform.position, new Vector3(0, 1, 0), 180);

            Vector3 distancia = target.transform.position-transform.position;
            target.transform.position = target.transform.position + distancia;
            Vector3 newDir = target.transform.position-(transform.position);
            float referencia = newDir.x + newDir.z; 

            if (Mathf.Abs(referencia) > 0.5) {
                transform.rotation = Quaternion.LookRotation(newDir+(GetComponent<Rigidbody>().velocity*2));
            } else {
                novaDirecao();
            }
            
        }

        if (collision.gameObject.CompareTag("NPC")) {
            Debug.Log("Colidiu com NPC");
            ControleNPC c = collision.gameObject.GetComponent<ControleNPC>();
            //o outro não está doente mas o objeto atual está doente
            //faz a contaminação
            if (c.doente==true && c.carregado==false && doente == false && vacinado == false) {
                Debug.Log("Contaminou");
                doente = true;
                aplicaMaterialCovid();
                hud.doentes++;
                hud.saudaveis--;
                hud.atualizarTextos();
            }
        }
    }

    public void desligaColisao() {
        cc2.isTrigger = true;
    }
    public void pegaNPC() {
        animator.SetBool("doente", false);
        animator.SetBool("carregando", true);
    }
    
    void ligaColisao() {
        cc2.isTrigger = false;
    }

    void aplicaMaterialCovid() {
        //permite aplicar um novo material em todos os filhos do objeto
        Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renders) {
            r.material = materialCovid;
        }
        animator.SetBool("doente", true);
        animator.SetBool("carregando", false);
    }
    public void aplicaMaterialVacinado() {
         vacinado = true;
        //permite aplicar um novo material em todos os filhos do objeto
        Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renders) {
            r.material = materialVacinado;
        }
    }
    public void aplicaMaterialSaudavel() {
        //permite aplicar um novo material em todos os filhos do objeto
        Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renders) {
            r.material = materialSaudavel;
        }
    }
    
    void onTriggerEnter(Collider other) {
        
    }
}
