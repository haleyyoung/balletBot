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
    /// Interaction logic for MainWindow.xaml
    /// Most of the code to get the kinect set up and tracking a skeleton came
    /// from http://channel9.msdn.com/Series/KinectQuickstart/
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor kinect = null;
        Skeleton[] skeletonData;
        Skeleton skeleton;
        Gesture pliesGesture;
        Gesture firstPositionGesture;
        Plie plie;
        Position position;
        public Boolean pliesMode = false;
        public Boolean firstPositionMode = false;
        private const int FRONT_VIEW = 0;
        private const int SIDE_VIEW = 1;

        public MainWindow()
        {
            InitializeComponent();
            StartKinectST();
        }

        void StartKinectST()
        {
            kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected); // Get first Kinect Sensor

            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.1f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
            };

            kinect.SkeletonStream.Enable(parameters); // Enable skeletal tracking

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
            try
            {
                skeleton = this.skeletonData.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
            }
            catch
            {
                Application.Current.Shutdown();
            }
            Render();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Render()
        {
            if (skeleton != null)
            {
                EllipseCanvas.Children.Clear();
                EllipseCanvas.Height = Canvas.ActualHeight;
                EllipseCanvas.Width = Canvas.ActualWidth;

                if (this.pliesGesture == null)
                {
                    this.pliesGesture = new Gesture(EllipseCanvas, skeleton, (Rectangle)Canvas.FindName("pliesButton"), this.pliesMode);
                }
                if (this.firstPositionGesture == null)
                {
                    this.firstPositionGesture = new Gesture(EllipseCanvas, skeleton, (Rectangle)Canvas.FindName("firstPositionButton"), this.firstPositionMode);
                }
                this.pliesMode = handleGesture(this.pliesGesture, "pliesButton", this.pliesMode);
                this.firstPositionMode = handleGesture(this.firstPositionGesture, "firstPositionButton", this.firstPositionMode);

                DrawSkeleton(skeleton, FRONT_VIEW);
                DrawSkeleton(skeleton, SIDE_VIEW);

                if (pliesMode)
                {
                    if (this.plie == null)
                    {
                        this.plie = new Plie(EllipseCanvas, skeleton);
                    }
                    if (this.plie.gestureComplete && this.plie.showSuccessBanner() ||
                        !this.plie.gestureComplete && !this.plie.trackPlie())
                    {
                        this.plie = null;
                    }
                }

                if (firstPositionMode)
                {
                    if (this.position == null)
                    {
                        this.position = new Position(EllipseCanvas, skeleton);
                    }
                    if (this.position.gestureComplete && this.position.showSuccessBanner() ||
                        !this.position.firstPosition())
                    {
                        this.position = null;
                    }
                }
            }
            else
            {
                System.Console.WriteLine("skeleton is null");
            }
        }

        // this event fires when Color/Depth/Skeleton are syncronized
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

        Boolean handleGesture(Gesture gesture, String modeButtonName, Boolean buttonOn)
        {
            if (gesture.gestureStart())
            {
                buttonOn = !buttonOn;
                turnOffOtherButtons(modeButtonName, buttonOn);
            }

            // Show demo button if we turned a mode on
            Rectangle demoButton = (Rectangle)Canvas.FindName("demoButton");
            demoButton.Visibility = buttonOn ?
                Visibility.Visible : Visibility.Hidden;
            TextBlock demoButtonText = (TextBlock)Canvas.FindName("demoButtonLabel");
            demoButtonText.Visibility = buttonOn ?
                Visibility.Visible : Visibility.Hidden;
            return buttonOn;
        }

        // This is used to make sure only one button is active at a time
        void turnOffOtherButtons(String modeButtonName, Boolean buttonState)
        {
            Rectangle modeButtonHandle = (Rectangle)this.Canvas.FindName(modeButtonName);
            if (!Object.Equals(modeButtonName, "pliesButton"))
            {
                this.pliesMode = false;
                this.pliesGesture.turnOffButton();
            }
            if (!Object.Equals(modeButtonName, "firstPositionButton"))
            {
                this.firstPositionMode = false;
                this.firstPositionGesture.turnOffButton();
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
            Application.Current.Shutdown();
        }
    }
}