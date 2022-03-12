using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yern.PlatformerController2D
{
    [RequireComponent(typeof(PlatformerController2D), typeof(Animator))]
    public class PlatformerController2DAnimation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlatformerController2D controller;
        [SerializeField] private Animator animator;

        private PlatformerController2DState _oldState;

        private void Awake()
        {
            controller.OnStateChange += HandleOnStateChange;
        }

        private void OnDestroy()
        {
            controller.OnStateChange -= HandleOnStateChange;
        }

        private void HandleOnStateChange(PlatformerController2DState state)
        {
            switch (state)
            {
                case PlatformerController2DState.Idle:
                    if (_oldState == PlatformerController2DState.Idle) break;
                    animator.Play("Idle");
                    break;
                case PlatformerController2DState.Running:
                    if (_oldState == PlatformerController2DState.Running) break;
                    animator.Play("Running");
                    break;
                case PlatformerController2DState.AirRaising:
                    if (_oldState == PlatformerController2DState.AirRaising) break;
                    animator.Play("Jumping");
                    break;
                case PlatformerController2DState.AirFalling:
                    if (_oldState == PlatformerController2DState.AirFalling) break;
                    animator.Play("Falling");
                    break;
                case PlatformerController2DState.Dashing:
                    if (_oldState == PlatformerController2DState.Dashing) break;
                    animator.Play("Dashing");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            _oldState = state;
        }
    }
}

