using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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
        End,
        Reset
    }
    public GameStage stage;
    public TMP_InputField inputField;
    public Button startButton;
    public WallController onSphereLanded;
    public Counter onSphereLandedInBox;
    public int fallenSphereCount;
    public int sphereInBoxCount;
    public float countdown;
    public GameObject gameOverPanel;
    public MainCameraController camera;
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

    public void StartGame()
    {
        startButton.interactable = false;
        inputField.interactable = false;
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
        ObstacleController.SharedInstance.AddObstacle();
        towerHeight = ObstacleController.SharedInstance.actualHeight;
        camera.maxElevation = towerHeight;
    }
}
