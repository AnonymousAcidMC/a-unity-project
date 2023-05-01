using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using UnityEngine;

//This system updates key presses made by the player such as movement, jumping, skill activation etc.
//It also makes the player move accordingly
//In the future, a separate system might be made for each type of input.
public partial class PlayerInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((
            ref PhysicsVelocity velocity,
            ref PlayerInputData playerInputData,
            ref MovementData movementData,
            in PlayerInputKeys keys
        ) => {
            #region x & z movement
            //Get movement directions
            int xDirection = Convert.ToInt32(Input.GetKey(keys.rightKey)) - Convert.ToInt32(Input.GetKey(keys.leftKey));
            int zDirection = Convert.ToInt32(Input.GetKey(keys.upKey)) - Convert.ToInt32(Input.GetKey(keys.downKey));

            //move player accordingly
            velocity.Linear.x = xDirection * movementData.movementSpeed;
            velocity.Linear.z = zDirection * movementData.movementSpeed;
            playerInputData.movementDirection.x = xDirection;
            playerInputData.movementDirection.z = zDirection;
            #endregion x & z movement
            
            

            #region jumping
            //get bool stating whether jump key is pressed
            bool jump = Input.GetKey(keys.jump);

            playerInputData.jump = jump;

            //Jump if button is pressed & on the ground 
            if(jump && movementData.isGrounded) {
                velocity.Linear.y += movementData.jumpHeight;
            }
            #endregion jumping
        }).WithoutBurst().Run();
    }
}