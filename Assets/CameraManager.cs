using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
public class CameraManager : MonoBehaviour 
{
    public static CameraManager instance;

    [SerializeField] private CinemachineCamera[] _allCameras;

    [Header("Control for lerping the Y damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float  _fallSpeedYDampingChangeTreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPLayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;

    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer _positionComposer;

    private float _normYpanAmount;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        for (int i =0; i < _allCameras.Length; i++)
        {
            if (_allCameras[i].enabled)
            {
                //set the current active camera
                _currentCamera = _allCameras[i];

                //set the position composer
                _positionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
            }
        }
        //set the YDamping amount so it's based on the inspector value
        _normYpanAmount = _positionComposer.Damping.y;
    }

    #region Lerp the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    public IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        //grab the starting damping amount
        float startDampAmount = _positionComposer.Damping.y;
        float endDampAmount = 0f;

        //determine the end damping amount
        if (isPlayerFalling )
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPLayerFalling = true;
        }

        else
        {
            endDampAmount = _normYpanAmount;
        }

        //lerp the pan amount
        float elapsedTime = 0f;
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime/_fallYPanTime));
            _positionComposer.Damping.y = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }
    #endregion
}
