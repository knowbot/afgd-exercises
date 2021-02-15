using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

// let's use a namespace for the course material, that is short for Algorithms for Games Development
namespace AfGD
{
    // Let's create a few classes to represent the objects in our game
    public struct Edge
    {
        public Vector2 from;
        public Vector2 to;
        public Vector2 normal;
    }

    public class Box
    {
        public Vector2 position;
        public Vector2 sides;
        public Color color;

        public Box(Vector2 boxPos, float boxWidth, float boxHeight, Color boxColor)
        {
            position = boxPos;
            sides = new Vector2(boxWidth, boxHeight);
            color = boxColor;
        }

        public Edge GetEdge(int index)
        {
            Assert.IsTrue(index >= 0 && index < 4);
            Edge edge;
            Vector2[] corners = new Vector2[] {
                new Vector2(-0.5f, -0.5f),
                new Vector2( 0.5f, -0.5f),
                new Vector2( 0.5f,  0.5f),
                new Vector2(-0.5f,  0.5f)
            };
            edge.from = position + corners[index] * sides;
            edge.to = position + corners[(index + 1) % 4] * sides;
            Vector2 dir = (edge.to - edge.from).normalized;
            edge.normal = new Vector2(dir.y, -dir.x);
            return edge;
        }

        public void DebugDraw()
        {
            for (int i = 0; i < 4; i++)
            {
                Edge ithEdge = GetEdge(i);
                Vector3 midPoint = Vector3.Lerp(ithEdge.from, ithEdge.to, 0.5f);
                // TODO exercise 1.1
                // use Debug.DrawLine and/or Debug.DrawRay to draw the edges of the box an the normal of each edge
                // https://docs.unity3d.com/2020.2/Documentation/Manual/class-Debug
                // implement it here
                Debug.DrawLine(ithEdge.@from, ithEdge.to, color);
                Debug.DrawRay(midPoint, ithEdge.normal* 0.25f, color);
            }
        }
    }

    public class Ball
    {
        public Vector2 position;
        public float radius;
        public Color color;
        public Vector2 velocity;

        public Ball(Vector2 ballPosition, float ballRadius, Color ballColor, Vector2 initVelocity)
        {
            position = ballPosition;
            radius = ballRadius;
            color = ballColor;
            velocity = initVelocity;
        }

        public void move(float deltaTime)
        {
            position += velocity * deltaTime;
        }

        public void DebugDraw()
        {
            // TODO exercise 1.1
            // draw the sphere using the draw functions in the Gizmos class
            // https://docs.unity3d.com/2020.2/Documentation/Manual/GizmosAndHandles
            // implement it here
            Gizmos.color = color;
            Gizmos.DrawWireSphere(position, radius);

            // OPTIONALLY, approximete a circle using a sequence of Debug.DrawLine calls
            // you can do that by using the sine and cosine functions in Mathf, 
            // and progressively increasing the angle (remember about the relaction if sine and cosine with the unit circle)
            // you also need the radius and position of the circle to define the start and end points of each line segment
            // https://docs.unity3d.com/2020.2/Documentation/Manual/class-Debug
            // implement it here
        }
    }


    public class BreakoutGame : MonoBehaviour
    {

        Box paddle;
        Box leftWall, rightWall, topWall;
        Ball ball;
        float fieldWidth = 10, fieldHeight = 20;
        float wallThickness = .5f;
        float ballRadius = .5f;
        float ballSpeed = 5f;
        float paddleSpeed = 5f;

        // exercise 1.5
        // create a data structure to hold the blocks. I recommend looking into:
        // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=net-5.0
        // https://learn.unity.com/tutorial/lists-and-dictionaries#5c89434eedbc2a0d28f48a70
        // implement it here

        List<Box> blocks = new List<Box>();
        int blocksHorizontal = 5;
        int blocksVertical = 4;

        bool HandleCollision(Box box)
        {
            for (int i = 0; i < 4; i++)
            {
                Edge e = box.GetEdge(i);
                // TODO exercise 1.4 
                // test intersection only if the angle between the normal and velocity is less than 90 degrees
                // why do you think that we run this test? Slide 45
                /*
                 * We run this test to determine whether the ball is approaching the colliding box (in which case we
                 * want to register the collision) or the ball is getting farther (in which case we don't really care).
                 */
                if (Vector2.Angle(ball.velocity, -e.normal) < 90) // replace this with your implementation
                {
                    if (HasCollision(e))
                    {
                        // TODO exercise 1.4
                        // How should the ball react to a collision with an edge? Slide 49
                        // ball.velocity -= e.normal * (2 * Vector2.Dot(ball.velocity, e.normal)); 
                        ball.velocity = Vector2.Reflect(ball.velocity, e.normal);
                        return true;
                    }
                }
            }
            return false;
        }

        bool HasCollision(Edge edge)
        {
            // the vector from edge.from to edge.to
            Vector2 edgeVector = edge.to - edge.from;
            float length = (edgeVector).magnitude;
            Vector2 edgePointToBall = ball.position - edge.from;

            // TODO exercise 1.3
            // smallestDist is the smallest distance from the ball to the line/edge, 
            // the PROJECTION of the vector from any point in the line to the ball onto 
            // the normal of the plane. Slide 44
            float smallestDist = Mathf.Abs(Vector2.Dot(edgePointToBall, edge.normal));

            // TODO exercise 1.3
            // Find the point in the line that is closes to the ball, 
            // you'll need the ball position, edge normal, and the smallestDist to compute it
            // make some drawings if it helps
            Vector2 closestPoint = ball.position - smallestDist * edge.normal; // replace Vector2.zero;

            // Here we certify that the closest point belongs to this line segment
            // otherwise we would be assuming a line that extends to infinity
            // What is the geometrical test below doing? Can you draw it? I use the info from Slide 45
            /*
             * The geometrical test is checking whether the vectors from the closest point to either extreme of the
             * edge are in the same direction as the direction from that extreme to the other. If they are, then the
             * point belongs to the line.
             */
            if (Vector2.Dot(edgeVector, closestPoint - edge.from) < 0)
                closestPoint = edge.from;
            if (Vector2.Dot(-edgeVector, closestPoint - edge.to) < 0)
                closestPoint = edge.to;

            // finally test if the closes point in the line segment is inside the ball
            if ((closestPoint - ball.position).magnitude < ball.radius)
            {
                return true;
            }
            return false;

        }

        void ResetBall()
        {
            // TODO exercise 1.2
            // set the initial position and velocity of the ball
            // implement it here
            ball.velocity = Vector2.up * 2;
            ball.position = new Vector2(0, paddle.position.y + paddle.sides.y + ball.radius);
        }

        void HandleOutOfBounds()
        {
            // TODO exercise 1.2
            // define condition when the ball is out of bounds and needs to be reset
            if (ball.position.y < fieldHeight * -.5f)
            {
                ResetBall();
            }
        }

        void Start()
        {
            // exercise 1.5
            // initialize the blocks
            // implement it here
            float blockWidth = (fieldWidth - wallThickness) / blocksHorizontal;
            float blockHeight = (fieldHeight/5 - wallThickness) / blocksVertical;
            
            for (int i = 0; i < blocksVertical * blocksHorizontal; i++)
            {
                float x = fieldWidth * -.5f + wallThickness*.5f + blockWidth *.5f + blockWidth * (i % blocksHorizontal);
                float y = fieldHeight * .4f + wallThickness* -.5f + blockHeight * -.5f - blockHeight * (i / blocksHorizontal);
                Box block = new Box(new Vector2(x, y), blockWidth, blockHeight, Color.blue);
                blocks.Add(block);
            }
            
            // create the walls and paddle
            paddle = new Box(new Vector2(0, fieldHeight * -.25f), 2, 1, Color.green);
            leftWall = new Box(new Vector2(fieldWidth * -.5f, 0), wallThickness, fieldHeight, Color.red);
            rightWall = new Box(new Vector2(fieldWidth * .5f, 0), wallThickness, fieldHeight, Color.red);
            topWall = new Box(new Vector2(0, fieldHeight * .5f), fieldWidth, wallThickness, Color.red);
            // create and initialize the ball
            ball = new Ball(Vector2.zero, ballRadius, Color.yellow, Vector2.zero);
            ResetBall();
        }

        // Update is called once per frame, at the start of the game logic block
        void Update()
        {
            // exercise 1.5
            // test collision with the blocks
            // implement it here
            foreach (var block in blocks)
            {
                if (HandleCollision(block))
                    blocks.Remove(block);
            }

            // compute intersections with the ball
            HandleCollision(leftWall);
            HandleCollision(rightWall);
            HandleCollision(topWall);
            if (HandleCollision(paddle))
            {
                // it becomes boring if we simply reflect the ball.velocity by the normal of the paddle
                // so let's do somethign different:
                // can you visualize what this operation is doing?
                /*
                 * It "reflects" the ball movement along the colliding direction instead! So if the ball hits the paddle
                 * at a certain angle, it will bounce back at the same exact angle but maintaining its "speed" (aka
                 * the magnitude of the velocity vector).
                 */
                    ball.velocity = (ball.position - paddle.position).normalized * ball.velocity.magnitude;
            }

            // process input
            float deltaTime = Time.deltaTime;
            if (Input.GetKey(KeyCode.A))
                paddle.position.x -= paddleSpeed * deltaTime;
            if (Input.GetKey(KeyCode.D))
                paddle.position.x += paddleSpeed * deltaTime;

            // prevent paddle from going out of bounds
            paddle.position.x = Mathf.Clamp(paddle.position.x,
                leftWall.position.x + wallThickness * .5f + paddle.sides.x * .5f,
                rightWall.position.x - wallThickness * .5f - paddle.sides.x * .5f);

            // update the position of the ball
            ball.move(Time.deltaTime);
            HandleOutOfBounds();

            // exercise 1.5
            // destroy the block if it has been hit by the ball
            // implement it here
        }

        private void OnDrawGizmos()
        {
            // exercise 1.5
            // draw the blocks
            // implement it here
            foreach (var block in blocks)
            {
                block?.DebugDraw();
            }

            // render the game
            leftWall?.DebugDraw();
            rightWall?.DebugDraw();
            topWall?.DebugDraw();
            paddle?.DebugDraw();
            ball?.DebugDraw();
        }

        // for a full overview of Unity's event functions execution order check
        // https://docs.unity3d.com/Manual/ExecutionOrder.html
    }
}