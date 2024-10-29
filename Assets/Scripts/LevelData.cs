using System;
using UnityEngine;
[CreateAssetMenu(fileName = "New Level", menuName = "Levels/New Level")]
[Serializable]
public class LevelData : ScriptableObject
{
    public Sprite backgroundImage;
    public GameObject levelObjects;
    public int ballCount;
    public int specialCount;
}
