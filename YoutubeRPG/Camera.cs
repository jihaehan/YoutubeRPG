using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        
        public void LockCamera(Layer layer)
        /*clamp the X and Y coordinates between 0 and the width of the map minus the width of the viewport
         for width and 0 and the height of the map minus the height of the viewport for height.*/
        {
            position.X = MathHelper.Clamp(position.X,
            0, layer.Width() - ScreenManager.Instance.Dimensions.X);
            position.Y = MathHelper.Clamp(position.Y,
            0, layer.Height() - ScreenManager.Instance.Dimensions.Y); 
        }
        public void LockToSprite(Layer layer, Image image)
        {
            position.X = (image.Position.X + image.SourceRect.Width / 2)
             - (ScreenManager.Instance.Dimensions.X / 2);
            position.Y = (image.Position.Y + image.SourceRect.Height / 2)
             - (ScreenManager.Instance.Dimensions.Y / 2);
            LockCamera(layer);
        }

    }
}
