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
    public InputActionReference uiUpAction;
    public InputActionReference uiDownAction;
    public InputActionReference submitAction;

    private EventSystem eventSystem;
    private bool isGameOver = false;

    public ScreenFader fader;

    void OnEnable()
    {
        uiUpAction.action.Enable();
        uiDownAction.action.Enable();
        submitAction.action.Enable();
    }

    void OnDisable()
    {
        uiUpAction.action.Disable();
        uiDownAction.action.Disable();
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

        // Navigate: UP
        if (uiUpAction.action.triggered)
        {
            MoveSelection(-1);
        }

        // Navigate: DOWN
        if (uiDownAction.action.triggered)
        {
            MoveSelection(1);
        }

        // Submit / Enter
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

        gameOverPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(firstSelectedButton.gameObject);

        Time.timeScale = 0f;
    }

    private void MoveSelection(int direction)
    {
        Button[] buttons = gameOverPanel.GetComponentsInChildren<Button>();
        if (buttons.Length == 0) return;

        GameObject current = eventSystem.currentSelectedGameObject;
        int index = System.Array.IndexOf(buttons, current?.GetComponent<Button>());

        if (index < 0) index = 0;

        index += direction;

        if (index < 0) index = buttons.Length - 1;
        if (index >= buttons.Length) index = 0;

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
        SceneManager.LoadScene("MainMenu");
    }
}
