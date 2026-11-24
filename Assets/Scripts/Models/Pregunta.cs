
 // ReQueTeKAHOOT! --------------------------------------------------
 // AUTOR: Roberto Adrian Hernando Tamayo
 // CFGS DAM-VI - DESARROLLO DE INTERFACES
 //------------------------------------------------------------------
 // Código Modelo de Datos de Preguntas


using System;

[Serializable]
public class Pregunta
{
    // Enunciado de la pregunta.

    public string enunciado;

    // Vector que almacena las cuatro respuestas posibles.

    public string[] respuestas;

    // Posición de la respuesta correcta dentro del vector.
    // Los índices comienzan en 0 y terminan en 3.

    public int indiceCorrecto;
}
