using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class CameraCollider : MonoBehaviour
    {
        public Transform target;
        public float distance = 5.0f;
        public float smoothSpeed = 10f;
        public float collisionRadius = 0.2f;
        public LayerMask collisionMask;

        private Vector3 desiredPosition;
        private Vector3 currentVelocity;

        void LateUpdate()
        {
            Vector3 direction = (transform.position - target.position).normalized;
            desiredPosition = target.position + direction * distance;

            if (Physics.SphereCast(target.position, collisionRadius, direction, out RaycastHit hit, distance, collisionMask))
            {
                desiredPosition = target.position + direction * (hit.distance - collisionRadius);
            }

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, Time.deltaTime * smoothSpeed);
            transform.LookAt(target); // Optional: always face the player
        }
    }
}
