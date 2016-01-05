using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class MapFileManager : MonoBehaviour {

    private string stringToSave = "";
    public string fileName = "";


    void Start()
    {
    }

    public void addStringToSave(string heightAsString) {
        if (stringToSave.Length == 0)
        {
            stringToSave += heightAsString;

        }
        else
        {
            stringToSave += ",";
            stringToSave += heightAsString;
        }
    }

    public void saveString()
    {
        System.IO.File.WriteAllText("Assets/Resources/" + fileName, stringToSave);
    }

    public ushort[] loadFile()
    {
        string fileData = System.IO.File.ReadAllText("Assets/Resources/" + fileName);
        string[] stringArray;
        
        stringArray = fileData.Split(',');
        ushort[] temp = new ushort[stringArray.Length];
        for (int i = 0; i < stringArray.Length; i++)
        {
            temp[i] = Convert.ToUInt16(stringArray[i]);
        }
        
        return temp;
    }

	public void changeFileNameFromMenu(InputField inputField) {
		this.fileName = inputField.text;
	}
}
