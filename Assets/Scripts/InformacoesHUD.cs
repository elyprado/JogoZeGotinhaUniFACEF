using System;
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
        atualizarTextos();
        Invoke("inicializaJogo", 1.0f);
    }
    
    void inicializaJogo() {
        //coloca um % dos NPCs como doentes aleatoriamente
        GameObject[] outros = GameObject.FindGameObjectsWithTag("NPC");
        //outros[0].GetComponent<ControleNPC>().marcaNPCDoente();
        int qut = outros.Length * 15 / 100;
        Debug.Log("qut: " + qut);
        int qutd = 0;
        int tentativas = 0;
        while (qutd < qut) {
            int n = UnityEngine.Random.Range(0, outros.Length-1);
            try {
                if (! outros[n].GetComponent<ControleNPC>().doente) {
                    outros[n].GetComponent<ControleNPC>().marcaNPCDoente();
                    qutd++;
                    Debug.Log("qutd: " + qutd);
                }
            } catch (Exception e) {
            }
            tentativas++;
            if (tentativas > (outros.Length*2)) {
                break;
            }
        }

        //Atualiza contador de NPCs
        doentes = 0;
        saudaveis = 0;
        foreach(GameObject go in outros) {
            ControleNPC n = go.GetComponent<ControleNPC>();
            if (n.doente) {
                doentes++;
            } else {
                saudaveis++;
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
