using UnityEngine;
using System.Collections;

namespace TransitionalObjects
{
    [AddComponentMenu("")]//hides this script from being added
    public class ScalingTransition : BaseTransition
    {
        //[HideInInspector]
        public Transform scalingTransform;

        public Vector3 startPoint = Vector3.zero, endPoint = Vector3.one;

        protected override void Transition(float transitionPercentage)
        {
            if(scalingTransform == null)
                scalingTransform = parent.transform;

            scalingTransform.localScale = startPoint + (endPoint - startPoint) * transitionPercentage;
        }

        public override void Clone(BaseTransition other)
        {
            base.Clone(other);

            ScalingTransition converted = (ScalingTransition)other;

            startPoint = converted.startPoint;
            endPoint = converted.endPoint;
        }

        #region Editor Externals
#if(UNITY_EDITOR)
        /// <summary>
        /// Called by the editor to view either the start of end point
        /// </summary>
        public override void ViewPosition(TransitionalObject.MovingDataType movingType)
        {
            if(movingType == TransitionalObject.MovingDataType.StartPoint)
                scalingTransform.localScale = startPoint;
            else
                scalingTransform.localScale = endPoint;
        }

        /// <summary>
        /// Called by the editor to update the start and end points based on the current position
        /// </summary>
        /// <param name="isStartPoint"></param>
        public override void UpdatePosition(TransitionalObject.MovingDataType movingType)
        {
            if(movingType == TransitionalObject.MovingDataType.StartPoint)
                startPoint = parent.transform.localScale;
            else
                endPoint = parent.transform.localScale;
        }
#endif
        #endregion
    }
}