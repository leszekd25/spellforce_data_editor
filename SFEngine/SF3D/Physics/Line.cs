/*
 * Line describes a straight line in 3D space using a starting point and a vector
 * Operation for retrieving distance between a point and the line is provided
 * */

using OpenTK;

namespace SFEngine.SF3D.Physics
{
    public class Line
    {
        public Vector3 point;
        public Vector3 vector;

        // sets line parameters for a line which connects two provided points
        public Line(Vector3 start, Vector3 end)
        {
            point = start;
            vector = (end - start).Normalized();
        }

        // distance from point to line
        public float Distance(Vector3 p)
        {
            Vector3 diff = point - p;
            return (diff - (Vector3.Dot(diff, vector) * vector)).Length;
        }

        // distance from point to line, squared
        public float Distance2(Vector3 p)
        {
            Vector3 diff = point - p;
            return (diff - (Vector3.Dot(diff, vector) * vector)).LengthSquared;
        }
    }
}
