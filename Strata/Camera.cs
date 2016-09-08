/*

Last Editted by : Kiel Regusters
Date            : 09/08/2016

Additions: 
-Main implementation of the class. Extended the MonoGame.Extended library's already powerful Camera2D class to support limiting rectangles.
-Explanatory comments written

Notes:

This should be our main Camera class for use throughout the game. Avoid using the Extended Camera2D class over this one.

*/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Strata
{
    /// <summary>
    /// Extends the MonoGame.Extended camera class to support a limiting rectangle the camera can move inside
    /// This will be useful for keeping the camera within the bounds of the level and the player never sees "the void"
    /// </summary>
    class Camera : Camera2D
    {
        private readonly Viewport _viewport;
        private readonly Vector2 _origin;

        private Rectangle? _limits;


        public Camera(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            _viewport = graphicsDevice.Viewport;
            _origin = new Vector2(_viewport.Width / 2.0f, _viewport.Height / 2.0f);
        }

        /// <summary>
        /// This property will return a new vector containing the origin of the camera.
        /// </summary>
        public new Vector2 Origin
        {
            get
            {
                return _origin;
            }
        }

        /// <summary>
        /// This property will return or set the position of the camera.
        /// </summary>
        public new Vector2 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
                ValidatePosition();
            }
        }

        /// <summary>
        /// This property will return or set the Zoom of the camera.
        /// </summary>
        public new float Zoom
        {
            get
            {
                return base.Zoom;
            }
            set
            {
                base.Zoom = value;
                ValidateZoom();
                ValidatePosition();
            }
        }

        /// <summary>
        /// This property will set the limits of the camera.
        /// </summary>
        public Rectangle? Limits
        {
            set
            {
                _limits = value;
                //If we set our limits we need to make sure that we initially follow the rules of the limiter.
                ValidateZoom();
                ValidatePosition();
            }
        }

        //Private Methods

        /// <summary>
        /// This function will make sure we do not zoom outside the limiting rectangle.
        /// </summary>
        private void ValidateZoom()
        {
            //Only validate the zoom if our limits have a value, otherwise the camera can freely zoom
            if (_limits.HasValue)
            {
                //Validating the camera's zoom is easier than checking the position
                //First we know that the camera isn't zoomed in at all with a value of 1.0f.
                //Knowing that, the area that the camera can see corresponds exactly to the games Viewport.
                //If we zoom in, we see less of the world, if we zoom out we see more.
                //This implies that CameraViewport = Viewport / Zoom
                //So CameraViewport <= LimitRectangle
                //Combining these equations, we get an easy to program expression:
                //Zoom >= ViewportSize / LimitRectangle

                float minimumZoom_X = (float)_viewport.Width / _limits.Value.Width;
                float minimumZoom_Y = (float)_viewport.Height / _limits.Value.Height;

                base.Zoom = MathHelper.Max(Zoom, MathHelper.Max(minimumZoom_X, minimumZoom_Y));
            }
        }

        /// <summary>
        /// This function will clamp the position within the bounds set by another class (probably the level class)
        /// </summary>
        private void ValidatePosition()
        {
            //Only validate the position if our limits have a value, otherwise the camera can freely roam
            if (_limits.HasValue)
            {
                //Validating the camera's position is a little hard.
                //First, we need to know where the top left corner of the camera. (No matter how zoomed it is.)
                //After we have that, we invert the ViewMatrix of the camera so that we can transform those points
                //from "View" space to "World" space. From there we can use that information to determine where
                //the camera's position(Which is essentially Vector2 in the "View" space) is relative to the world space.
                //All that's left is to clamp the position of the  camera relative to the Rectangle given to us by another class.

                //So,

                //Get the top left corner of the camera relative to worldspace, luckily our parent has our inverse matrix for us.
                Vector2 cameraRelative = Vector2.Transform(Vector2.Zero, GetInverseViewMatrix());
                //Next, get the camera size. We must take into account the zoom of the camera.
                Vector2 cameraSize = new Vector2(_viewport.Width, _viewport.Height) / Zoom;
                //Next, we get the limits of the camera in vector form.
                Vector2 limitWorldLower = new Vector2(_limits.Value.Left, _limits.Value.Top);
                Vector2 limitWorldUpper = new Vector2(_limits.Value.Right, _limits.Value.Bottom);
                //Next, calculate the position relative to the world
                Vector2 positionRelative = Position - cameraRelative;
                base.Position = Vector2.Clamp(cameraRelative, limitWorldLower, limitWorldUpper - cameraSize) + positionRelative;

                //Done!

            }
        }

        //Public Methods

        /// <summary>
        /// Helper function that resets the camera.
        /// </summary>
        public void ResetCamera()
        {
            Zoom = 1f;
            Position = Vector2.Zero;
        }
    }
}
