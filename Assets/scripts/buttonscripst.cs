using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour {

    // Reference to the text input field.
    public GameObject theText;
    // Reference to the panel.
    public GameObject thePanel;

    // Function to clear the text in the input field.
    public void ClearText()
    {
        theText.GetComponent<InputField>().text = "";
    }

    // Function to hide the panel.
    public void CancelButton()
    {
        thePanel.SetActive(false);
    }

    // Function to show the panel.
    public void CloseButton()
    {
        thePanel.SetActive(true);
    } 
   
    // Function to quit the application.
    public void QuitButton()
    {
        Application.Quit();
    }
}
