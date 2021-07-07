using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformacoesHUD : MonoBehaviour
{
    public int doentes = 0;
    public int saudaveis = 0;
    public int vacinados = 0;
    public int hospitalizados = 0;
    public Text txtDoentes;
    public Text txtSaudaveis;
    public Text txtVacinados;
    public Text txtHospitalizados;

    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
        foreach(GameObject go in allObjects) {
            if (go.CompareTag("NPC")) {
                ControleNPC n = go.GetComponent<ControleNPC>();
                if (n.doente) {
                    doentes++;
                } else {
                    saudaveis++;
                }
            }
        }
        atualizarTextos();
    }


    public void atualizarTextos() {
        txtDoentes.text = "" + doentes;
        txtSaudaveis.text  = "" + saudaveis;
        txtVacinados.text  = "" + vacinados;
        txtHospitalizados.text  = "" + hospitalizados;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
