
// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Modelo de datos de los resultados de una partida


using System;

[Serializable]
public class ResultadoPartida
{
    // Nombre del jugador.

    public string nombreJugador;

    // Identificador del cuestionario utilizado.

    public string idCuestionario;

    // Nombre del cuestionario.

    public string tituloCuestionario;

    // Fecha y hora de finalización de la partida.

    public string fechaPartida;

    // Puntuación total obtenida.

    public int puntuacion;

    // Número de respuestas correctas.

    public int aciertos;

    // Número total de preguntas.

    public int totalPreguntas;

    // Tiempo total empleado, expresado en segundos.

    public float tiempoTotal;
}
