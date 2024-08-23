using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterScript : MonoBehaviour
{
    private const string FinishTag = "Finish";

	[SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private float speed;
    [SerializeField] private float turnSmoothTime;

    private Transform transformCached;
    private Transform cameraTransform;
    private float targetAngle;
    private float turnSmoothVelocity;
    private Vector2 moveInput;
    private bool isMove;
    private bool canMove;


    private void Start()
    {
        transformCached = transform;
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (canMove && isMove)
        {
            targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;    // Вычислить целевой угол поворота персонажа
            transformCached.rotation = Quaternion.Euler(0f, Mathf.SmoothDampAngle(transformCached.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime), 0f); // Сгладить поворот персонажа
            controller.Move(speed * Time.deltaTime * (transformCached.rotation * Vector3.forward));                 // Двигать персонажа по направлению его вглядя
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag(FinishTag))
        {
            Game2.instance.ShowResult();
        }
    }

    /// <summary>Обрабатывает ввод передвижения InputSystem</summary>
    private void OnMove(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            moveInput = context.ReadValue<Vector2>();
            isMove = moveInput.y != 0 || moveInput.x != 0;
            animator.SetBool("isRunning", canMove && isMove);  // Включить/выключить анимацию бега
        }
    }

    public void CanMove(bool value)
    {
        canMove = value;
        if (!canMove)
        {
            isMove = false;
            animator.SetBool("isRunning", false);
        }
    }

    public void OnExit(InputAction.CallbackContext _)
    {
        Cursor.lockState = CursorLockMode.None; // Вернуть курсор перед выходом
        SceneManager.LoadScene("Menu");
    }
}
