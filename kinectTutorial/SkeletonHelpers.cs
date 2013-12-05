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
    public partial class MainWindow: Window
    {
        private void DrawSkeleton(Skeleton skeleton, int view)
        {
            Joint[] topOfBodyDrawOrder = {
              skeleton.Joints[JointType.HandLeft],
              skeleton.Joints[JointType.WristLeft],
              skeleton.Joints[JointType.ElbowLeft],
              skeleton.Joints[JointType.ShoulderLeft],
              skeleton.Joints[JointType.ShoulderCenter],
              skeleton.Joints[JointType.Head],
              skeleton.Joints[JointType.ShoulderCenter],
              skeleton.Joints[JointType.ShoulderRight],
              skeleton.Joints[JointType.ElbowRight],
              skeleton.Joints[JointType.WristRight],
              skeleton.Joints[JointType.HandRight]
            };

            Joint[] bottomOfBodyDrawOrder = {
              skeleton.Joints[JointType.FootLeft],
              skeleton.Joints[JointType.AnkleLeft],
              skeleton.Joints[JointType.KneeLeft],
              skeleton.Joints[JointType.HipLeft],
              skeleton.Joints[JointType.HipCenter],
              skeleton.Joints[JointType.Spine],
              skeleton.Joints[JointType.ShoulderCenter],
              skeleton.Joints[JointType.Spine],
              skeleton.Joints[JointType.HipCenter],
              skeleton.Joints[JointType.HipRight],
              skeleton.Joints[JointType.KneeRight],
              skeleton.Joints[JointType.AnkleRight],
              skeleton.Joints[JointType.FootRight]
            };

            // Draw the joints
            foreach (Joint joint in skeleton.Joints)
            {
                if (joint.TrackingState == JointTrackingState.Tracked || joint.TrackingState == JointTrackingState.Inferred)
                {
                    if (view == FRONT_VIEW)
                    {
                        DrawJointFrontView(joint);
                    }
                    else if (view == SIDE_VIEW)
                    {
                        DrawJointSideView(joint);
                    }
                }
            }

            // Draw the bones
            for (int i = 0; i < topOfBodyDrawOrder.Length - 1; i++)
            {
                if ((topOfBodyDrawOrder[i].TrackingState == JointTrackingState.Tracked ||
                        topOfBodyDrawOrder[i].TrackingState == JointTrackingState.Inferred) &&
                    (topOfBodyDrawOrder[i + 1].TrackingState == JointTrackingState.Tracked ||
                        topOfBodyDrawOrder[i + 1].TrackingState == JointTrackingState.Inferred))
                {
                    if (view == FRONT_VIEW)
                    {
                        DrawBoneFrontView(topOfBodyDrawOrder[i], topOfBodyDrawOrder[i + 1]);
                    }
                    else if (view == SIDE_VIEW)
                    {
                        DrawBoneSideView(topOfBodyDrawOrder[i], topOfBodyDrawOrder[i + 1]);
                    }
                }
            }
            for (int i = 0; i < bottomOfBodyDrawOrder.Length - 1; i++)
            {
                if ((bottomOfBodyDrawOrder[i].TrackingState == JointTrackingState.Tracked ||
                        bottomOfBodyDrawOrder[i].TrackingState == JointTrackingState.Inferred) &&
                    (bottomOfBodyDrawOrder[i + 1].TrackingState == JointTrackingState.Tracked ||
                        bottomOfBodyDrawOrder[i + 1].TrackingState == JointTrackingState.Inferred))
                {
                    if (view == FRONT_VIEW)
                    {
                        DrawBoneFrontView(bottomOfBodyDrawOrder[i], bottomOfBodyDrawOrder[i + 1]);
                    }
                    else if (view == SIDE_VIEW)
                    {
                        DrawBoneSideView(bottomOfBodyDrawOrder[i], bottomOfBodyDrawOrder[i + 1]);
                    }
                }
            }
        }

        private void DrawJointFrontView(Joint joint)
        {
            double windowWidth = Canvas.ActualWidth;
            double windowHeight = Canvas.ActualHeight;
            if (joint.TrackingState == JointTrackingState.Tracked || joint.TrackingState == JointTrackingState.Inferred)
            {
                Ellipse follow = new Ellipse() {Height = 10, Width = 10, Fill = Brushes.BlueViolet};
                EllipseCanvas.Children.Add(follow);
                Canvas.SetTop(follow, (windowHeight/1.25 - 5) + joint.Position.Y * -(windowHeight/2));
                Canvas.SetLeft(follow, joint.Position.X * (windowWidth/2) + (windowWidth/2 - 5));
            }
        }

        private void DrawJointSideView(Joint joint)
        {
            double windowWidth = Canvas.ActualWidth;
            double windowHeight = Canvas.ActualHeight;
            if (joint.TrackingState == JointTrackingState.Tracked || joint.TrackingState == JointTrackingState.Inferred)
            {
                Ellipse sideJoint = new Ellipse() {Height = 10, Width  = 10, Fill = Brushes.BlueViolet};
                EllipseCanvas.Children.Add(sideJoint);
                Canvas.SetTop(sideJoint, joint.Position.Y * -(windowHeight/2) + (windowHeight/1.25 - 5));
                Canvas.SetLeft(sideJoint, joint.Position.Z * (windowWidth/4) - 5);
            }
        }

        private void DrawBoneFrontView(Joint start, Joint end)
        {
            double windowWidth = Canvas.ActualWidth;
            double windowHeight = Canvas.ActualHeight;
            if ((start.TrackingState == JointTrackingState.Tracked ||
                 start.TrackingState == JointTrackingState.Inferred) &&
                (end.TrackingState == JointTrackingState.Tracked ||
                 end.TrackingState == JointTrackingState.Inferred))
            {
                Point p1 = new Point(start.Position.X * (windowWidth / 2) + (windowWidth / 2), (windowHeight/1.25) + start.Position.Y * -(windowHeight / 2));
                Point p2 = new Point(end.Position.X * (windowWidth / 2) + (windowWidth / 2), (windowHeight/1.25) + end.Position.Y * -(windowHeight / 2));
                Line line = new Line() {X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = Brushes.MediumOrchid, StrokeThickness = 7};
                EllipseCanvas.Children.Add(line);
            }
        }

        private void DrawBoneSideView(Joint start, Joint end)
        {
            double windowWidth = Canvas.ActualWidth;
            double windowHeight = Canvas.ActualHeight;
            if ((start.TrackingState == JointTrackingState.Tracked ||
                 start.TrackingState == JointTrackingState.Inferred) &&
                (end.TrackingState == JointTrackingState.Tracked ||
                 end.TrackingState == JointTrackingState.Inferred))
            {
                Point p1 = new Point(start.Position.Z * (windowWidth/4), start.Position.Y * -(windowHeight/2) + (windowHeight/1.25));
                Point p2 = new Point(end.Position.Z * (windowWidth/4), end.Position.Y * -(windowHeight/2) + (windowHeight/1.25));
                Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = Brushes.CornflowerBlue, StrokeThickness = 7};
                EllipseCanvas.Children.Add(line);
            }
        }

        private void TrackClosestSkeleton()
        {
            if (this.kinect != null && this.kinect.SkeletonStream != null)
            {
                if (!this.kinect.SkeletonStream.AppChoosesSkeletons)
                {
                    this.kinect.SkeletonStream.AppChoosesSkeletons = true; // Ensure AppChoosesSkeletons is set
                }

                float closestDistance = 10000f; // Start with a far enough distance
                int closestID = 0;

                foreach (Skeleton skeleton in this.skeletonData.Where(s => s.TrackingState != SkeletonTrackingState.NotTracked))
                {
                    if (skeleton.Position.Z < closestDistance)
                    {
                        closestID = skeleton.TrackingId;
                        closestDistance = skeleton.Position.Z;
                    }
                }

                if (closestID > 0)
                {
                    this.kinect.SkeletonStream.ChooseSkeletons(closestID); // Track this skeleton
                }
            }
        }
    }
}
