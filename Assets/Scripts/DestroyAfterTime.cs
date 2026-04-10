using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float timeToDestroy = 2f;

    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }
}