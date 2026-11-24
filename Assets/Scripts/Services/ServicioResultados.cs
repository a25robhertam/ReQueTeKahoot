
// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Servicio de almacenamiento de resultados en XML


using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;


// Clase que agrupa los resultados de un mismo cuestionario.

[Serializable]
public class HistorialResultados
{
    // Lista de partidas realizadas.

    public List<ResultadoPartida> resultados =
        new List<ResultadoPartida>();
}


// Servicio para guardar y leer los resultados de las partidas.

public static class ServicioResultados
{
    // Nombre de la carpeta donde se almacenan los XML.

    private const string CARPETA_RESULTADOS = "Resultados";


    // Obtiene la ruta del XML de un cuestionario.

    private static string ObtenerRutaArchivo(string idCuestionario)
    {
        // Construye la carpeta dentro de persistentDataPath.

        string carpeta = Path.Combine(
            Application.persistentDataPath,
            CARPETA_RESULTADOS
        );

        // Evita caracteres no permitidos en el nombre del archivo.

        string nombreArchivo = "";

        foreach (char caracter in idCuestionario)
        {
            if (char.IsLetterOrDigit(caracter) ||
                caracter == '_' || caracter == '-')
            {
                nombreArchivo += caracter;
            }
        }

        // Añade la extensión XML.

        nombreArchivo += ".xml";

        // Devuelve la ruta completa.

        return Path.Combine(carpeta, nombreArchivo);
    }


    // Guarda el resultado de una partida en su XML.

    public static bool GuardarResultado(ResultadoPartida resultado)
    {
        bool guardadoCorrectamente = false;

        // Conserva la ruta para identificar el archivo si falla.

        string rutaArchivo = "";

        try
        {
            // Obtiene la ruta del XML.

            rutaArchivo =
                ObtenerRutaArchivo(resultado.idCuestionario);

            // Crea la carpeta si todavía no existe.

            string carpeta = Path.GetDirectoryName(rutaArchivo);

            Directory.CreateDirectory(carpeta);

            // Prepara el historial de resultados.

            HistorialResultados historial =
                new HistorialResultados();

            // Recupera las partidas anteriores si existe el XML.

            if (File.Exists(rutaArchivo))
            {
                XmlSerializer lector =
                    new XmlSerializer(typeof(HistorialResultados));

                using (FileStream archivo =
                    new FileStream(rutaArchivo, FileMode.Open))
                {
                    historial = (HistorialResultados)
                        lector.Deserialize(archivo);
                }
            }

            // Añade la nueva partida.

            historial.resultados.Add(resultado);

            // Guarda el historial actualizado.

            XmlSerializer escritor =
                new XmlSerializer(typeof(HistorialResultados));

            using (FileStream archivo =
                new FileStream(rutaArchivo, FileMode.Create))
            {
                escritor.Serialize(archivo, historial);
            }

            guardadoCorrectamente = true;

            Debug.Log(
                "RESULTADO GUARDADO EN XML: " + rutaArchivo
            );
        }
        catch (Exception error)
        {
            // Informa del error en la consola.

            Debug.LogError(
                "ERROR AL GUARDAR EL RESULTADO XML: " +
                error.Message
            );

            // Genera un informe TXT de la excepción.

            ServicioInformes.GenerarInforme(
                "XML",
                rutaArchivo,
                error
            );
        }

        return guardadoCorrectamente;
    }


    // Recupera los resultados guardados de un cuestionario.

    public static List<ResultadoPartida> CargarResultados(
        string idCuestionario)
    {
        // Prepara una lista vacía.

        List<ResultadoPartida> resultados =
            new List<ResultadoPartida>();

        // Conserva la ruta para identificar el archivo si falla.

        string rutaArchivo = "";

        try
        {
            // Obtiene la ruta del XML.

            rutaArchivo =
                ObtenerRutaArchivo(idCuestionario);

            // Comprueba si existe el archivo.

            if (File.Exists(rutaArchivo))
            {
                // Deserializa el historial del cuestionario.

                XmlSerializer lector =
                    new XmlSerializer(typeof(HistorialResultados));

                using (FileStream archivo =
                    new FileStream(rutaArchivo, FileMode.Open))
                {
                    HistorialResultados historial =
                        (HistorialResultados)
                        lector.Deserialize(archivo);

                    // Recupera la lista si contiene resultados.

                    if (historial != null &&
                        historial.resultados != null)
                    {
                        resultados = historial.resultados;
                    }
                }
            }
            else
            {
                // Un cuestionario sin partidas todavía
                // no es un error: su clasificación estará vacía.

                Debug.Log(
                    "TODAVÍA NO EXISTEN RESULTADOS XML PARA: " +
                    idCuestionario
                );
            }
        }
        catch (Exception error)
        {
            // Informa del error de lectura.

            Debug.LogError(
                "ERROR AL LEER LOS RESULTADOS XML: " +
                error.Message
            );

            // Genera el informe TXT correspondiente.

            ServicioInformes.GenerarInforme(
                "XML",
                rutaArchivo,
                error
            );
        }

        return resultados;
    }
}
