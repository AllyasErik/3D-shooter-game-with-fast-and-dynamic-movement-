using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillCollision : MonoBehaviour
{
    public SkillShots skillShot;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            if(gameObject.CompareTag("Fireball"))
            {
                skillShot.CollisionWithFireball(other);
                Destroy(gameObject, 0.1f);
            }
            if (gameObject.CompareTag("Lightning"))
            {
                skillShot.CollisionWithLightning(other);
            }
            if (gameObject.CompareTag("Explosion"))
            {
                skillShot.CollisionWithExplosion(other);
            }

        }
    }
}
