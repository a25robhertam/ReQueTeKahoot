
// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Código Gestión de la Partida


using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Partida : MonoBehaviour
{
    // Referencia al subtítulo del cuestionario.

    [SerializeField] private TMP_Text subtituloCuestionario;

    // Referencias a las estadísticas de la partida.

    [SerializeField] private TMP_Text valorJugador;
    [SerializeField] private TMP_Text valorPregunta;
    [SerializeField] private TMP_Text valorPuntuacion;
    [SerializeField] private TMP_Text valorAciertos;

    // Referencia al enunciado de la pregunta.

    [SerializeField] private TMP_Text textoPregunta;

    // Referencias a los textos de las respuestas.

    [SerializeField] private TMP_Text textoRespuestaRoja;
    [SerializeField] private TMP_Text textoRespuestaVerde;
    [SerializeField] private TMP_Text textoRespuestaAmarilla;
    [SerializeField] private TMP_Text textoRespuestaAzul;

    // Referencias a los botones de respuesta.

    [SerializeField] private Button botonRespuestaRojo;
    [SerializeField] private Button botonRespuestaVerde;
    [SerializeField] private Button botonRespuestaAmarillo;
    [SerializeField] private Button botonRespuestaAzul;

    // Botón para abandonar la partida.

    [SerializeField] private Button botonAbandonarPartida;

    // Indicadores de respuestas correctas e incorrectas.

    [SerializeField] private TMP_Text indicadorRojo;
    [SerializeField] private TMP_Text indicadorVerde;
    [SerializeField] private TMP_Text indicadorAmarillo;
    [SerializeField] private TMP_Text indicadorAzul;

    // Marcos de selección de las respuestas.

    [SerializeField] private GameObject marcoRojo;
    [SerializeField] private GameObject marcoVerde;
    [SerializeField] private GameObject marcoAmarillo;
    [SerializeField] private GameObject marcoAzul;

    // Temporizador.

    [SerializeField] private TMP_Text valorSegundos;

    // Barra de progreso.

    [SerializeField] private RectTransform barraProgreso;
    [SerializeField] private RectTransform rellenoProgreso;

    // Panel de resultados finales.

    [SerializeField] private GameObject panelResultadoFinal;

    // Textos del resultado final.

    [SerializeField] private TMP_Text textoPuntuacionFinal;
    [SerializeField] private TMP_Text textoAciertosFinal;
    [SerializeField] private TMP_Text textoTiempoFinal;

    // Botones del resultado final.

    [SerializeField] private Button botonHallFamaFinal;
    [SerializeField] private Button botonVolverMenuFinal;

    // Cuestionario seleccionado.

    private Cuestionario cuestionarioActual;

    // Estadísticas de la partida.

    private int indicePregunta = 0;
    private int puntuacion = 0;
    private int aciertos = 0;

    // Control del tiempo.

    private float tiempoRestante = 0f;
    private float tiempoTotalRespuestas = 0f;

    // Control del estado de la partida.

    private bool partidaActiva = false;
    private bool respuestaProcesada = false;

    // Control para evitar guardar dos veces la misma partida.

    private bool partidaFinalizada = false;

    // Constantes.

    private const int PUNTUACION_MAXIMA = 1000;
    private const float ESPERA_RESPUESTA = 1.5f;


    // Inicializa los botones y comienza la partida.

    private void Start()
    {
        // Asocia los eventos de los botones de respuesta.

        botonRespuestaRojo.onClick.AddListener(ResponderRojo);
        botonRespuestaVerde.onClick.AddListener(ResponderVerde);
        botonRespuestaAmarillo.onClick.AddListener(ResponderAmarillo);
        botonRespuestaAzul.onClick.AddListener(ResponderAzul);

        // Asocia los eventos de navegación.

        botonAbandonarPartida.onClick.AddListener(AbandonarPartida);

        botonHallFamaFinal.onClick.AddListener(AbrirHallFama);
        botonVolverMenuFinal.onClick.AddListener(VolverMenuPrincipal);

        // Oculta los indicadores de respuesta.

        OcultarIndicadores();
        OcultarMarcos();

        // Oculta el panel de resultados hasta terminar la partida.

        panelResultadoFinal.SetActive(false);

        // Recupera el cuestionario seleccionado.

        cuestionarioActual = DatosSesion.cuestionarioSeleccionado;

        // Comprueba que el cuestionario contiene preguntas.

        if (cuestionarioActual != null &&
            cuestionarioActual.preguntas != null &&
            cuestionarioActual.preguntas.Length > 0)
        {
            // Inicializa las estadísticas.

            indicePregunta = 0;
            puntuacion = 0;
            aciertos = 0;
            tiempoTotalRespuestas = 0f;
            partidaFinalizada = false;

            // Muestra el cuestionario y el jugador.

            subtituloCuestionario.text = cuestionarioActual.titulo;
            valorJugador.text = DatosSesion.nombreJugador;

            // Inicia la primera pregunta.

            partidaActiva = true;

            MostrarPregunta();
        }
        else
        {
            Debug.LogWarning(
                "No hay ningún cuestionario seleccionado para iniciar la partida."
            );

            ActivarBotones(false);
        }
    }


    // Actualiza la cuenta atrás de la pregunta.

    private void Update()
    {
        if (partidaActiva && !respuestaProcesada)
        {
            tiempoRestante -= Time.deltaTime;

            if (tiempoRestante <= 0f)
            {
                tiempoRestante = 0f;

                ActualizarTemporizador();

                ComprobarRespuesta(-1);
            }
            else
            {
                ActualizarTemporizador();
            }
        }
    }


    // Muestra la pregunta actual y sus respuestas.

    private void MostrarPregunta()
    {
        Pregunta preguntaActual =
            cuestionarioActual.preguntas[indicePregunta];

        // Permite procesar una nueva respuesta.

        respuestaProcesada = false;

        // Oculta los resultados de la pregunta anterior.

        OcultarIndicadores();
        OcultarMarcos();

        // Reinicia el temporizador.

        tiempoRestante = cuestionarioActual.tiempoPorPregunta;

        // Actualiza las estadísticas.

        valorPregunta.text =
            (indicePregunta + 1) + "/" +
            cuestionarioActual.preguntas.Length;

        valorPuntuacion.text = puntuacion.ToString();
        valorAciertos.text = aciertos.ToString();

        // Muestra el enunciado.

        textoPregunta.text = preguntaActual.enunciado;

        // Muestra las respuestas en el orden del JSON.

        textoRespuestaRoja.text = preguntaActual.respuestas[0];
        textoRespuestaVerde.text = preguntaActual.respuestas[1];
        textoRespuestaAmarilla.text = preguntaActual.respuestas[2];
        textoRespuestaAzul.text = preguntaActual.respuestas[3];

        // Actualiza la interfaz.

        ActualizarTemporizador();
        ActualizarBarraProgreso();

        ActivarBotones(true);

        Debug.Log("MOSTRANDO PREGUNTA: " +
            (indicePregunta + 1) + "/" +
            cuestionarioActual.preguntas.Length);
    }


    // Actualiza el contador de segundos.

    private void ActualizarTemporizador()
    {
        int segundos = Mathf.CeilToInt(tiempoRestante);

        valorSegundos.text = segundos.ToString();
    }


    // Gestiona la pulsación de cada botón.

    private void ResponderRojo()
    {
        ComprobarRespuesta(0);
    }

    private void ResponderVerde()
    {
        ComprobarRespuesta(1);
    }

    private void ResponderAmarillo()
    {
        ComprobarRespuesta(2);
    }

    private void ResponderAzul()
    {
        ComprobarRespuesta(3);
    }


    // Comprueba la respuesta y actualiza las estadísticas.

    private void ComprobarRespuesta(int indiceRespuesta)
    {
        if (partidaActiva && !respuestaProcesada)
        {
            // Evita responder más de una vez.

            respuestaProcesada = true;

            ActivarBotones(false);

            // Recupera la respuesta correcta del JSON.

            Pregunta preguntaActual =
                cuestionarioActual.preguntas[indicePregunta];

            int indiceCorrecto = preguntaActual.indiceCorrecto;

            // Calcula el tiempo empleado.

            float tiempoMaximo = cuestionarioActual.tiempoPorPregunta;

            float tiempoEmpleado = tiempoMaximo - tiempoRestante;

            tiempoTotalRespuestas += tiempoEmpleado;

            // Comprueba si el jugador ha acertado.

            if (indiceRespuesta == -1 || tiempoRestante <= 0f)
            {
                Debug.Log("TIEMPO AGOTADO");
            }
            else if (indiceRespuesta == indiceCorrecto)
            {
                aciertos++;

                int puntos =
                    CalcularPuntuacion(tiempoEmpleado, tiempoMaximo);

                puntuacion += puntos;

                Debug.Log("RESPUESTA CORRECTA: +" +
                    puntos + " PUNTOS");
            }
            else
            {
                Debug.Log("RESPUESTA INCORRECTA");
            }

            // Actualiza los valores visibles.

            valorPuntuacion.text = puntuacion.ToString();
            valorAciertos.text = aciertos.ToString();

            // Muestra las respuestas correctas e incorrectas.

            MostrarResultado(indiceCorrecto);

            // Resalta únicamente el botón pulsado.

            MostrarMarcoSeleccionado(indiceRespuesta);

            // Espera antes de avanzar.

            StartCoroutine(AvanzarPregunta());
        }
    }


    // Calcula los puntos según el tiempo de respuesta.

    private int CalcularPuntuacion(float tiempoEmpleado, float tiempoMaximo)
    {
        int puntos = PUNTUACION_MAXIMA;

        if (tiempoEmpleado >= 0.5f)
        {
            float resultado = PUNTUACION_MAXIMA *
                (1f - tiempoEmpleado / (2f * tiempoMaximo));

            puntos = Mathf.RoundToInt(resultado);
        }

        // Una respuesta correcta obtiene entre 500 y 1000 puntos.

        puntos = Mathf.Clamp(puntos, 500, PUNTUACION_MAXIMA);

        return puntos;
    }


    // Muestra un tick en la respuesta correcta y cruces en las demás.

    private void MostrarResultado(int indiceCorrecto)
    {
        TMP_Text[] indicadores = {
            indicadorRojo,
            indicadorVerde,
            indicadorAmarillo,
            indicadorAzul
        };

        for (int i = 0; i < indicadores.Length; i++)
        {
            if (i == indiceCorrecto)
            {
                indicadores[i].text = "✓";
                indicadores[i].color = Color.green;
            }
            else
            {
                indicadores[i].text = "✗";
                indicadores[i].color = Color.red;
            }

            indicadores[i].gameObject.SetActive(true);
        }
    }


    // Muestra el marco del botón seleccionado.

    private void MostrarMarcoSeleccionado(int indiceRespuesta)
    {
        GameObject[] marcos = {
            marcoRojo,
            marcoVerde,
            marcoAmarillo,
            marcoAzul
        };

        OcultarMarcos();

        // No muestra ningún marco si se agota el tiempo.

        if (indiceRespuesta >= 0 && indiceRespuesta < marcos.Length)
        {
            marcos[indiceRespuesta].SetActive(true);
        }
    }


    // Oculta los indicadores de resultado.

    private void OcultarIndicadores()
    {
        indicadorRojo.gameObject.SetActive(false);
        indicadorVerde.gameObject.SetActive(false);
        indicadorAmarillo.gameObject.SetActive(false);
        indicadorAzul.gameObject.SetActive(false);
    }


    // Oculta los marcos de selección.

    private void OcultarMarcos()
    {
        marcoRojo.SetActive(false);
        marcoVerde.SetActive(false);
        marcoAmarillo.SetActive(false);
        marcoAzul.SetActive(false);
    }


    // Espera y avanza a la siguiente pregunta.

    private IEnumerator AvanzarPregunta()
    {
        yield return new WaitForSeconds(ESPERA_RESPUESTA);

        indicePregunta++;

        if (indicePregunta < cuestionarioActual.preguntas.Length)
        {
            MostrarPregunta();
        }
        else
        {
            FinalizarPartida();
        }
    }


    // Activa o desactiva las respuestas.

    private void ActivarBotones(bool activar)
    {
        botonRespuestaRojo.interactable = activar;
        botonRespuestaVerde.interactable = activar;
        botonRespuestaAmarillo.interactable = activar;
        botonRespuestaAzul.interactable = activar;
    }


    // Actualiza la barra de progreso.

    private void ActualizarBarraProgreso()
    {
        float progreso =
            (float)(indicePregunta + 1) /
            cuestionarioActual.preguntas.Length;

        float anchuraRelleno =
            barraProgreso.rect.width * progreso;

        rellenoProgreso.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            anchuraRelleno
        );
    }


    // Finaliza la partida, guarda el XML y muestra el resultado.

    private void FinalizarPartida()
    {
        // Evita finalizar y guardar dos veces la misma partida.

        if (!partidaFinalizada)
        {
            partidaFinalizada = true;

            // Detiene definitivamente la partida.

            partidaActiva = false;
            respuestaProcesada = true;

            ActivarBotones(false);

            // Oculta los indicadores de la última pregunta.

            OcultarIndicadores();
            OcultarMarcos();

            // Completa la barra de progreso.

            rellenoProgreso.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                barraProgreso.rect.width
            );

            // Prepara los datos que se guardarán en el XML.

            ResultadoPartida resultado = new ResultadoPartida();

            resultado.nombreJugador = DatosSesion.nombreJugador;

            resultado.idCuestionario = cuestionarioActual.id;
            resultado.tituloCuestionario = cuestionarioActual.titulo;

            resultado.fechaPartida =
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            resultado.puntuacion = puntuacion;
            resultado.aciertos = aciertos;

            resultado.totalPreguntas =
                cuestionarioActual.preguntas.Length;

            resultado.tiempoTotal = tiempoTotalRespuestas;

            // Guarda la partida en el XML del cuestionario.

            bool guardadoCorrectamente =
                ServicioResultados.GuardarResultado(resultado);

            if (guardadoCorrectamente)
            {
                Debug.Log("PARTIDA GUARDADA CORRECTAMENTE EN XML.");
            }
            else
            {
                Debug.LogError(
                    "NO SE HA PODIDO GUARDAR LA PARTIDA EN XML."
                );
            }

            // Muestra los resultados reales en pantalla.

            textoPuntuacionFinal.text =
                "PUNTUACIÓN FINAL: " + puntuacion;

            textoAciertosFinal.text =
                "ACIERTOS: " + aciertos + "/" +
                cuestionarioActual.preguntas.Length;

            textoTiempoFinal.text =
                "TIEMPO TOTAL: " +
                tiempoTotalRespuestas.ToString("F2") +
                " SEGUNDOS";

            // Activa el panel de resultados.

            panelResultadoFinal.SetActive(true);

            // Muestra los resultados en la consola.

            Debug.Log("==================================");
            Debug.Log("PARTIDA FINALIZADA");
            Debug.Log("JUGADOR: " + DatosSesion.nombreJugador);
            Debug.Log("CUESTIONARIO: " + cuestionarioActual.titulo);
            Debug.Log("ACIERTOS: " + aciertos + "/" +
                cuestionarioActual.preguntas.Length);
            Debug.Log("PUNTUACIÓN FINAL: " + puntuacion);
            Debug.Log("TIEMPO TOTAL: " +
                tiempoTotalRespuestas.ToString("F2") + " SEGUNDOS");
            Debug.Log("==================================");
        }
    }


    // Abandona la partida sin guardar resultados.

    public void AbandonarPartida()
    {
        partidaActiva = false;

        StopAllCoroutines();

        SceneManager.LoadScene("SelectorCuestionario");
    }


    // Abre la escena del Hall de la Fama.

    public void AbrirHallFama()
    {
        SceneManager.LoadScene("HallFama");
    }


    // Regresa al menú principal.

    public void VolverMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }


    // Elimina los eventos asociados al destruir el objeto.

    private void OnDestroy()
    {
        if (botonRespuestaRojo != null)
        {
            botonRespuestaRojo.onClick.RemoveListener(ResponderRojo);
        }

        if (botonRespuestaVerde != null)
        {
            botonRespuestaVerde.onClick.RemoveListener(ResponderVerde);
        }

        if (botonRespuestaAmarillo != null)
        {
            botonRespuestaAmarillo.onClick.RemoveListener(ResponderAmarillo);
        }

        if (botonRespuestaAzul != null)
        {
            botonRespuestaAzul.onClick.RemoveListener(ResponderAzul);
        }

        if (botonAbandonarPartida != null)
        {
            botonAbandonarPartida.onClick.RemoveListener(AbandonarPartida);
        }

        if (botonHallFamaFinal != null)
        {
            botonHallFamaFinal.onClick.RemoveListener(AbrirHallFama);
        }

        if (botonVolverMenuFinal != null)
        {
            botonVolverMenuFinal.onClick.RemoveListener(VolverMenuPrincipal);
        }
    }
}
