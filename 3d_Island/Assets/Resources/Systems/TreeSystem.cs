using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSystem : MonoBehaviour
{
    [SerializeField] Animator Animator;

    public void Shake()
    {
        Animator.SetTrigger("Shake");
    }
}
