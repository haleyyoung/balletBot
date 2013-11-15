using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using kinectTutorial;

namespace kinectTutorial
{
    /// <summary>
    /// The Plie class handles the movement gesture of a ballet move called
    /// a plie. This will call the correct methods in MovementMode to check
    /// the user's alignment.
    /// </summary>
    class Plie
    {
        public Boolean gestureComplete = false;
        public Skeleton skeleton = null;
        public Canvas canvas;
        public Joint leftKneeTop;
        public Joint rightKneeTop;
        public Joint leftAnkleTop;
        public Joint rightAnkleTop;
        public Joint leftFootTop;
        public Joint rightFootTop;
        public Joint leftKneeBottom;
        public Joint rightKneeBottom;
        public Joint leftKneePreviousFrame;
        public Joint rightKneePreviousFrame;
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
            MovementMode plie = new MovementMode(this.canvas, this.skeleton);
            plie.hipAlignmentYAxis();
            plie.hipAlignmentZAxis();
            plie.spineAlignmentYAxis();
            plie.shoulderAlignmentYAxis();
            plie.shoulderAlignmentZAxis();

            // Grab joints observed in a plie
            Joint leftKnee = skeleton.Joints[JointType.KneeLeft];
            Joint rightKnee = skeleton.Joints[JointType.KneeRight];
            Joint leftFoot = skeleton.Joints[JointType.FootLeft];
            Joint rightFoot = skeleton.Joints[JointType.FootRight];
            Joint leftAnkle = skeleton.Joints[JointType.AnkleLeft];
            Joint rightAnkle = skeleton.Joints[JointType.AnkleRight];
            Range leftKneeRange = new Range(leftKneePreviousFrame.Position.Y, Range.hipIntermediateRange);
            Range rightKneeRange = new Range(rightKneePreviousFrame.Position.Y, Range.hipIntermediateRange);

            if (this.leftKneeTop == null)
            {
                this.leftKneeTop = leftKnee;
            }
            if (this.rightKneeTop == null)
            {
                this.rightKneeTop = leftKnee;
            }
            if (this.leftFootTop == null)
            {
                this.leftFootTop = leftKnee;
            }
            if (this.rightFootTop == null)
            {
                this.rightFootTop = leftKnee;
            }
            if (this.leftAnkleTop == null)
            {
                this.leftAnkleTop = leftKnee;
            }
            if (this.rightAnkleTop == null)
            {
                this.rightAnkleTop = leftKnee;
            }
            if (this.leftFootXRange == null)
            {
                this.leftFootXRange = new Range(leftFoot.Position.X, Range.hipIntermediateRange);
            }
            if (this.leftFootYRange == null)
            {
                this.leftFootYRange = new Range(leftFoot.Position.Y, Range.hipIntermediateRange);
            }
            if (this.leftFootZRange == null)
            {
                this.leftFootZRange = new Range(leftFoot.Position.Z, Range.hipIntermediateRange);
            }
            if (this.rightFootXRange == null)
            {
                this.rightFootXRange = new Range(rightFoot.Position.X, Range.hipIntermediateRange);
            }
            if (this.rightFootYRange == null)
            {
                this.rightFootYRange = new Range(rightFoot.Position.Y, Range.hipIntermediateRange);
            }
            if (this.rightFootZRange == null)
            {
                this.rightFootZRange = new Range(rightFoot.Position.Z, Range.hipIntermediateRange);
            }

            // Folllow process of movement for a plie

            // On our way down
            if (this.leftKneeBottom.TrackingState == JointTrackingState.NotTracked ||
                this.rightKneeBottom.TrackingState == JointTrackingState.NotTracked)
            {
                TextBlock blah = new TextBlock();
                if (leftKnee.Position.Y <= leftKneeRange.maximum &&
                    rightKnee.Position.Y <= rightKneeRange.maximum &&
                    // Make sure feet are turned out
                    leftFootTurnout() &&
                    rightFootTurnout() &&
                    // Check to make sure feet haven't moved
                    leftFootStable() &&
                    rightFootStable()
                )
                {
                    blah.Text = "tracking plie " + leftKnee.Position.Y + " " + leftKneeRange.maximum + " " + this.leftKneeBottom.TrackingState;
                    blah.Background = Brushes.Red;
                    canvas.Children.Add(blah);
                    Canvas.SetTop(blah, 100);
                    return true;
                }
                blah.Text = "setting bottom";
                blah.Background = Brushes.Pink;
                canvas.Children.Add(blah);
                Canvas.SetTop(blah, 700);
                this.leftKneeBottom = leftKnee;
                this.rightKneeBottom = rightKnee;
                return true;
            }
            // On our way up
            else
            {
                TextBlock blah = new TextBlock();
                if (leftKnee.Position.Y >= leftKneeRange.minimum &&
                    rightKnee.Position.Y >= rightKneeRange.minimum &&
                    // Make sure feet are turned out
                    leftFootTurnout() &&
                    rightFootTurnout() &&
                    // Check to make sure feet haven't moved
                    leftFootStable() &&
                    rightFootStable()
                   )
                {
                    blah.Text = "on our way up";
                    blah.Background = Brushes.Red;
                    canvas.Children.Add(blah);
                    Canvas.SetTop(blah, 600);
                    return true;
                }
                blah.Text = "returning false";
                blah.Background = Brushes.Red;
                canvas.Children.Add(blah);
                Canvas.SetTop(blah, 100);
                return true;
                return false;
            }
        }

        public Boolean leftFootStable()
        {
            TextBlock blah = new TextBlock();
            blah.Background = Brushes.Orange;
            canvas.Children.Add(blah);
            Canvas.SetTop(blah, 200);
            Joint leftFoot = this.skeleton.Joints[JointType.FootLeft];

            if (this.leftFootXRange.minimum <= leftFoot.Position.X &&
                this.leftFootXRange.maximum >= leftFoot.Position.X &&
                this.leftFootYRange.minimum <= leftFoot.Position.Y &&
                this.leftFootYRange.maximum >= leftFoot.Position.Y &&
                this.leftFootZRange.minimum <= leftFoot.Position.Z &&
                this.leftFootZRange.maximum >= leftFoot.Position.Z
            )
            {
                blah.Text = "stable";
                return true;
            }
            blah.Text = "left foot NOT stable";
            return false;
        }

        public Boolean rightFootStable()
        {
            TextBlock blah = new TextBlock();
            blah.Background = Brushes.Orange;
            canvas.Children.Add(blah);
            Canvas.SetTop(blah, 300);
            Joint rightFoot = this.skeleton.Joints[JointType.FootRight];

            if (this.rightFootXRange.minimum <= rightFoot.Position.X &&
                this.rightFootXRange.maximum >= rightFoot.Position.X &&
                this.rightFootYRange.minimum <= rightFoot.Position.Y &&
                this.rightFootYRange.maximum >= rightFoot.Position.Y &&
                this.rightFootZRange.minimum <= rightFoot.Position.Z &&
                this.rightFootZRange.maximum >= rightFoot.Position.Z
            )
            {
                blah.Text = "stable";
                return true;
            }
            blah.Text = "right foot NOT stable";
            return false;
        }

        public Boolean leftFootTurnout()
        {
            TextBlock blah = new TextBlock();
            blah.Background = Brushes.Orange;
            canvas.Children.Add(blah);
            Canvas.SetTop(blah, 400);
            Joint leftFoot = this.skeleton.Joints[JointType.FootLeft];
            Joint leftAnkle = this.skeleton.Joints[JointType.AnkleLeft];

            if (leftFoot.Position.X >= leftAnkle.Position.X)
            {
                blah.Text = "turned out";
                return true;
            }
            blah.Text = "left foot NOT turned out";
            return false;
        }

        public Boolean rightFootTurnout()
        {
            TextBlock blah = new TextBlock();
            blah.Background = Brushes.Orange;
            canvas.Children.Add(blah);
            Canvas.SetTop(blah, 500);
            Joint rightFoot = this.skeleton.Joints[JointType.FootRight];
            Joint rightAnkle = this.skeleton.Joints[JointType.AnkleRight];

            if (rightFoot.Position.X <= rightAnkle.Position.X)
            {
                blah.Text = "turned out";
                return true;
            }
            blah.Text = "right foot NOT turned out";
            return false;
        }
    }
}
