using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleNPC : MonoBehaviour {
    public bool doente = false;
    public bool carregado = false;
    public bool hospitalizado = false;
    public float tempoHospital = 0F;

    //ponto de destino
    private Vector3 target;
    //velocidade
    private float speed = 1.0F;

    void Start() {
        //inicia para nova direção
        novaDirecao();
    }
    void novaDirecao() {
        target = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        transform.rotation = Quaternion.LookRotation(target);
    }

    void Update() {
        //move NPC
        if (carregado == false && hospitalizado == false) {
            float step =  speed * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }

        if (hospitalizado==true) {
            tempoHospital += Time.deltaTime;
            if (tempoHospital > 60) { //tempo para ficar curado em segundos
                //ativa todos os renders do npc (tornar visivel)
                Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renders) {
                    r.enabled = true;
                }
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
            novaDirecao();
        }
    }
}
