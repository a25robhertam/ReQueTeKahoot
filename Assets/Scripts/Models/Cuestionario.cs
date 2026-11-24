
// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Código Modelo de Datos de Cuestionarios


using System;

[Serializable]
public class Cuestionario
{
    // Identificador único del cuestionario.

    public string id;

    // Título del cuestionario.

    public string titulo;

    // Tiempo disponible para responder cada pregunta, en segundos.

    public int tiempoPorPregunta;

    // Vector que contiene las preguntas del cuestionario.

    public Pregunta[] preguntas;
}
