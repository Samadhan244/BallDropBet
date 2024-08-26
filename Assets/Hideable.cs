using UnityEngine;

[SelectionBase]
public class Hideable : MonoBehaviour
{
    [SerializeField] float delayTimer;

    void ToggleVisibility()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void Randomizer()
    {
        delayTimer = Random.Range(1f, 4f);
        InvokeRepeating(nameof(ToggleVisibility), 0f, delayTimer);
    }
}