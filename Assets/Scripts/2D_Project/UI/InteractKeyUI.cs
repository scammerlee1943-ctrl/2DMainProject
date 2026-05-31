using UnityEngine;

public enum InteractKeyType
{
    None = 0,
    F
}
public class InteractKeyUI : DaniTechUIBase
{
    [SerializeField] private SpriteRenderer SpriteRenderer_KeyF;

    private void Awake()
    {
        HideKey();
    }

    public void ShowKey(InteractKeyType keyType)
    {
        this.gameObject.SetActive(true);
    }

    public void HideKey()
    {
        this.gameObject.SetActive(false);
    }
}