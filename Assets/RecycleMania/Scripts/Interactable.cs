using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    public bool isInRange;
    public KeyCode interactKey = KeyCode.F;
    public UnityEvent interactAction;

    public HUD hud;
    public String MsgToDisplay;

    private bool messageShown;

    void Update()
    {
        if (isInRange) {
            if (Input.GetKeyDown(interactKey)) {
                interactAction.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            isInRange = true;

            if (!messageShown) {
                hud.OpenMessagePanel(MsgToDisplay);
                messageShown = true;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            isInRange = false;
            messageShown = false;
            hud.CloseMessagePanel();
        }
    }
}
