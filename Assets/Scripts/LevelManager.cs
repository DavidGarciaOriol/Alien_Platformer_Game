using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private List<GameObject> levelsPrefabs; // Lista de prefabs de niveles
    [SerializeField] private GameObject player; // El jugador
    [SerializeField] private Transform levelParent; // Objeto vacío donde se instancian los niveles
    [SerializeField] private Text tiempoTexto; // Texto en UI para el tiempo
    [SerializeField] private Image[] iconosObjetos; // Iconos en UI de coleccionables

    [Header("Pausa")]
    [SerializeField] private GameObject menuPausa;

    // Datos de nivel
    private GameObject nivelActual;
    private float tiempoNivel;
    private bool nivelIniciado;
    private bool enPausa;
    private int objetosRecogidos;
    private int nivelIndex;

    // Guardado
    private Dictionary<int, LevelData> datosNiveles = new Dictionary<int, LevelData>(); //revisar //probar a modificarlo para que guarde en json

    void Start()
    {
        // Tomar el nivel que decidió el jugador
        if (GameManager.Instance != null)
        {
            IniciarNivel(GameManager.Instance.NivelSeleccionado());
            CargarDatos(); //revisar
        }
        else
        {
            Debug.LogWarning("GameManager no encontrado, cargando nivel 0 por defecto.");
            //IniciarNivel(0);
        }
    }

    void Update()
    {
        // Conteo de tiempo
        if (nivelIniciado && !enPausa)
        {
            tiempoNivel += Time.deltaTime;
            tiempoTexto.text = FormatearTiempo(tiempoNivel);
        }

        // Menú de pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausa();
        }
    }

    public void IniciarNivel(int index)
    {
        nivelIndex = index;

        // Instanciar nivel
        if (nivelActual != null) Destroy(nivelActual);
        nivelActual = Instantiate(levelsPrefabs[index], levelParent);//revisar

        // Buscar teleporter de entrada
        GameObject teleporterEntrada = GameObject.FindGameObjectWithTag("TeleporterEntrada");
        if (teleporterEntrada != null)
        {
            player.transform.position = teleporterEntrada.transform.position;
        }
        else
        {
            Debug.LogWarning("No se encontró TeleporterEntrada en el nivel.");
        }

        // Reset de datos
        tiempoNivel = 0f;
        objetosRecogidos = 0; //revisar
        nivelIniciado = true;

        // Reset de iconos a translúcido
        foreach (var icono in iconosObjetos)
        {
            var c = icono.color;
            c.a = 0.3f; // Opacidad baja
            icono.color = c;
        }
    }

    //revisar
    public void RecogerObjeto(int id)
    {
        objetosRecogidos++;
        // Marcar icono como opaco
        if (id >= 0 && id < iconosObjetos.Length)
        {
            var c = iconosObjetos[id].color;
            c.a = 1f;
            iconosObjetos[id].color = c;
        }
    }

    public void CompletarNivel()
    {
        nivelIniciado = false;

        // Guardar datos //revisar
        if (!datosNiveles.ContainsKey(nivelIndex))
            datosNiveles[nivelIndex] = new LevelData();

        var datos = datosNiveles[nivelIndex];
        datos.completado = true;
        datos.objetosRecogidos = objetosRecogidos;
        if (datos.mejorTiempo == 0f || tiempoNivel < datos.mejorTiempo)
        {
            datos.mejorTiempo = tiempoNivel;
        }

        GuardarDatos();

        // Aquí podrías cargar la escena de selección de nivel
        // SceneManager.LoadScene("LevelSelector");
    }

    private void TogglePausa()
    {
        enPausa = !enPausa;
        menuPausa.SetActive(enPausa);
        Time.timeScale = enPausa ? 0f : 1f;
    }

    private string FormatearTiempo(float t)
    {
        int minutos = Mathf.FloorToInt(t / 60f);
        int segundos = Mathf.FloorToInt(t % 60f);
        return string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    #region Guardado
    //revisar guardado de tiempo
    private void GuardarDatos()
    {
        foreach (var data in datosNiveles)
        {
            PlayerPrefs.SetInt($"Nivel_{data.Key}_Completado", data.Value.completado ? 1 : 0);
            PlayerPrefs.SetInt($"Nivel_{data.Key}_Objetos", data.Value.objetosRecogidos);
            PlayerPrefs.SetFloat($"Nivel_{data.Key}_Tiempo", data.Value.mejorTiempo);
        }
        PlayerPrefs.Save();
    }

    private void CargarDatos()
    {
        for (int i = 0; i < levelsPrefabs.Count; i++)
        {
            var datos = new LevelData();
            datos.completado = PlayerPrefs.GetInt($"Nivel_{i}_Completado", 0) == 1;
            datos.objetosRecogidos = PlayerPrefs.GetInt($"Nivel_{i}_Objetos", 0);
            datos.mejorTiempo = PlayerPrefs.GetFloat($"Nivel_{i}_Tiempo", 0f);
            datosNiveles[i] = datos;
        }
    }
    #endregion
}
[System.Serializable]
public class LevelData
{
    public bool completado;
    public float mejorTiempo;
    public int objetosRecogidos;
}