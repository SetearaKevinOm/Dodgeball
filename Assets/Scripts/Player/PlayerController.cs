using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kevin
{
    public class PlayerController : NetworkBehaviour
    {
        public GameObject player;
        public Transform playerTransform;
        
        private PlayerControls _playerControls;
        private InputAction _move;
        private Vector2 _moveInput;
        private Rigidbody _rb;
        public float playerSpeed;
        public void FixedUpdate()
        {
            if (IsServer)
            {
                MovePlayer();
            }
        }

        public IEnumerator Start()
        {
            _rb = GetComponent<Rigidbody>();
            yield return new WaitForSeconds(0.5f);
            if (IsOwner)
            {
                _playerControls = new();
                _move = _playerControls.Player.Move;
                _move.Enable();
                _move.performed += MovePerformed;
                _move.canceled += MoveCancelled;
            }
        }

        public void OnDisable()
        {
            if (IsOwner)
            {
                _move.Disable();
                _move.performed -= MovePerformed;
                _move.canceled -= MoveCancelled;
            }
        }
        
        private void MovePerformed(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
            RequestMoveServerRpc(_moveInput);
        }
        
        private void MoveCancelled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
            RequestMoveCancelledServerRpc(_moveInput);
        }
        
        
        private void MovePlayer()
        {
            Vector3 targetDirection = new Vector3(_moveInput.x * playerSpeed, 0, _moveInput.y * playerSpeed);
            if (_rb != null) _rb.AddForce(targetDirection, ForceMode.Acceleration);
            /*Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime);*/
        }

        [ServerRpc]
        private void RequestMoveServerRpc(Vector2 clientMoveInput)
        {
            _moveInput = clientMoveInput;
        }

        [ServerRpc]
        private void RequestMoveCancelledServerRpc(Vector2 clientMoveCancelledInput)
        {
            _moveInput = clientMoveCancelledInput;
        }

    }
}

