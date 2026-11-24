
// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Código Gestión de los Datos de la Sesión


public static class DatosSesion
{
    // Cuestionario seleccionado por el usuario.

    public static Cuestionario cuestionarioSeleccionado;

    // Nombre del jugador que realizará la partida.

    public static string nombreJugador = "";


    // Limpia los datos almacenados en la sesión.

    public static void LimpiarDatos()
    {
        cuestionarioSeleccionado = null;
        nombreJugador = "";
    }
}
