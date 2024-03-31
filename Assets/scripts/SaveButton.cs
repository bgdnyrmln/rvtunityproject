using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        theText = PlayerPrefs.GetString("NoteContents");
        placeHolder.GetComponent<InputField>().text = theText;
    }

    public void SaveNote()
    {
        theText = ourNote.GetComponent<Text>().text;
        PlayerPrefs.SetString("NoteContents", theText);
        StartCoroutine(SaveTextRoll());

        // Save to file
        string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
        string filePath = Path.Combine(Application.dataPath, "TextSaves", fileName);

        try
        {
            File.WriteAllText(filePath, theText);
            Debug.Log("Note saved to: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving note: " + e.Message);
        }
    }

    IEnumerator SaveTextRoll()
    {
        saveAnim.GetComponent<Animator>().Play("SavedAnim");
        yield return new WaitForSeconds(1);
        saveAnim.GetComponent<Animator>().Play("New State");
    }
}