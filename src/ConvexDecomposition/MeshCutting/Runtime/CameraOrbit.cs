﻿#if UNITY_EDITOR
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using UnityEngine;

namespace Appalachia.Spatial.ConvexDecomposition.MeshCutting.Runtime
{
    /// <summary>
    ///     Adapted from
    ///     https://answers.unity.com/questions/1257281/how-to-rotate-camera-orbit-around-a-game-object-on.html
    /// </summary>
    public sealed class CameraOrbit : AppalachiaBehaviour<CameraOrbit>
    {
        #region Fields and Autoproperties

        public Transform target;
        private Vector3 fixedPosition;

        public float distance = 2.0f;
        public float xSpeed = 5.0f;
        public float ySpeed = 5.0f;
        public float yMinLimit = -90f;
        public float yMaxLimit = 90f;
        public float distanceMin = 2f;
        public float distanceMax = 10f;
        private float rotationYAxis;
        private float rotationXAxis;

        private Rigidbody _rigidbody;

        #endregion

        #region Event Functions

        // Called after Update
        private void LateUpdate()
        {
            if (target)
            {
                if (Input.GetMouseButton(1))
                {
                    rotationYAxis += xSpeed * Input.GetAxis("Mouse X") * distance;
                    rotationXAxis -= ySpeed * Input.GetAxis("Mouse Y");
                    rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
                }

                var toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
                var rotation = toRotation;

                distance = Mathf.Clamp(
                    distance - (Input.GetAxis("Mouse ScrollWheel") * 5),
                    distanceMin,
                    distanceMax
                );
                var negDistance = new Vector3(0.0f, 0.0f, -distance);
                var position = (rotation * negDistance) + fixedPosition;

                var transform1 = transform;
                transform1.rotation = rotation;
                transform1.position = position;
            }
        }

        #endregion

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
            {
                angle += 360F;
            }

            if (angle > 360F)
            {
                angle -= 360F;
            }

            return Mathf.Clamp(angle, min, max);
        }

        /// <inheritdoc />
        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

            var angles = transform.eulerAngles;
            rotationYAxis = angles.y;
            rotationXAxis = angles.x;

            _rigidbody = initializer.Get(this, _rigidbody, nameof(Rigidbody));

            _rigidbody.freezeRotation = true;

            // Clone the target's position so that it stays fixed
            if (target)
            {
                fixedPosition = target.position;
            }
        }
    }
}

#endif
