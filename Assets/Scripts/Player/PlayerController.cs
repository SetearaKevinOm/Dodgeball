using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kevin
{
    public class PlayerController : NetworkBehaviour
    {
        private PlayerControls _playerControls;
        private InputAction _move;
        private Vector2 _moveInput;
        private Rigidbody _rb;
        public float playerSpeed;
        public void FixedUpdate()
        {
            MovePlayerClientRpc();
        }

        public void OnEnable()
        {
            _rb = GetComponent<Rigidbody>();
            _playerControls = new();
            _move = _playerControls.Player.Move;
            _move.Enable();
            _move.performed += MovePerformed;
            _move.canceled += MoveCancelled;
        }

        public void OnDisable()
        {
            _move.Disable();
            _move.performed -= MovePerformed;
            _move.canceled -= MoveCancelled;
        }
        
        private void MovePerformed(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }
        
        private void MoveCancelled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }
        
        [ClientRpc]
        private void MovePlayerClientRpc()
        {
            Vector3 targetDirection = new Vector3(_moveInput.x * playerSpeed, 0, _moveInput.y * playerSpeed);
            _rb.AddForce(targetDirection,ForceMode.Acceleration);
            
            /*Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime);*/
        }
        
    }
}

