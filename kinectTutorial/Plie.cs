using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Kinect;

namespace balletBot
{
    /// <summary>
    /// The Plie class handles the movement gesture of a ballet move called
    /// a plie. This will call the correct methods in MovementMode to check
    /// the user's alignment.
    /// </summary>
    class Plie
    {
        public Skeleton skeleton = null;
        public Canvas canvas;
        public Boolean gestureComplete = false;
        public Boolean plieBottomReached = false;
        public int plieMovingUpFrames = 0;
        public int plieCompleteBannerFrames = 0;
        public Position position;
        public Range leftKneeTopRange;
        public Range rightKneeTopRange;
        public Point3D leftKneePreviousFrame;
        public Point3D rightKneePreviousFrame;
        public Range leftFootXRange;
        public Range leftFootYRange;
        public Range leftFootZRange;
        public Range rightFootXRange;
        public Range rightFootYRange;
        public Range rightFootZRange;

        public Plie(Canvas canvas, Skeleton skeleton)
        {
            this.skeleton = skeleton;
            this.canvas = canvas;
            this.position = new Position(canvas, skeleton);

            Point3D leftKnee = new Point3D(skeleton.Joints[JointType.KneeLeft].Position.X,
                skeleton.Joints[JointType.KneeLeft].Position.Y,
                skeleton.Joints[JointType.KneeLeft].Position.Z);
            Point3D rightKnee = new Point3D(skeleton.Joints[JointType.KneeRight].Position.X,
                skeleton.Joints[JointType.KneeRight].Position.Y,
                skeleton.Joints[JointType.KneeRight].Position.Z);

            // Set initial "previous frame" to current frame position
            this.leftKneePreviousFrame = leftKnee;
            this.rightKneePreviousFrame = rightKnee;

            this.leftKneeTopRange = new Range(leftKnee.y, Range.kneeIntermediateRange);
            this.rightKneeTopRange = new Range(rightKnee.y, Range.kneeIntermediateRange);

            this.leftFootXRange = new Range(skeleton.Joints[JointType.FootLeft].Position.X, Range.footEasyRange);
            this.leftFootYRange = new Range(skeleton.Joints[JointType.FootLeft].Position.Y, Range.footEasyRange);
            this.leftFootZRange = new Range(skeleton.Joints[JointType.FootLeft].Position.Z, Range.footEasyRange);
            this.rightFootXRange = new Range(skeleton.Joints[JointType.FootRight].Position.X, Range.footEasyRange);
            this.rightFootYRange = new Range(skeleton.Joints[JointType.FootRight].Position.Y, Range.footEasyRange);
            this.rightFootZRange = new Range(skeleton.Joints[JointType.FootRight].Position.Z, Range.footEasyRange);

        }

        // This returns whether or not the user is still in the correct movement
        // sequence to complete a plie.
        // If the user deviates from the movements involved to complete a plie,
        // the method will return false.
        public Boolean trackPlie()
        {
            if (skeleton == null)
            {
                return false;
            }

            // Alignment checks
            this.position.checkAlignment();

            // Grab joints observed in a plie
            Joint leftKnee = skeleton.Joints[JointType.KneeLeft];
            Joint rightKnee = skeleton.Joints[JointType.KneeRight];
            Joint leftFoot = skeleton.Joints[JointType.FootLeft];
            Joint rightFoot = skeleton.Joints[JointType.FootRight];
            Joint leftAnkle = skeleton.Joints[JointType.AnkleLeft];
            Joint rightAnkle = skeleton.Joints[JointType.AnkleRight];

            Range leftKneeRange = new Range(this.leftKneePreviousFrame.y, Range.kneeEasyRange);
            Range rightKneeRange = new Range(this.rightKneePreviousFrame.y, Range.kneeEasyRange);

            // Folllow process of movement for a plie
            // Make sure feet are turned out and haven't moved
            if (this.position.leftFootTurnout() &&
                this.position.rightFootTurnout() &&
                this.position.leftFootStable() &&
                this.position.rightFootStable()
            )
            {
                // On our way down
                if (!this.plieBottomReached)
                {
                    {
                        if (!(leftKnee.Position.Y <= leftKneeRange.maximum ||
                            rightKnee.Position.Y <= rightKneeRange.maximum)
                        )
                        {
                            this.plieMovingUpFrames++;

                            if (this.plieMovingUpFrames >= 5)
                            {
                                this.plieBottomReached = true;
                            }
                        }

                        this.leftKneePreviousFrame = new Point3D(leftKnee.Position.X, leftKnee.Position.Y, leftKnee.Position.Z);
                        this.rightKneePreviousFrame = new Point3D(rightKnee.Position.X, rightKnee.Position.Y, rightKnee.Position.Z);
                        return true;
                    }
                }
                // On our way up
                else
                {
                    if (leftKnee.Position.Y >= leftKneeRange.minimum ||
                        rightKnee.Position.Y >= rightKneeRange.minimum
                    )
                    {
                        if (leftKnee.Position.Y >= this.leftKneeTopRange.minimum &&
                            rightKnee.Position.Y >= this.rightKneeTopRange.minimum)
                        {
                            this.gestureComplete = true;
                        }
                        this.leftKneePreviousFrame = new Point3D(leftKnee.Position.X, leftKnee.Position.Y, leftKnee.Position.Z);
                        this.rightKneePreviousFrame = new Point3D(rightKnee.Position.X, rightKnee.Position.Y, rightKnee.Position.Z);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
