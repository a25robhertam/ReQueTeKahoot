
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NavegacionTabulador : MonoBehaviour
{
    // ReQueTeKAHOOT! --------------------------------------------------
    // AUTOR: Roberto Adrian Hernando Tamayo
    // CFGS DAM-VI - DESARROLLO DE INTERFACES
    //------------------------------------------------------------------
    // Código Gestión Navegación con Tabulador.
    //------------------------------------------------------------------

    [Header("CONTROLES EN ORDEN DE TABULACION")]
    [SerializeField] private Selectable[] controles;

    private void LateUpdate()
    {
        // Comprobamos que existe un teclado y un EventSystem.

        Keyboard teclado = Keyboard.current;

        if (teclado == null || EventSystem.current == null)
        {
            return;
        }

        // Solamente actuamos cuando se pulsa TAB.

        if (!teclado.tabKey.wasPressedThisFrame)
        {
            return;
        }

        if (controles == null || controles.Length == 0)
        {
            return;
        }

        // Identificamos el control que tiene actualmente el foco.

        GameObject objetoSeleccionado =
            EventSystem.current.currentSelectedGameObject;

        int indiceActual = -1;

        for (int i = 0; i < controles.Length; i++)
        {
            if (controles[i] != null &&
                controles[i].gameObject == objetoSeleccionado)
            {
                indiceActual = i;
                break;
            }
        }

        // No interferimos con otros controles de la pantalla.

        if (objetoSeleccionado != null && indiceActual == -1)
        {
            return;
        }

        // Comprobamos si queremos avanzar o retroceder.

        bool retroceder = teclado.shiftKey.isPressed;

        int siguienteIndice = indiceActual;

        // Buscamos el siguiente control disponible.

        for (int i = 0; i < controles.Length; i++)
        {
            if (retroceder)
            {
                siguienteIndice--;

                if (siguienteIndice < 0)
                {
                    siguienteIndice = controles.Length - 1;
                }
            }
            else
            {
                siguienteIndice++;

                if (siguienteIndice >= controles.Length)
                {
                    siguienteIndice = 0;
                }
            }

            Selectable siguienteControl = controles[siguienteIndice];

            if (siguienteControl != null &&
                siguienteControl.IsActive() &&
                siguienteControl.IsInteractable())
            {
                // Finalizamos la edición del campo anterior.

                if (indiceActual >= 0 &&
                    controles[indiceActual] is TMP_InputField)
                {
                    TMP_InputField campoAnterior =
                        (TMP_InputField)controles[indiceActual];

                    campoAnterior.DeactivateInputField();
                }

                // Seleccionamos el siguiente control.

                EventSystem.current.SetSelectedGameObject(
                    siguienteControl.gameObject);

                // Si es un campo de texto, activamos su edición.

                if (siguienteControl is TMP_InputField)
                {
                    TMP_InputField campo =
                        (TMP_InputField)siguienteControl;

                    campo.ActivateInputField();

                    campo.caretPosition = campo.text.Length;
                }

                // Si es un Toggle, dejamos el foco sobre él.
                // La selección de respuesta sigue controlada
                // por el Toggle Group existente.

                return;
            }
        }
    }
}
