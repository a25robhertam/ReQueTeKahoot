
// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Código Gestión UI del Visor de Informes


using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VisorInforme : MonoBehaviour
{
    // Etiqueta que muestra el nombre del informe seleccionado.

    [SerializeField] private TMP_Text etiquetaArchivoInforme;

    // Texto donde se muestra el contenido completo del TXT.

    [SerializeField] private TMP_Text textoContenidoInforme;

    // Referencia al Scroll View del informe.

    [SerializeField] private ScrollRect scrollContenidoInformes;

    // Botones de navegación.

    [SerializeField] private Button botonMenuPrincipalVisor;
    [SerializeField] private Button botonVolverInformes;


    // Inicializa el visor y carga el informe seleccionado.

    private void Start()
    {
        // Asocia los eventos de navegación.

        botonMenuPrincipalVisor.onClick.AddListener(
            VolverMenuPrincipal
        );

        botonVolverInformes.onClick.AddListener(
            VolverInformes
        );

        // Recupera y muestra el contenido del informe.

        CargarInforme();
    }


    // Lee el archivo TXT seleccionado.

    private void CargarInforme()
    {
        // Recupera la ruta guardada por el selector de informes.

        string rutaInforme =
            SelectorInforme.rutaInformeSeleccionado;

        // Comprueba que se haya seleccionado un informe.

        if (string.IsNullOrEmpty(rutaInforme))
        {
            etiquetaArchivoInforme.text =
                "ARCHIVO: NINGUNO SELECCIONADO";

            textoContenidoInforme.text =
                "No se ha seleccionado ningún informe para visualizar.";

            Debug.LogWarning(
                "No existe ningún informe seleccionado."
            );
        }
        else
        {
            try
            {
                // Muestra el nombre del archivo en la cabecera.

                etiquetaArchivoInforme.text =
                    "ARCHIVO: " + Path.GetFileName(rutaInforme);

                // Comprueba que el archivo todavía exista.

                if (!File.Exists(rutaInforme))
                {
                    throw new FileNotFoundException(
                        "El informe seleccionado ya no existe.",
                        rutaInforme
                    );
                }

                // Lee el contenido completo del archivo TXT.

                string contenido = File.ReadAllText(rutaInforme);

                // Muestra el texto del informe.

                textoContenidoInforme.text = contenido;

                Debug.Log(
                    "INFORME CARGADO CORRECTAMENTE: " +
                    Path.GetFileName(rutaInforme)
                );
            }
            catch (Exception error)
            {
                // Muestra un mensaje comprensible al usuario.

                textoContenidoInforme.text =
                    "NO SE HA PODIDO CARGAR EL INFORME." +
                    "\n\n" +
                    "Archivo: " + Path.GetFileName(rutaInforme) +
                    "\n\n" +
                    "Descripción del error: " + error.Message;

                // Registra el error en la consola.

                Debug.LogError(
                    "ERROR AL LEER EL INFORME: " +
                    error.Message
                );

                // Genera un informe TXT del error producido.

                ServicioInformes.GenerarInforme(
                    "TXT",
                    rutaInforme,
                    error
                );
            }
        }

        // Actualiza la distribución del texto antes del desplazamiento.

        Canvas.ForceUpdateCanvases();

        // Sitúa el Scroll al principio del informe.

        scrollContenidoInformes.verticalNormalizedPosition = 1f;
    }


    // Regresa a la escena de selección de informes.

    public void VolverInformes()
    {
        SceneManager.LoadScene("SelectorInforme");
    }


    // Regresa al menú principal.

    public void VolverMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }


    // Elimina los eventos asociados a los botones.

    private void OnDestroy()
    {
        if (botonMenuPrincipalVisor != null)
        {
            botonMenuPrincipalVisor.onClick.RemoveListener(
                VolverMenuPrincipal
            );
        }

        if (botonVolverInformes != null)
        {
            botonVolverInformes.onClick.RemoveListener(
                VolverInformes
            );
        }
    }
}
