using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace AfGD
{
    // exercise 2.4
    // attach to an object so that it moves along a path
    public class MoveAlongPath : MonoBehaviour
    {
        [Tooltip("Path to be followed")] [SerializeField]
        private SplinePath path;

        [Tooltip("Movement duration")] [Range(1.0f, 100.0f)]
        public float movementDuration = 5.0f;

        [Tooltip("Start/stop movement")] [SerializeField]
        private bool stopMovement = true;

        [SerializeField] private bool parameterizeByArclength = true;
        [SerializeField] private bool useEasingCurve = true;

        // we keep an internal clock for this object for higher flexibility
        private float _localTime = 0;

        void Start()
        {
            if (!path)
                Debug.LogError("No path assigned.");
        }

        // Update is called once per frame
        void Update()
        {
            if (!path || stopMovement)
                return;
            
            _localTime += Time.deltaTime;

            // t is normalized: [0, speed] -> [0,1]
            float t = _localTime / movementDuration;
            
            // determine s with easing function (if necessary)
            float s = (useEasingCurve) ? t : EasingFunctions.Crossfade(EasingFunctions.SmoothStart2, EasingFunctions.SmoothStop2, 0.5f, t);
            
            transform.position = path.Evaluate(s, parameterizeByArclength);
            var tangentVec = path.Evaluate(s, parameterizeByArclength, 1);
            transform.rotation = Quaternion.LookRotation(tangentVec.normalized, transform.up);
        }
    }
}