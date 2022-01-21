using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// The forward acceleration value.
    /// </summary>
    private const float FORWARD_ACCELERATION        = 10.0f;

    /// <summary>
    /// The backward acceleration value.
    /// </summary>
    private const float BACKWARD_ACCELERATION       = 10.0f;

    /// <summary>
    /// The strafe acceleration value.
    /// </summary>
    private const float STRAFE_ACCELERATION         = 10.0f;

    /// <summary>
    /// The jump acceleration value.
    /// </summary>
    private const float JUMP_ACCELERATION           = 200.0f;

    /// <summary>
    /// The fly acceleration value.
    /// </summary>
    private const float FLY_ACCELERATION            = 5.0f;

    /// <summary>
    /// The gravity acceleration value.
    /// </summary>
    private const float GRAVITY_ACCELERATION        = 10.0f;

    /// <summary>
    /// The max forward velocity value.
    /// </summary>
    private const float MAX_FORWARD_VELOCITY        = 4.0f;

    /// <summary>
    /// The max backward velocity value.
    /// </summary>
    private const float MAX_BACKWARD_VELOCITY       = 2.0f;

    /// <summary>
    /// The max strafe velocity value.
    /// </summary>
    private const float MAX_STRAFE_VELOCITY         = 3.0f;

    /// <summary>
    /// The max jump velocity value.
    /// </summary>
    private const float MAX_JUMP_VELOCITY           = 50.0f;

    /// <summary>
    /// The max fall velocity value.
    /// </summary>
    private const float MAX_FALL_VELOCITY           = 100.0f;

    /// <summary>
    /// The rotation velocity factor value.
    /// </summary>
    private const float ROTATION_VELOCITY_FACTOR    = 2.0f;

    /// <summary>
    /// The min tilt rotation value.
    /// </summary>
    private const float MIN_TILT_ROTATION           = 70.0f;

    /// <summary>
    /// The max tilt rotation value.
    /// </summary>
    private const float MAX_TILT_ROTATION           = 290.0f;

    /// <summary>
    /// Reference to the character controller.
    /// </summary>
    private CharacterController _controller;

    /// <summary>
    /// Reference to the camera's <c>Transform</c>.
    /// </summary>
    private Transform           _cameraTransform;

    /// <summary>
    /// The Player's acceleration.
    /// </summary>
    private Vector3             _acceleration;

    /// <summary>
    /// The Player's velocity.
    /// </summary>
    private Vector3             _velocity;

    /// <summary>
    /// Contains information about wether the Player jumped or not.
    /// </summary>
    private bool                _jump;

    /// <summary>
    /// Contains information about wether the player is flying or not.
    /// </summary>
    private bool                _flying;

    /// <summary>
    /// Contains information about wether the player can fly or not.
    /// </summary>
    private bool                _canFly;

    /// <summary>
    /// Initializes all the instance variables and hides the mouse cursor.
    /// </summary>
    void Start()
    {
        _controller      = GetComponent<CharacterController>();
        _cameraTransform = GetComponentInChildren<Camera>().transform;
        _acceleration    = Vector3.zero;
        _velocity        = Vector3.zero;
        _jump            = false;
        _flying          = false;
        _canFly          = false;

        HideCursor();
    }

    /// <summary>
    /// Hides the mouse cursor.
    /// </summary>
    private void HideCursor()
    {
        Cursor.visible      = false;
        Cursor.lockState    = CursorLockMode.Locked;
    }

    /// <summary>
    /// Called every frame. Updates the value changes caused by the Player's
    /// movement.
    /// </summary>
    void Update()
    {
        UpdateJump();
        UpdateFly();
        UpdateRotation();
        UpdateTilt();
    }

    /// <summary>
    /// Updates the jump status.
    /// </summary>
    private void UpdateJump()
    {
        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
            _jump = true;
    }

    /// <summary>
    /// Updates the fly status.
    /// </summary>
    private void UpdateFly()
    {
        if (Input.GetButtonDown("Jump") && !_controller.isGrounded && _canFly)
        {
            _jump = false;
            _flying = true;
        }
        else if (_controller.isGrounded)
            _flying = false;
    }

    /// <summary>
    /// Updates the camera's rotation.
    /// </summary>
    private void UpdateRotation()
    {
        float rotation = Input.GetAxis("Mouse X") * ROTATION_VELOCITY_FACTOR;

        transform.Rotate(0f, rotation, 0f);
    }

    /// <summary>
    /// Updates the camera's tilt.
    /// </summary>
    private void UpdateTilt()
    {
        Vector3 cameraRotation = _cameraTransform.localEulerAngles;

        cameraRotation.x -= Input.GetAxis("Mouse Y") * ROTATION_VELOCITY_FACTOR;

        if (cameraRotation.x < 180f)
            cameraRotation.x = Mathf.Min(cameraRotation.x, MIN_TILT_ROTATION);
        else
            cameraRotation.x = Mathf.Max(cameraRotation.x, MAX_TILT_ROTATION);

        _cameraTransform.localEulerAngles = cameraRotation;
    }

    /// <summary>
    /// Updates physics related values. 
    /// </summary>
    void FixedUpdate()
    {
        UpdateAcceleration();
        UpdateVelocity();
        UpdatePosition();
    }

    /// <summary>
    /// Updates Player's acceleration value.
    /// </summary>
    private void UpdateAcceleration()
    {
        _acceleration.z = Input.GetAxis("Forward");
        _acceleration.z *= (_acceleration.z > 0f) ? FORWARD_ACCELERATION : BACKWARD_ACCELERATION;

        _acceleration.x = Input.GetAxis("Strafe") * STRAFE_ACCELERATION;

        if (_jump)
        {
            _acceleration.y = JUMP_ACCELERATION;
            _jump = false;
        }
        else if (_controller.isGrounded)
            _acceleration.y = 0f;
        else
            _acceleration.y = -GRAVITY_ACCELERATION;
        
        if (_flying)
            if (Input.GetButton("Jump"))
                _acceleration.y = FLY_ACCELERATION;
            else if (Input.GetButton("Descend"))
                _acceleration.y = -FLY_ACCELERATION;
            else
                _acceleration.y = -GRAVITY_ACCELERATION;
    }

    /// <summary>
    /// Updates Player's velocity value.
    /// </summary>
    private void UpdateVelocity()
    {
        _velocity += _acceleration * Time.fixedDeltaTime;

        _velocity.z = (_acceleration.z == 0f || _acceleration.z * _velocity.z < 0f) ?
            0f : Mathf.Clamp(_velocity.z, -MAX_BACKWARD_VELOCITY, MAX_FORWARD_VELOCITY);
        
        _velocity.x = (_acceleration.x == 0f || _acceleration.x * _velocity.x < 0f) ?
            0f : Mathf.Clamp(_velocity.x, -MAX_STRAFE_VELOCITY, MAX_STRAFE_VELOCITY);

        _velocity.y = (_acceleration.y == 0f) ?
                -0.1f : Mathf.Clamp(_velocity.y, -MAX_FALL_VELOCITY, MAX_JUMP_VELOCITY);
    }

    /// <summary>
    /// Updates Player's position.
    /// </summary>
    private void UpdatePosition()
    {
        Vector3 motion = _velocity * Time.fixedDeltaTime;

        _controller.Move(transform.TransformVector(motion));
    }

    public void GetWings()
    {
        _canFly = true;
    }
}