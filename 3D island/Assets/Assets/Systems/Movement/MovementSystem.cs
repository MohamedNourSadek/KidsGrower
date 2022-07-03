using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSystem 
{
    private Rigidbody player_body;
    private AnimationSystem player_animator;

    public MovementSystem(Rigidbody _body, AnimationSystem _anim)
    {
        player_body = _body;
        player_animator = _anim;
    }



}
