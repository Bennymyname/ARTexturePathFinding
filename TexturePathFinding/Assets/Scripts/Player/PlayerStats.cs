using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.UI;
// using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
// using static FileWriting;
using TMPro;
using System.Text.RegularExpressions;

public class PlayerStats : MonoBehaviour
{
    public TMP_Text textField;
    // private InputManager inputManager;
    public GameObject textFieldParent;
    public GameObject inputSuccess;
    public GameObject goalText;
    public bool isInputable = true;
    public static string participantNumber = "P00";
    [SerializeField] public string CorrTex = "";
    [SerializeField] public string Resolution = "";
    [SerializeField] public string stageName= "";
    // this is when you want to call variable mask from PlayerInteract Script;
    // public PlayerInteract pScript;
    // private GameObject Player;



    [SerializeField] public string[] sceneArray = new string[11];
    private bool hasCollidedWithGoal = false;

    private int currSceneNo = 0;

    public static string TT = "TutorialStage";
    public static string AH = "#1Stage1H";
    public static string AM = "#1Stage2M";
    public static string AL = "#1Stage3L";
    public static string BH = "#2Stage4H";
    public static string BM = "#2Stage5M";
    public static string BL = "#2Stage6L";
    public static string CH = "#3Stage7H";
    public static string CM = "#3Stage8M";
    public static string CL = "#3Stage9L";
    public static string SS = "StageSelector";


    //private string[] sceneArray = { TT, CL, CM, CH, BH, BM, BL, AL, AM, AH};

    // private string[] sceneArray = { "1Stage1H", "AM", "AL", "BH", "BM", "BL", "CH", "CM", "CL", };

    float startTime;
    float duration;
    public float damagePoints;

    // string folderPath = "";
    string folderPath = "";
    string filePath = "";

    public static PlayerStats Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
            // this is when you want to call variable mask from PlayerInteract Script;
            // player = GaneObject.Find("Player"); 
            // pScript = player.GetComponent<PlayerInteractt>();
            // pScript.mask = "AAA"; 

            // inputManager = GetComponent<InputManager>();
            //Path of the file
            folderPath = Application.dataPath + "/Logs"; // Specify the folder path
            // folderPath = Application.dataPath; // Specify the folder path
            filePath = Path.Combine(folderPath, participantNumber + "_" + "Log.csv"); // Combine folder path and file name

            // Create the folder if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            //Create File if it doesn't exist
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "File created" + System.DateTime.Now + "\n");
                string header = "Time"+ "," + "PX" + "," +  "Stage" + "," + "Damage" + "," + "Duration" + "," + "CorrTex" + "," + "Resolution" + "\n";
                File.AppendAllText(filePath, header);
            }
            //Content of the file
            // string played = "Played" + System.DateTime.Now + "\n";
            // string order = participantNumber + ": Order: " + presetOrder + " Stage: " + stageName + "\n";
            // string content = "Goal! Time: " + duration + " Damage: " + damagePoints + "\n";
            // //Add some to text to it
            // File.AppendAllText(filePath, played);
            // File.AppendAllText(filePath, order);
            // File.AppendAllText(filePath, content);

           //Debug.Log(SceneManager.GetActiveScene().name);

            sceneArray = new string[] { TT, AH, AM, AL, BH, BM, BL, CH, CM, CL, SS};
            currSceneNo = System.Array.IndexOf(sceneArray, SceneManager.GetActiveScene().name);

            if (currSceneNo + 1 >= sceneArray.Length)
            {
                currSceneNo = 0;
            }
            else
            { currSceneNo++; }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("false"))
        {
            damagePoints++;
            other.gameObject.tag = "unfalse";
            print("Damage:" + damagePoints);
        }
        if (other.gameObject.CompareTag("start"))
        {
            startTime = Time.time;
            print("Start" + startTime);
        }
        if (other.gameObject.CompareTag("goal") && !hasCollidedWithGoal)
        {
            goalText.SetActive(true);
            Invoke("TurnOffGoalText", 5.0f);
            hasCollidedWithGoal = true;
            
            
                duration = Time.time - startTime;
                print("Goal! Time: " + duration);
            
                if (filePath != "")
                {
                    string stagelog = System.DateTime.Now + "," + participantNumber + "," + stageName + "," + damagePoints + "," + duration + "," + CorrTex + "," + Resolution + "\n";
                    File.AppendAllText(filePath, stagelog);
                }
            
            

        }
        
    }


    // Update is called once per frame
    void Update()
    {
        // Debug.Log(isInputable);
        if(isInputable == true)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                // participantNumber = textField.text.ToString().TrimEnd('\r');  // Remove trailing carriage return and newline characters
                participantNumber = RemoveNonPrintableCharacters(textField.text);
                textField.text = "You have Entered!";
                inputSuccess.SetActive(true);
                textFieldParent.SetActive(false);
            }
        }
        
    }

    void TurnOffGoalText()
    {
        goalText.SetActive(false);
    }
    private string RemoveNonPrintableCharacters(string input)
    {
        return Regex.Replace(input, @"[^\w\s]", "");
    }
}
