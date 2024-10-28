using System;
using UnityEngine;

public class PegObject : MonoBehaviour
{

    public static event Action<PegType> OnPegPopped;
    [SerializeField] private PegType pegType; // maybe randomize these
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnPegPopped?.Invoke(pegType);
        Debug.Log("popped peg");
        gameObject.SetActive(false);
    }
}

public enum PegType
{
    Normal,
    Bonus,
    Special
}