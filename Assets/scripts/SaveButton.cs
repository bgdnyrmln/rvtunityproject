using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;




public class SaveLoad : MonoBehaviour
{
    public string theText;
    public string fileName;
    public string username;
    public GameObject filenameholder;
    public GameObject ourNote;
    public GameObject saveAnim;
    public GameObject errorSaveAnim;

    void Start()
    {
        username = PlayerPrefs.GetString("Username", "UnknownUser");
    }

    public void SaveNote()
    {
        fileName = filenameholder.GetComponent<Text>().text;
        switch (fileName)
        {
            case "":
                PlayerPrefs.SetString("NoteContents", theText);
                fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + username + ".txt";
                break;
            default:
                fileName = fileName + ".txt";            
                break;
        }


        theText = ourNote.GetComponent<Text>().text;

        if (FileSaver.SaveText(theText, fileName, username))
        {
            StartCoroutine(SaveTextRoll());
        }
        else
        {
            StartCoroutine(ErrorSaveTextRoll());
        }
    }

    IEnumerator SaveTextRoll()
    {
        saveAnim.GetComponent<Animator>().Play("SavedAnim");
        yield return new WaitForSeconds(1);
        saveAnim.GetComponent<Animator>().Play("New State");
    }

    IEnumerator ErrorSaveTextRoll()
    {
        errorSaveAnim.GetComponent<Animator>().Play("ErrorSavedAnim");
        yield return new WaitForSeconds(1);
        errorSaveAnim.GetComponent<Animator>().Play("New State");
    }
}
