using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

// http://www.theappguruz.com/blog/unity-csv-parsing-unity
// https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-
public class CSVParsing : MonoBehaviour 
{
	public InputField emailInputField;// Reference of rollno input field
	public InputField nameInputField; // Reference of name input filed
	public Text contentArea; // Reference of contentArea where records are displayed

	private char lineSeperater = '\n'; // It defines line seperate character
	private char fieldSeperator = ','; // It defines field seperate chracter
	private string fileName = "my_data_storage.csv"; // path in android will be data/com.company.appname/filename

	void Start () 
	{
		readDataFromFilePath();
	}

	void readDataFromFilePath()
	{
	string file_path = getPath() + fileName;
	StreamReader csvFile = new StreamReader(file_path);
    string csvContents = csvFile.ReadToEnd();
    csvFile.Close();

    string[] records = csvContents.Split(lineSeperater);
	foreach (string record in records)
	{
		string[] fields = record.Split(fieldSeperator);
		foreach(string field in fields)
		{
			contentArea.text += field + "\t";
		}
		contentArea.text += '\n';
	}

	
	}

	// Add data to CSV file
	public void addDataToFilePath()
	{
		// Following line adds data to CSV file
		File.AppendAllText(getPath() + fileName, lineSeperater + nameInputField.text + fieldSeperator + emailInputField.text);
		// Following lines refresh the edotor and print data
		emailInputField.text = "";
		nameInputField.text = "";
		contentArea.text = "";
		#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh ();
		#endif
		readDataFromFilePath(); 
	}

	// Get path for given CSV file
	private static string getPath(){
		#if UNITY_EDITOR
		return Application.dataPath;
		#elif UNITY_ANDROID
		return Application.persistentDataPath;// +fileName;
		#else
		return Application.dataPath;// +"/"+ fileName;
		#endif
	}




}
