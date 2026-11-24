
// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Servicio de generación de informes de excepciones


using System;
using System.IO;
using UnityEngine;

public static class ServicioInformes
{
    // Carpeta donde se guardarán los informes TXT.

    private const string CARPETA_INFORMES = "Informes";


    // Genera un informe cuando se produce una excepción.

    public static void GenerarInforme(
        string tipoError,
        string archivoAfectado,
        Exception excepcion)
    {
        try
        {
            // Obtiene la carpeta de informes.

            string carpeta = Path.Combine(
                Application.persistentDataPath,
                CARPETA_INFORMES
            );

            // Crea la carpeta si todavía no existe.

            Directory.CreateDirectory(carpeta);

            // Obtiene la fecha y hora actuales.

            DateTime fechaActual = DateTime.Now;

            // Normaliza el tipo de error para el nombre del archivo.

            string tipo = tipoError.ToUpper();

            // Evita caracteres no válidos en el nombre del fichero.

            foreach (char caracter in Path.GetInvalidFileNameChars())
            {
                tipo = tipo.Replace(caracter.ToString(), "");
            }

            // Construye un nombre único para el informe.

            string nombreInforme =
                "ERROR_" + tipo + "_" +
                fechaActual.ToString("yyyyMMdd_HHmmss_fff") +
                ".txt";

            string rutaInforme = Path.Combine(
                carpeta,
                nombreInforme
            );

            // Prepara el contenido del informe.

            string contenido =
                "ReQueTeKAHOOT! - INFORME DE EXCEPCIÓN" +
                Environment.NewLine +
                "========================================" +
                Environment.NewLine +
                Environment.NewLine +
                "Fecha: " +
                fechaActual.ToString("dd/MM/yyyy HH:mm:ss") +
                Environment.NewLine +
                "Tipo de error: " + tipoError +
                Environment.NewLine +
                "Archivo afectado: " + archivoAfectado +
                Environment.NewLine +
                Environment.NewLine +
                "DESCRIPCIÓN DEL ERROR" +
                Environment.NewLine +
                "----------------------------------------" +
                Environment.NewLine +
                excepcion.Message +
                Environment.NewLine +
                Environment.NewLine +
                "DETALLES TÉCNICOS" +
                Environment.NewLine +
                "----------------------------------------" +
                Environment.NewLine +
                excepcion.ToString();

            // Escribe el informe en el archivo TXT.

            File.WriteAllText(rutaInforme, contenido);

            Debug.Log(
                "INFORME DE EXCEPCIÓN GENERADO: " +
                rutaInforme
            );
        }
        catch (Exception error)
        {
            // Evita que un fallo al generar el informe
            // provoque otra excepción sin controlar.

            Debug.LogError(
                "ERROR AL GENERAR EL INFORME TXT: " +
                error.Message
            );
        }
    }
}
