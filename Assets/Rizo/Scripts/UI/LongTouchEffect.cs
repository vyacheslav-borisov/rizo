using UnityEngine;

namespace Pegas.Rizo
{
    public class LongTouchEffect : MonoBehaviour
    {
        ParticleSystem _ps;

        [Range(1, 10)]
        public float _speedUPTo = 3.0f;
        [Range(1, 10)]
        public float _speedUpTime = 2.0f;
        public float _focusDistance = 40.0f;
        private float _elapsedTime = 0.0f;

        private void Awake()
        {
            _ps = GetComponentInChildren<ParticleSystem>();
        }

        private void Update()
        {
            if (!Input.GetMouseButton(0))
            {
                Destroy(gameObject);
                return;
            }

            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo, 100.0f, 1 << LayerMask.NameToLayer("Player")))
            {
                var bodyPart = hitInfo.collider.GetComponent<BodyPart>();
                if (!bodyPart)
                {
                    Destroy(gameObject);
                    return;
                }

                transform.position = ray.origin + ray.direction * _focusDistance;
                transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _elapsedTime += Time.deltaTime;
            float k = _elapsedTime / _speedUpTime;
            k = Mathf.Clamp01(k);

            var main = _ps.main;
            main.simulationSpeed = Mathf.Lerp(1.0f, _speedUPTo, k);
        }
    }
}
