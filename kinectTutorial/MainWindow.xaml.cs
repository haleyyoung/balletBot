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

namespace kinectTutorial
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor kinect = null;
        Skeleton[] skeletonData;
        public MainWindow()
        {
            InitializeComponent();
            StartKinectST();
        }

        void StartKinectST()
        {
            kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected); // Get first Kinect Sensor
            kinect.SkeletonStream.Enable(); // Enable skeletal tracking

            skeletonData = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength]; // Allocate ST data

            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(SensorSkeletonFrameReady); // Get Ready for Skeleton Ready Events

            kinect.Start(); // Start Kinect sensor
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) // Open the Skeleton frame
            {
                if (skeletonFrame != null && this.skeletonData != null) // check that a frame is available
                {
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData); // get the skeletal information in this frame
                }
            }

            

            Skeleton skeleton = this.skeletonData.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
            Render(skeleton);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Render(Skeleton skeleton)
        {
            if (skeleton != null)
            {
                System.Console.WriteLine("skeleton is not null");
                // Obtain the left elbow joint; if tracked, print its position
                Joint leftElbow = skeleton.Joints[JointType.ElbowLeft];
                Joint rightElbow = skeleton.Joints[JointType.ElbowRight];
                Joint head = skeleton.Joints[JointType.Head];

                double rangeLow = leftElbow.Position.Y - 0.1;
                double rangeHigh = leftElbow.Position.Y + 0.1;

                if (head.TrackingState == JointTrackingState.Tracked)
                {
                   ///SetEllipsePosition(head);
                    System.Console.WriteLine(head.Position.X);
                    System.Console.WriteLine(head.Position.Y);
                    
                    EllipseCanvas.Children.Clear();

                    //Ellipse follow = new Ellipse() { Height = 100, Width = 100, Fill = Brushes.Red };
                    //EllipseCanvas.Children.Add(follow);
                    //Canvas.SetTop(follow, head.Position.Y * 250 + 250);
                    //Canvas.SetLeft(follow, head.Position.X * 250 + 250);
                    foreach (Joint joint in skeleton.Joints)
                    {
                       DrawJoint(joint);
                    }
                    DrawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
                    DrawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
                    DrawBone(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
                    DrawBone(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
                    DrawBone(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);
                    DrawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
                    DrawBone(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
                    DrawBone(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                    DrawBone(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);
                    DrawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);
                    DrawBone(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
                    DrawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
                    DrawBone(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
                    DrawBone(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
                    DrawBone(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);
                    DrawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
                    DrawBone(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
                    DrawBone(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
                    DrawBone(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);

                    if (rightElbow.Position.Y < leftElbow.Position.Y)
                    {
                        // Console.WriteLine("Keep your elbows at the same height");
                        //Console.WriteLine("Left elbow: " + leftElbow.Position.X +
                        //    ", " + leftElbow.Position.Y + ", " + leftElbow.Position.Z);
                    }
                }
            }
            else
            {
                System.Console.WriteLine("skeleton is null");
            }
        }

        private void DrawJoint(Joint joint)
        {
            if (joint.TrackingState == JointTrackingState.Tracked)
            {
                Ellipse follow = new Ellipse() {Height = 25, Width = 25, Fill = Brushes.MediumOrchid};
                EllipseCanvas.Children.Add(follow);
                Canvas.SetTop(follow, joint.Position.Y * 250 + 250);
                Canvas.SetLeft(follow, joint.Position.X * 250 + 250);
            }
        }

        private void DrawBone(Joint start, Joint end)
        {
            if (start.TrackingState == JointTrackingState.Tracked && end.TrackingState == JointTrackingState.Tracked)
            {
                Point p1 = new Point(start.Position.X * 250 + 250, start.Position.Y * 250 + 250);
                Point p2 = new Point(end.Position.X * 250 + 250, end.Position.Y * 250 + 250);
                Line line = new Line() {X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = Brushes.Red, StrokeThickness = 3};
                EllipseCanvas.Children.Add(line);
            }
        }
       
        /*private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Console.Out.Write("loaded");
            if (KinectSensor.KinectSensors.Count > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];
                _tracker = new Tracker(_sensor);

                Console.Read();

                if (_sensor.Status == KinectStatus.Connected)
                {
                    _sensor.ColorStream.Enable();
                    _sensor.DepthStream.Enable();
                    _sensor.SkeletonStream.Enable();
                    _sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(_sensor_AllFramesReady);
                    try
                    {
                        _sensor.Start();
                    }
                    catch (System.IO.IOException)
                    {
                        Message.Text = "Something is wrong, try unplugging and plugging your kinect back in.";
                        Message.Background = new SolidColorBrush(Colors.Azure);
                        Message.Foreground = new SolidColorBrush(Colors.BlanchedAlmond);
                    }
                }
                else if (_sensor.Status == KinectStatus.Disconnected)
                {
                    Message.Text = "Please connect your kinect sensor.";
                    Message.Background = new SolidColorBrush(Colors.Azure);
                    Message.Foreground = new SolidColorBrush(Colors.BlanchedAlmond);
                }
                else if (_sensor.Status == KinectStatus.NotPowered)
                {
                    Message.Text = "Please plug in your kinect to a power source.";
                    Message.Background = new SolidColorBrush(Colors.Azure);
                    Message.Foreground = new SolidColorBrush(Colors.BlanchedAlmond);
                }
            }
        }*/


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
        // this event fires wehn Color/Depth/Skeleton are syncronized
        void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }
                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                int stride = colorFrame.Width * 4;
                image1.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96,
                    96, PixelFormats.Bgr32, null, pixels, stride);
            }
        }

        void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                sensor.Stop();
                sensor.AudioSource.Stop();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs c)
        {
            //StopKinect(_sensor);
            Application.Current.Shutdown();
        }


        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            System.Windows.Point position = e.GetPosition(this);
            double pX = position.X;
            double pY = position.Y;


          // Canvas.SetLeft(headEllipse, pX);
           //Canvas.SetTop(headEllipse, pY);
        }


    }
}


    // @Author - John Elsbree (Microsoft Corporation)
    /*public class Tracker : MainWindow
    {
        private Skeleton[] skeletons = null;
        

        public Tracker()
        {
            kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected); // Get first Kinect Sensor
            kinect.SkeletonStream.Enable();
            // Connect the skeleton frame handler and enable skeleton tracking
            //sensor.SkeletonFrameReady += SensorSkeletonFrameReady;
            //sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(SensorSkeletonFrameReady);
            //sensor.SkeletonStream.Enable();

            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(SensorSkeletonFrameReady); // Get Ready for Skeleton Ready Events

            kinect.Start(); // Start Kinect sensor
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Access the skeleton frame
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if (this.skeletons == null)
                    {
                        // Allocate array of skeletons
                        this.skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    // Copy skeletons from this frame
                    skeletonFrame.CopySkeletonDataTo(this.skeletons);

                    // Find first tracked skeleton, if any
                    Skeleton skeleton = this.skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();

                    if (skeleton != null)
                    {
                        // Obtain the left elbow joint; if tracked, print its position
                        Joint leftElbow = skeleton.Joints[JointType.ElbowLeft];
                        Joint rightElbow = skeleton.Joints[JointType.ElbowRight];
                        Joint head = skeleton.Joints[JointType.Head];

                        double rangeLow = leftElbow.Position.Y - 0.1;
                        double rangeHigh = leftElbow.Position.Y + 0.1;

                        if (head.TrackingState == JointTrackingState.Tracked)
                        {
                            SetEllipsePosition(head);

                            if (rightElbow.Position.Y < leftElbow.Position.Y)
                            {
                               // Console.WriteLine("Keep your elbows at the same height");
                                //Console.WriteLine("Left elbow: " + leftElbow.Position.X +
                                //    ", " + leftElbow.Position.Y + ", " + leftElbow.Position.Z);
                            }
                        }
                    }
                }
            }
        }
        public void SetEllipsePosition(Joint joint)
        {


            Console.WriteLine("what are we looking at? " + headEllipse.Name + " left " + Canvas.GetLeft(headEllipse) + " top " + Canvas.GetTop(headEllipse));
            Console.WriteLine("x position " + (joint.Position.X * 250 + 250));

            Canvas.SetLeft(headEllipse, joint.Position.X * 250 + 250);
            Canvas.SetTop(headEllipse, joint.Position.Y * 250 + 250);
            EllipseCanvas.Children.Clear();
            Console.WriteLine("now we are we looking a? " + headEllipse.Name + " left " + Canvas.GetLeft(headEllipse) + " top " + Canvas.GetTop(headEllipse));

       /*     Dispatcher.Invoke((Action)(() =>
            {
                            System.Console.WriteLine("in dispatcher");
            Ellipse follow = new Ellipse() { Height = 100, Width = 100, Fill = Brushes.Red };


            double pX = joint.Position.X;
            double pY = joint.Position.Y;



            Canvas.SetLeft(follow, (pX * 100) + 50);
            Canvas.SetTop(follow, (pY * 100) + 50);

            EllipseCanvas.Children.Add(follow);
            EllipseCanvas.InvalidateVisual();
                
            }));*/
        //}
    // End @Author - John Elsbree
    //}
//}
