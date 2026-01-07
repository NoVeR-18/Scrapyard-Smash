using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class PlayerMovementSettings
    {
        public float Speed { get => _speed; }
        public AnimationCurve SpeedSensitivityCurve { get => _speedSensitivityCurve; }
        public float ModelRotationLerp { get => _modelRotationLerp; }
        public float SteeringAngle { get => _steeringAngle; } // Свойство для угла поворота колес

        [SerializeField] private float _speed = 5;
        [SerializeField] private float _modelRotationLerp = 10f;
        [SerializeField] private AnimationCurve _speedSensitivityCurve;
        [SerializeField] private float _steeringAngle = 30f; // Максимальный угол поворота колес
    }
}
