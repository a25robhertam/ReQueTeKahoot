
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreadorCuestionario : MonoBehaviour
{
    // ReQueTeKAHOOT! --------------------------------------------------
    // AUTOR: Roberto Adrian Hernando Tamayo
    // CFGS DAM-VI - DESARROLLO DE INTERFACES
    //------------------------------------------------------------------
    // Código Gestión Creación de Cuestionarios.
    //------------------------------------------------------------------

    [Header("DATOS GENERALES")]
    [SerializeField] private TMP_InputField campoTituloCuestionario;
    [SerializeField] private TMP_InputField campoTiempoCuestionario;

    [Header("DATOS DE LA PREGUNTA")]
    [SerializeField] private TMP_Text etiquetaNumeroPregunta;
    [SerializeField] private TMP_InputField campoEnunciado;
    [SerializeField] private TMP_InputField[] camposRespuestas;
    [SerializeField] private Toggle[] selectoresRespuestas;

    [Header("BOTONES DE NAVEGACION")]
    [SerializeField] private Button botonAnteriorPregunta;
    [SerializeField] private Button botonSiguientePregunta;
    [SerializeField] private Button botonEliminarPregunta;
    [SerializeField] private Button botonAnadirPregunta;

    [Header("BOTONES GENERALES")]
    [SerializeField] private Button botonCancelar;
    [SerializeField] private Button botonGuardarCuestionario;

    [Header("MENSAJES")]
    [SerializeField] private TMP_Text mensajeValidacion;

    // Lista de preguntas del cuestionario.

    private List<Pregunta> preguntas = new List<Pregunta>();

    // Índice de la pregunta que estamos editando.

    private int indicePreguntaActual = 0;

    // Controlamos la actualización de los campos.

    private bool cargandoPregunta = false;

    private void Start()
    {
        // Comprobamos que tenemos cuatro campos y cuatro selectores.

        if (camposRespuestas == null || camposRespuestas.Length != 4 ||
            selectoresRespuestas == null || selectoresRespuestas.Length != 4)
        {
            Debug.LogError("El creador necesita cuatro respuestas y cuatro selectores.");
            enabled = false;
            return;
        }

        // Registramos los eventos de los botones.

        botonAnteriorPregunta.onClick.AddListener(AnteriorPregunta);
        botonSiguientePregunta.onClick.AddListener(SiguientePregunta);
        botonEliminarPregunta.onClick.AddListener(EliminarPregunta);
        botonAnadirPregunta.onClick.AddListener(AnadirPregunta);

        botonCancelar.onClick.AddListener(CancelarCreacion);
        botonGuardarCuestionario.onClick.AddListener(GuardarCuestionario);

        // Permitimos que las preguntas comiencen sin respuesta correcta.

        for (int i = 0; i < 4; i++)
        {
            if (selectoresRespuestas[i].group != null)
            {
                selectoresRespuestas[i].group.allowSwitchOff = true;
            }
        }

        // Creamos la primera pregunta vacía.

        preguntas.Add(CrearPreguntaVacia());

        // Mostramos la primera pregunta.

        MostrarPregunta();

        OcultarMensaje();
    }

    private Pregunta CrearPreguntaVacia()
    {
        // Creamos una pregunta sin respuesta correcta seleccionada.

        Pregunta pregunta = new Pregunta();

        pregunta.enunciado = "";
        pregunta.respuestas = new string[4];
        pregunta.indiceCorrecto = -1;

        for (int i = 0; i < 4; i++)
        {
            pregunta.respuestas[i] = "";
        }

        return pregunta;
    }

    private void GuardarPreguntaActual()
    {
        // Recuperamos los datos de la pregunta visible.

        Pregunta pregunta = preguntas[indicePreguntaActual];

        pregunta.enunciado = campoEnunciado.text.Trim();

        for (int i = 0; i < 4; i++)
        {
            pregunta.respuestas[i] = camposRespuestas[i].text.Trim();
        }

        // Buscamos la respuesta correcta seleccionada.

        pregunta.indiceCorrecto = -1;

        for (int i = 0; i < 4; i++)
        {
            if (selectoresRespuestas[i].isOn)
            {
                pregunta.indiceCorrecto = i;
            }
        }
    }

    private void MostrarPregunta()
    {
        // Evitamos procesar cambios mientras actualizamos la pantalla.

        cargandoPregunta = true;

        Pregunta pregunta = preguntas[indicePreguntaActual];

        // Actualizamos el contador de preguntas.

        etiquetaNumeroPregunta.text =
            "PREGUNTA " + (indicePreguntaActual + 1) +
            " DE " + preguntas.Count;

        // Recuperamos el enunciado y las cuatro respuestas.

        campoEnunciado.SetTextWithoutNotify(pregunta.enunciado);

        for (int i = 0; i < 4; i++)
        {
            camposRespuestas[i].SetTextWithoutNotify(pregunta.respuestas[i]);
        }

        // Desmarcamos las respuestas antes de recuperar la selección.

        ToggleGroup grupo = selectoresRespuestas[0].group;

        if (grupo != null)
        {
            grupo.SetAllTogglesOff(false);
        }

        for (int i = 0; i < 4; i++)
        {
            selectoresRespuestas[i].SetIsOnWithoutNotify(false);
        }

        // Marcamos solamente la respuesta correcta previamente guardada.

        if (pregunta.indiceCorrecto >= 0 &&
            pregunta.indiceCorrecto < 4)
        {
            selectoresRespuestas[pregunta.indiceCorrecto]
                .SetIsOnWithoutNotify(true);
        }

        // Activamos los botones según la posición actual.

        botonAnteriorPregunta.interactable = indicePreguntaActual > 0;

        botonSiguientePregunta.interactable =
            indicePreguntaActual < preguntas.Count - 1;

        botonEliminarPregunta.interactable = preguntas.Count > 1;

        cargandoPregunta = false;
    }

    private bool ValidarDatosGenerales()
    {
        // Comprobamos que el título no esté vacío.

        if (string.IsNullOrWhiteSpace(campoTituloCuestionario.text))
        {
            MostrarMensaje("Introduce un título para el cuestionario.");
            return false;
        }

        // Comprobamos que el tiempo sea un número entero positivo.

        int tiempo;

        if (!int.TryParse(campoTiempoCuestionario.text, out tiempo) ||
            tiempo <= 0)
        {
            MostrarMensaje("Introduce un tiempo válido, superior a cero.");
            return false;
        }

        return true;
    }

    private bool ValidarPregunta(int indice)
    {
        // Comprobamos que el enunciado esté cumplimentado.

        Pregunta pregunta = preguntas[indice];

        if (string.IsNullOrWhiteSpace(pregunta.enunciado))
        {
            MostrarMensaje(
                "Falta el enunciado de la pregunta " + (indice + 1) + ".");

            return false;
        }

        // Comprobamos las cuatro respuestas.

        for (int i = 0; i < 4; i++)
        {
            if (string.IsNullOrWhiteSpace(pregunta.respuestas[i]))
            {
                MostrarMensaje(
                    "Completa las cuatro respuestas de la pregunta " +
                    (indice + 1) + ".");

                return false;
            }
        }

        // Comprobamos que exista una respuesta correcta.

        if (pregunta.indiceCorrecto < 0 ||
            pregunta.indiceCorrecto > 3)
        {
            MostrarMensaje(
                "Selecciona la respuesta correcta de la pregunta " +
                (indice + 1) + ".");

            return false;
        }

        return true;
    }

    private bool ValidarPreguntaActual()
    {
        // Guardamos lo introducido antes de realizar las comprobaciones.

        GuardarPreguntaActual();

        // Primero comprobamos el título y el tiempo.

        if (!ValidarDatosGenerales())
        {
            return false;
        }

        // Después comprobamos la pregunta actual.

        if (!ValidarPregunta(indicePreguntaActual))
        {
            return false;
        }

        OcultarMensaje();

        return true;
    }

    private void AnteriorPregunta()
    {
        // Permitimos retroceder aunque la pregunta esté incompleta.

        if (!cargandoPregunta && indicePreguntaActual > 0)
        {
            GuardarPreguntaActual();

            indicePreguntaActual--;

            MostrarPregunta();
            OcultarMensaje();
        }
    }

    private void SiguientePregunta()
    {
        // Solamente avanzamos si la pregunta actual es válida.

        if (!cargandoPregunta &&
            indicePreguntaActual < preguntas.Count - 1)
        {
            if (ValidarPreguntaActual())
            {
                indicePreguntaActual++;

                MostrarPregunta();
                OcultarMensaje();
            }
        }
    }

    private void AnadirPregunta()
    {
        // No permitimos añadir preguntas si la actual está incompleta.

        if (!ValidarPreguntaActual())
        {
            return;
        }

        // Añadimos una nueva pregunta al final del cuestionario.

        preguntas.Add(CrearPreguntaVacia());

        indicePreguntaActual = preguntas.Count - 1;

        MostrarPregunta();
        OcultarMensaje();
    }

    private void EliminarPregunta()
    {
        // Conservamos al menos una pregunta en el cuestionario.

        if (preguntas.Count <= 1)
        {
            return;
        }

        // Eliminamos la pregunta actual.

        preguntas.RemoveAt(indicePreguntaActual);

        // Ajustamos el índice cuando eliminamos la última.

        if (indicePreguntaActual >= preguntas.Count)
        {
            indicePreguntaActual = preguntas.Count - 1;
        }

        MostrarPregunta();
        OcultarMensaje();
    }

    private bool ValidarCuestionario()
    {
        // Comprobamos los datos generales.

        if (!ValidarDatosGenerales())
        {
            return false;
        }

        // Comprobamos todas las preguntas antes de guardar.

        for (int i = 0; i < preguntas.Count; i++)
        {
            if (!ValidarPregunta(i))
            {
                // Mostramos la pregunta que contiene el error.

                indicePreguntaActual = i;

                MostrarPregunta();

                return false;
            }
        }

        return true;
    }

    private void GuardarCuestionario()
    {
        // Guardamos los datos de la pregunta visible.

        GuardarPreguntaActual();

        // Comprobamos nuevamente todo el cuestionario.

        if (!ValidarCuestionario())
        {
            return;
        }

        // Construimos el cuestionario utilizando el modelo existente.

        Cuestionario cuestionario = new Cuestionario();

        cuestionario.id = "custom_" +
            DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "_" +
            Guid.NewGuid().ToString("N").Substring(0, 8);

        cuestionario.titulo = campoTituloCuestionario.text.Trim();

        cuestionario.tiempoPorPregunta =
            int.Parse(campoTiempoCuestionario.text);

        cuestionario.preguntas = preguntas.ToArray();

        // Guardamos el JSON en el directorio de cuestionarios propios.

        try
        {
            string carpeta = Path.Combine(
                Application.persistentDataPath, "CustomKahoots");

            Directory.CreateDirectory(carpeta);

            string rutaArchivo = Path.Combine(
                carpeta, cuestionario.id + ".json");

            string contenidoJSON =
                JsonUtility.ToJson(cuestionario, true);

            File.WriteAllText(rutaArchivo, contenidoJSON);

            Debug.Log("Cuestionario guardado: " + rutaArchivo);

            // Regresamos al selector para utilizar el nuevo cuestionario.

            SceneManager.LoadScene("SelectorCuestionario");
        }
        catch (Exception error)
        {
            Debug.LogError("Error al guardar el cuestionario: " + error);

            MostrarMensaje("No se ha podido guardar el cuestionario.");
        }
    }

    private void CancelarCreacion()
    {
        // Regresamos al menú principal sin guardar.

        SceneManager.LoadScene("MenuPrincipal");
    }

    private void MostrarMensaje(string texto)
    {
        // Mostramos el mensaje de validación.

        if (mensajeValidacion != null)
        {
            mensajeValidacion.text = texto;
            mensajeValidacion.gameObject.SetActive(true);
        }
    }

    private void OcultarMensaje()
    {
        // Ocultamos el mensaje de validación.

        if (mensajeValidacion != null)
        {
            mensajeValidacion.text = "";
            mensajeValidacion.gameObject.SetActive(false);
        }
    }
}
