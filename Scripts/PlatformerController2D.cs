using System;
using UnityEngine;

namespace Yern.PlatformerController2D
{
    /// <summary>
    /// Hi, it's Yern!
    /// Thank you for using my platformer controller. I'm trying to improve it as much
    /// as I can, and your feedback is highly appreciated. If you have any issues, please
    /// send me a message at @yerndev or just send an email to yigit@yern.co. Hope you
    /// enjoy the controller!
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class PlatformerController2D : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Horizontal Movement Settings")] 
        [SerializeField] private float horizontalVelocityLimit = 10f;
        [SerializeField] [Range(0, 1)] private float horizontalAccelerationRate = 0.9f;
        [SerializeField] [Range(0, 1)] private float horizontalDecelerationRate = 0.9f;
        [SerializeField] private bool flipXScaleOnDirectionChange = true;

        [Header("Gravity Settings")] 
        [SerializeField] private float gravity = 9.8f;
        [SerializeField] private bool clampFallSpeed = true;
        [SerializeField] private float maximumFallSpeed = 20f;

        [Header("Jump Settings")] 
        [SerializeField] private bool enableJumping = true;
        [SerializeField] private bool enableVariableJumpHeight = true;
        [SerializeField] private float jumpHeight = 3f;
        [SerializeField] private uint jumpCount = 2;
        [SerializeField] private bool allowFirstJumpWhileFalling = false;
        [SerializeField] private float coyoteTime = 0.1f;
        [SerializeField] private float maxJumpBufferTime = 0.2f;

        [Header("Dash Settings")]
        [SerializeField] private bool enableDashing = true;
        [SerializeField] private float dashPower = 35f;
        [SerializeField] private float dashTime = 0.075f;
        [SerializeField] private bool allowMovementWhileDash = false;
        [SerializeField] private float dashMovementModifier = 0.4f;
        [SerializeField] private bool allowDashWhileGrounded = false;
        [SerializeField] private bool disableGravityWhileDashing = true;
        
        [Header("Ground Check Settings")]
        [SerializeField] private LayerMask groundCheckMask;
        [SerializeField] [Min(2)] private uint groundCheckRayCount = 3;
        [SerializeField] private float groundCheckRayDistance = 0.05f;
        [SerializeField] private bool enableGroundCheckStuckProtection = true;
        [SerializeField] [Min(0)] private float groundCheckUnstuckThreshold = 0.045f;
        [SerializeField] private float groundCheckUnstuckVelocity = 0.3f;
        
        [Header("Ceiling Check Settings")]
        [SerializeField] private LayerMask ceilingCheckLayerMask;
        [SerializeField] [Min(2)] private uint ceilingCheckRayCount = 3;
        [SerializeField] private float ceilingCheckRayDistance = 0.05f;
        [SerializeField] private string[] ceilingCheckIgnoreTags;

        [Header("Horizontal Check Settings")] 
        [SerializeField] private LayerMask horizontalCheckLayerMask;
        [SerializeField] [Min(2)] private uint horizontalCheckRayCount = 3;
        [SerializeField] private float horizontalCheckRayDistance = 0.05f;

        [Header("References")]
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private BoxCollider2D boxCollider;
        #endregion
        
        public PlatformerController2DState State { get; private set; }
        public PlatformerController2DDirection Direction { get; private set; } = PlatformerController2DDirection.Right;
        public bool Grounded { get; private set; }
        public Vector2 Velocity { get; private set; }

        public Action<PlatformerController2DState> OnStateChange;
        public Action<PlatformerController2DDirection> OnDirectionChange;
        public Action<bool> OnGroundedChange;

        /// <summary>
        /// Replenishes the jump left by given amount. Zero replenishes to max value.
        /// Automatically clamped to max jump count.
        /// </summary>
        public void ReplenishJump(uint amount = 0)
        {
            if (amount == 0)
            {
                _jumpLeft = jumpCount;
                return;
            }
            
            _jumpLeft += amount;
            if (_jumpLeft > jumpCount) 
                _jumpLeft = jumpCount;
        }

        #region Controller Mechanics
        private float InternalGravity => gravity * 10;
        
        private PlatformerControllerInput _currentInput;
        private Vector2 _velocity;
        private Vector2 _unstuckVelocity;
        private Vector2 _dashVelocity;
        
        private uint _jumpLeft;
        private float _currentTime;
        private float _lastGroundedTime;
        private float _lastJumpTime;
        private float _lastJumpPressTime;
        private float _lastJumpReleaseTime;
        private bool _executedBufferedJump;
        private float _lastDashTime;

        private void Reset()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();

            rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
            rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rigidbody2D.gravityScale = 0;
            rigidbody2D.freezeRotation = true;
        }
        private void Update()
        {
            _currentInput.Horizontal = Input.GetAxisRaw("Horizontal");
            _currentInput.Vertical = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Space))
                _currentInput.StartJump = true;
            
            if (Input.GetKeyUp(KeyCode.Space))
                _currentInput.StopJump = true;

            if (Input.GetKeyDown(KeyCode.LeftShift) && _dashVelocity.magnitude == 0)
                _currentInput.StartDash = true;
        }
        private void FixedUpdate()
        {
            _currentTime = Time.fixedTime;
            
            CalculateGrounded();
            CalculateGravity();
            CalculateDash();
            CalculateJump();
            CalculateCeiling();
            CalculateMovement();
            CalculateState();
            CalculateDirection();
            
            MoveRigidbody();
        }
        
        private void CalculateGrounded()
        {
            var bounds = boxCollider.bounds;
            var start = new Vector2(bounds.min.x, bounds.min.y);
            var end = new Vector2(bounds.max.x, bounds.min.y);
            var points = GenerateEvenlySpacedPoints(start, end, groundCheckRayCount);
            var lastGrounded = Grounded;
            
            Grounded = false;
            if (_velocity.y > 0)
            {
                if (lastGrounded != Grounded) OnGroundedChange?.Invoke(Grounded);
                return;
            }

            var distanceFromHitPoint = groundCheckRayDistance;
            foreach (var point in points)
            {
                var hit = Physics2D.Raycast(point, Vector2.down, groundCheckRayDistance, groundCheckMask);
                if (hit.collider == null) continue;

                Grounded = true;
                distanceFromHitPoint = (point - hit.point).magnitude;
                _lastGroundedTime = Time.fixedTime;
                break;
            }
            
            if (lastGrounded != Grounded) OnGroundedChange?.Invoke(Grounded);
            
            if (enableGroundCheckStuckProtection && _velocity.y == 0 && distanceFromHitPoint < groundCheckRayDistance - groundCheckUnstuckThreshold)
                _unstuckVelocity.y = groundCheckUnstuckVelocity;
            else
                _unstuckVelocity.y = 0;
        }
        private void CalculateGravity()
        {
            if (Grounded)
            {
                if (_velocity.y < 0)
                    _velocity.y = 0;
                return;
            }

            _velocity.y -= InternalGravity * Time.deltaTime;

            if (clampFallSpeed && _velocity.y < -maximumFallSpeed)
                _velocity.y = -maximumFallSpeed;
        }
        private void CalculateDash()
        {
            if (!enableDashing) return;
            if (_lastDashTime + dashTime <= Time.fixedTime)
                _dashVelocity = Vector2.zero;
            
            if (!_currentInput.StartDash) return;
            if (!allowDashWhileGrounded && Grounded)
            {
                _currentInput.StartDash = false;
                return;
            }
            
            var dashDirection = new Vector2(_currentInput.Horizontal, _currentInput.Vertical).normalized;
            _dashVelocity = dashDirection * dashPower;
            _lastDashTime = Time.fixedTime;
            _currentInput.StartDash = false;
        }
        private void CalculateJump()
        {
            if (!enableJumping) return;
            if (_currentInput.StartJump) _lastJumpPressTime = _currentTime;
            if (_currentInput.StopJump) _lastJumpReleaseTime = _currentTime;
            
            if (_unstuckVelocity.magnitude > 0) return;
            if (_dashVelocity.magnitude > 0)
            {
                _currentInput.StartJump = false;
                return;
            }
            
            var canJumpWhileGrounded = Grounded && _lastGroundedTime + coyoteTime > _currentTime && _jumpLeft == jumpCount;
            var canJumpWhileNotGrounded = !Grounded && (allowFirstJumpWhileFalling || _lastGroundedTime + coyoteTime > _currentTime && _jumpLeft == jumpCount || _jumpLeft < jumpCount);
            var canJump = (canJumpWhileGrounded || canJumpWhileNotGrounded) && _jumpLeft > 0;
            var canJumpBuffered = Grounded && maxJumpBufferTime + _lastJumpPressTime > _currentTime && _lastJumpReleaseTime < _lastJumpPressTime;
            
            if (!canJump)
                _currentInput.StartJump = false;
            
            if ((_currentInput.StartJump || canJumpBuffered) && canJump)
            {
                var jumpVelocity = Mathf.Sqrt(2 * jumpHeight * InternalGravity);
                _lastJumpTime = Time.fixedTime;
                _velocity.y = jumpVelocity;
                _jumpLeft--;
                _currentInput.StartJump = false;
                return;
            }

            if (_currentInput.StopJump && enableVariableJumpHeight && _lastJumpTime <= Time.fixedTime)
            {
                if (_velocity.y > 0)
                    _velocity.y = 0;
                
                _currentInput.StopJump = false;
            }

            if (Grounded)
                _jumpLeft = jumpCount;
        }
        private void CalculateCeiling()
        {
            var bounds = boxCollider.bounds;
            var start = new Vector2(bounds.min.x, bounds.max.y);
            var end = new Vector2(bounds.max.x, bounds.max.y);
            var points = GenerateEvenlySpacedPoints(start, end, ceilingCheckRayCount);

            foreach (var point in points)
            {
                var hit = Physics2D.Raycast(point, Vector2.up, ceilingCheckRayDistance, ceilingCheckLayerMask);
                if (hit.collider == null) continue;

                var shouldIgnore = false;
                foreach (var ignoreTag in ceilingCheckIgnoreTags)
                    if (hit.collider.CompareTag(ignoreTag))
                    {
                        shouldIgnore = true;
                        break;
                    }
                
                if (shouldIgnore) continue;

                if (_velocity.y > 0)
                    _velocity.y = 0;
                
                break;
            }
        }
        private void CalculateMovement()
        {
            if (_currentInput.Horizontal == 0)
            {
                _velocity.x = _velocity.x switch
                {
                    > 0 => Mathf.Max(0, _velocity.x - horizontalVelocityLimit / Mathf.Abs(1 - horizontalDecelerationRate) * Time.deltaTime),
                    < 0 => Mathf.Min(0, _velocity.x + horizontalVelocityLimit / Mathf.Abs(1 - horizontalDecelerationRate) * Time.deltaTime),
                    _ => _velocity.x
                };

                return;
            }

            _velocity.x += horizontalVelocityLimit / Mathf.Abs(1 - horizontalAccelerationRate) * _currentInput.Horizontal * Time.deltaTime;
            _velocity.x = Mathf.Clamp(_velocity.x, -horizontalVelocityLimit, horizontalVelocityLimit);

            var bounds = boxCollider.bounds;

            if (Grounded && _currentInput.Horizontal < 0 && _velocity.x < 0)
            {
                var start = new Vector2(bounds.min.x, bounds.min.y);
                var end = new Vector2(bounds.min.x, bounds.max.y);
                var points = GenerateEvenlySpacedPoints(start, end, horizontalCheckRayCount);
                
                foreach (var point in  points)
                {
                    var hit = Physics2D.Raycast(point, Vector2.left, horizontalCheckRayDistance, horizontalCheckLayerMask);
                    if (hit.collider == null) continue;

                    _velocity.x = 0;
                    break;
                }
            }
            
            if (Grounded && _currentInput.Horizontal > 0 && _velocity.x > 0)
            {
                var start = new Vector2(bounds.max.x, bounds.min.y);
                var end = new Vector2(bounds.max.x, bounds.max.y);
                var points = GenerateEvenlySpacedPoints(start, end, horizontalCheckRayCount);
                
                foreach (var point in  points)
                {
                    var hit = Physics2D.Raycast(point, Vector2.right, horizontalCheckRayDistance, groundCheckMask);
                    if (hit.collider == null) continue;

                    _velocity.x = 0;
                    break;
                }
            }
        }
        private void CalculateState()
        {
            var oldState = State;
            
            if (_dashVelocity.magnitude > 0)
            {
                State = PlatformerController2DState.Dashing;
                if (State != oldState) OnStateChange?.Invoke(State);
                return;
            }

            if (!Grounded)
            {
                State = _velocity.y > 0 ? 
                    PlatformerController2DState.AirRaising : 
                    PlatformerController2DState.AirFalling;
                if (State != oldState) OnStateChange?.Invoke(State);
                return;
            }

            if (_velocity.magnitude > 0)
            {
                State = PlatformerController2DState.Running;
                if (State != oldState) OnStateChange?.Invoke(State);
                return;
            }
                
            State = PlatformerController2DState.Idle;
            if (State != oldState) OnStateChange?.Invoke(State);
        }
        private void CalculateDirection()
        {
            var oldDirection = Direction;
            
            Direction = _currentInput.Horizontal switch
            {
                > 0 => PlatformerController2DDirection.Right,
                < 0 => PlatformerController2DDirection.Left,
                _ => Direction
            };

            if (Direction == oldDirection) return;
            if (flipXScaleOnDirectionChange)
            {
                var t = transform;
                var oldScale = t.localScale;
                oldScale.x = Direction == PlatformerController2DDirection.Right ? Mathf.Abs(oldScale.x) * 1 : Mathf.Abs(oldScale.x) * -1;
                t.localScale = oldScale;
            }

            OnDirectionChange?.Invoke(Direction);
        }
        private void MoveRigidbody()
        {
            if (_unstuckVelocity.magnitude > 0)
            {
                rigidbody2D.MovePosition(rigidbody2D.position + _unstuckVelocity);
                return;
            }
            
            Velocity = Vector2.zero;

            if (_dashVelocity.magnitude > 0)
            {
                Velocity += _dashVelocity;
                
                if (disableGravityWhileDashing) _velocity.y = 0;
                if (allowMovementWhileDash) Velocity += _velocity * dashMovementModifier;
                
                rigidbody2D.MovePosition(rigidbody2D.position + Velocity * Time.fixedDeltaTime);
                return;
            }

            Velocity += _velocity;
            rigidbody2D.MovePosition(rigidbody2D.position + Velocity * Time.fixedDeltaTime);
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmosSelected()
        {
            GizmosDrawGroundLines();
            GizmosDrawCeilingLines();
            GizmosDrawHorizontalLines();
        }
        private void GizmosDrawHorizontalLines()
        {
            var bounds = boxCollider.bounds;
            var leftStart = new Vector2(bounds.min.x, bounds.min.y);
            var leftEnd = new Vector2(bounds.min.x, bounds.max.y);
            var leftPoints = GenerateEvenlySpacedPoints(leftStart, leftEnd, horizontalCheckRayCount);
            var rightStart = new Vector2(bounds.max.x, bounds.min.y);
            var rightEnd = new Vector2(bounds.max.x, bounds.max.y);
            var rightPoints = GenerateEvenlySpacedPoints(rightStart, rightEnd, horizontalCheckRayCount);

            Gizmos.color = Color.cyan;
            foreach (var point in leftPoints)
                Gizmos.DrawLine(point, point + Vector2.left * horizontalCheckRayDistance);
            
            foreach (var point in rightPoints)
                Gizmos.DrawLine(point, point + Vector2.right * horizontalCheckRayDistance);
        }
        private void GizmosDrawGroundLines()
        {
            var bounds = boxCollider.bounds;
            var start = new Vector2(bounds.min.x, bounds.min.y);
            var end = new Vector2(bounds.max.x, bounds.min.y);
            var points = GenerateEvenlySpacedPoints(start, end, groundCheckRayCount);
            
            Gizmos.color = Color.cyan;
            foreach (var point in points)
                Gizmos.DrawLine(point, point + Vector2.down * groundCheckRayDistance);
        }
        private void GizmosDrawCeilingLines()
        {
            var bounds = boxCollider.bounds;
            var start = new Vector2(bounds.min.x, bounds.max.y);
            var end = new Vector2(bounds.max.x, bounds.max.y);
            var points = GenerateEvenlySpacedPoints(start, end, ceilingCheckRayCount);
            
            Gizmos.color = Color.cyan;
            foreach (var point in points)
                Gizmos.DrawLine(point, point + Vector2.up * ceilingCheckRayDistance);
        }
        #endregion

        #region Utility
        private Vector2[] GenerateEvenlySpacedPoints(Vector2 start, Vector2 end, uint amount)
        {
            var pointsArray = new Vector2[amount];
            var differencePerStep = (end - start) / (amount - 1);

            for (var i = 0; i < amount; i++)
                pointsArray[i] = start + differencePerStep * i;

            return pointsArray;
        }
        #endregion
    }

}