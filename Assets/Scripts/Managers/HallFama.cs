
// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Código Gestión del Hall de la Fama


using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HallFama : MonoBehaviour
{
    // Nombre del cuestionario mostrado en pantalla.

    [SerializeField] private TMP_Text nombreCuestionarioHallFama;

    // Referencias al contenido del Scroll y al prefab de las filas.

    [SerializeField] private Transform contenidoClasificacion;
    [SerializeField] private GameObject filaClasificacionPrefab;

    // Referencias a las tres medallas.

    [SerializeField] private GameObject medallaOro;
    [SerializeField] private GameObject medallaPlata;
    [SerializeField] private GameObject medallaBronce;

    // Referencias a los jugadores del podio.

    [SerializeField] private TMP_Text jugadorOro;
    [SerializeField] private TMP_Text jugadorPlata;
    [SerializeField] private TMP_Text jugadorBronce;

    // Referencias a los botones de navegación.

    [SerializeField] private Button botonMenuPrincipalHall;
    [SerializeField] private Button botonCambiarCuestionarioHall;
    [SerializeField] private Button botonJugarDeNuevoHall;


    // Inicializa el Hall de la Fama.

    private void Start()
    {
        // Asocia los eventos de los tres botones.

        botonMenuPrincipalHall.onClick.AddListener(VolverMenuPrincipal);

        botonCambiarCuestionarioHall.onClick.AddListener(
            CambiarCuestionario
        );

        botonJugarDeNuevoHall.onClick.AddListener(JugarDeNuevo);

        // Comprueba si existe un cuestionario seleccionado.

        if (DatosSesion.cuestionarioSeleccionado != null)
        {
            // Recupera el identificador y el título del cuestionario.

            string idCuestionario =
                DatosSesion.cuestionarioSeleccionado.id;

            string tituloCuestionario =
                DatosSesion.cuestionarioSeleccionado.titulo;

            // Carga su clasificación desde el XML correspondiente.

            MostrarClasificacion(idCuestionario, tituloCuestionario);
        }
        else
        {
            // No existe ningún cuestionario seleccionado.

            nombreCuestionarioHallFama.text =
                "SELECCIONA UN CUESTIONARIO";

            ActualizarPodio(new List<ResultadoPartida>());

            // Impide comenzar una partida sin selección.

            botonJugarDeNuevoHall.interactable = false;

            Debug.Log(
                "HALL DE LA FAMA: No hay cuestionario seleccionado."
            );
        }
    }


    // Carga y ordena la clasificación del cuestionario seleccionado.

    private void MostrarClasificacion(
        string idCuestionario,
        string tituloCuestionario)
    {
        // Recupera los resultados del XML correspondiente.

        List<ResultadoPartida> resultados =
            ServicioResultados.CargarResultados(idCuestionario);

        // Ordena por puntuación descendente y menor tiempo en empates.

        resultados.Sort((primero, segundo) =>
        {
            int comparacion =
                segundo.puntuacion.CompareTo(primero.puntuacion);

            if (comparacion == 0)
            {
                comparacion =
                    primero.tiempoTotal.CompareTo(segundo.tiempoTotal);
            }

            return comparacion;
        });

        // Elimina las filas generadas anteriormente.

        foreach (Transform fila in contenidoClasificacion)
        {
            Destroy(fila.gameObject);
        }

        // Muestra el título recuperado del cuestionario JSON.

        nombreCuestionarioHallFama.text = tituloCuestionario;

        // Genera una fila por cada resultado del XML.

        for (int i = 0; i < resultados.Count; i++)
        {
            CrearFila(i + 1, resultados[i]);
        }

        // Actualiza los tres primeros puestos del podio.

        ActualizarPodio(resultados);

        Debug.Log(
            "HALL DE LA FAMA - CUESTIONARIO: " + idCuestionario
        );

        Debug.Log(
            "RESULTADOS CARGADOS: " + resultados.Count
        );
    }


    // Crea una fila utilizando el prefab FilaClasificacion.

    private void CrearFila(int posicion, ResultadoPartida resultado)
    {
        // Crea la fila dentro del Content del Scroll View.

        GameObject nuevaFila = Instantiate(
            filaClasificacionPrefab,
            contenidoClasificacion
        );

        nuevaFila.SetActive(true);

        nuevaFila.name = "FilaClasificacion_" + posicion;

        // Recupera los textos de las cinco columnas.

        TMP_Text valorPosicion =
            nuevaFila.transform.Find("ValorPosicion")
                .GetComponent<TMP_Text>();

        TMP_Text valorJugador =
            nuevaFila.transform.Find("ValorJugador")
                .GetComponent<TMP_Text>();

        TMP_Text valorAciertos =
            nuevaFila.transform.Find("ValorAciertos")
                .GetComponent<TMP_Text>();

        TMP_Text valorTiempo =
            nuevaFila.transform.Find("ValorTiempo")
                .GetComponent<TMP_Text>();

        TMP_Text valorPuntuacion =
            nuevaFila.transform.Find("ValorPuntuacion")
                .GetComponent<TMP_Text>();

        // Muestra la posición y el nombre del jugador.

        valorPosicion.text = posicion.ToString();

        valorJugador.text = resultado.nombreJugador;

        // Muestra el número de aciertos.

        valorAciertos.text =
            resultado.aciertos + "/" + resultado.totalPreguntas;

        // Convierte el tiempo a minutos y segundos.

        int segundos = Mathf.FloorToInt(resultado.tiempoTotal);

        int minutos = segundos / 60;
        int segundosRestantes = segundos % 60;

        valorTiempo.text =
            minutos + ":" + segundosRestantes.ToString("00");

        // Formatea la puntuación con separador de millares.

        valorPuntuacion.text =
            resultado.puntuacion.ToString(
                "N0",
                CultureInfo.GetCultureInfo("es-ES")
            );
    }


    // Actualiza las medallas y los nombres del podio.

    private void ActualizarPodio(List<ResultadoPartida> resultados)
    {
        // Muestra solo las medallas que tengan un resultado asociado.

        medallaOro.SetActive(resultados.Count >= 1);
        jugadorOro.gameObject.SetActive(resultados.Count >= 1);

        medallaPlata.SetActive(resultados.Count >= 2);
        jugadorPlata.gameObject.SetActive(resultados.Count >= 2);

        medallaBronce.SetActive(resultados.Count >= 3);
        jugadorBronce.gameObject.SetActive(resultados.Count >= 3);

        // Primer puesto.

        if (resultados.Count >= 1)
        {
            jugadorOro.text = resultados[0].nombreJugador;
        }

        // Segundo puesto.

        if (resultados.Count >= 2)
        {
            jugadorPlata.text = resultados[1].nombreJugador;
        }

        // Tercer puesto.

        if (resultados.Count >= 3)
        {
            jugadorBronce.text = resultados[2].nombreJugador;
        }
    }


    // Regresa al menú principal.

    public void VolverMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }


    // Abre el selector para elegir otro cuestionario.

    public void CambiarCuestionario()
    {
        SceneManager.LoadScene("SelectorCuestionario");
    }


    // Inicia una nueva partida con el cuestionario y jugador actuales.

    public void JugarDeNuevo()
    {
        // Comprueba que existen los datos necesarios.

        if (DatosSesion.cuestionarioSeleccionado != null &&
            !string.IsNullOrWhiteSpace(DatosSesion.nombreJugador))
        {
            SceneManager.LoadScene("Partida");
        }
        else
        {
            // Si faltan datos, vuelve al selector.

            Debug.LogWarning(
                "No se puede iniciar la partida sin " +
                "cuestionario y nombre de jugador."
            );

            SceneManager.LoadScene("SelectorCuestionario");
        }
    }


    // Elimina los eventos cuando se destruye el objeto.

    private void OnDestroy()
    {
        if (botonMenuPrincipalHall != null)
        {
            botonMenuPrincipalHall.onClick.RemoveListener(
                VolverMenuPrincipal
            );
        }

        if (botonCambiarCuestionarioHall != null)
        {
            botonCambiarCuestionarioHall.onClick.RemoveListener(
                CambiarCuestionario
            );
        }

        if (botonJugarDeNuevoHall != null)
        {
            botonJugarDeNuevoHall.onClick.RemoveListener(
                JugarDeNuevo
            );
        }
    }
}
