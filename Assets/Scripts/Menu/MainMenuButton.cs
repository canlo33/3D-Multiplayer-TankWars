using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Highlighted()
    {
        anim.SetBool("Highlighted", true);
        anim.SetBool("Pressed", false);
        anim.SetBool("Normal", false);
    }
    public void Normal()
    {
        anim.SetBool("Highlighted", false);
        anim.SetBool("Pressed", false);
        anim.SetBool("Normal", true);
    }
    public void Pressed()
    {
        anim.SetTrigger("Pressed");
    }
}
