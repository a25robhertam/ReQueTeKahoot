
// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Código Gestión UI del Selector de Informes


using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectorInforme : MonoBehaviour
{
    // Prefab utilizado para representar cada informe.

    [SerializeField] private FilaInforme prefabFilaInforme;

    // Contenedor donde se generarán las filas del Scroll.

    [SerializeField] private Transform contenidoListado;

    // Botones de navegación.

    [SerializeField] private Button botonMenuPrincipalInformes;
    [SerializeField] private Button botonVerInforme;

    // Ruta del informe seleccionado, compartida con VisorInforme.

    public static string rutaInformeSeleccionado;

    // Lista de filas generadas.

    private List<FilaInforme> filasInformes =
        new List<FilaInforme>();

    // Ruta del informe seleccionado en esta pantalla.

    private string informeSeleccionado;


    // Inicializa el selector de informes.

    private void Start()
    {
        // Asocia los eventos de los botones.

        botonMenuPrincipalInformes.onClick.AddListener(
            VolverMenuPrincipal
        );

        botonVerInforme.onClick.AddListener(VerInforme);

        // Desactiva VER INFORME hasta que se seleccione uno.

        botonVerInforme.interactable = false;

        informeSeleccionado = null;
        rutaInformeSeleccionado = null;

        // Busca los archivos TXT y genera el listado.

        CargarListado();

        // Establece el foco inicial de navegación.

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(
                botonMenuPrincipalInformes.gameObject
            );
        }
    }


    // Busca los informes disponibles y genera sus filas.

    private void CargarListado()
    {
        // Carpeta donde se almacenarán los informes de errores.

        string carpetaInformes = Path.Combine(
            Application.persistentDataPath,
            "Informes"
        );

        // Elimina las filas anteriores.

        foreach (FilaInforme fila in filasInformes)
        {
            if (fila != null)
            {
                Destroy(fila.gameObject);
            }
        }

        filasInformes.Clear();

        try
        {
            // Comprueba que exista la carpeta de informes.

            if (Directory.Exists(carpetaInformes))
            {
                // Recupera todos los archivos TXT de la carpeta.

                string[] archivos = Directory.GetFiles(
                    carpetaInformes,
                    "*.txt"
                );

                // Ordena los informes por nombre descendente.
                // El formato de fecha del nombre permitirá
                // mostrar primero los informes más recientes.

                Array.Sort(archivos);
                Array.Reverse(archivos);

                // Genera una fila para cada archivo encontrado.

                foreach (string archivo in archivos)
                {
                    CrearFila(archivo);
                }

                Debug.Log(
                    "TOTAL INFORMES DISPONIBLES: " + archivos.Length
                );
            }
            else
            {
                // Si todavía no existen informes, el listado queda vacío.

                Debug.Log(
                    "No existen informes de errores almacenados."
                );
            }
        }
        catch (Exception error)
        {
            Debug.LogError(
                "ERROR AL CARGAR LOS INFORMES: " + error.Message
            );
        }
    }


    // Crea una fila utilizando el prefab FilaInforme.

    private void CrearFila(string rutaArchivo)
    {
        // Instancia la fila dentro del Content del Scroll.

        FilaInforme nuevaFila = Instantiate(
            prefabFilaInforme,
            contenidoListado
        );

        // Configura la información y el evento de selección.

        nuevaFila.Configurar(
            rutaArchivo,
            SeleccionarInforme
        );

        // Guarda la fila generada.

        filasInformes.Add(nuevaFila);
    }


    // Guarda el informe seleccionado por el usuario.

    private void SeleccionarInforme(string rutaArchivo)
    {
        // Actualiza la selección actual.

        informeSeleccionado = rutaArchivo;

        // Actualiza el resaltado de las filas.

        foreach (FilaInforme fila in filasInformes)
        {
            bool seleccionado =
                fila.ObtenerRutaInforme() == informeSeleccionado;

            fila.MarcarSeleccion(seleccionado);
        }

        // Permite abrir el informe seleccionado.

        botonVerInforme.interactable = true;

        Debug.Log(
            "INFORME SELECCIONADO: " +
            Path.GetFileName(informeSeleccionado)
        );
    }


    // Abre el informe seleccionado.

    public void VerInforme()
    {
        if (!string.IsNullOrEmpty(informeSeleccionado))
        {
            // Conserva la ruta para la pantalla VisorInforme.

            rutaInformeSeleccionado = informeSeleccionado;

            // Abre la escena del visor.

            SceneManager.LoadScene("VisorInforme");
        }
        else
        {
            Debug.LogWarning(
                "Debes seleccionar un informe para visualizarlo."
            );
        }
    }


    // Regresa al menú principal.

    public void VolverMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }


    // Elimina los eventos asociados a los botones.

    private void OnDestroy()
    {
        if (botonMenuPrincipalInformes != null)
        {
            botonMenuPrincipalInformes.onClick.RemoveListener(
                VolverMenuPrincipal
            );
        }

        if (botonVerInforme != null)
        {
            botonVerInforme.onClick.RemoveListener(VerInforme);
        }
    }
}
