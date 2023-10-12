using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectCamera : MonoBehaviour
{
    [SerializeField] Transform _truckTransform;
    [SerializeField] RectTransform _backgroundTransform;

    private float maxYPos;
    private float minYPos;

    // Start is called before the first frame update
    void Start()
    {
        CalculateContraints();
        ResetCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckCanMove(_truckTransform.position.y))
        {
            Camera.main.transform.position = new Vector3(0, _truckTransform.position.y, 0);
        }
    }

    /// <summary>
    /// Calculate the maximum and minimum positions that the camera can move to
    /// カメラが移動できる最大位置と最小位置を計算する
    /// </summary>
    private void CalculateContraints()
    {
        float halfCameraWorldHeight = Camera.main.orthographicSize;
        Vector3[] worldCorners = new Vector3[4];
        _backgroundTransform.GetWorldCorners(worldCorners);

        minYPos = worldCorners[0].y + halfCameraWorldHeight;
        maxYPos = worldCorners[1].y - halfCameraWorldHeight;
    }

    /// <summary>
    /// Constrain the camera position in the case when the current level causes the camera to go out of bounds during dungeon initialization
    /// ダンジョン初期化時に現在のレベルによりカメラが圏外になる場合、カメラ位置を拘束する
    /// </summary>
    private void ResetCamera()
    {
        if (Camera.main.transform.position.y > maxYPos)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, maxYPos, Camera.main.transform.position.z);
        }
        else if (Camera.main.transform.position.y < minYPos)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, minYPos, Camera.main.transform.position.z);
        }
    }

    /// <summary>
    /// Checks if destination is within constraints
    /// 目的地が制約の範囲内にあるかどうかをチェックする
    /// </summary>
    /// <returns>whether destinaiton is possible デスティネーションが可能かどうか</returns>
    private bool CheckCanMove(float destinationYPos)
    {
        return destinationYPos > minYPos && destinationYPos < maxYPos;
    }
}
