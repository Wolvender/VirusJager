using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverPanel;
    public Button firstSelectedButton;

    [Header("Input Actions")]
    public InputActionReference steerAction;   // G29 horizontal axis
    public InputActionReference submitAction;  // G29 submit button

    private EventSystem eventSystem;
    private bool isGameOver = false;
    private float lastSteer = 0f;

    void OnEnable()
    {
        steerAction.action.Enable();
        submitAction.action.Enable();
    }

    void OnDisable()
    {
        steerAction.action.Disable();
        submitAction.action.Disable();
    }

    void Start()
    {
        eventSystem = EventSystem.current;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (!isGameOver) return;

        float steer = steerAction.action.ReadValue<float>();

        // Navigate UI when steer moves significantly
        if (steer > 0.5f && lastSteer <= 0.5f)
            MoveSelection(1); // right/down
        else if (steer < -0.5f && lastSteer >= -0.5f)
            MoveSelection(-1); // left/up

        lastSteer = steer;

        // Submit button
        if (submitAction.action.triggered)
        {
            if (eventSystem.currentSelectedGameObject != null)
            {
                Button btn = eventSystem.currentSelectedGameObject.GetComponent<Button>();
                if (btn != null) btn.onClick.Invoke();
            }
        }
    }

    public void ShowGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Set the first selected button
        eventSystem.SetSelectedGameObject(firstSelectedButton.gameObject);

        Time.timeScale = 0f; // pause game
    }

    private void MoveSelection(int direction)
    {
        if (eventSystem.currentSelectedGameObject == null) return;

        Button[] buttons = gameOverPanel.GetComponentsInChildren<Button>();
        int index = System.Array.IndexOf(buttons, eventSystem.currentSelectedGameObject.GetComponent<Button>());
        if (index == -1) return;

        index += direction;
        if (index < 0) index = buttons.Length - 1;
        else if (index >= buttons.Length) index = 0;

        eventSystem.SetSelectedGameObject(buttons[index].gameObject);
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Replace with your main menu scene
    }
}
