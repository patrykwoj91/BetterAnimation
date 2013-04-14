using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BetterSkinned
{
    /// <summary>
    /// Simple class that implements a camera that can be manipulated using the mouse
    /// </summary>
    public class Camera
    {
        #region Fields

        private GraphicsDeviceManager graphics;

        /// <summary>
        /// The eye position in space
        /// </summary>
        private Vector3 eye = new Vector3(250, 200, 350);

        /// <summary>
        /// The location we are looking at in space.
        /// </summary>
        private Vector3 center = new Vector3(0, 0, 0);

        /// <summary>
        /// The up direction
        /// </summary>
        private Vector3 up = new Vector3(0, 1, 0);

        private float fov = MathHelper.ToRadians(35);
        private float znear = 10;
        private float zfar = 10000;

        /// <summary>
        /// The computed view matrix
        /// </summary>
        private Matrix view;

        /// <summary>
        /// The computed projection matrix
        /// </summary>
        private Matrix projection;

        private bool mousePitchYaw = true;
        private bool mousePanTilt = true;
        private bool padPitchYaw = true;

        private MouseState lastMouseState;

        #endregion

        #region Properties

        public Matrix View { get { return view; } }
        public Matrix Projection { get { return projection; } }

        public Vector3 Center { get { return center; } set { center = value; ComputeView(); } }
        public Vector3 Eye { get { return eye; } set { eye = value; ComputeView(); } }

        public float ZNear { get { return znear; } set { znear = value; ComputeProjection(); } }
        public float ZFar { get { return zfar; } set { zfar = value; ComputeProjection(); } }

        public bool MousePitchYaw { get { return mousePitchYaw; } set { mousePitchYaw = value; } }
        public bool MousePanTilt { get { return mousePanTilt; } set { mousePanTilt = value; } }
        public bool PadPitchYaw { get { return padPitchYaw; } set { padPitchYaw = value; } }

        #endregion

        #region Construction and Initialization

        /// <summary>
        /// Constructor. Initializes the graphics field from a passed parameter.
        /// </summary>
        /// <param name="graphics">The graphics device manager for our program</param>
        public Camera(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
        }

        public void Initialize()
        {
            ComputeView();
            ComputeProjection();
            lastMouseState = Mouse.GetState();
        }

        #endregion

        #region Update

        public void Update(GraphicsDevice graphics, GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if (padPitchYaw)
            {
                Yaw(-gamePadState.ThumbSticks.Right.X * 0.05f);
                Pitch(gamePadState.ThumbSticks.Right.Y * 0.05f);
            }

            MouseState mouseState = Mouse.GetState();
            if (graphics.Viewport.Bounds.Contains(mouseState.X, mouseState.Y))
            {

                if (mousePitchYaw && mouseState.LeftButton == ButtonState.Pressed &&
                    lastMouseState.LeftButton == ButtonState.Pressed)
                {
                    float changeY = mouseState.Y - lastMouseState.Y;
                    Pitch(-changeY * 0.005f);

                    float changeX = mouseState.X - lastMouseState.X;
                    Yaw(changeX * 0.005f);
                }

                if (mousePanTilt && mouseState.RightButton == ButtonState.Pressed &&
                         lastMouseState.RightButton == ButtonState.Pressed)
                {
                    float changeY = mouseState.Y - lastMouseState.Y;
                    Tilt(changeY * 0.0025f);

                    float changeX = mouseState.X - lastMouseState.X;
                    Pan(changeX * 0.0025f);
                }

                lastMouseState = mouseState;
            }
        }

        #endregion

        #region Matrix Computations

        private void ComputeView()
        {
            view = Matrix.CreateLookAt(eye, center, up);
        }

        private void ComputeProjection()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(fov,
                graphics.GraphicsDevice.Viewport.AspectRatio, znear, zfar);
        }

        #endregion

        #region Camera Control

        public void Pitch(float angle)
        {
            // Need a vector in the camera X direction
            Vector3 cameraZ = eye - center;
            Vector3 cameraX = Vector3.Cross(up, cameraZ);
            float len = cameraX.LengthSquared();
            if (len > 0)
                cameraX.Normalize();
            else
                cameraX = new Vector3(1, 0, 0);

            Matrix t1 = Matrix.CreateTranslation(-center);
            Matrix r = Matrix.CreateFromAxisAngle(cameraX, angle);
            Matrix t2 = Matrix.CreateTranslation(center);

            Matrix M = t1 * r * t2;
            eye = Vector3.Transform(eye, M);
            ComputeView();
        }

        public void Yaw(float angle)
        {
            // Need a vector in the camera X direction
            Vector3 cameraZ = eye - center;
            Vector3 cameraX = Vector3.Cross(up, cameraZ);
            Vector3 cameraY = Vector3.Cross(cameraZ, cameraX);
            float len = cameraY.LengthSquared();
            if (len > 0)
                cameraY.Normalize();
            else
                cameraY = new Vector3(0, 1, 0);

            Matrix t1 = Matrix.CreateTranslation(-center);
            Matrix r = Matrix.CreateFromAxisAngle(cameraY, angle);
            Matrix t2 = Matrix.CreateTranslation(center);

            Matrix M = t1 * r * t2;
            eye = Vector3.Transform(eye, M);
            ComputeView();
        }

        public void Tilt(float angle)
        {
            // Need a vector in the camera X direction
            Vector3 cameraZ = eye - center;
            Vector3 cameraX = Vector3.Cross(up, cameraZ);
            float len = cameraX.LengthSquared();
            if (len > 0)
                cameraX.Normalize();
            else
                cameraX = new Vector3(1, 0, 0);

            Matrix t1 = Matrix.CreateTranslation(-eye);
            Matrix r = Matrix.CreateFromAxisAngle(cameraX, angle);
            Matrix t2 = Matrix.CreateTranslation(eye);

            Matrix M = t1 * r * t2;
            center = Vector3.Transform(center, M);
            ComputeView();
        }


        public void Pan(float angle)
        {
            // Need a vector in the camera X direction
            Vector3 cameraZ = eye - center;
            Vector3 cameraX = Vector3.Cross(up, cameraZ);
            Vector3 cameraY = Vector3.Cross(cameraZ, cameraX);
            float len = cameraY.LengthSquared();
            if (len > 0)
                cameraY.Normalize();
            else
                cameraY = new Vector3(0, 1, 0);

            Matrix t1 = Matrix.CreateTranslation(-eye);
            Matrix r = Matrix.CreateFromAxisAngle(cameraY, angle);
            Matrix t2 = Matrix.CreateTranslation(eye);

            Matrix M = t1 * r * t2;
            center = Vector3.Transform(center, M);
            ComputeView();
        }


        #endregion

    }
}
