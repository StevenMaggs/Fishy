using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager instance = null;
    
    [HideInInspector] public EnemyPooling poolScript;
    [HideInInspector] public PlayerController playerScript;
    [HideInInspector] public Menu menuScript;
    public bool gamePaused = false;
    public bool gameOver = false;
    public bool gameOptions = false;
    public bool changesSaved = true;
    [HideInInspector] public float camWidth = 0;
    [HideInInspector] public float camHeight = 0;
    public float volumeMaster = 1f;
    public float volumeSFX = 1f;
    public float volumeMusic = 1f;

    [SerializeField] private GameObject poolObject = null;
    [SerializeField] private GameObject playerObject = null;
    [SerializeField] private GameObject menuObject = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        Camera cam = Camera.main;
        camHeight = 2f * cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        poolScript = poolObject.GetComponent<EnemyPooling>();
        playerScript = playerObject.GetComponent<PlayerController>();
        menuScript = menuObject.GetComponent<Menu>();
    }

    public void ResetGame()
    {
        playerScript.Reset();
        poolScript.Reset();
    }

    public void PauseGame()
    {
        menuScript.EnablePauseMenu();
        gamePaused = true;
        playerScript.rb2d.velocity = Vector2.zero;
    }

    public void ResumeGame()
    {
        menuScript.DisablePauseMenu();
        gamePaused = false;
        playerScript.rb2d.velocity = playerScript.velocity;
    }

    public void OpenOptionsMenu()
    {
        menuScript.EnableOptionsMenu();
    }

    public void CloseOptionsMenu()
    {
        menuScript.DisableOptionsMenu();
    }

    public void OpenOptionsQuitConfirmationMenu()
    {
        menuScript.EnableSettingsConfirmationMenu();
    }
}