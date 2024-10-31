using System;
using UnityEngine;
[CreateAssetMenu(fileName = "New Level", menuName = "Levels/New Level")]
[Serializable]
public class LevelData : ScriptableObject
{
    public uint levelID;
    public Sprite backgroundImage;
    public Color  backgroundTint = Color.white;
    public Color basketTint = Color.white;
    public GameObject levelObjects;
    public int ballCount;
    public int specialCount;
}
