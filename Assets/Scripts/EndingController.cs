using UnityEngine;

public class EndingController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LevelChanger.Instance.FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SaveManager.Instance.runtimeData.currentLevel = LevelChanger.Instance.GetLevels()[0];
            LevelChanger.Instance.FadeToLevel("PegoScene");
        }
    }
}
