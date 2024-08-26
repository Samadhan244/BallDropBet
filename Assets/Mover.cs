using UnityEngine;

[SelectionBase]
public class Mover : MonoBehaviour
{
    [SerializeField] float moveX, moveY, touchDistance = 1f;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(moveX, 0, moveY), out hit, touchDistance))
        {
            if (!hit.collider.CompareTag("Ball"))
            {
                moveX = -moveX;
                moveY = -moveY;
            }
        }
        transform.position += new Vector3(moveX, 0, moveY) * Time.deltaTime;
    }

    public void Randomizer()
    {
        moveX = Random.Range(5f, 10f);
        moveX = Random.Range(1, 3) == 1 ? moveX : -moveX;
    }
}