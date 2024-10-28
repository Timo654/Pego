using System;
using UnityEngine;

public class PegObject : MonoBehaviour
{

    public static event Action<PegType> OnPegPopped;
    [SerializeField] Color normalColor = Color.blue;
    [SerializeField] Color specialColor = Color.green;
    [SerializeField] Color redColor = Color.red;
    [SerializeField] Color bonusColor = Color.magenta;
    [SerializeField] private PegType pegType; // maybe randomize these
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnPegPopped?.Invoke(pegType);
        Debug.Log("popped peg");
        gameObject.SetActive(false);
    }

    public PegType GetPegType()
    {
        return pegType;
    }
    public void SetPegType(PegType pegType)
    {
        var spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        this.pegType = pegType;
        switch (pegType)
        {
            case PegType.Normal:
                spriteRenderer.color = normalColor;
                break;
            case PegType.Bonus:
                spriteRenderer.color = bonusColor;
                break;
            case PegType.Red:
                spriteRenderer.color = redColor;
                break;
            case PegType.Special:
                spriteRenderer.color = specialColor;
                break;
            default:
                break;
        }
    }
}

public enum PegType
{
    Normal,
    Red,
    Bonus,
    Special
}