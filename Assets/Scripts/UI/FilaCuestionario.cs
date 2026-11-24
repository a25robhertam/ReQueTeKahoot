
 // ReQueTeKAHOOT! --------------------------------------------------
 // AUTOR: Roberto Adrian Hernando Tamayo
 // CFGS DAM-VI - DESARROLLO DE INTERFACES
 //------------------------------------------------------------------
 // Código Gestión UI de las Filas de Cuestionarios


using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilaCuestionario : MonoBehaviour
{
    // Referencias a los tres textos de la fila.

    [SerializeField] private TMP_Text textoTitulo;
    [SerializeField] private TMP_Text textoPreguntas;
    [SerializeField] private TMP_Text textoTipo;

    // Referencia al botón que representa la fila.

    [SerializeField] private Button botonFila;

    // Referencia al indicador visual de selección persistente.

    [SerializeField] private GameObject fondoSeleccionado;

    // Colores para identificar el tipo de cuestionario.

    [SerializeField] private Color colorPredeterminado = Color.white;
    [SerializeField] private Color colorPersonalizado = Color.green;

    // Cuestionario asociado a esta fila.

    private Cuestionario cuestionario;

    // Acción que se ejecutará al seleccionar el cuestionario.

    private Action<Cuestionario> accionSeleccionar;


    // Configura los datos de la fila a partir del cuestionario recibido.

    public void Configurar(Cuestionario datos, bool personalizado,
        Action<Cuestionario> alSeleccionar)
    {
        // Guarda las referencias necesarias.

        cuestionario = datos;
        accionSeleccionar = alSeleccionar;

        // Inicialmente, la fila no está seleccionada.

        MarcarSeleccion(false);

        // Muestra los datos del cuestionario.

        textoTitulo.text = cuestionario.titulo;

        textoPreguntas.text =
            cuestionario.preguntas.Length + " PREGUNTAS";

        // Identifica visualmente los cuestionarios personalizados.

        if (personalizado)
        {
            textoTipo.text = "PERSONALIZADO";
            textoTipo.color = colorPersonalizado;
        }
        else
        {
            textoTipo.text = "PREDETERMINADO";
            textoTipo.color = colorPredeterminado;
        }

        // Asocia la selección al evento de pulsación del botón.

        botonFila.onClick.RemoveListener(Seleccionar);
        botonFila.onClick.AddListener(Seleccionar);
    }


    // Comunica al selector qué cuestionario ha elegido el usuario.

    private void Seleccionar()
    {
        if (cuestionario != null && accionSeleccionar != null)
        {
            accionSeleccionar(cuestionario);
        }
    }


    // Activa o desactiva el resaltado persistente de la fila.

    public void MarcarSeleccion(bool seleccionado)
    {
        fondoSeleccionado.SetActive(seleccionado);
    }


    // Devuelve el cuestionario asociado a esta fila.

    public Cuestionario ObtenerCuestionario()
    {
        return cuestionario;
    }


    // Elimina la asociación al evento cuando se destruye la fila.

    private void OnDestroy()
    {
        if (botonFila != null)
        {
            botonFila.onClick.RemoveListener(Seleccionar);
        }
    }
}

