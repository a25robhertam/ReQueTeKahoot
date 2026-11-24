
// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Código Servicio de Lectura de Cuestionarios JSON


using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ServicioCuestionarios
{
    // Ruta de los cuestionarios personalizados.

    private string rutaCustom = Path.Combine(
        Application.persistentDataPath, "CustomKahoots"
    );


    // Lee los cuestionarios predeterminados incluidos en el proyecto.

    public List<Cuestionario> CargarPredeterminados()
    {
        List<Cuestionario> cuestionarios = new List<Cuestionario>();

        // Obtiene los archivos JSON de la carpeta Resources/Kahoots.

        TextAsset[] archivos = Resources.LoadAll<TextAsset>("Kahoots");

        // Recorre los archivos encontrados.

        foreach (TextAsset archivo in archivos)
        {
            try
            {
                // Convierte el contenido JSON en un objeto Cuestionario.

                Cuestionario cuestionario =
                    JsonUtility.FromJson<Cuestionario>(archivo.text);

                // Incorpora el cuestionario si sus datos son correctos.

                if (EsValido(cuestionario))
                {
                    cuestionarios.Add(cuestionario);
                }
                else
                {
                    // Genera una excepción cuando los datos del
                    // cuestionario no cumplen las condiciones necesarias.

                    throw new FormatException(
                        "El cuestionario JSON no contiene todos " +
                        "los datos obligatorios o alguno de " +
                        "sus valores es incorrecto."
                    );
                }
            }
            catch (Exception error)
            {
                // Muestra el error en la consola de Unity.

                Debug.LogError(
                    "Error leyendo " + archivo.name +
                    ": " + error.Message
                );

                // Genera el informe TXT correspondiente.

                ServicioInformes.GenerarInforme(
                    "JSON",
                    "Resources/Kahoots/" + archivo.name + ".json",
                    error
                );
            }
        }

        return cuestionarios;
    }


    // Lee los cuestionarios personalizados almacenados en el disco.

    public List<Cuestionario> CargarPersonalizados()
    {
        List<Cuestionario> cuestionarios = new List<Cuestionario>();

        try
        {
            // Crea la carpeta de cuestionarios personalizados si no existe.

            Directory.CreateDirectory(rutaCustom);

            // Obtiene los archivos JSON de la carpeta.

            string[] archivos = Directory.GetFiles(rutaCustom, "*.json");

            // Recorre los archivos encontrados.

            foreach (string archivo in archivos)
            {
                try
                {
                    // Lee el contenido del archivo JSON.

                    string contenido = File.ReadAllText(archivo);

                    // Convierte el JSON en un objeto Cuestionario.

                    Cuestionario cuestionario =
                        JsonUtility.FromJson<Cuestionario>(contenido);

                    // Incorpora el cuestionario si sus datos son correctos.

                    if (EsValido(cuestionario))
                    {
                        cuestionarios.Add(cuestionario);
                    }
                    else
                    {
                        // Genera una excepción cuando faltan
                        // datos obligatorios o son incorrectos.

                        throw new FormatException(
                            "El cuestionario JSON no contiene todos " +
                            "los datos obligatorios o alguno de " +
                            "sus valores es incorrecto."
                        );
                    }
                }
                catch (Exception error)
                {
                    // Muestra el error del archivo en la consola.

                    Debug.LogError(
                        "Error leyendo " + archivo +
                        ": " + error.Message
                    );

                    // Genera un informe TXT del archivo afectado.

                    ServicioInformes.GenerarInforme(
                        "JSON",
                        archivo,
                        error
                    );
                }
            }
        }
        catch (Exception error)
        {
            // Muestra el error de acceso a la carpeta.

            Debug.LogError(
                "Error accediendo a CustomKahoots: " +
                error.Message
            );

            // Genera un informe TXT del problema de acceso.

            ServicioInformes.GenerarInforme(
                "JSON",
                rutaCustom,
                error
            );
        }

        return cuestionarios;
    }


    // Comprueba que un cuestionario contiene los datos necesarios.

    private bool EsValido(Cuestionario cuestionario)
    {
        bool valido = cuestionario != null;

        // Comprueba los datos generales del cuestionario.

        if (valido)
        {
            valido = !string.IsNullOrWhiteSpace(cuestionario.id)
                && !string.IsNullOrWhiteSpace(cuestionario.titulo)
                && cuestionario.tiempoPorPregunta > 0
                && cuestionario.preguntas != null
                && cuestionario.preguntas.Length > 0;
        }

        // Comprueba todas las preguntas del cuestionario.

        int posicion = 0;

        while (valido && posicion < cuestionario.preguntas.Length)
        {
            Pregunta pregunta = cuestionario.preguntas[posicion];

            valido = pregunta != null
                && !string.IsNullOrWhiteSpace(pregunta.enunciado)
                && pregunta.respuestas != null
                && pregunta.respuestas.Length == 4
                && pregunta.indiceCorrecto >= 0
                && pregunta.indiceCorrecto < 4;

            // Comprueba que las cuatro respuestas contienen texto.

            int respuesta = 0;

            while (valido && respuesta < 4)
            {
                valido = !string.IsNullOrWhiteSpace(
                    pregunta.respuestas[respuesta]
                );

                respuesta++;
            }

            posicion++;
        }

        return valido;
    }


    // Devuelve la ruta de los cuestionarios personalizados.

    public string ObtenerRutaCustom()
    {
        return rutaCustom;
    }
}
