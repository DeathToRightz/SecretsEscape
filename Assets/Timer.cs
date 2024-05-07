using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class Timer : MonoBehaviour
{
    public static Timer Instance;
    private float score = 1000;
    private float finalScore;
    
    [SerializeField] public TMP_Text timerDisplayTxt;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
   

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        if(SceneManager.GetActiveScene().buildIndex >= 5)
        {
            score = 1000;
        }
        if(SceneManager.GetActiveScene().name == "Win")
        {
            DisplayScore();
            return;
        }
        else
        {
            score -= Time.deltaTime;
            timerDisplayTxt.text = score.ToString();
        }
        
        

    }

    private void DisplayScore()
    {      
        finalScore = score;
        TMP_Text scoreDisplay = GameObject.Find("Score Text").GetComponent<TMP_Text>();
        scoreDisplay.text = $"You finished in : {finalScore} seconds";
    }
}
