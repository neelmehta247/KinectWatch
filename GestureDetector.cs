//------------------------------------------------------------------------------
// <copyright file="GestureDetector.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.DiscreteGestureBasics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Kinect;
    using Microsoft.Kinect.VisualGestureBuilder;
    using WindowsInput;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Runtime.InteropServices;    /// <summary>
                                             /// Gesture Detector class which listens for VisualGestureBuilderFrame events from the service
                                             /// and updates the associated GestureResultView object with the latest results for the 'Seated' gesture
                                             /// </summary>
    public class GestureDetector : IDisposable
    {
        /// <summary> Path to the gesture database that was trained with VGB </summary>
        private readonly string gestureDatabase = @"Database\Seated.gbd";

        /// <summary> Name of the discrete gesture in the database that we want to track </summary>
        private readonly string leanr = "leanr";
        private readonly string leanl = "leanl";
        private readonly string leanf = "leanf";
        private readonly string shoot = "shoot";
        private readonly string reload = "reload";
        private readonly string abilityl = "abilityl";
        private readonly string abilityr = "abilityr";
        private readonly string ultimate = "ultimate";

        /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        /// <summary>
        /// Initializes a new instance of the GestureDetector class along with the gesture frame source and reader
        /// </summary>
        /// <param name="kinectSensor">Active sensor to initialize the VisualGestureBuilderFrameSource object with</param>
        /// <param name="gestureResultView">GestureResultView object to store gesture results of a single body to</param>
        public GestureDetector(KinectSensor kinectSensor, GestureResultView gestureResultView)
        {
            if (kinectSensor == null)
            {
                throw new ArgumentNullException("kinectSensor");
            }

            if (gestureResultView == null)
            {
                throw new ArgumentNullException("gestureResultView");
            }
            
            this.GestureResultView = gestureResultView;
            
            // create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
            this.vgbFrameSource = new VisualGestureBuilderFrameSource(kinectSensor, 0);
            this.vgbFrameSource.TrackingIdLost += this.Source_TrackingIdLost;

            // open the reader for the vgb frames
            this.vgbFrameReader = this.vgbFrameSource.OpenReader();
            if (this.vgbFrameReader != null)
            {
                this.vgbFrameReader.IsPaused = true;
                this.vgbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
            }

            // load the 'Seated' gesture from the gesture database
            using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(this.gestureDatabase))
            {
                // we could load all available gestures in the database with a call to vgbFrameSource.AddGestures(database.AvailableGestures), 
                // but for this program, we only want to track one discrete gesture from the database, so we'll load it by name
                foreach (Gesture gesture in database.AvailableGestures)
                {
                    if (gesture.Name.Equals(this.leanr))
                    {
                        this.vgbFrameSource.AddGesture(gesture);
                    }
                    if (gesture.Name.Equals(this.leanl))
                    {
                        this.vgbFrameSource.AddGesture(gesture);
                    }
                    if (gesture.Name.Equals(this.leanf))
                    {
                        this.vgbFrameSource.AddGesture(gesture);
                    }
                    if (gesture.Name.Equals(this.shoot))
                    {
                        this.vgbFrameSource.AddGesture(gesture);
                    }
                    if (gesture.Name.Equals(this.reload))
                    {
                        this.vgbFrameSource.AddGesture(gesture);
                    }
                    if (gesture.Name.Equals(this.abilityl))
                    {
                        this.vgbFrameSource.AddGesture(gesture);
                    }
                    if (gesture.Name.Equals(this.abilityr))
                    {
                        this.vgbFrameSource.AddGesture(gesture);
                    }
                    if (gesture.Name.Equals(this.ultimate))
                    {
                        this.vgbFrameSource.AddGesture(gesture);
                    }
                }
            }
        }

        /// <summary> Gets the GestureResultView object which stores the detector results for display in the UI </summary>
        public GestureResultView GestureResultView { get; private set; }

        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public ulong TrackingId
        {
            get
            {
                return this.vgbFrameSource.TrackingId;
            }

            set
            {
                if (this.vgbFrameSource.TrackingId != value)
                {
                    this.vgbFrameSource.TrackingId = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the detector is currently paused
        /// If the body tracking ID associated with the detector is not valid, then the detector should be paused
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return this.vgbFrameReader.IsPaused;
            }

            set
            {
                if (this.vgbFrameReader.IsPaused != value)
                {
                    this.vgbFrameReader.IsPaused = value;
                }
            }
        }

        /// <summary>
        /// Disposes all unmanaged resources for the class
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader objects
        /// </summary>
        /// <param name="disposing">True if Dispose was called directly, false if the GC handles the disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.vgbFrameReader != null)
                {
                    this.vgbFrameReader.FrameArrived -= this.Reader_GestureFrameArrived;
                    this.vgbFrameReader.Dispose();
                    this.vgbFrameReader = null;
                }

                if (this.vgbFrameSource != null)
                {
                    this.vgbFrameSource.TrackingIdLost -= this.Source_TrackingIdLost;
                    this.vgbFrameSource.Dispose();
                    this.vgbFrameSource = null;
                }
            }
        }

        // P/Invoke function for controlling the mouse
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        /// <summary>
        /// structure for mouse data
        /// </summary>
        struct MouseInput
        {
            public int X; // X coordinate
            public int Y; // Y coordinate
            public uint MouseData; // mouse data, e.g. for mouse wheel
            public uint DwFlags; // further mouse data, e.g. for mouse buttons
            public uint Time; // time of the event
            public IntPtr DwExtraInfo; // further information
        }

        /// <summary>
        /// super structure for input data of the function SendInput
        /// </summary>
        struct Input
        {
            public int Type; // type of the input, 0 for mouse  
            public MouseInput Data; // mouse data
        }

        // constants for mouse flags
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002; // press left mouse button
        const uint MOUSEEVENTF_LEFTUP = 0x0004; // release left mouse button
        const uint MOUSEEVENTF_ABSOLUTE = 0x8000; // whole screen, not just application window
        const uint MOUSEEVENTF_MOVE = 0x0001; // move mouse

        private MouseInput CreateMouseInput(int x, int y, uint data, uint time, uint flag)
        {
            // create from the given data an object of the type MouseInput, which then can be send
            MouseInput Result = new MouseInput();
            Result.X = x;
            Result.Y = y;
            Result.MouseData = data;
            Result.Time = time;
            Result.DwFlags = flag;
            return Result;
        }

        private void SimulateMouseMove(int x, int y)
        {
            Input[] MouseEvent = new Input[1];
            MouseEvent[0].Type = 0;
            // move mouse: Flags ABSOLUTE (whole screen) and MOVE (move)
            MouseEvent[0].Data = CreateMouseInput(x, y, 0, 0, MOUSEEVENTF_MOVE);
            SendInput((uint)MouseEvent.Length, MouseEvent, Marshal.SizeOf(MouseEvent[0].GetType()));
        }

        /// <summary>
        /// Handles gesture detection results arriving from the sensor for the associated body tracking Id
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;
            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    // get the discrete gesture results which arrived with the latest frame
                    IReadOnlyDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;

                    if (discreteResults != null)
                    {
                        // we only have one gesture in this source object, but you can get multiple gestures
                        foreach (Gesture gesture in this.vgbFrameSource.Gestures)
                        {
                            if (gesture.Name.Equals(this.leanr) && gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    // update the GestureResultView object with new gesture result values
                                    this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence);
                                    if(result.Detected) {
                                        SimulateMouseMove(10, 0);
                                    }
                                }
                            }
                            if (gesture.Name.Equals(this.leanf) && gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    // update the GestureResultView object with new gesture result values
                                    this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence);
                                    if (result.Detected)
                                    {
                                        InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_W);
                                    }
                                }
                            }
                            if (gesture.Name.Equals(this.abilityl) && gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    // update the GestureResultView object with new gesture result values
                                    this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence);
                                    if (result.Detected)
                                    {
                                        InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_E);
                                    }
                                }
                            }
                            if (gesture.Name.Equals(this.abilityr) && gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    // update the GestureResultView object with new gesture result values
                                    this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence);
                                    if (result.Detected)
                                    {
                                        InputSimulator.SimulateKeyPress(VirtualKeyCode.SHIFT);
                                    }
                                }
                            }
                            if (gesture.Name.Equals(this.ultimate) && gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    // update the GestureResultView object with new gesture result values
                                    this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence);
                                    if (result.Detected)
                                    {
                                        InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_Q);
                                    }


                                }
                            }
                            if (gesture.Name.Equals(this.shoot) && gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    // update the GestureResultView object with new gesture result values
                                    this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence);
                                    if (result.Detected)
                                    {
                                        InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_M);
                                    }
                                }
                            }
                            if (gesture.Name.Equals(this.reload) && gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    // update the GestureResultView object with new gesture result values
                                    this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence);
                                    if (result.Detected)
                                    {
                                        InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_R);
                                    }
                                }
                            }
                            if (gesture.Name.Equals(this.leanl) && gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    // update the GestureResultView object with new gesture result values
                                    this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence);
                                    if (result.Detected)
                                    {
                                        SimulateMouseMove(-10, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the TrackingIdLost event for the VisualGestureBuilderSource object
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            // update the GestureResultView object to show the 'Not Tracked' image in the UI
            this.GestureResultView.UpdateGestureResult(false, false, 0.0f);
        }
    }
}
