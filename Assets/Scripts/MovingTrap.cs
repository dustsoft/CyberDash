using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrap : Trap
{
    [SerializeField] float _speed;
    [SerializeField] float _rotationSpeed;
    [SerializeField] Transform[] _movePoint;

    int _i;

    protected override void Start()
    {
        base.Start();
        transform.position = _movePoint[0].position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _movePoint[_i].position, _speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, _movePoint[_i].position) < 0.25f)
        {
            _i++;

            if (_i >= _movePoint.Length)
                _i = 0;
        }

        if (transform.position.x > _movePoint[_i].position.x)
            transform.Rotate(new Vector3(0, 0, _rotationSpeed * Time.deltaTime));
        else
            transform.Rotate(new Vector3(0, 0, -_rotationSpeed * Time.deltaTime));
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
