using System;
using Microsoft.Xna.Framework;

namespace YoutubeRPG
{
    //For Right Triangles only, with equal side lengths
    public struct Triangle
    {
        public Vector2 Corner;
        public float Length;
        public float Offset;
        public float Slope;

        public Triangle(int x, int y, float offset, float length, int slope)
        {
            Corner = new Vector2(x, y);
            Offset = offset;
            Length = length;
            Slope = slope;
        }
        public bool Intersects(Circle circle)
        {
            Vector2 v = circle.Center - Corner;
            Vector2 vi = v - (v / v.Length() * circle.Radius);
            Vector2 vii = new Vector2(vi.X, Offset + (Slope) * vi.X);

            float distanceSquared = vi.LengthSquared();
            float internalSquared = vii.LengthSquared();

            return (distanceSquared < Length * Length && distanceSquared <= internalSquared);
        }
    }
}
