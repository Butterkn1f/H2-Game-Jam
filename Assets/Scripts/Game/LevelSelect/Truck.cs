using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Rigidbody2D))]
public class Truck : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _drag = 0.5f;
    [SerializeField] private RectTransform parent;
    //[SerializeField] private GameObject _hitEffect;
    //private Animator _animator;
    private Rigidbody2D _rigidbody;
    private CompositeDisposable _pressDisposables;

    private Vector3? _targetPos;

    /// <summary>
    /// Initialize variables
    /// 変数の初期化
    /// </summary>
    private void Start()
    {
        //_animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _targetPos = null;

        SubscribePressFunctions();
    }

    private void SubscribePressFunctions()
    {
        _pressDisposables = new CompositeDisposable();
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            .Subscribe(_ => MoveToMouseTap())
            .AddTo(_pressDisposables);
    }

    void Update()
    {
        CheckIfReachTarget();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //_rigidbody.velocity = Vector3.zero;
        _targetPos = null;
        //_animator.SetBool("IsRunning", false);
    }

    /*private void HitEffect()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Instantiate(_hitEffect, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
    }*/

    private void MoveToMouseTap()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _targetPos = new Vector3(pos.x, pos.y, 0);
        Debug.Log(_targetPos);

        Vector3 destination = _targetPos.Value;
        float distance = Vector3.Distance(destination, transform.position);
        float speedNeeded = distance * _drag + _speed;

        Vector3 direction = (destination - transform.position).normalized;

        _rigidbody.velocity = direction * speedNeeded;
        //_animator.SetBool("IsRunning", true);
    }

    private void CheckIfReachTarget()
    {
        if (!_targetPos.HasValue)
            return;

        if (Round(transform.position) == Round(_targetPos.Value))
        {
            _rigidbody.velocity = Vector3.zero;
            _targetPos = null;
            //_animator.SetBool("IsRunning", false);
        }
    }
    private Vector3 Round(Vector3 vector3, int decimalPlaces = 1)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            0);
    }
}
