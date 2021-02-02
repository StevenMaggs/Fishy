using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject[] menus = null;
    [SerializeField] private FullScreenMode tempMode;
    [SerializeField] private int tempWidth = Screen.width;
    [SerializeField] private int tempHeight = Screen.height;
    [SerializeField] private int tempRefreshRate;
    [SerializeField] private float tempVolumeMaster;
    [SerializeField] private float tempVolumeSFX;
    [SerializeField] private float tempVolumeMusic;
    private TMP_Dropdown resolutionsDropdown = null;
    private TMP_Dropdown fullscreenModeDropdown = null;
    private TMP_Dropdown refreshRateDropdown = null;
    private Slider volumeMasterSlider;
    private Slider volumeSFXSlider;
    private Slider volumeMusicSlider;
    private int tempResolutionIndex;
    private int tempFullscreenModeIndex;
    private int tempRefreshRateIndex;

    private void Start()
    {
        GameObject go = (GameObject) Resources.FindObjectsOfTypeAll(typeof(GameObject)).Where(x => x.name == "Resolutions").ToArray()[0];
        resolutionsDropdown = go.GetComponent<TMP_Dropdown>();
        go = (GameObject) Resources.FindObjectsOfTypeAll(typeof(GameObject)).Where(x => x.name == "FullscreenMode").ToArray()[0];
        fullscreenModeDropdown = go.GetComponent<TMP_Dropdown>();
        go = (GameObject) Resources.FindObjectsOfTypeAll(typeof(GameObject)).Where(x => x.name == "RefreshRate").ToArray()[0];
        refreshRateDropdown = go.GetComponent<TMP_Dropdown>();

        go = (GameObject) Resources.FindObjectsOfTypeAll(typeof(GameObject)).Where(x => x.name == "VolumeMaster").ToArray()[0];
        volumeMasterSlider = go.GetComponent<Slider>();
        go = (GameObject) Resources.FindObjectsOfTypeAll(typeof(GameObject)).Where(x => x.name == "VolumeSFX").ToArray()[0];
        volumeSFXSlider = go.GetComponent<Slider>();
        go = (GameObject) Resources.FindObjectsOfTypeAll(typeof(GameObject)).Where(x => x.name == "VolumeMusic").ToArray()[0];
        volumeMusicSlider = go.GetComponent<Slider>();

        SetTempValues();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void EnablePauseMenu()
    {
        GameObject go = System.Array.Find(menus, x => x.name == "PauseMenu");
        go.SetActive(true);
    }

    public void DisablePauseMenu()
    {
        GameObject go = System.Array.Find(menus, x => x.name == "PauseMenu");
        go.SetActive(false);
    }

    public void EnableOptionsMenu()
    {
        GameObject go = System.Array.Find(menus, x => x.name == "OptionsMenu");
        GameManager.instance.gameOptions = true;
        go.SetActive(true);
    }

    public void DisableOptionsMenu()
    {
        GameObject go = System.Array.Find(menus, x => x.name == "OptionsMenu");
        GameManager.instance.gameOptions = false;
        go.SetActive(false);
    }

    public void DisableOptionsMenuInteractable()
    {
        GameObject go = System.Array.Find(menus, x => x.name == "SettingsConfirmation");
    }

    public void EnableSettingsConfirmationMenu()
    {
        GameObject go = System.Array.Find(menus, x => x.name == "SettingsConfirmation");
        go.SetActive(true);
    }

    public void DisableSettingsConfirmationMenu()
    {
        GameObject go = System.Array.Find(menus, x => x.name == "SettingsConfirmation");
        go.SetActive(false);
    }

    public void SetResolution(int index)
    {
        string text = GameObject.Find("Resolutions").GetComponent<TMP_Dropdown>().options[index].text.ToLower();
        string[] splitText = text.Split('x');

        tempWidth = int.Parse(splitText[0].TrimEnd(' '));
        tempHeight = int.Parse(splitText[1].TrimStart(' '));

        GameManager.instance.changesSaved = false;
    }

    public void SetFullscreenMode(int index)
    {
        string text = GameObject.Find("FullscreenMode").GetComponent<TMP_Dropdown>().options[index].text.ToLower();

        switch (text)
        {
            case "fullscreen":
                tempMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case "borderless":
                tempMode = FullScreenMode.FullScreenWindow;
                break;
            case "windowed":
                tempMode = FullScreenMode.Windowed;
                break;
            default:
                Debug.LogError("Given FullscreenMode does not exist.");
                break;
        }

        GameManager.instance.changesSaved = false;
    }

    public void SetRefreshRate(int index)
    {
        string text = GameObject.Find("RefreshRate").GetComponent<TMP_Dropdown>().options[index].text;
        tempRefreshRate = int.Parse(text);

        GameManager.instance.changesSaved = false;
    }

    public void SetMasterVolume(float value)
    {
        tempVolumeMaster = value;
        GameManager.instance.changesSaved = false;
    }

    public void SetSFXVolume(float value)
    {
        tempVolumeSFX = value;
        GameManager.instance.changesSaved = false;
    }

    public void SetMusicVolume(float value)
    {
        tempVolumeMusic = value;
        GameManager.instance.changesSaved = false;
    }

    public void ExitWithoutSavingChanges()
    {
        ResetTempValues();

        DisableSettingsConfirmationMenu();
        DisableOptionsMenu();
        EnablePauseMenu();

        GameManager.instance.changesSaved = true;
    }

    public void ReturnToSettings()
    {
        DisableSettingsConfirmationMenu();
    }

    public void CheckForUnappliedChangesBeforeExit()
    {
        if (GameManager.instance.changesSaved)
        {
            DisableOptionsMenu();
            EnablePauseMenu();
        }
        else
        {
            EnableSettingsConfirmationMenu();
            // prevent interaction with options menu
        }
    }

    public void ApplyChanges()
    {
        Screen.SetResolution(tempWidth, tempHeight, tempMode, tempRefreshRate);

        GameManager.instance.volumeMaster = tempVolumeMaster;
        GameManager.instance.volumeSFX = tempVolumeSFX;
        GameManager.instance.volumeMusic = tempVolumeMusic;

        SetTempValues();
        AudioManager.instance.UpdateVolumes();

        GameManager.instance.changesSaved = true;
    }

    private void SetTempValues()
    {
        tempWidth = Screen.currentResolution.width;
        tempHeight = Screen.currentResolution.height;
        tempMode = Screen.fullScreenMode;
        tempRefreshRate = Screen.currentResolution.refreshRate;
        tempVolumeMaster = GameManager.instance.volumeMaster;
        tempVolumeSFX = GameManager.instance.volumeSFX;
        tempVolumeMusic = GameManager.instance.volumeMusic;

        tempResolutionIndex = resolutionsDropdown.value;
        tempFullscreenModeIndex = fullscreenModeDropdown.value;
        tempRefreshRateIndex = refreshRateDropdown.value;
    }

    private void ResetTempValues()
    {
        tempWidth = Screen.currentResolution.width;
        tempHeight = Screen.currentResolution.height;
        tempMode = Screen.fullScreenMode;
        tempRefreshRate = Screen.currentResolution.refreshRate;
        tempVolumeMaster = GameManager.instance.volumeMaster;
        tempVolumeSFX = GameManager.instance.volumeSFX;
        tempVolumeMusic = GameManager.instance.volumeMusic;

        resolutionsDropdown.value = tempResolutionIndex;
        fullscreenModeDropdown.value = tempFullscreenModeIndex;
        refreshRateDropdown.value = tempRefreshRateIndex;

        volumeMasterSlider.value = tempVolumeMaster;
        volumeSFXSlider.value = tempVolumeSFX;
        volumeMusicSlider.value = tempVolumeMusic;
    }
}