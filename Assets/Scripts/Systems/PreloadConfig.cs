using UnityEngine;

public class PreloadConfig : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (BuildConsts.isMobile) Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        if (BuildConsts.isWebGL) Application.runInBackground = false;
    }
}
