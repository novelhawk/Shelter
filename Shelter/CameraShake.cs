using JetBrains.Annotations;
using UnityEngine;

public class CameraShake : MonoBehaviour //TODO: Removed unusued class?
{
    private float _forceDecay;
    private float _duration;
    private bool _direction;
    private float _force;

    [UsedImplicitly]
    private void shakeUpdate()
    {
        if (_duration > 0f)
        {
            _duration -= Time.deltaTime;
            Transform cameraTransform = gameObject.transform;
            
            if (_direction)
                cameraTransform.position += Vector3.up * this._force;
            else
                cameraTransform.position -= Vector3.up * this._force;
            
            _direction = !_direction;
            _force *= _forceDecay;
        }
    }

    [UsedImplicitly]
    public void startShake(float force, float duration, float decay = 0.95f)
    {
        if (_duration <= 0)
        {
            _force = force;
            _duration = duration;
            _forceDecay = decay;
        }
    }
}

