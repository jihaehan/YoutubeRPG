using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace YoutubeRPG
{
    public class Camera
    {
        Vector2 position;   //position of the camera on the map
        float speed;        //speed is the speed at which the camera moves, in pixels

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float Speed
        {
            get { return speed; }
            set { speed = (float)MathHelper.Clamp(speed, 4f, 16f); }        //clamp the speed
        }
        public Matrix Transformation
        {
            get { return Matrix.CreateTranslation(new Vector3(-Position, 0f)); }
        }
        public Camera()
        {
            speed = 4f;
        }
        public Camera(Vector2 position)
        {
            speed = 4f;
            Position = position;
        }
        public void LockCamera(Layer map, Rectangle viewport)
        /*clamp the X and Y coordinates between 0 and the width of the map minus the width of the viewport
         for width and 0 and the height of the map minus the height of the viewport for height.*/
        {
            /*
            position.X = MathHelper.Clamp(position.X,
            0, map.WidthInPixels - viewport.Width);
            position.Y = MathHelper.Clamp(position.Y,
            0, map.HeightInPixels - viewport.Height); */
        }
        public void LockToSprite(Layer map, Image sprite, Rectangle viewport)
        {
            position.X = (sprite.Position.X + sprite.SourceRect.Width / 2)
            - (viewport.Width / 2);
            position.Y = (sprite.Position.Y + sprite.SourceRect.Height / 2)
            - (viewport.Height / 2);
            LockCamera(map, viewport);
        }

    }
}
