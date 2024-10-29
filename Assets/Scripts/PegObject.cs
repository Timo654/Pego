using DG.Tweening;
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
    private SpriteRenderer spriteRenderer;
    private bool popped = false;
    // Start is called before the first frame update
    private void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (popped) return;
        OnPegPopped?.Invoke(pegType);
        popped = true;
        Debug.Log("popped peg");
        spriteRenderer.DOFade(0f, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }

    public PegType GetPegType()
    {
        return pegType;
    }
    public void SetPegType(PegType pegType)
    {
        
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