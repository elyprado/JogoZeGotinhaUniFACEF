 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEngine.SceneManagement;
  using UnityEngine.UI;

public class ScriptMenu : MonoBehaviour
{

    public GameObject sobre;
    private AudioSource som;

    // Start is called before the first frame update
    void Start()
    {
        sobre.SetActive (false);
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
        SceneManager.LoadScene("Unifacef");
    }

    public void abrirSobre() {
        som.Play();
        sobre.SetActive (true);
    }
    public void fecharSobre() {
        som.Play();
        sobre.SetActive (false);
    }
}
