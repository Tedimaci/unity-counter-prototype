using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[DefaultExecutionOrder(1000)]
public class GameManager : MonoBehaviour
{
    [Header("Game objects to be mapped")]
    [SerializeField] private TMP_Text sphereCountText;
    [SerializeField] private TMP_Text roundEndText;
    [SerializeField] private Text CounterText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button startButton;
    [SerializeField] private Counter onSphereLanded;
    [SerializeField] private Counter onSphereLandedInBox;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private new MainCameraController camera;
    // Game state variables
    private float towerHeight = 0;
    private int sphereCount = 20;
    private int fallenSphereCount;
    private int sphereInBoxCount;
    private float countdown;
    private enum GameStage
    {
        Guess,
        Fall,
        Wait,
        End,
        Reset
    }
    private GameStage stage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the game for the first round
        stage = GameStage.Guess;
        fallenSphereCount = 0;
        sphereInBoxCount = 0;
        countdown = 0.0f;
        sphereCountText.text = "Total number of spheres: " + sphereCount;
        SphereController.SharedInstance.GenerateSpheres(sphereCount, towerHeight);

        // Bind the trigger functions to the exposed Collider events
        onSphereLanded.onTriggerEnter.AddListener(SphereLanded);
        onSphereLandedInBox.onTriggerEnter.AddListener(SphereLandedInBox);
    }

    // Update is called once per frame
    void Update()
    {
        gameOverPanel.GetComponent<RectTransform>().sizeDelta = gameObject.GetComponent<RectTransform>().sizeDelta + new Vector2(20,20);
        switch (stage)
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
                    if (int.TryParse(inputField.text, out int num))
                    {
                        if (num == sphereInBoxCount)
                        {
                            gameOverPanel.GetComponent<Image>().color = new Color(0,200,0);
                            gameOverPanel.GetComponentInChildren<TMP_Text>().text = "Good Job";
                            IncreaseTowerSize();
                        }
                        else
                        {
                            gameOverPanel.GetComponent<Image>().color = new Color(200, 0, 0);
                            gameOverPanel.GetComponentInChildren<TMP_Text>().text = "Try Again";
                        }
                    }
                    else
                    {
                        gameOverPanel.GetComponent<Image>().color = new Color(200, 0, 0);
                        gameOverPanel.GetComponentInChildren<TMP_Text>().text = "Huh?";
                    }
                    gameOverPanel.SetActive(true);
                    SphereController.SharedInstance.ResetPooledObject();
                    SphereController.SharedInstance.GenerateSpheres(sphereCount, towerHeight);
                    fallenSphereCount = 0;
                    sphereInBoxCount = 0;
                    CounterText.text = "Number of spheres in the box : " + sphereInBoxCount;
                    inputField.text = string.Empty;
                    StartCoroutine(NewGame());
                    stage = GameStage.Reset;
                    break;
            }
            case GameStage.Reset:
                {
                    break;
                }
        }
    }

    private IEnumerator NewGame()
    {
        yield return new WaitForSeconds(3.0f);
        stage = GameStage.Guess;
        inputField.interactable = true;
        gameOverPanel.SetActive(false);
    }

    // Function to start the round
    public void StartGame()
    {
        startButton.interactable = false;
        inputField.interactable = false;
        stage = GameStage.Fall;
        countdown = 30.0f;
        SphereController.SharedInstance.DropBalls();
    }

    // Function to count number of Spheres that are landed in the box
    public void SphereLanded(Collider col)
    {
        fallenSphereCount++;
        switch (stage)
        {
            case GameStage.Fall:
                {
                    if (countdown >= 10.0f)
                        countdown = 10.0f;
                    CounterText.text = "Number of spheres in the box : " + sphereInBoxCount;
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

    // Function to count number of Spheres that are landed in the box
    public void SphereLandedInBox(Collider col)
    {
        sphereInBoxCount += 1;
        CounterText.text = "Number of spheres in the box : " + sphereInBoxCount;
    }

    // Function to control if the Start Round button is iteractable
    public void OnInputFieldValueChange()
    {
        // Only make the button interactable if a number input was made
        if (string.IsNullOrEmpty(inputField.text))
        {
            startButton.interactable = false;
        }
        else
        {
            startButton.interactable = true;
        }
    }

    // Function for increasing tower size and setting game parameters
    private void IncreaseTowerSize()
    {
        ObstacleController.SharedInstance.AddObstacle();
        towerHeight = ObstacleController.SharedInstance.actualHeight;
        camera.maxElevation = towerHeight;
    }
}
