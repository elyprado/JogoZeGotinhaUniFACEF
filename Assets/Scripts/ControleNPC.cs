using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleNPC : MonoBehaviour {
    public bool doente = false;
    public bool carregado = false;
    public bool hospitalizado = false;
    public float tempoHospital = 0F;

    public Material materialCovid;
    public Material materialSaudavel;

    //ponto de destino
    private GameObject target;
    //velocidade
    private float speed = 1.0F;

    void Start() {
        target = new GameObject();
        Debug.Log("Inicio");
        //inicia para nova direção
        novaDirecao();
    }
    void novaDirecao() {
        Vector3 p = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        //Quaternion LookAtRotation = Quaternion.LookRotation(p);
        //Quaternion LookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, LookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        //transform.rotation = LookAtRotationOnly_Y;
        target.transform.position = p + transform.position;
    }

    void Update() {

        //sorteia um novo ponto de destino quando o NPC chegou ao target sorteado
        if (transform.position.x == target.transform.position.x && transform.position.z == target.transform.position.z) {
            novaDirecao();
        }

        //move NPC
        if (carregado == false && hospitalizado == false) {
            float step =  speed * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);

            //rotacionar   
            Vector3 originalRotation = transform.rotation.eulerAngles;
            Vector3 targetDir = target.transform.position - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        
        
        }

        if (hospitalizado==true) {
            tempoHospital += Time.deltaTime;
            if (tempoHospital > 60) { //tempo para ficar curado em segundos
                //ativa todos os renders do npc (tornar visivel)
                Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renders) {
                    r.enabled = true;
                }
                //TODO REATIVAR COLISAO

                //tira o NPC do hospital
                hospitalizado = false;
                GetComponent<Rigidbody>().isKinematic = false;
                novaDirecao();
            }
        }
    }
    void OnCollisionEnter(Collision collision) {
        //vai para nova direção caso haja colisão
        //importante que o objeto tenha um RigidBody 
        //desabilite a rotação XYZ
        if (carregado == false && hospitalizado == false) {
            //NovaDirecao();
            //target.transform.RotateAround(transform.position, Vector3.up, 180);
            target.transform.RotateAround(transform.position, new Vector3(0, 1, 0), 180);
            transform.rotation = Quaternion.LookRotation(target.transform.position-transform.position);
        }

        if (collision.gameObject.CompareTag("NPC")) {
            Debug.Log("Colidiu com NPC");
            ControleNPC c = collision.gameObject.GetComponent<ControleNPC>();
            //o outro não está doente mas o objeto atual está doente
            //faz a contaminação
            if (c.doente==true && c.carregado==false && doente == false) {
                Debug.Log("Contaminou");
                doente = true;
                //permite aplicar um novo material em todos os filhos do objeto
                Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renders) {
                    r.material = materialCovid;
                }
            }
        }
    }

    void onTriggerEnter(Collider other) {
        
    }
}
