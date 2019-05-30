using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirtCOCO
{
    public class GameLayer : CCLayerColor
    {
        CCSprite paddleSprite;
        CCSprite ballSprite;
        int score;
        CCLabel scoreLabel;
        CCLabel GameOverLabel;
        float ballXVelocity;
        float ballYVelocity;
        // How much to modify the ball's y velocity per second:
        const float gravity = 140;
        bool gamestop = false;
        public GameLayer() : base(CCColor4B.Black)
        {
            // "paddle" refers to the paddle.png image
            paddleSprite = new CCSprite("paddle");
            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 100;
            AddChild(paddleSprite);

            ballSprite = new CCSprite("ball");
            ballSprite.PositionX = 320;
            ballSprite.PositionY = 600;
            AddChild(ballSprite);

            scoreLabel = new CCLabel("Score: 0", "Arial", 30, CCLabelFormat.SystemFont);
            scoreLabel.PositionX = 50;
            scoreLabel.PositionY = 1000;

            GameOverLabel = new CCLabel("Game Over", "Arial", 70, CCLabelFormat.SystemFont);
            GameOverLabel.PositionX = 380;
            GameOverLabel.PositionY = 500;

            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreLabel);
            
            
            Schedule(RunGameLogic);
            
        }
        protected override void AddedToScene()
        {
            base.AddedToScene();
            // Use the bounds to layout the positioning of the drawable assets
            CCRect bounds = VisibleBoundsWorldspace;
            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            AddEventListener(touchListener, this);
            touchListener.OnTouchesMoved = HandleTouchesMoved;
            AddEventListener(touchListener, this);

        }
        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                GameOverLabel.Text = "";
                if (gamestop)
                {
                    Schedule(RunGameLogic);
                    gamestop = false;
                    Re(GameOverLabel);
                }
            }
        }
        void RunGameLogic(float frameTimeInSeconds)
        {
            // This is a linear approximation, so not 100% accurate
            ballYVelocity += frameTimeInSeconds * -gravity;
            ballSprite.PositionX += ballXVelocity * frameTimeInSeconds;
            ballSprite.PositionY += ballYVelocity * frameTimeInSeconds;
            // New Code:
            // Check if the two CCSprites overlap...
            bool doesBallOverlapPaddle = ballSprite.BoundingBoxTransformedToParent.IntersectsRect(
                paddleSprite.BoundingBoxTransformedToParent);
            // ... and if the ball is moving downward.
            bool isMovingDownward = ballYVelocity < 0;
            if (doesBallOverlapPaddle && isMovingDownward)
            {
                // First let's invert the velocity:
                ballYVelocity *= -1;
                // Then let's assign a random value to the ball's x velocity:
                const float minXVelocity = -300;
                const float maxXVelocity = 300;
                ballXVelocity = CCRandom.GetRandomFloat(minXVelocity, maxXVelocity);
                score++;
                scoreLabel.Text = "Score: " + score;
            }
            // First let’s get the ball position:   
            float ballRight = ballSprite.BoundingBoxTransformedToParent.MaxX;
            float ballLeft = ballSprite.BoundingBoxTransformedToParent.MinX;
            float ballTop = ballSprite.BoundingBoxTransformedToParent.MaxY;
            float ballBot = ballSprite.BoundingBoxTransformedToParent.MinY;
            // Then let’s get the screen edges
            float screenRight = VisibleBoundsWorldspace.MaxX;
            float screenLeft = VisibleBoundsWorldspace.MinX;
            float screenTop = VisibleBoundsWorldspace.MaxY;
            float screenBot = VisibleBoundsWorldspace.MinY;
            // Check if the ball is either too far to the right or left:    
            bool shouldReflectXVelocity =
                (ballRight > screenRight && ballXVelocity > 0) ||
                (ballLeft < screenLeft && ballXVelocity < 0);
            if (shouldReflectXVelocity)
            {
                ballXVelocity *= -1;
            }

            if (ballTop < screenBot)
            {
                ballSprite.PositionX = 320;
                ballSprite.PositionY = 1200;
                score = 0;
                scoreLabel.Text = "Score: " + score;
                Unschedule(RunGameLogic);
                AddChild(GameOverLabel);
                gamestop = true;
            }

        }
       

        void HandleTouchesMoved(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            // we only care about the first touch:
            var locationOnScreen = touches[0].Location;
            paddleSprite.PositionX = locationOnScreen.X;
        }
    }
}
