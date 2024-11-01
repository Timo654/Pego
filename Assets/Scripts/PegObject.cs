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
    private ParticleSystem particles;
    private ParticleSystem.MainModule ma;
    private bool popped = false;
    // Start is called before the first frame update
    private void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        particles = GetComponent<ParticleSystem>();
        ma = particles.main;
        ma.startColor = normalColor;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (popped) return;
        particles.Play();
        OnPegPopped?.Invoke(pegType);
        popped = true;
        spriteRenderer.DOFade(0f, 0.5f).OnComplete(() => Destroy(gameObject));
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
                ma.startColor = normalColor;
                break;
            case PegType.Bonus:
                spriteRenderer.color = bonusColor;
                ma.startColor = bonusColor;
                break;
            case PegType.Red:
                spriteRenderer.color = redColor;
                ma.startColor = redColor;
                break;
            case PegType.Special:
                spriteRenderer.color = specialColor;
                ma.startColor = specialColor;
                break;
            default:
                Debug.Log($"Unknown peg type {pegType}");
                break;
        }
    }
}