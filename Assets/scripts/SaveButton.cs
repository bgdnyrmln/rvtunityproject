using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class SaveLoad : MonoBehaviour
{
    public string theText;
    public GameObject ourNote;
    public GameObject saveAnim;
    public GameObject errorSaveAnim;

    void Start()
    {
        string savedNoteContent = PlayerPrefs.GetString("FileContent", "");
        ourNote.GetComponent<Text>().text = savedNoteContent;
    }

    public void SaveNote()
    {
        theText = ourNote.GetComponent<Text>().text;
        PlayerPrefs.SetString("NoteContents", theText);

        string username = PlayerPrefs.GetString("Username", "UnknownUser");
        string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + username + ".txt";

        if (FileSaver.SaveText(theText, fileName))
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
