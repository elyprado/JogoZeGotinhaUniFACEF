 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ScriptMenu : MonoBehaviour
{

    public GameObject sobre;
    public GameObject multiplayer;
    public InputField nome; 
    private AudioSource som;
    

    // Start is called before the first frame update
    void Start()
    {
        sobre.SetActive (false);
        multiplayer.SetActive (false);
        som = GetComponents<AudioSource>()[0];

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void sair() {
        som.Play();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void iniciarJogo() {
        som.Play();
        SceneManager.LoadScene("Unifacef", LoadSceneMode.Single);
    }

    public void abrirSobre() {
        som.Play();
        sobre.SetActive (true);
    }
    public void fecharSobre() {
        som.Play();
        sobre.SetActive (false);
    }
    public void abrirMultiplayer() {
        som.Play();
        multiplayer.SetActive (true);
    }
    public void fecharMuliplayer() {
        som.Play();
        multiplayer.SetActive (false);
    }
    public void iniciarJogoMultiplayer() {
        som.Play();
        StartCoroutine(requestPost());
    }

    public void abrirFaleConosco() {
         Application.OpenURL("https://forms.gle/6uFef6bEZcjP9Rtx5");
    }




    
    private IEnumerator requestPost()  {
      
        string url = "https://sistemaagely.com.br:8345/elyze/ZeGotinha";
        Debug.Log("requestPost: " + url);
        WWWForm form = new WWWForm();
        form.AddField("nome", nome.text);
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log("ERROR");
            Debug.Log("Erro conexão: " + www.error);
        } else {
             Debug.Log("OK!!!");
            Debug.Log(www.downloadHandler.text);
            
            SceneManager.LoadScene("Unifacef", LoadSceneMode.Single);
        }

    }

}
