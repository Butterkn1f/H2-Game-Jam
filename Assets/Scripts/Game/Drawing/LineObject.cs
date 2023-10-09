using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(MeshFilter))]

public class LineObject : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Camera _camera;

    public readonly List<Vector2> _points = new List<Vector2>();
    public int PointCount => _points.Count;

    /// <summary>
    /// Initialize variables
    /// 変数の初期化
    /// </summary>
    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _camera = Camera.main;
    }

    /// <summary>
    /// Destroy line when it goes out of camera view
    /// カメラの視界から外れるとラインを破壊する
    /// </summary>
    private void OnBecameInvisible()
    {
        // Do not destroy if just turning briefly invisible to switch to mesh
        // メッシュに切り替えるために短時間だけ透明化する場合は、破壊しないでください
        if (_camera && _camera.WorldToScreenPoint(transform.position).y >= Screen.height * 0.5f)
            return;

        Destroy(gameObject);
    }

    /// <summary>
    /// Check whether new point is not the first point and distance from last point is less than the minimum required
    /// 新しい点が最初の点でなく、最後の点からの距離が必要最小限の距離以下であるかどうかを確認します。
    /// </summary>
    /// <param name="newPoint">position of new point 新点の位置</param>
    /// <returns>whether a new point can be added 新規点数の追加可否</returns>
    private bool CheckCannotAddPoint(Vector2 newPoint)
    {
        return _points.Count >= 1 && Vector2.Distance(newPoint, _points.Last()) < DrawManager.Instance.MinPointDistance;
    }

    /// <summary>
    /// Add a new point to line renderer
    /// ラインレンダラーに新しい点を追加する
    /// </summary>
    /// <param name="newPoint">point to add 追加点</param>
    public void AddPoint(Vector2 newPoint)
    {
        if (CheckCannotAddPoint(newPoint) || !_lineRenderer)
            return;

        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(PointCount, newPoint);
        _points.Add(newPoint);
    }
}
