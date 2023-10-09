using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ViewportHandler : MonoBehaviour
{
    [Tooltip("Which side to constrain\nどちらの面を拘束するのか")]
    [SerializeField] private Constraint _constraint = Constraint.Portrait;
    [Tooltip("Size of scene in Unity units, based on constraints\n制約に基づくUnity単位でのシーンの大きさ")]
    [SerializeField] private float CameraUnitSize = 10f;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        UpdateCameraSize();
    }

    /// <summary>
    /// Only need to constantly refresh resolution inside editor, when testing different aspect ratios
    /// 異なるアスペクト比をテストする場合、エディター内で解像度を常にリフレッシュする必要があります
    /// </summary>
    private void Update()
    {
#if UNITY_EDITOR
        UpdateCameraSize();
#endif
    }

    /// <summary>
    /// Update camera's orthographic size based on screen resolution
    /// Ensures that the distance of each object from the edge of the screens stays the same, depending on the axis set by Constraint
    /// スクリーンの解像度に応じてカメラの正射影サイズを更新する
    /// Constraintで設定された軸によって、画面の端からの各オブジェクトの距離が同じになるようにする。
    /// </summary>
    private void UpdateCameraSize()
    {
        switch (_constraint)
        {
            case Constraint.Portrait:
                _camera.orthographicSize = CameraUnitSize * 0.5f;
                break;

            case Constraint.Landscape:
                _camera.orthographicSize = 1f / _camera.aspect * CameraUnitSize / 2f;
                break;
        }
    }
}

public enum Constraint { Landscape, Portrait }
