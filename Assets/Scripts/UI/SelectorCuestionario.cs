
// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Código Gestión UI del Selector de Cuestionarios


using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SelectorCuestionario : MonoBehaviour
{
    // Prefab utilizado para representar cada cuestionario.

    [SerializeField] private FilaCuestionario prefabFilaCuestionario;

    // Contenedor donde se generarán las filas.

    [SerializeField] private Transform contenidoListado;

    // Campo donde el usuario introduce su nombre.

    [SerializeField] private TMP_InputField campoNombreJugador;

    // Texto para mostrar los mensajes de validación al usuario.

    [SerializeField] private TMP_Text mensajeValidacion;

    // Servicio encargado de leer los cuestionarios JSON.

    private ServicioCuestionarios servicioCuestionarios;

    // Cuestionario seleccionado por el usuario.

    private Cuestionario cuestionarioSeleccionado;

    // Lista de filas generadas para controlar la selección visual.

    private List<FilaCuestionario> filasCuestionarios =
        new List<FilaCuestionario>();

    // Control de la última operación que ha requerido validación.

    private bool validacionActiva = false;
    private bool validacionHallFama = false;


    // Inicializa el selector al comenzar la escena.

    private void Start()
    {
        // Crea una instancia del servicio de cuestionarios.

        servicioCuestionarios = new ServicioCuestionarios();

        // Recupera el nombre del jugador.

        campoNombreJugador.text = DatosSesion.nombreJugador;

        // Oculta el mensaje al iniciar la escena.

        OcultarMensajeValidacion();

        // Detecta los cambios en el nombre del jugador.

        campoNombreJugador.onValueChanged.AddListener(
            ComprobarCambioNombre
        );

        // Genera las filas y recupera la selección anterior.

        CargarListado();

        // Establece el foco inicial de navegación.

        EstablecerFocoInicial();
    }


    // Lee los cuestionarios y genera sus filas en pantalla.

    private void CargarListado()
    {
        // Obtiene los cuestionarios predeterminados.

        List<Cuestionario> predeterminados =
            servicioCuestionarios.CargarPredeterminados();

        // Obtiene los cuestionarios personalizados.

        List<Cuestionario> personalizados =
            servicioCuestionarios.CargarPersonalizados();

        // Elimina las filas generadas anteriormente.

        foreach (FilaCuestionario fila in filasCuestionarios)
        {
            if (fila != null)
            {
                Destroy(fila.gameObject);
            }
        }

        // Limpia las referencias anteriores.

        filasCuestionarios.Clear();
        cuestionarioSeleccionado = null;

        // Genera las filas de los cuestionarios predeterminados.

        foreach (Cuestionario cuestionario in predeterminados)
        {
            CrearFila(cuestionario, false);
        }

        // Genera las filas de los cuestionarios personalizados.

        foreach (Cuestionario cuestionario in personalizados)
        {
            CrearFila(cuestionario, true);
        }

        // Recupera el cuestionario seleccionado anteriormente.

        RecuperarSeleccion();

        // Muestra el número total de cuestionarios cargados.

        Debug.Log("TOTAL CUESTIONARIOS: "
            + (predeterminados.Count + personalizados.Count));
    }


    // Crea una fila utilizando el Prefab y los datos del JSON.

    private void CrearFila(Cuestionario cuestionario, bool personalizado)
    {
        // Instancia una fila dentro del contenedor del Scroll View.

        FilaCuestionario nuevaFila = Instantiate(
            prefabFilaCuestionario, contenidoListado
        );

        // Configura los textos y la acción de selección.

        nuevaFila.Configurar(
            cuestionario,
            personalizado,
            SeleccionarCuestionario
        );

        // Guarda la fila para gestionar su selección visual.

        filasCuestionarios.Add(nuevaFila);
    }


    // Guarda el cuestionario elegido por el usuario.

    private void SeleccionarCuestionario(Cuestionario cuestionario)
    {
        // Actualiza la selección actual.

        cuestionarioSeleccionado = cuestionario;

        // Conserva el cuestionario entre escenas.

        DatosSesion.cuestionarioSeleccionado = cuestionario;

        // Actualiza el resaltado de las filas.

        ActualizarSeleccionVisual();

        // Actualiza el aviso si había un error de validación.

        ActualizarMensajeValidacion();

        // Muestra la selección en Console.

        Debug.Log("CUESTIONARIO SELECCIONADO: "
            + cuestionarioSeleccionado.titulo);
    }


    // Recupera la selección guardada al regresar a esta escena.

    private void RecuperarSeleccion()
    {
        // Comprueba si existe un cuestionario guardado.

        if (DatosSesion.cuestionarioSeleccionado != null)
        {
            // Busca su identificador entre las filas cargadas.

            foreach (FilaCuestionario fila in filasCuestionarios)
            {
                if (fila.ObtenerCuestionario().id ==
                    DatosSesion.cuestionarioSeleccionado.id)
                {
                    cuestionarioSeleccionado =
                        fila.ObtenerCuestionario();
                }
            }

            // Si el cuestionario ya no existe, limpia el dato guardado.

            if (cuestionarioSeleccionado == null)
            {
                DatosSesion.cuestionarioSeleccionado = null;
            }
            else
            {
                // Actualiza la referencia con los datos recién cargados.

                DatosSesion.cuestionarioSeleccionado =
                    cuestionarioSeleccionado;
            }
        }

        // Actualiza el aspecto de las filas.

        ActualizarSeleccionVisual();
    }


    // Activa el resaltado únicamente en la fila seleccionada.

    private void ActualizarSeleccionVisual()
    {
        foreach (FilaCuestionario fila in filasCuestionarios)
        {
            bool seleccionado =
                fila.ObtenerCuestionario() == cuestionarioSeleccionado;

            fila.MarcarSeleccion(seleccionado);
        }
    }


    // Establece el foco sobre el cuestionario previamente seleccionado.

    private void EstablecerFocoInicial()
    {
        // Comprueba que exista un EventSystem.

        if (EventSystem.current != null)
        {
            // Si ya había un cuestionario seleccionado,
            // busca su fila para asignarle también el foco.

            if (cuestionarioSeleccionado != null)
            {
                foreach (FilaCuestionario fila in filasCuestionarios)
                {
                    if (fila.ObtenerCuestionario() ==
                        cuestionarioSeleccionado)
                    {
                        EventSystem.current.SetSelectedGameObject(
                            fila.gameObject
                        );
                    }
                }
            }
            else
            {
                // Si no hay cuestionario seleccionado,
                // sitúa el foco en el campo del nombre.

                EventSystem.current.SetSelectedGameObject(
                    campoNombreJugador.gameObject
                );
            }
        }
    }


    // Muestra un mensaje de error en la interfaz.

    private void MostrarMensajeValidacion(string mensaje)
    {
        mensajeValidacion.text = mensaje;

        mensajeValidacion.gameObject.SetActive(true);
    }


    // Oculta el mensaje de validación.

    private void OcultarMensajeValidacion()
    {
        mensajeValidacion.gameObject.SetActive(false);
    }


    // Comprueba los datos y actualiza el mensaje visible.

    private void ActualizarMensajeValidacion()
    {
        // Solo comprueba los datos si existe un error previo.

        if (validacionActiva)
        {
            bool faltaNombre = string.IsNullOrWhiteSpace(
                campoNombreJugador.text
            );

            bool faltaCuestionario =
                cuestionarioSeleccionado == null;

            // Para consultar el Hall solo se necesita un cuestionario.

            if (validacionHallFama)
            {
                if (faltaCuestionario)
                {
                    MostrarMensajeValidacion(
                        "SELECCIONA UN CUESTIONARIO PARA CONSULTAR EL HALL DE LA FAMA"
                    );
                }
                else
                {
                    validacionActiva = false;

                    OcultarMensajeValidacion();
                }
            }
            else
            {
                // Para jugar se necesitan nombre y cuestionario.

                if (faltaNombre && faltaCuestionario)
                {
                    MostrarMensajeValidacion(
                        "INTRODUCE TU NOMBRE Y SELECCIONA UN CUESTIONARIO"
                    );
                }
                else if (faltaNombre)
                {
                    MostrarMensajeValidacion(
                        "INTRODUCE TU NOMBRE PARA JUGAR"
                    );
                }
                else if (faltaCuestionario)
                {
                    MostrarMensajeValidacion(
                        "SELECCIONA UN CUESTIONARIO PARA JUGAR"
                    );
                }
                else
                {
                    // Los dos datos ya son correctos.

                    validacionActiva = false;

                    OcultarMensajeValidacion();
                }
            }
        }
    }


    // Comprueba el mensaje al modificar el nombre del jugador.

    private void ComprobarCambioNombre(string nuevoNombre)
    {
        ActualizarMensajeValidacion();
    }


    // Regresa a la escena del menú principal.

    public void Volver()
    {
        // Conserva el nombre introducido.

        DatosSesion.nombreJugador =
            campoNombreJugador.text.Trim();

        // Abre el menú principal.

        SceneManager.LoadScene("MenuPrincipal");
    }


    // Abre el Hall de la Fama del cuestionario seleccionado.

    public void AbrirHallFama()
    {
        if (cuestionarioSeleccionado != null)
        {
            // Conserva los datos de la sesión.

            DatosSesion.cuestionarioSeleccionado =
                cuestionarioSeleccionado;

            DatosSesion.nombreJugador =
                campoNombreJugador.text.Trim();

            // Abre el Hall de la Fama.

            SceneManager.LoadScene("HallFama");
        }
        else
        {
            // Activa la validación específica del Hall de la Fama.

            validacionActiva = true;
            validacionHallFama = true;

            ActualizarMensajeValidacion();

            Debug.LogWarning(
                "Debes seleccionar un cuestionario para consultar el Hall de la Fama."
            );
        }
    }


    // Inicia una partida con el cuestionario seleccionado.

    public void Jugar()
    {
        // Obtiene el nombre del jugador.

        string nombre = campoNombreJugador.text.Trim();

        // Comprueba si falta algún dato.

        bool faltaNombre = string.IsNullOrWhiteSpace(nombre);

        bool faltaCuestionario =
            cuestionarioSeleccionado == null;

        if (faltaNombre || faltaCuestionario)
        {
            // Activa la validación específica de la partida.

            validacionActiva = true;
            validacionHallFama = false;

            ActualizarMensajeValidacion();

            // Conserva también el aviso en Console.

            Debug.LogWarning(
                "No se puede iniciar la partida: faltan datos obligatorios."
            );
        }
        else
        {
            // Oculta cualquier mensaje anterior.

            validacionActiva = false;

            OcultarMensajeValidacion();

            // Guarda los datos de la partida.

            DatosSesion.nombreJugador = nombre;

            DatosSesion.cuestionarioSeleccionado =
                cuestionarioSeleccionado;

            Debug.Log("INICIANDO PARTIDA: "
                + DatosSesion.nombreJugador
                + " - "
                + DatosSesion.cuestionarioSeleccionado.titulo);

            // Abre la escena de la partida.

            SceneManager.LoadScene("Partida");
        }
    }


    // Elimina los eventos asociados al destruir el objeto.

    private void OnDestroy()
    {
        if (campoNombreJugador != null)
        {
            campoNombreJugador.onValueChanged.RemoveListener(
                ComprobarCambioNombre
            );
        }
    }
}
