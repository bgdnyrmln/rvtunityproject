using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;


public class loadinginto : MonoBehaviour
{
    public GameObject placeHolder;

    // Start is called before the first frame update
    void Start()
    {
        string savedValue = PlayerPrefs.GetString("fileContentString", "");
        placeHolder.GetComponent<InputField>().text = savedValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
