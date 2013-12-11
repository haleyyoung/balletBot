using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Kinect;

namespace kinectTutorial
{
    class Position
    {
        public Boolean gestureComplete = false;
        public int positionBufferFrames = 0;
        public int positionCompleteBannerFrames = 0;
        public Skeleton skeleton = null;
        public Canvas canvas;
        public TextBlock correction;
        public Range leftFootXRange;
        public Range leftFootYRange;
        public Range leftFootZRange;
        public Range rightFootXRange;
        public Range rightFootYRange;
        public Range rightFootZRange;

        public Position(Canvas canvas, Skeleton skeleton)
        {
            this.skeleton = skeleton;
            this.canvas = canvas;

            this.leftFootXRange = new Range(skeleton.Joints[JointType.FootLeft].Position.X, Range.footEasyRange);
            this.leftFootYRange = new Range(skeleton.Joints[JointType.FootLeft].Position.Y, Range.footEasyRange);
            this.leftFootZRange = new Range(skeleton.Joints[JointType.FootLeft].Position.Z, Range.footEasyRange);
            this.rightFootXRange = new Range(skeleton.Joints[JointType.FootRight].Position.X, Range.footEasyRange);
            this.rightFootYRange = new Range(skeleton.Joints[JointType.FootRight].Position.Y, Range.footEasyRange);
            this.rightFootZRange = new Range(skeleton.Joints[JointType.FootRight].Position.Z, Range.footEasyRange);
            this.correction = new TextBlock();
            correction.Background = Brushes.Orange;
            correction.FontSize = 40;
            Canvas.SetTop(correction, 200);
        }

        public Boolean firstPosition()
        {
            if (skeleton == null)
            {
                return false;
            }

            checkAlignment();

            Joint rightAnkle = this.skeleton.Joints[JointType.AnkleRight];
            Joint leftAnkle = this.skeleton.Joints[JointType.AnkleLeft];
            Joint rightHip = this.skeleton.Joints[JointType.HipRight];
            Joint leftHip = this.skeleton.Joints[JointType.HipLeft];

            // Make sure feet are equally positioned on the floor (one foot isn't in front of the other)
            Range rightAnkleYComparison = new Range(rightAnkle.Position.Y, Range.ankleEasyRange);
            Range rightAnkleZComparison = new Range(rightAnkle.Position.Z, Range.ankleEasyRange);
            // Make sure feet are not wider than the hips
            Range rightHipXComparison = new Range(rightHip.Position.X, Range.hipEasyRange);
            Range leftHipXComparison = new Range(leftHip.Position.X, Range.hipEasyRange);

            return (rightAnkleYComparison.inRange(leftAnkle.Position.Y) &&
                    rightAnkleZComparison.inRange(leftAnkle.Position.Z) &&
                    rightAnkle.Position.X <= rightHipXComparison.maximum &&
                    leftAnkle.Position.X >= leftHipXComparison.minimum &&
                    rightFootStable() &&
                    leftFootStable() &&
                    rightFootTurnout() &&
                    leftFootTurnout()
            );
        }

        // The other 3 positions have yet to be implemented

        public void checkAlignment()
        {
            MovementMode position = new MovementMode(this.canvas, this.skeleton);
            position.hipAlignmentYAxis();
            position.hipAlignmentZAxis();
            position.spineAlignmentYAxis();
            position.shoulderAlignmentYAxis();
            position.shoulderAlignmentZAxis();
        }

        public Boolean leftFootStable()
        {
            Joint leftFoot = this.skeleton.Joints[JointType.FootLeft];

            return (this.leftFootXRange.minimum <= leftFoot.Position.X &&
                this.leftFootXRange.maximum >= leftFoot.Position.X &&
                this.leftFootYRange.minimum <= leftFoot.Position.Y &&
                this.leftFootYRange.maximum >= leftFoot.Position.Y &&
                this.leftFootZRange.minimum <= leftFoot.Position.Z &&
                this.leftFootZRange.maximum >= leftFoot.Position.Z
            );
        }

        public Boolean rightFootStable()
        {
            Joint rightFoot = this.skeleton.Joints[JointType.FootRight];

            return (this.rightFootXRange.minimum <= rightFoot.Position.X &&
                this.rightFootXRange.maximum >= rightFoot.Position.X &&
                this.rightFootYRange.minimum <= rightFoot.Position.Y &&
                this.rightFootYRange.maximum >= rightFoot.Position.Y &&
                this.rightFootZRange.minimum <= rightFoot.Position.Z &&
                this.rightFootZRange.maximum >= rightFoot.Position.Z
            );
        }

        public Boolean leftFootTurnout()
        {
            Joint leftFoot = this.skeleton.Joints[JointType.FootLeft];
            Joint leftAnkle = this.skeleton.Joints[JointType.AnkleLeft];

            return (leftFoot.Position.X <= leftAnkle.Position.X);
        }

        public Boolean rightFootTurnout()
        {
            Joint rightFoot = this.skeleton.Joints[JointType.FootRight];
            Joint rightAnkle = this.skeleton.Joints[JointType.AnkleRight];

            return (rightFoot.Position.X >= rightAnkle.Position.X);
        }

        public Boolean showSuccessBanner(String imageName)
        {
            Image imageToShow = (Image)this.canvas.FindName(imageName);

            this.positionCompleteBannerFrames++;

            if (this.positionCompleteBannerFrames < 120)
            {
                imageToShow.Width = this.canvas.ActualWidth;
                imageToShow.Height = this.canvas.ActualHeight;
                imageToShow.Visibility = Visibility.Visible;
            }
            else
            {
                imageToShow.Visibility = Visibility.Hidden;
                return true;
            }
            return false;
        }
    }
}
