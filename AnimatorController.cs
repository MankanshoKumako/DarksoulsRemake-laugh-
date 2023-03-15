using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public GameObject model;
    public PlayerInput pi;
    public Rigidbody rigid;
    public float walkSpeed = 2.0f;
    public float runMultiplier = 2.0f;
    public float jumpVelocity = 5f;
    [SerializeField]
    private Animator anim;
    private bool lockPlanar = false;
    private Vector3 planarVec;
    private Vector3 thrustVec;
    void Awake()
    {
        pi = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        //print(pi.Dup);
        float targetRunMulti = (pi.run ? 2.0f : 1.0f);
        anim.SetFloat("forward",pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"),targetRunMulti,0.5f));
        if (pi.jump)
        {
            anim.SetTrigger("jump");
        }
        
        if (pi.Dmag > 0.1f)
        {
            Vector3 targetForward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.2f);
            model.transform.forward = targetForward;
        }

        if (lockPlanar == false)
        {
            planarVec = pi.Dmag * model.transform.forward * walkSpeed * (pi.run?runMultiplier:1.0f);
        }
        
        
    }

    private void FixedUpdate()
    {
        //rigid.position += planarVec * Time.fixedDeltaTime;
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;
        thrustVec = Vector3.zero;
    }
/// <summary>
/// message processing block
/// </summary>
/// 
    public void OnJumpEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
        thrustVec = new Vector3(0, jumpVelocity, 0);
        //print("On jump enter");
    }


    public void IsGround()
    {
        anim.SetBool("isGround",true);
    }

    public void IsNotGround()
    {
        anim.SetBool("isGround",false);
    }

    public void OnGroundEnter()
    {
        pi.inputEnable = true;
        lockPlanar = false;
    }

    public void OnFallEnter()
    {
        pi.inputEnable = false;
        lockPlanar = true;
    }
}
