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

            foreach (Joint joint in skeleton.Joints)
            {
                if (joint.TrackingState == JointTrackingState.Tracked)
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
            if (view == FRONT_VIEW)
            {
                for (int i = 0; i < topOfBodyDrawOrder.Length - 1; i++)
                {
                    if (topOfBodyDrawOrder[i].TrackingState == JointTrackingState.Tracked &&
                        topOfBodyDrawOrder[i + 1].TrackingState == JointTrackingState.Tracked)
                    {
                        DrawBoneFrontView(topOfBodyDrawOrder[i], topOfBodyDrawOrder[i + 1]);
                    }
                }
                for (int i = 0; i < bottomOfBodyDrawOrder.Length - 1; i++)
                {
                    if (bottomOfBodyDrawOrder[i].TrackingState == JointTrackingState.Tracked &&
                        bottomOfBodyDrawOrder[i + 1].TrackingState == JointTrackingState.Tracked)
                    {
                        DrawBoneFrontView(bottomOfBodyDrawOrder[i], bottomOfBodyDrawOrder[i + 1]);
                    }
                }
            }
            else if (view == SIDE_VIEW)
            {
                for (int i = 0; i < topOfBodyDrawOrder.Length - 1; i++)
                {
                    if (topOfBodyDrawOrder[i].TrackingState == JointTrackingState.Tracked &&
                        topOfBodyDrawOrder[i + 1].TrackingState == JointTrackingState.Tracked)
                    {
                        DrawBoneSideView(topOfBodyDrawOrder[i], topOfBodyDrawOrder[i + 1]);
                    }
                }
                for (int i = 0; i < bottomOfBodyDrawOrder.Length - 1; i++)
                {
                    if (bottomOfBodyDrawOrder[i].TrackingState == JointTrackingState.Tracked &&
                        bottomOfBodyDrawOrder[i + 1].TrackingState == JointTrackingState.Tracked)
                    {
                        DrawBoneSideView(bottomOfBodyDrawOrder[i], bottomOfBodyDrawOrder[i + 1]);
                    }
                }
            }
        }

        private void DrawJointFrontView(Joint joint)
        {
            if (joint.TrackingState == JointTrackingState.Tracked)
            {
                Ellipse follow = new Ellipse() {Height = 5, Width = 5, Fill = Brushes.BlueViolet};
                EllipseCanvas.Children.Add(follow);
                Canvas.SetTop(follow, joint.Position.Y * -250 + 247.5);
                Canvas.SetLeft(follow, joint.Position.X * 250 + 247.5);
            }
        }

        private void DrawJointSideView(Joint joint)
        {
            if (joint.TrackingState == JointTrackingState.Tracked)
            {
                Ellipse sideJoint = new Ellipse() {Height = 5, Width  = 5, Fill = Brushes.BlueViolet};
                EllipseCanvas.Children.Add(sideJoint);
                Canvas.SetTop(sideJoint, joint.Position.Y * -250 + 247.5);
                Canvas.SetLeft(sideJoint, joint.Position.Z * 200 + 147.5);
            }
        }

        private void DrawBoneFrontView(Joint start, Joint end)
        {
            if (start.TrackingState == JointTrackingState.Tracked && end.TrackingState == JointTrackingState.Tracked)
            {
                Point p1 = new Point(start.Position.X * 250 + 250, start.Position.Y * -250 + 250);
                Point p2 = new Point(end.Position.X * 250 + 250, end.Position.Y * -250 + 250);
                Line line = new Line() {X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = Brushes.MediumOrchid, StrokeThickness = 3};
                EllipseCanvas.Children.Add(line);
            }
        }

        private void DrawBoneSideView(Joint start, Joint end)
        {
            if (start.TrackingState == JointTrackingState.Tracked && end.TrackingState == JointTrackingState.Tracked)
            {
                Point p1 = new Point(start.Position.Z * 200 + 150, start.Position.Y * -250 + 250);
                Point p2 = new Point( end.Position.Z * 200 + 150, end.Position.Y * -250 + 250);
                Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = Brushes.CornflowerBlue, StrokeThickness = 3 };
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
