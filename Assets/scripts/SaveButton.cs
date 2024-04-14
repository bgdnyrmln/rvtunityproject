using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour
{
    public string theText;
    public GameObject ourNote;
    public GameObject placeHolder;
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
        string path = EditorUtility.SaveFilePanel("Save txt", "", DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + username + ".txt", "txt");

        try
        {
            File.WriteAllText(path, theText);
            Debug.Log("Note saved to: " + path);
            StartCoroutine(SaveTextRoll());
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving note: " + e.Message);
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