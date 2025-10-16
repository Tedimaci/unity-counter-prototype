using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

[DefaultExecutionOrder(1000)]
public class GameManager : MonoBehaviour
{
    public float towerHeight = 0;
    public int sphereCount = 20;
    public TMP_Text sphereCountText;
    public TMP_Text roundEndText;
    public Text CounterText;
    public enum GameStage
    {
        Guess,
        Fall,
        Wait,
        End
    }
    public GameStage stage;
    public TMP_InputField inputField;
    public Button startButton;
    public WallController onSphereLanded;
    public Counter onSphereLandedInBox;
    public int fallenSphereCount;
    public int sphereInBoxCount;
    public float countdown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stage = GameStage.Guess;
        fallenSphereCount = 0;
        sphereInBoxCount = 0;
        countdown = 0.0f;
        sphereCountText.text = "Total number of spheres: " + sphereCount;
        SphereController.SharedInstance.GenerateSpheres(sphereCount, towerHeight);
        onSphereLanded.onTriggerEnter.AddListener(SphereLanded);
        onSphereLandedInBox.onTriggerEnter.AddListener(SphereLandedInBox);
    }

    // Update is called once per frame
    void Update()
    { 
        switch(stage)
        {
            case GameStage.Guess:
            {
                    roundEndText.gameObject.SetActive(false);
                    break;
            }
            case GameStage.Fall:
            {
                    if (countdown < 10.0f)
                    {
                        roundEndText.text = "Round ends in: " + Mathf.CeilToInt(countdown);
                        roundEndText.gameObject.SetActive(true);
                    }
                    else if (countdown < 0.0f)
                    {
                        roundEndText.gameObject.SetActive(false);
                        stage = GameStage.End;
                    }
                    countdown -= Time.deltaTime;
                    break;
            }
            case GameStage.Wait:
            {
                    if (countdown < 0.0f)
                    {
                        roundEndText.gameObject.SetActive(false);
                        stage = GameStage.End;
                    }
                    countdown -= Time.deltaTime;
                    roundEndText.text = "Round ends in: " + Mathf.CeilToInt(countdown);
                    break;
            }
            case GameStage.End:
            {
                    IncreaseTowerSize();
                    fallenSphereCount = 0;
                    sphereInBoxCount = 0;
                    CounterText.text = "Number of spheres in the box : " + sphereInBoxCount;
                    inputField.text = string.Empty;
                    stage = GameStage.Guess;
                    break;
            }
        }
    }

    public void StartGame()
    {
        startButton.interactable = false;
        stage = GameStage.Fall;
        countdown = 30.0f;
        SphereController.SharedInstance.DropBalls();
    }

    public void SphereLanded(Collider col)
    {
        fallenSphereCount++;
        switch (stage)
        {
            case GameStage.Fall:
                {
                    if (countdown >= 10.0f)
                        countdown = 10.0f;
                    CounterText.text = "Number of spheres in the box : 10";
                    roundEndText.gameObject.SetActive(true);
                    stage = GameStage.Wait;
                    break;
                }
            case GameStage.Wait:
                {
                    if ( fallenSphereCount == sphereCount)
                        stage = GameStage.End;
                    break;
                }
        }
    }

    public void SphereLandedInBox(Collider col)
    {
        sphereInBoxCount += 1;
        CounterText.text = "Number of spheres in the box : " + sphereInBoxCount;
    }

    public void OnInputFieldValueChange()
    {
        if (string.IsNullOrEmpty(inputField.text))
        {
            startButton.interactable = false;
        }
        else
        {
            startButton.interactable = true;
        }
    }

    private void IncreaseTowerSize()
    {
        SphereController.SharedInstance.ResetPooledObject();
        ObstacleController.SharedInstance.AddObstacle();
        towerHeight = ObstacleController.SharedInstance.actualHeight;
        SphereController.SharedInstance.GenerateSpheres(sphereCount, towerHeight);
    }
}
