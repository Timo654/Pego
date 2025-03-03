using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private Button continueButton;
    private bool continuePressed;
    private void Awake()
    {
        SaveManager.Instance.gameData.lastPlayedLevel = 1; // reset to start of the game
        AudioManager.Instance.InitializeMusic(FMODEvents.Instance.CreditsTheme);
        AudioManager.Instance.StartMusic();
        if (BuildConsts.isWebGL)
        {
            endText.text = endText.text.Replace("ALT+F4 to QUIT :)", "");

        }
        if (BuildConsts.isMobile)
        {
            continueButton.gameObject.SetActive(BuildConsts.isMobile);
            continueButton.onClick.AddListener(delegate { PressContinue(); });
        }

    }

    private void PressContinue()
    {
        continuePressed = true;
    }

    void Start()
    {
        LevelChanger.Instance.FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || continuePressed)
        {
            continuePressed = false;
            SaveManager.Instance.runtimeData.currentLevel = LevelChanger.Instance.GetLevels()[0];
            LevelChanger.Instance.FadeToLevel("PegoScene");
        }
    }
}
