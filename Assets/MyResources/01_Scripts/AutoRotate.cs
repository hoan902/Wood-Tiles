using UnityEngine;

namespace Ez.Examples
{
    public class AutoRotate : MonoBehaviour
    {
        public bool X = true;
        public bool Y = false;
        public bool Z = false;
        [Range(-7200, 7200)]
        public float Speed = 30;

        public float RotationX { get { return transform.localRotation.x; } set { } }
        public float RotationY { get { return transform.localRotation.y; } set { } }
        public float RotationZ { get { return transform.localRotation.z; } set { } }

        public Vector3 RotationDirection
        {
            get
            {
                return new Vector3(X ? 1 : 0, Y ? 1 : 0, Z ? 1 : 0);
            }
            set
            {
                X = value.x >= 1 ? true : false;
                Y = value.y >= 1 ? true : false;
                Z = value.z >= 1 ? true : false;
            }
        }

        private bool ShouldRotate { get { return X || Y || Z; } }

        private void Update()
        {
            if (!ShouldRotate) { return; }
            transform.Rotate(new Vector3(X ? 1 : 0,
                                         Y ? 1 : 0,
                                         Z ? 1 : 0) * Time.deltaTime * Speed);
        }
    }
}
