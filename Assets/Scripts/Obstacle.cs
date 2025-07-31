using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(this.gameObject);
    }
}
