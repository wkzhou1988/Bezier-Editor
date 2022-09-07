using System;
using UnityEngine;

namespace AnimationSpline.Runtime
{
    public enum HandleType
    {
        Smooth,
        Broken,
    }
    
    [Serializable]
    public class SplineNode
    {
        public Vector3    Position;
        public Vector3    InControl;
        public Vector3    OutControl;
        public HandleType HandleType;

        public void SetPosition(Vector3 newPos)
        {
            InControl  += newPos - Position;
            OutControl += newPos - Position;
            Position   =  newPos;
        }

        public void SetInControl(Vector3 coord)
        {
            InControl = coord;

            if (HandleType == HandleType.Smooth)
            {
                OutControl = Position - (InControl - Position);
            }
        }

        public void SetOutControl(Vector3 coord)
        {
            OutControl = coord;
            
            if (HandleType == HandleType.Smooth)
            {
                InControl = Position - (OutControl - Position);
            }
        }
    }
}