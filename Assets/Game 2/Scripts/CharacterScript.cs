using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterScript : MonoBehaviour
{
	public CharacterController controller;
    public Animator animator;
    public float speed;
    public float turnSmoothTime;
    private Transform _transform;
    private Transform cameraTransform;
    private float targetAngle;
    private float turnSmoothVelocity;
    private Vector2 move;
    private bool moveInput;
    private bool canMove;


    private void Start()
    {
        _transform = transform;
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (canMove && moveInput)
        {
            targetAngle = Mathf.Atan2(move.x, move.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;  // ��������� ������� ���� �������� ���������
            _transform.rotation = Quaternion.Euler(0f, Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime), 0f);   // �������� ������� ���������
            controller.Move(speed * Time.deltaTime * (_transform.rotation * Vector3.forward));          // ������� ��������� �� ����������� ��� ������
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Finish"))
        {
            Game2.ShowResult();
        }
    }

    // ����� ����� ������������ InputSystem
    public void OnMove(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            move = context.ReadValue<Vector2>();
            moveInput = move.y != 0 || move.x != 0;
            animator.SetBool("isRunning", canMove && moveInput);  // ��������/��������� �������� ����
        }
    }

    public void CanMove(bool value)
    {
        canMove = value;
        if (!canMove)
        {
            moveInput = false;
            animator.SetBool("isRunning", false);
        }
    }

    public void OnExit(InputAction.CallbackContext context)
    {
        Cursor.lockState = CursorLockMode.None; // ������� ������ ����� �������
        SceneManager.LoadScene("Menu");
    }
}
