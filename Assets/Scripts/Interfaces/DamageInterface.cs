using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DamageInterface
{
    public void TakeDamage(int damage, int speed, out int newSpeed);
}
