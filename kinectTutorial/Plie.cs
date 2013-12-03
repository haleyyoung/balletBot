﻿using System;
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
        public Boolean plieBottomReached = false;
        public int plieMovingUpFrames = 0;
        public int plieCompleteBannerFrames = 0;
        public Skeleton skeleton = null;
        public Canvas canvas;
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
            Point3D leftKnee = new Point3D(skeleton.Joints[JointType.KneeLeft].Position.X,
                skeleton.Joints[JointType.KneeLeft].Position.Y,
                skeleton.Joints[JointType.KneeLeft].Position.Z);
            Point3D rightKnee = new Point3D(skeleton.Joints[JointType.KneeRight].Position.X,
                skeleton.Joints[JointType.KneeRight].Position.Y,
                skeleton.Joints[JointType.KneeRight].Position.Z);

            // Set initial "previous frame" to current frame position
            this.leftKneePreviousFrame = leftKnee;
            this.rightKneePreviousFrame = rightKnee;

            this.leftKneeTopRange = new Range(leftKnee.y, Range.hipIntermediateRange);
            this.rightKneeTopRange = new Range(rightKnee.y, Range.hipIntermediateRange);

            this.leftFootXRange = new Range(skeleton.Joints[JointType.FootLeft].Position.X, Range.hipEasyRange);
            this.leftFootYRange = new Range(skeleton.Joints[JointType.FootLeft].Position.Y, Range.hipEasyRange);
            this.leftFootZRange = new Range(skeleton.Joints[JointType.FootLeft].Position.Z, Range.hipEasyRange);
            this.rightFootXRange = new Range(skeleton.Joints[JointType.FootRight].Position.X, Range.hipEasyRange);
            this.rightFootYRange = new Range(skeleton.Joints[JointType.FootRight].Position.Y, Range.hipEasyRange);
            this.rightFootZRange = new Range(skeleton.Joints[JointType.FootRight].Position.Z, Range.hipEasyRange);

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

            Range leftKneeRange = new Range(this.leftKneePreviousFrame.y, Range.hipHardRange);
            Range rightKneeRange = new Range(this.rightKneePreviousFrame.y, Range.hipHardRange);

            // Folllow process of movement for a plie
            // Make sure feet are turned out and haven't moved
            if (leftFootTurnout() &&
                rightFootTurnout() &&
                leftFootStable() &&
                rightFootStable()
            )
            {
                // On our way down
                if (!this.plieBottomReached)
                {
                    {
                        if (leftKnee.Position.Y <= leftKneeRange.maximum ||
                            rightKnee.Position.Y <= rightKneeRange.maximum
                        )
                        {
                            //suggestions.Text = "tracking plie " + (leftKnee.Position.Y == leftKneePreviousFrame.y);
                        }
                        else
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

            if (leftFoot.Position.X <= leftAnkle.Position.X)
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

            if (rightFoot.Position.X >= rightAnkle.Position.X)
            {
                blah.Text = "turned out";
                return true;
            }
            blah.Text = "right foot NOT turned out";
            return false;
        }

        public Boolean showSuccessBanner()
        {
            Image imageToShow = (Image)this.canvas.FindName("moveCompletedImage");
            TextBlock textToShow = (TextBlock)this.canvas.FindName("plieCompletedText");

            this.plieCompleteBannerFrames++;

            if (this.plieCompleteBannerFrames < 120)
            {
                imageToShow.Visibility = Visibility.Visible;
                textToShow.Visibility = Visibility.Visible;
            }
            else
            {   
                imageToShow.Visibility = Visibility.Hidden;
                textToShow.Visibility = Visibility.Hidden;
                return true;
            }
            return false;
        }
    }
}
