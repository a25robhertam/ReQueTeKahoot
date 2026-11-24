// ReQueTeKAHOOT! --------------------------------------------------
// AUTOR: Roberto Adrian Hernando Tamayo
// CFGS DAM-VI - DESARROLLO DE INTERFACES
//------------------------------------------------------------------
// Código Gestión UI Menu Principal


using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    // Abre el selector de cuestionarios en modo Juego.

    public void Jugar()
    {
        PlayerPrefs.SetString("ModoSelector", "Jugar");
        SceneManager.LoadScene("SelectorCuestionario");
    }

    // Abre el selector de cuestionarios en modo Hall de la Fama.

    public void HallFama()
    {
        PlayerPrefs.SetString("ModoSelector", "HallFama");
        SceneManager.LoadScene("SelectorCuestionario");
    }

    // Abre la pantalla de creación de cuestionarios.

    public void CrearCuestionario()
    {
        SceneManager.LoadScene("CreadorCuestionario");
    }

    // Abre la pantalla de selección de informes.

    public void Informes()
    {
        SceneManager.LoadScene("SelectorInforme");
    }

    // Abre la pantalla de versión y créditos.

    public void VersionCreditos()
    {
        SceneManager.LoadScene("VersionCreditos");
    }

    // Cierra la aplicación.

    public void Salir()
    {
        Application.Quit();
    }
}