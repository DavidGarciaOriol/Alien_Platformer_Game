using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    private int nivelSeleccionado;

    public static GameManager Instance
    {
        get => _instance;
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Se mantiene entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SeleccionarNivel(int index)
    {
        nivelSeleccionado = index;
        // Aquí cargas la escena del juego
        // SceneManager.LoadScene("GameplayScene");
    }
    public int NivelSeleccionado()
    {
        return nivelSeleccionado;
    }
}
