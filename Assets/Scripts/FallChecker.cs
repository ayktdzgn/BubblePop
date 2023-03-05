using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallChecker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            var bubble = collision.GetComponent<Bubble>();
            if (bubble != null)
            {
                bubble.PopEffect();
                bubble.Pop();
            }
        }
    }
}
