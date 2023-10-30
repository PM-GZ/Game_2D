using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;




public class UiLoopScroll : UiBaseScroll
{
    [Range(0, 360)] public float AngleRange;
    public float Radius;
    public bool AutoResetPose;
    [SerializeField] public float AutoResetPoseSpeed;

    private float mAngle;
    private List<RectTransform> mSortList = new();
    private Vector3[] mInitPosArray;
    private float mMoveAngle;
    private Tween mTween;
    private Coroutine mAutoPoseCoroutine;


    private void OnDestroy()
    {
        mTween?.Kill();
        if (mAutoPoseCoroutine != null)
        {
            StopCoroutine(mAutoPoseCoroutine);
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        mTween?.Kill();
        if(mAutoPoseCoroutine != null)
        {
            StopCoroutine(mAutoPoseCoroutine);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        mMoveAngle += (mAxisHorizontal ? mDelta.x : mDelta.y) / Radius;
        Move();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        if (!AutoResetPose) return;

        float delta = mAxisHorizontal ? mDelta.x : mDelta.y;
        mTween = DOTween.To(MoveBuffer, delta, 0, 0.3f).OnComplete(AutoRestPose).SetLink(gameObject);
    }

    private void MoveBuffer(float a)
    {
        mMoveAngle += a / Radius;
        Move();
    }

    public override void InitScroll()
    {
        base.InitScroll();

        mInitPosArray = new Vector3[transform.childCount];
        InitCellRect(Vector2.one / 2, Vector2.one / 2);
        foreach (var item in mCellList)
        {
            mSortList.Add(item.rectTransform);
        }
        mAngle = AngleRange / transform.childCount * Mathf.Deg2Rad;
        Move();
        SetInitPosArray();
    }

    private void AutoRestPose()
    {
        var last = transform.GetChild(transform.childCount - 1);
        float delta = -(mAxisHorizontal ? last.localPosition.x : last.localPosition.y) / Radius;
        mAutoPoseCoroutine = StartCoroutine(StartAutoReset(delta));
    }

    private IEnumerator StartAutoReset(float delta)
    {
        float remainDelta = delta;
        while (true)
        {
            mMoveAngle += delta * Time.deltaTime * AutoResetPoseSpeed;
            remainDelta -= delta * Time.deltaTime * AutoResetPoseSpeed;
            Move();
            if (delta < 0 && remainDelta >= 0 || delta > 0 && remainDelta <= 0)
            {
                break;
            }
            yield return null;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition = mInitPosArray[i];
            mScrollData.OnCellMove?.Invoke(transform.childCount - 1 - i);
        }
        mAutoPoseCoroutine = null;
    }

    private void SetInitPosArray()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            mInitPosArray[i] = transform.GetChild(i).localPosition;
        }
    }

    private void Move()
    {
        SetCellPos();
        SetCellSibling();
    }

    private void SetCellPos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var cell = mCellList[i].rectTransform;
            cell.localPosition = GetCellPos(i);
            float scale = Mathf.Cos(i * mAngle + mMoveAngle) * Radius;
            scale = (scale + Radius) / (2 * Radius) * (1 - 0.5f) + 0.5f;
            cell.localScale = Vector3.one * scale;
        }
    }

    private Vector3 GetCellPos(int index)
    {
        Vector3 pos = Vector3.zero;
        float x = Mathf.Sin(index * mAngle + mMoveAngle) * Radius;
        if (mAxisHorizontal)
        {
            pos.x = x;
        }
        else
        {
            pos.y = x;
        }
        return pos;
    }

    private void SetCellSibling()
    {
        mSortList.Sort((l, r) =>
        {
            if (l.localScale.z < r.localScale.z)
                return -1;
            if (l.localScale.z > r.localScale.z)
                return 1;
            return 0;
        });
        for (int i = 0; i < mSortList.Count; i++)
        {
            mSortList[i].SetSiblingIndex(i);
        }
    }
}