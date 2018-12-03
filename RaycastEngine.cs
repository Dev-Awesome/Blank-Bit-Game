// Copyright (c) 2017, Timothy Ned Atton.
// All rights reserved.
// nedmakesgames@gmail.com
// This code was written while streaming on twitch.tv/nedmakesgames
//
// This file is part of Raycast Platformer.
//
// Raycast Platformer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Raycast Platformer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Raycast Platformer.  If not, see <http://www.gnu.org/licenses/>.

using UnityEngine;
using System.Collections;

public class RaycastEngine : MonoBehaviour {

    private enum JumpState
    {
        None = 0, Holding, 
    }

    private enum DashState
    {
        None = 0, Holding,
    }

    private enum BoostState
    {
        None = 0, Holding,
    }

    private enum CrouchState
    {
        None = 0, Holding,
    }

    private enum EraseState
    {
        None = 0, Holding,
    }

    [SerializeField]
    private LayerMask platformMask;
    [SerializeField]
    private float parallelInsetLen;
    [SerializeField]
    private float perpendicularInsetLen;
    [SerializeField]
    private float groundTestLen;
    [SerializeField]
    private float gravity;
    [SerializeField]
    private float vertBoost;
    [SerializeField]
    private float horizSpeedUpAccel;
    [SerializeField]
    private float horizSpeedDownAccel;
    [SerializeField]
    private float horizSnapSpeed;
    [SerializeField]
    private float horizMaxSpeed;
    [SerializeField]
    private float jumpInputLeewayPeriod;
    [SerializeField]
    private float jumpStartSpeed;
    [SerializeField]
    private float jumpMaxHoldPeriod;
    [SerializeField]
    private float jumpMinSpeed;
    [SerializeField]
    private float dashInputLeewayPeriod;
    [SerializeField]
    private float dashMaxHoldPeriod;
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float boostInputLeewayPeriod;
    [SerializeField]
    private float boostMaxHoldPeriod;
    [SerializeField]
    private float boostSpeed;
    [SerializeField]
    private float eraseInputLeewayPeriod;
    [SerializeField]
    private float eraseMaxHoldPeriod;
    [SerializeField]
    private float stunPeriod;
    [SerializeField]
    private float stunBounce;
    [SerializeField]
    private float crouchInputLeewayPeriod;
    [SerializeField]
    private float crouchDropPeriod;
    [SerializeField]
    //private AudioSource jumpSFX, landSFX, startMoveSFX;
    
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer blankSR;
    public AudioClip startMoveFX;
    public AudioClip landFX;

    private Vector2 velocity;

    private RaycastMoveDirection moveDown;
    private RaycastMoveDirection moveLeft;
    private RaycastMoveDirection moveRight;
    private RaycastMoveDirection moveUp;

    private RaycastCheckTouch groundDown;
    private PlayerAnim playerAnim;
    private BoxCollider2D boxCollider;
    private AudioManager audioManager;

    private Vector2 lastStandingOnPos;
    private Vector2 lastStandingOnVel;
    private Collider2D lastStandingOn;
    private Collider2D dropPlatform;

    private float jumpStartTimer;
    private float jumpHoldTimer;
    private bool jumpInputDown;
    private bool dashInputDown;
    private float dashStartTimer;
    private float dashHoldTimer;
    private bool boostInputDown;
    private float boostStartTimer;
    private float boostHoldTimer;
    private bool crouchInputDown;
    private float crouchStartTimer;
    private float crouchHoldTimer;
    private float eraseStartTimer;
    private float erasehHoldTimer;
    private bool eraseInputDown;
    private bool stunned;
    private float stunTimer;
    private float currentX;
    private float enemyX;
    private JumpState jumpState;
    private DashState dashState;
    private BoostState boostState;
    private CrouchState crouchState;
    private EraseState eraseState;
    private bool lastGrounded;
    private int dash;
    private int boost;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnim = GetComponent<PlayerAnim>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioManager = FindObjectOfType<AudioManager>();

        moveDown = new RaycastMoveDirection(new Vector2(-0.5f, -0.75f), new Vector2(0.5f, -0.75f), Vector2.down, platformMask, 
            Vector2.right * parallelInsetLen, Vector2.up * perpendicularInsetLen);
        moveLeft = new RaycastMoveDirection(new Vector2(-0.5f, -0.75f), new Vector2(-0.5f, 0.75f), Vector2.left, platformMask,
            Vector2.up * parallelInsetLen, Vector2.right * perpendicularInsetLen);
        moveUp = new RaycastMoveDirection(new Vector2(-0.5f, 0.75f), new Vector2(0.5f, 0.75f), Vector2.up, platformMask,
            Vector2.right * parallelInsetLen, Vector2.down * perpendicularInsetLen);
        moveRight= new RaycastMoveDirection(new Vector2(0.5f, -0.75f), new Vector2(0.5f, 0.75f), Vector2.right, platformMask,
            Vector2.up * parallelInsetLen, Vector2.left * perpendicularInsetLen);

        groundDown = new RaycastCheckTouch(new Vector2(-0.5f, -0.75f), new Vector2(0.5f, -0.75f), Vector2.down, platformMask,
            Vector2.right * parallelInsetLen, Vector2.up * perpendicularInsetLen, groundTestLen);
    }

    private int GetSign(float v) {
        if(Mathf.Approximately(v, 0)) {
            return 0;
        } else if(v > 0) {
            return 1;
        } else {
            return -1;
        }
    }

    private void Update() {
        jumpStartTimer -= Time.deltaTime;
        bool jumpBtn = GameManager.isJumping;
        if(jumpBtn && jumpInputDown == false) {
            jumpStartTimer = jumpInputLeewayPeriod;
        }
        jumpInputDown = jumpBtn;

        dashStartTimer -= Time.deltaTime;
        bool dashBtn = GameManager.isDashing;
        if (dashBtn && dashInputDown == false)
        {
            dashStartTimer = dashInputLeewayPeriod;
        }
        dashInputDown = dashBtn;

        boostStartTimer -= Time.deltaTime;
        bool boostBtn = GameManager.isBoosting;
        if (boostBtn && boostInputDown == false)
        {
            boostStartTimer = boostInputLeewayPeriod;
        }
        boostInputDown = boostBtn;

        crouchStartTimer -= Time.deltaTime;
        bool crouchBtn = GameManager.isCrouching;
        if(crouchBtn && crouchInputDown == false)
        {
            crouchStartTimer = crouchInputLeewayPeriod;
        }
        crouchInputDown = crouchBtn;

        eraseStartTimer -= Time.deltaTime;
        bool eraseBtn = GameManager.isErasing;
        if(eraseBtn && eraseInputDown == false)
        {
            eraseStartTimer = eraseInputLeewayPeriod;
        }
        eraseInputDown = eraseBtn;
    }

    private void FixedUpdate() {

        Collider2D standingOn = groundDown.DoRaycast(transform.position);
        bool grounded = standingOn != null;
        if(grounded && lastGrounded == false) {
            audioManager.fx.PlayOneShot(landFX, 0.25f);
        }
        lastGrounded = grounded;

        switch(jumpState)
        {
            case JumpState.None:
                /*if(grounded && jumpStartTimer > 0) {
                    jumpStartTimer = 0;
                    jumpState = JumpState.Holding;
                    jumpHoldTimer = 0;
                    velocity.y = jumpStartSpeed;
                    jumpSFX.Play();
                }*/
                if(!stunned)
                {
                    if (jumpStartTimer > 0)
                    {
                        jumpStartTimer = 0;
                        jumpState = JumpState.Holding;
                        jumpHoldTimer = 0;
                        velocity.y = jumpStartSpeed;
                        //jumpSFX.Play();
                    }
                }
                break;
            case JumpState.Holding:
                jumpHoldTimer += Time.deltaTime;
                if(jumpInputDown == false || jumpHoldTimer >= jumpMaxHoldPeriod) {
                    jumpState = JumpState.None;
                    velocity.y = Mathf.Lerp(jumpMinSpeed, jumpStartSpeed, jumpHoldTimer / jumpMaxHoldPeriod);

                    // Lerp!
                    //float p = jumpHoldTimer / jumpMaxHoldPeriod;
                    //velocity.y = jumpMinSpeed + (jumpStartSpeed - jumpMinSpeed) * p;
                }
                break;
        }

        switch(dashState)
        {
            case DashState.None:
                if(!stunned)
                {
                    if (dashStartTimer > 0)
                    {
                        dashStartTimer = 0;
                        dashState = DashState.Holding;
                        dashHoldTimer = 0;
                        //dashFX.Play();
                    }
                }
                break;
            case DashState.Holding:
                dashHoldTimer += Time.deltaTime;
                if(dashInputDown == false || dashHoldTimer >= dashMaxHoldPeriod)
                {
                    dashState = DashState.None;
                }
                break;
        }

        switch (boostState)
        {
            case BoostState.None:
                if (!stunned)
                {
                    if (boostStartTimer > 0)
                    {
                        boostStartTimer = 0;
                        boostState = BoostState.Holding;
                        boostHoldTimer = 0;
                        //boostFX.Play();
                    }
                }
                break;
            case BoostState.Holding:
                boostHoldTimer += Time.deltaTime;
                if (boostInputDown == false || boostHoldTimer >= boostMaxHoldPeriod)
                {
                    boostState = BoostState.None;
                }
                break;
        }

        switch(eraseState)
        {
            case EraseState.None:
                if(!stunned)
                {
                    if(eraseStartTimer > 0)
                    {
                        eraseStartTimer = 0;
                        eraseState = EraseState.Holding;
                        erasehHoldTimer = 0;
                        //eraseFX.Play();
                    }
                }
                break;
            case EraseState.Holding:
                erasehHoldTimer += Time.deltaTime;
                if(eraseInputDown == false || erasehHoldTimer >= eraseMaxHoldPeriod)
                {
                    eraseState = EraseState.None;
                }
                break;
        }

        switch(crouchState)
        {
            case CrouchState.None:
                if (grounded && !stunned)
                {
                    if(crouchStartTimer > 0)
                    {
                        crouchStartTimer = 0;
                        crouchState = CrouchState.Holding;
                        crouchHoldTimer = 0;
                        boxCollider.enabled = false;
                    }

                    if (dropPlatform != null)
                    {
                        dropPlatform.enabled = true;
                    }
                }
                break;
            case CrouchState.Holding:
                crouchHoldTimer += Time.deltaTime;
                dropPlatform = lastStandingOn;

                if(dropPlatform.gameObject.tag == "Droppable")
                {
                    if (crouchInputDown == false || stunned)
                    {
                        crouchState = CrouchState.None;
                        boxCollider.enabled = true;
                    }
                    else if (crouchHoldTimer >= crouchDropPeriod)
                    {
                        crouchState = CrouchState.None;
                        boxCollider.enabled = true;
                        lastStandingOn.enabled = false;
                    }
                }
                else
                {
                    if (crouchInputDown == false || stunned)
                    {
                        crouchState = CrouchState.None;
                        boxCollider.enabled = true;
                    }
                }

                break;
        }
        
        float horizInput = Input.GetAxisRaw("Horizontal");
        int wantedDirection = GetSign(horizInput);
        int velocityDirection = GetSign(velocity.x);

        if(wantedDirection != 0)
        {
            if(wantedDirection != velocityDirection)
            {
                if(boost <= 0)
                {
                    velocity.x = horizSnapSpeed * wantedDirection;
                    //audioManager.fx.PlayOneShot(startMoveFX, 0.3f);
                }
                else
                {
                    if (dashState == DashState.Holding)
                    {
                        dash += (int)dashSpeed;

                        if (dash < 32)
                        {
                            if (!spriteRenderer.flipX)
                            {
                                velocity.x = dash;
                            }
                            else if (spriteRenderer.flipX)
                            {
                                velocity.x = -dash;
                            }
                        }
                        else
                        {
                            velocity.x = Mathf.MoveTowards(velocity.x, 0, 75 * Time.deltaTime);
                        }
                    }
                    else
                    {
                        velocity.x = 0;
                    }
                }
            }
            else
            {
                if(!stunned)
                {
                    if (dashState == DashState.Holding)
                    {
                        dash += (int)dashSpeed;

                        if (dash < 32)
                        {
                            if (!spriteRenderer.flipX)
                            {
                                velocity.x = dash;
                            }
                            else if (spriteRenderer.flipX)
                            {
                                velocity.x = -dash;
                            }
                        }
                        else
                        {
                            velocity.x = Mathf.MoveTowards(velocity.x, 0, 75 * Time.deltaTime);
                        }
                    }
                    else if (boostState == BoostState.Holding)
                    {
                        boost += (int)boostSpeed;
                        velocity.x = 0;

                        if (boost <= 36)
                        {
                            velocity.y = boost;
                        }
                        else
                        {
                            velocity.y = Mathf.MoveTowards(velocity.y, 0, 125 * Time.deltaTime);
                        }
                    }
                    else
                    {
                        if(boost <= 0)
                        {
                            dash = 0;
                            boost = 0;
                            velocity.x = Mathf.MoveTowards(velocity.x, horizMaxSpeed * wantedDirection, horizSpeedUpAccel * Time.deltaTime);
                        }
                        else
                        {
                            velocity.x = 0;
                            velocity.y = Mathf.MoveTowards(velocity.y, 0, 200 * Time.deltaTime);
                        }
                    }
                }
            }
        }
        else
        {
            if (dashState == DashState.Holding)
            {
                dash += (int)dashSpeed;

                if (dash < 32)
                {
                    if (!spriteRenderer.flipX)
                    {
                        velocity.x = dash;
                    }
                    else if (spriteRenderer.flipX)
                    {
                        velocity.x = -dash;
                    }
                }
                else
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, 0, 75 * Time.deltaTime);
                }
            }
            else if (boostState == BoostState.Holding)
            {
                boost += (int)boostSpeed;
                velocity.x = 0;

                if (boost <= 36)
                {
                    velocity.y = boost;
                }
                else
                {
                    velocity.y = Mathf.MoveTowards(velocity.y, 0, 125 * Time.deltaTime);
                }
            }
            else
            {
                if (dash > 0)
                {
                    dash -= (int)dashSpeed;
                    velocity.x = Mathf.MoveTowards(velocity.x, 0, 150 * Time.deltaTime);
                }
                else
                {
                    if(boost > 0)
                    {
                        velocity.x = 0;
                        velocity.y = Mathf.MoveTowards(velocity.y, 0, 200 * Time.deltaTime);
                    }
                    else
                    {
                        dash = 0;
                        boost = 0;
                        velocity.x = Mathf.MoveTowards(velocity.x, 0, horizSpeedDownAccel * Time.deltaTime);
                    }
                }
            }
        }

        if(dashState != DashState.Holding)
        {
            if(jumpState == JumpState.None)
            {
                if(boostState != BoostState.Holding)
                {
                    if(eraseState != EraseState.Holding)
                    {
                        if (boost > 0)
                        {
                            boost -= (int)boostSpeed / 2;
                            velocity.y -= gravity * Time.deltaTime;
                            velocity.x = 0;
                        }
                        else
                        {
                            velocity.y -= gravity * Time.deltaTime;

                            if (crouchState == CrouchState.Holding)
                            {
                                velocity.x = 0;
                            }
                        }
                    }
                    else
                    {
                        velocity.x = 0;
                        velocity.y = 0;
                    }
                }
                else if(boostState == BoostState.Holding)
                {
                    velocity.x = 0;
                    stunned = false;
                }
            }

            if (stunned)
            {
                if(currentX != 0)
                {
                    velocity.x = (currentX * -1) * stunBounce;
                    stunTimer += Time.deltaTime;
                }
                else
                {
                    velocity.x = (enemyX * -3) * stunBounce;
                    stunTimer += Time.deltaTime;
                }

                if(stunTimer >= stunPeriod)
                {
                    stunned = false;
                    velocity.x = 0;
                }
            }
            else
            {
                currentX = velocity.x;
                stunTimer = 0;
            }
        }
        else if (dashState == DashState.Holding)
        {
            velocity.y += vertBoost * Time.deltaTime;
            stunned = false;
        }

        Vector2 displacement = Vector2.zero;
        Vector2 wantedDispl = velocity * Time.deltaTime;

        if(standingOn != null) {
            if(lastStandingOn == standingOn) {
                lastStandingOnVel = (Vector2)standingOn.transform.position - lastStandingOnPos;
                wantedDispl += lastStandingOnVel;
            } else if(standingOn == null) {
                velocity += lastStandingOnVel / Time.deltaTime;
                wantedDispl += lastStandingOnVel;
            }
            lastStandingOnPos = standingOn.transform.position;
        }
        lastStandingOn = standingOn;

        if(wantedDispl.x > 0) {
            displacement.x = moveRight.DoRaycast(transform.position, wantedDispl.x);
        } else if(wantedDispl.x < 0) {
            displacement.x = -moveLeft.DoRaycast(transform.position, -wantedDispl.x);
        }
        if(wantedDispl.y > 0) {
            displacement.y = moveUp.DoRaycast(transform.position, wantedDispl.y);
        } else if(wantedDispl.y < 0) {
            displacement.y = -moveDown.DoRaycast(transform.position, -wantedDispl.y);
        }

        if(Mathf.Approximately(displacement.x, wantedDispl.x) == false) {
            velocity.x = 0;
        }
        if(Mathf.Approximately(displacement.y, wantedDispl.y) == false) {
            velocity.y = 0;
        }

        transform.Translate(displacement);

        if(!stunned)
        {
            if (dashState == DashState.Holding)
            {
                if (playerAnim.cycleCounter != 4)
                {
                    playerAnim.flip = true;
                    playerAnim.cycleCounter = 4;
                }
            }
            else
            {
                if (jumpState == JumpState.Holding)
                {
                    if (playerAnim.cycleCounter != 3)
                    {
                        playerAnim.flip = true;
                        playerAnim.cycleCounter = 3;
                    }
                }
                else
                {
                    if(eraseState == EraseState.Holding)
                    {
                        if (playerAnim.cycleCounter != 8)
                        {
                            playerAnim.flip = true;
                            playerAnim.cycleCounter = 8;
                        }
                    }
                    else
                    {
                        if (grounded)
                        {
                            if (wantedDirection == 0)
                            {
                                if (crouchState == CrouchState.Holding)
                                {
                                    if (playerAnim.cycleCounter != 6)
                                    {
                                        playerAnim.flip = true;
                                        playerAnim.cycleCounter = 6;
                                    }
                                }
                                else
                                {
                                    if (playerAnim.cycleCounter != 0)
                                    {
                                        playerAnim.flip = true;
                                        playerAnim.cycleCounter = 0;
                                    }
                                }
                            }
                            else
                            {
                                if (playerAnim.cycleCounter != 1)
                                {
                                    playerAnim.flip = true;
                                    playerAnim.cycleCounter = 1;
                                }
                            }
                        }
                        else
                        {
                            if (velocity.y < 0)
                            {
                                if (playerAnim.cycleCounter != 2)
                                {
                                    playerAnim.flip = true;
                                    playerAnim.cycleCounter = 2;
                                }
                            }
                            else
                            {
                                if (boostState == BoostState.Holding)
                                {
                                    if (playerAnim.cycleCounter != 5)
                                    {
                                        playerAnim.flip = true;
                                        playerAnim.cycleCounter = 5;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (stunned)
        {
            if (playerAnim.cycleCounter != 7)
            {
                playerAnim.flip = true;
                playerAnim.cycleCounter = 7;
            }
        }

        if(wantedDirection != 0) {
            spriteRenderer.flipX = wantedDirection < 0;
            blankSR.flipX = wantedDirection < 0;
        }
    }

    public void Stun(float enemyDir)
    {
        if (!stunned && stunTimer < stunPeriod)
        {
            enemyX = enemyDir - this.transform.position.x;
            stunned = true;
        }
    }

    public void Stop()
    {
        if (dashState == DashState.Holding)
        {
            velocity.x = 0;
        }
        else if (boostState == BoostState.Holding)
        {
            velocity.y = 0;
        }
        else
        {
            if(velocity.y > 0)
            {
                velocity.y = 0;
            }
        }
    }
}
