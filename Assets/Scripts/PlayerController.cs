using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float JumpForce = 850;

    [SerializeField]
    private float walkSpeed = 5f;

    private float _xAxis;
    private float _yAxis;
    private Rigidbody2D _rb2d;
    private Animator _animator;
    private bool _isJumpPressed;
    private bool _isGrounded = true;
    private string _currentAnimator;
    private bool _isAttackPressed;
    private bool _isAttacking;

    [SerializeField]
    private float attackDelay = 0.1f;

    private static readonly int IsPlayerRun = Animator.StringToHash("isPlayerRun");
    private static readonly int IsPlayerIdle = Animator.StringToHash("isPlayerIdle");
    private static readonly int IsPlayerJump = Animator.StringToHash("isPlayerJump");
    private static readonly int IsPlayerAttack = Animator.StringToHash("isPlayerAttack");
    private static readonly int IsPlayerAirAttack = Animator.StringToHash("isPlayerAirAttack");

    //=====================================================
    // Start is called before the first frame update
    //=====================================================
    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    } 
    
    //=====================================================
    // Physics based time step loop
    //=====================================================
    private void FixedUpdate()
    {
        //Checking for inputs
        _xAxis = Input.GetAxisRaw("Horizontal");
        //space jump key pressed?
        _isJumpPressed = Input.GetKeyDown(KeyCode.Space);
        //space Attack key pressed?
        _isAttackPressed = Input.GetKey(KeyCode.LeftControl);

        //Check update movement based on input
        var v = new Vector2(0, _rb2d.velocity.y);

        if (_xAxis < 0)
        {
            v.x = -walkSpeed;
            transform.localScale = new Vector2(-1, 1);
        }
        else if (_xAxis > 0)
        {
            v.x = walkSpeed;
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            v.x = 0;
        }

        if (_isGrounded && !_isAttacking)
        {
            if (_xAxis != 0)
            {
                _animator.SetBool(IsPlayerRun, true);
                _animator.SetBool(IsPlayerIdle, false);
            }
            else
            {
                _animator.SetBool(IsPlayerRun, false);
                _animator.SetBool(IsPlayerIdle, true);
            }
        }

        //------------------------------------------

        //Check if trying to jump 
        if (_isJumpPressed && _isGrounded)
        {
            _rb2d.AddForce(new Vector2(0, JumpForce));
            _isJumpPressed = false;
            _animator.SetBool(IsPlayerJump, true);
            _animator.SetBool(IsPlayerIdle, false);
        }
        else _animator.SetBool(IsPlayerJump, false);

        //assign the new velocity to the rigidbody
        _rb2d.velocity = v;

        //attack
        if (_isAttackPressed)
        {
            _isAttackPressed = false;
            if (!_isAttacking)
            {
                _isAttacking = true;
                if(_isGrounded)
                {
                    _animator.SetBool(IsPlayerAttack, true);
                    _animator.SetBool(IsPlayerIdle, false);
                }
                else
                {
                    _animator.SetBool(IsPlayerAirAttack, true);
                    _animator.SetBool(IsPlayerIdle, false);
                }
                StartCoroutine(nameof(AttackComplete), attackDelay);
            }
            else
            {
                _animator.SetBool(IsPlayerAttack, false);
                _animator.SetBool(IsPlayerAirAttack, false);
            }
        }
        else
        {
            _animator.SetBool(IsPlayerAttack, false);
            _animator.SetBool(IsPlayerAirAttack, false);
        }
    }

    private void AttackComplete()
    {
        _isAttacking = false;
    }

    private void OnCollisionExit2D()
    {
        _isGrounded = false;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.collider.tag)
        {
            case "Ground":
                _isGrounded = true;
                break;
        }
    }
}
