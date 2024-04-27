using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinAnimation4 : MonoBehaviour
{
    public Animator animator;
    // Update is called once per frame
    void Update()
    {
     animator.SetBool("IsActive", true);
    }
}
