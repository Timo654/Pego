using TMPro;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI endText;

    private void Awake()
    {
        SaveManager.Instance.gameData.lastPlayedLevel = 1; // reset to start of the game
        AudioManager.Instance.InitializeMusic(FMODEvents.Instance.CreditsTheme);
        AudioManager.Instance.StartMusic();
        if (BuildConstants.isWebGL)
        {
            endText.text = endText.text.Replace("ALT+F4 to QUIT :)", "");
        }
    }
    void Start()
    {
        LevelChanger.Instance.FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveManager.Instance.runtimeData.currentLevel = LevelChanger.Instance.GetLevels()[0];
            LevelChanger.Instance.FadeToLevel("PegoScene");
        }
    }
}
