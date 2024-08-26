using UnityEngine;

[SelectionBase]
public class Rotator : MonoBehaviour
{
    [SerializeField] float rotationSpeed;

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    public void Randomizer()
    {
        rotationSpeed = Random.Range(100f, 200f);
        rotationSpeed = Random.Range(1, 3) == 1 ? rotationSpeed : -rotationSpeed;
    }
}