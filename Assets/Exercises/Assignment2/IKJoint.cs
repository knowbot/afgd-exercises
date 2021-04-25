using System;
using UnityEngine;

namespace Exercises.Assignment2
{
    public class IKJoint : MonoBehaviour
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public IKJoint Parent { get; set; }
        // Start is called before the first frame update

        private void Awake()
        {
            Transform transformVar = transform;
            Position = transformVar.position;
            Rotation = transformVar.rotation;
            Transform parent = transformVar.parent;
            Parent = parent != null ? parent.gameObject.GetComponent<IKJoint>() : null;
            Debug.Log(name + " " + Position);
        }

        public void UpdateTransform()
        {
            Transform transformVar = transform;
            transformVar.rotation = Rotation;
            transformVar.position = Position;
        }
    }
}
