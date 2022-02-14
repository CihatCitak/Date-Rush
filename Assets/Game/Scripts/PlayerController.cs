using UnityEngine;
using Inputs;
using DG.Tweening;
using NaughtyAttributes;
using System;
using Cinemachine;
using UnityEngine.Animations.Rigging;


public class PlayerController : MonoBehaviour
{
    #region Singleton
    public static PlayerController Instance { get { return instance; } }
    private static PlayerController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    #region PlayerStates

    public enum PlayerStates
    {
        START,
        RUN,
        GAMEEND
    }
    private PlayerStates playerState;

    public void StartRun()
    {
        playerState = PlayerStates.RUN;
        animator.SetTrigger("SlowRun");
    }

    public void WinTheGame(Transform kissTransform)
    {
        playerState = PlayerStates.GAMEEND;
        animator.SetTrigger("Dance");

        GoToEndPosition(kissTransform);
    }

    public void GoToEndPosition(Transform kissTransform)
    {
        camera.gameObject.SetActive(false);
        sideMovementRoot.DORotate(kissTransform.eulerAngles, 1f).SetEase(Ease.Linear);
        sideMovementRoot.DOMove(kissTransform.position, 1f).SetEase(Ease.Linear).OnComplete(() => KissAnim(kissTransform));

        Debug.Log("�pmek i�in o�lana gidiyoruz");
    }

    private void KissAnim(Transform kissTransform)
    {
        animator.SetTrigger("Kiss");
        maleAnimator.SetTrigger("Kiss");
    }

    public void LoseTheGame()
    {
        playerState = PlayerStates.GAMEEND;

        animator.SetTrigger("Lose");
        maleAnimator.SetTrigger("Defeat");
    }

    #endregion

    [SerializeField] private InputSettings inputSettings;
    [SerializeField] private Transform sideMovementRoot;
    [SerializeField] Animator animator;
    [SerializeField] Animator maleAnimator;
    [SerializeField] CinemachineVirtualCamera camera;

    [SerializeField]
    [BoxGroup("Speed Settings")]
    private float forwardMovementSpeed = 1f, sideMovementSensitivity = 1f, rotationSpeed = 1f;

    [SerializeField]
    [BoxGroup("Limits")]
    private Transform leftLimit, rightLimit, leftLimitWithEnemy, rightLimitWithEnemy;

    [SerializeField]
    [BoxGroup("ClothPullTransforms")]
    private Transform clothPullLeftTransform, clothPullRightTransform, enemyPullLeftTransform, enemyPullRightTransform;

    [SerializeField]
    [BoxGroup("Two Bone IK Constraint")]
    private TwoBoneIKConstraint leftArmIK, rightArmIK;

    [SerializeField] DressController dressController;

    private EnemyWithCloth leftEnemyWithCloth, rightEnemyWithCloth;
    private float leftLimitX, rightLimitX;

    void Update()
    {
        if (playerState == PlayerStates.RUN)
        {
            HandleForwardMovement();
            HandleSideMovement();
        }
    }


    #region Movement

    private void HandleForwardMovement()
    {
        transform.Translate(Vector3.forward * forwardMovementSpeed * Time.deltaTime);
    }

    private void HandleSideMovement()
    {
        FindLimitXPosition();

        var localPos = sideMovementRoot.localPosition;
        localPos += Vector3.right * inputSettings.InputDrag.x * sideMovementSensitivity;

        localPos.x = Mathf.Clamp(localPos.x, leftLimitX, rightLimitX);

        sideMovementRoot.localPosition = localPos;

        //var moveDirection = Vector3.forward * 0.5f;
        //moveDirection += sideMovementRoot.right * inputSettings.InputDrag.x * sideMovementSensitivity;

        //moveDirection.Normalize();

        //var targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

        //sideMovementRoot.rotation = Quaternion.Lerp(sideMovementRoot.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void FindLimitXPosition()
    {
        if (leftEnemyWithCloth != null)
            leftLimitX = leftLimitWithEnemy.localPosition.x;
        else
            leftLimitX = leftLimit.localPosition.x;

        if (rightEnemyWithCloth != null)
            rightLimitX = rightLimitWithEnemy.localPosition.x;
        else
            rightLimitX = rightLimit.localPosition.x;
    }

    #endregion 

    #region Interact with Enemy And Cloth

    public void ChangeCloth(EnemyWithCloth.WitchSides witchSide, DressData cloth)
    {
        dressController.DressUp(cloth);

        if (witchSide == EnemyWithCloth.WitchSides.LEFT)
        {
            leftEnemyWithCloth = null;
            ChangeIKWeight(leftArmIK, 0f);
        }
        else
        {
            rightEnemyWithCloth = null;
            ChangeIKWeight(rightArmIK, 0f);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        //Collide with cloth enemy
        if (other.CompareTag("EnemyWithCloth"))
        {
            if (other.TryGetComponent<EnemyWithCloth>(out EnemyWithCloth enemyWithCloth))
            {
                if (enemyWithCloth.IsInteractable)
                {
                    PullTheCloth(enemyWithCloth);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            CheckAndDropTheLeftCloth();

            CheckAndDropTheRightCloth();
        }
    }

    private void PullTheCloth(EnemyWithCloth enemyWithCloth)
    {
        if (enemyWithCloth.WitchSide == EnemyWithCloth.WitchSides.LEFT)
        {
            CheckAndDropTheLeftCloth();

            leftEnemyWithCloth = enemyWithCloth;
            enemyWithCloth.PullTheCloth(sideMovementRoot, enemyPullLeftTransform, clothPullLeftTransform);

            ChangeIKWeight(leftArmIK, 1f);

            return;
        }

        if (enemyWithCloth.WitchSide == EnemyWithCloth.WitchSides.RIGHT)
        {
            CheckAndDropTheRightCloth();

            rightEnemyWithCloth = enemyWithCloth;
            enemyWithCloth.PullTheCloth(sideMovementRoot, enemyPullRightTransform, clothPullRightTransform);

            ChangeIKWeight(rightArmIK, 1f);

            return;
        }
    }

    private void CheckAndDropTheLeftCloth()
    {
        if (leftEnemyWithCloth != null)
        {
            ChangeIKWeight(leftArmIK, 0f);
            leftEnemyWithCloth.DropTheCloth();
            leftEnemyWithCloth = null;
        }
    }

    private void CheckAndDropTheRightCloth()
    {
        if (rightEnemyWithCloth != null)
        {
            ChangeIKWeight(rightArmIK, 0f);
            rightEnemyWithCloth.DropTheCloth();
            rightEnemyWithCloth = null;
        }
    }

    #endregion

    private void ChangeIKWeight(TwoBoneIKConstraint ik, float weight)
    {
        ik.weight = weight;
    }

}
