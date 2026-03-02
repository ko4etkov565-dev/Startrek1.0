using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LandedUI : MonoBehaviour
{
   
  [SerializeField] private TextMeshProUGUI titleTextMesh;
  [SerializeField] private TextMeshProUGUI statsTextMesh;
  [SerializeField] private TextMeshProUGUI nextButtonTextMesh;
  [SerializeField] private Button nextButton;


  private Action nextButtonClickAction;
  private void Awake() {
    nextButton.onClick.AddListener(() => {
      nextButtonClickAction();
    });
  }
    // it is the same as this(simple code with simple method)
  // private void Awake() {
  //   nextButton.onClick.AddListener(LoadScene);
  //   void LoadScene()    {
  //     SceneManager.LoadScene(0);
  //   }
  // }

  private void Start()     {
    Lander.Instance.OnLanded += Lander_OnLanded;
      
    
    //   hiding the stats from start game( should write this in Start than Awake)
      Hide();
  }


   // showing up board with stats and type of landing
  private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)    {
    if (e.landingType == Lander.LandingType.Success)      {
      titleTextMesh.text = "GOOD JOB!";
      nextButtonTextMesh.text = "CONTINUE";
      nextButtonClickAction = GameManager.Instance.GoToNextLevel;
    } else  {
    titleTextMesh.text = "<Color=#ff0000>LOSER!</Color>";
    nextButtonTextMesh.text = "TRY AGAIN";
    nextButtonClickAction = GameManager.Instance.RetryLevel;
    }
  statsTextMesh.text =
  Mathf.Round(e.landingSpeed * 2f) + "\n" +
  Mathf.Round(e.dotVector * 100f) + "\n" +
  "x" + e.scoreMultiplier + "\n" +
  e.score;

  // showing this stats when lander is arrived
  Show();
  }

  private void Show()   {
    gameObject.SetActive(true);
    nextButton.Select();  
  }
  private void Hide()   {
    gameObject.SetActive(false);
  }
}