
// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Código Gestión UI de las Filas de Informes


using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilaInforme : MonoBehaviour
{
    // Textos que muestran los datos del informe.

    [SerializeField] private TMP_Text textoNombreInforme;
    [SerializeField] private TMP_Text textoTipoInforme;

    // Fondo utilizado para resaltar la fila seleccionada.

    [SerializeField] private GameObject fondoSeleccionado;

    // Botón que permite seleccionar el informe.

    [SerializeField] private Button botonInforme;

    // Ruta completa del archivo TXT asociado a la fila.

    private string rutaInforme;

    // Acción que se ejecutará al seleccionar la fila.

    private Action<string> accionSeleccionar;


    // Configura la fila con los datos del informe.

    public void Configurar(string rutaArchivo,
        Action<string> alSeleccionar)
    {
        // Guarda la ruta y la acción de selección.

        rutaInforme = rutaArchivo;
        accionSeleccionar = alSeleccionar;

        // Muestra únicamente el nombre del archivo.

        textoNombreInforme.text =
            Path.GetFileName(rutaArchivo);

        // Identifica el tipo de informe.

        string nombreArchivo =
            Path.GetFileName(rutaArchivo).ToUpper();

        if (nombreArchivo.Contains("JSON"))
        {
            textoTipoInforme.text = "JSON";
        }
        else if (nombreArchivo.Contains("XML"))
        {
            textoTipoInforme.text = "XML";
        }
        else
        {
            textoTipoInforme.text = "OTRO";
        }

        // La fila comienza sin estar seleccionada.

        MarcarSeleccion(false);

        // Asocia el evento del botón.

        botonInforme.onClick.RemoveListener(Seleccionar);
        botonInforme.onClick.AddListener(Seleccionar);
    }


    // Comunica al gestor qué informe se ha seleccionado.

    private void Seleccionar()
    {
        if (accionSeleccionar != null)
        {
            accionSeleccionar(rutaInforme);
        }
    }


    // Activa o desactiva el resaltado visual.

    public void MarcarSeleccion(bool seleccionado)
    {
        fondoSeleccionado.SetActive(seleccionado);
    }


    // Devuelve la ruta completa del informe.

    public string ObtenerRutaInforme()
    {
        return rutaInforme;
    }


    // Elimina el evento cuando se destruye la fila.

    private void OnDestroy()
    {
        if (botonInforme != null)
        {
            botonInforme.onClick.RemoveListener(Seleccionar);
        }
    }
}
