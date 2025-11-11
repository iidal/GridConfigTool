using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
   private Rigidbody rb;

   // Movement along X and Y axes.
   private float movementX;
   private float movementY;

   // Speed at which the player moves.
   public float speed = 0;

   // Start is called before the first frame update.
   void Start()
   {
      rb = GetComponent<Rigidbody>();
   }

   // This function is called when a move input is detected.
   void OnMove(InputValue movementValue)
   {
      Vector2 movementVector = movementValue.Get<Vector2>();
      movementX = movementVector.x;
      movementY = movementVector.y;
   }

   private void FixedUpdate()
   {
      Vector3 movement = new Vector3(movementX, 0.0f, movementY);
      rb.AddForce(movement * speed);
   }
}
