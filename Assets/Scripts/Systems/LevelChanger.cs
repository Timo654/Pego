using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public static event Action<LevelData> OnGameplayLevelLoaded;
    public static event Action OnFadeInFinished;
    public float transitionTime = 1f; // couldnt get animation events to work right now
    public LevelData debugLevelData;
    private bool loadInProgess = false;
    private LevelData levelData;
    private List<Animator> animators = new();
    [SerializeField] private LevelData[] levels;
    public static LevelChanger Instance { get; private set; }

    private Animator GetRandomAnimator()
    {
        Animator animator = animators[UnityEngine.Random.Range(0, animators.Count)];
        animator.gameObject.SetActive(true);
        return animator;
    }
    public void Awake()
    {
        Instance = this;
        Animator childAnimator;
        // find all transitions
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out childAnimator))
            {
                animators.Add(childAnimator);
            }
        }
    }

    public LevelData[] GetLevels()
    {
        return levels;
    }

    public LevelData GetNextLevel(LevelData currentLevel)
    {
        var currentLvlIndex = Array.IndexOf(levels, currentLevel);
        if (currentLvlIndex == -1) // level does not exist in list
        {
            return null;
        }
        else if (currentLvlIndex + 1 < levels.Length)
        {
            return levels[currentLvlIndex + 1];
        }
        else
        {
            return null;
        }
    }

    private void SetTrigger(Animator animator, string trigger)
    {
        animator.SetTrigger(trigger);
    }
    public void HandleLevelLoad()
    {
        if (SaveManager.Instance.runtimeData.currentLevel == null)
        {
            if (Application.isEditor)
            {
                SaveManager.Instance.runtimeData.currentLevel = debugLevelData;
                Debug.LogWarning("Save levelData is null, loading debug level data instead.");
            }
            else
            {
                SaveManager.Instance.runtimeData.currentLevel = levels.FirstOrDefault(item => item.levelID == SaveManager.Instance.gameData.lastPlayedLevel);
            } 
        } 
        levelData = SaveManager.Instance.runtimeData.currentLevel;
        SaveManager.Instance.gameData.lastPlayedLevel = levelData.levelID;
        OnGameplayLevelLoaded?.Invoke(levelData);
        FadeIn();
    }
    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }
    public void FadeToLevel(string levelName, bool endMusic = true)
    {
        if (!loadInProgess) StartCoroutine(LoadLevel(levelName, endMusic));
        else Debug.LogWarning($"Already loading a level, cannot load {levelName}!");
    }
    public void FadeToDesktop()
    {
        if (!loadInProgess) StartCoroutine(QuitToDesktop());
        else Debug.LogWarning($"Already loading a level!");
    }
    IEnumerator LoadLevel(string levelToLoad, bool endMusic)
    {
        if (SaveManager.Instance != null) SaveManager.Instance.runtimeData.previousSceneName = SceneManager.GetActiveScene().name;
        loadInProgess = true;
        if (AudioManager.Instance != null)
        {
            if (endMusic) AudioManager.Instance.FadeOutMusic();
            AudioManager.Instance.StopSFX();
        }
        SetTrigger(GetRandomAnimator(), "FadeOut");
        yield return new WaitForSecondsRealtime(transitionTime);
        SceneManager.LoadSceneAsync(levelToLoad);
        loadInProgess = false;
    }
    IEnumerator QuitToDesktop()
    {
        Debug.Log("Quitting.");
        loadInProgess = true;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.FadeOutMusic();
            AudioManager.Instance.StopSFX();
        }
        SetTrigger(GetRandomAnimator(), "FadeOut");
        yield return new WaitForSecondsRealtime(transitionTime);
        loadInProgess = false;
        Application.Quit();
    }
    IEnumerator FadeInCoroutine()
    {
        SetTrigger(GetRandomAnimator(), "FadeIn");
        yield return new WaitForSecondsRealtime(transitionTime);
        OnFadeInFinished?.Invoke();
    }
}