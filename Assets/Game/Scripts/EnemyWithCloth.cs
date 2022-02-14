using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class EnemyWithCloth : MonoBehaviour
{
    public enum WitchSides
    {
        LEFT, RIGHT
    }

    public WitchSides WitchSide;
    [SerializeField] GameObject clothPrefab;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform rigTarget, clothHoldTransform;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;

    public bool IsInteractable { get { return isInteractable; } }
    private bool isInteractable = true;

    private Vector3 clothStartLocalPosition, clothStartLocalEulerAngles;
    private DressData clothTemp;

    private void FixedUpdate()
    {
        rigTarget.position = clothHoldTransform.position;
    }


    private void Awake()
    {
        clothStartLocalPosition = clothHoldTransform.localPosition;
        clothStartLocalEulerAngles = clothHoldTransform.localEulerAngles;

        clothTemp = Instantiate(clothPrefab).GetComponent<DressData>();

        clothTemp.transform.SetParent(clothHoldTransform, false);
        clothTemp.transform.position = clothHoldTransform.position;
        //clothTemp.transform.rotation = clothHoldTransform.rotation;
    }

    public void PullTheCloth(Transform parent, Transform enemyPullTransform, Transform clothPullTransform)
    {
        if (isInteractable)
        {
            animator.SetTrigger("SlowRun");

            EnemyTransformSettingsForPull(parent, enemyPullTransform);

            ClothTransformSettingsForPull(parent, clothPullTransform);

            isInteractable = false;

            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void DropTheCloth()
    {
        animator.SetTrigger("Arguring");

        EnemyTransformSettingsForDrop();

        ClothTransformSettingsForDrop();
    }

    public void GiveClothToPlayer()
    {
        if (!isInteractable)
        {
            Debug.Log("Kýyafet Player a geçti");

            EnemyTransformSettingsForDrop();

            isInteractable = true;

            PlayerController.Instance.ChangeCloth(WitchSide, clothTemp);

        }
    }

    #region Cloth Transform Settings

    private void ClothTransformSettingsForPull(Transform parent, Transform clothPullTransform)
    {
        ClothGoToPosition(clothPullTransform.localPosition);

        //ClothDoRotation(clothPullTransform.localEulerAngles);

        ClothSetTransformParent(parent, false);
    }

    private void ClothTransformSettingsForDrop()
    {
        ClothGoToPosition(clothStartLocalPosition);

        //ClothDoRotation(clothStartLocalEulerAngles);

        ClothSetTransformParent(transform, false);
    }

    private void ClothGoToPosition(Vector3 localPosition)
    {
        clothHoldTransform.DOLocalMove(localPosition, 0f);
    }

    private void ClothDoRotation(Vector3 localRotation)
    {
        clothHoldTransform.DOLocalRotate(localRotation, 0f);
    }

    private void ClothSetTransformParent(Transform parent, bool worldPosiitonStays)
    {
        clothHoldTransform.SetParent(parent, worldPosiitonStays);
    }

    #endregion

    #region Enemy Transform Settings

    private void EnemyTransformSettingsForPull(Transform parent, Transform enemyPullTransform)
    {
        EnemyGoToPosiiton(enemyPullTransform.localPosition);

        EnemyLookToDirection(Vector3.forward);

        EnemySetTransformParent(parent, true);
    }

    private void EnemyTransformSettingsForDrop()
    {
        EnemySetTransformParent(null, true);
    }

    private void EnemyGoToPosiiton(Vector3 localPosition)
    {
        transform.DOLocalMove(localPosition, moveSpeed)
            .SetSpeedBased();
    }

    private void EnemyLookToDirection(Vector3 direction)
    {
        transform.DOLocalRotate(direction, 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
    }

    private void EnemySetTransformParent(Transform parent, bool worldPosiitonStays)
    {
        transform.SetParent(parent, worldPosiitonStays);
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        //Collide with cloth enemy
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if(!isInteractable)
            {
                DropTheCloth();

                PlayerController.Instance.ChangeCloth(WitchSide ,clothTemp);

                animator.SetTrigger("Jogging");

                isInteractable = true;
            }
        }
    }
}
