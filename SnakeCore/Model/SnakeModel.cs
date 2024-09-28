using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Windows.Media.Effects;

namespace Snake
{
    public class SnakeModel
    {
        // Properties for setting the grid size, number of rows, and columns in the game
        public int GridSize { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }

        // Private field for snake length
        private int _snakeLength;

        // Property to get and set the snake length with validation
        public int SnakeLength
        {
            get { return _snakeLength; } // Return the private field
            set
            {
                // Ensure that the snake length is greater than 1 and less or equal to the number of columns
                if (value > 1 && value <= Cols)
                {
                    _snakeLength = value; // Set the private field
                }
                else
                {
                    // Throw an exception if the snake length is out of the valid range
                    throw new ArgumentOutOfRangeException($"Snake length must be between 2 and num of cols, in this case ({Cols}).");
                }
            }
        }

        // ObservableCollection to store grid elements (lines for the grid)
        public ObservableCollection<UIElement> GameElements { get; private set; }

        // ObservableCollection to store the snake's parts (each part is a rectangle)
        public ObservableCollection<Rectangle> Snake { get; private set; }

        // Integer to store the current direction of the snake's movement
        // -1 = No movement, 0 = Left, 1 = Up, 2 = Right, 3 = Down
        public int CurrentDirection { get; set; }

        // Constructor to initialize the SnakeModel with grid size, number of rows/columns, and snake length
        public SnakeModel(int gridSize, int rows, int cols, int snakeLength)
        {
            GridSize = gridSize; // Set the grid's each cell size
            Rows = rows; // Set the number of rows in the game
            Cols = cols; // Set the number of columns in the game
            SnakeLength = snakeLength; // Set the initial snake length

            // Initialize the game by setting up the grid and the snake
            InitGame();
        }

        // Method to initialize the game components: grid and snake
        public void InitGame()
        {
            // Initialize collections to store game elements (grid lines) and the snake's parts
            GameElements = new ObservableCollection<UIElement>();
            Snake = new ObservableCollection<Rectangle>();

            // Draw the grid on the canvas
            DrawGrid();

            // Create the snake parts and position them on the center of the grid
            for (int i = 0; i < SnakeLength; i++)
            {
                var rectangle = new Rectangle
                {
                    // Set the snake part width and height to be slightly smaller than the grid size
                    Width = GridSize * 0.99, 
                    Height = GridSize * 0.99,
                    Fill = Brushes.Green // Set the snake part color to green
                };
                // Add each snake part (rectangle) to the Snake collection
                Snake.Add(rectangle);

                // Initial placement of the snake at the center of the grid
                int initialX = ((Cols + SnakeLength - 1) / 2 - i) * GridSize;
                int initialY = (Rows / 2) * GridSize;

                // Ensure the snake's initial position doesn't go off the grid
                if (initialX < 0)
                {
                    initialX = 0; // Ensure no negative positioning
                }

                // Set the position of the snake part on the canvas
                Canvas.SetLeft(rectangle, initialX);
                Canvas.SetTop(rectangle, initialY);

            }

            // Apply a glow effect to the head
            var headRectangle = Snake[0]; // Get the first part of the snake (head)
            headRectangle.Fill = Brushes.DarkGreen;  // Darker head color
            headRectangle.Effect = new DropShadowEffect
            {
                Color = Colors.LightGreen,
                BlurRadius = 20,
                ShadowDepth = 0
            };

            CurrentDirection = -1; // No movement initially
        }

        // Method to draw the grid
        private void DrawGrid()
        {
            // Draw vertical grid lines
            for (int i = 0; i <= Cols; i++)
            {
                Line line = new Line
                {
                    Stroke = Brushes.Gray,
                    X1 = i * GridSize,
                    Y1 = 0,
                    X2 = i * GridSize,
                    Y2 = Rows * GridSize
                };
                // Apply the dashed line pattern for all lines except the first and last columns
                if (i != 0 && i != Cols)
                {
                    line.StrokeDashArray = new DoubleCollection { 2, 2 }; // Dashed line pattern
                }
                // Add the line to the game elements (grid)
                GameElements.Add(line);
            }

            // Draw horizontal grid lines
            for (int i = 0; i <= Rows; i++)
            {
                Line line = new Line
                {
                    Stroke = Brushes.Gray,
                    X1 = 0,
                    Y1 = i * GridSize,
                    X2 = Cols * GridSize,
                    Y2 = i * GridSize
                };
                // Apply the dashed line pattern for all lines except the first and last rows
                if (i != 0 && i != Rows)
                {
                    line.StrokeDashArray = new DoubleCollection { 2, 2 }; // Dashed line pattern
                }
                // Add the line to the game elements (grid)
                GameElements.Add(line);
            }
        }

        // Method to move the snake based on the current direction
        public void MoveSnake()
        {
            // If no direction is set, do nothing
            if (CurrentDirection == -1) return;

            // Get the head's current position
            double newHeadX = Canvas.GetLeft(Snake[0]);
            double newHeadY = Canvas.GetTop(Snake[0]);

            // Update the head position based on the current direction
            switch (CurrentDirection)
            {
                case 0: // Left
                    newHeadX -= GridSize;
                    break;
                case 1: // Up
                    newHeadY -= GridSize;
                    break;
                case 2: // Right
                    newHeadX += GridSize;
                    break;
                case 3: // Down
                    newHeadY += GridSize;
                    break;
            }

            // Ensure the new position doesn't exceed the grid boundaries
            if (IsOutOfBounds(newHeadX, newHeadY))
            {
                return; // Prevent movement if out of bounds
            }

            // Move the snake's body (shift each part to the position of the one ahead)
            for (int i = SnakeLength - 1; i > 0; i--)
            {
                Canvas.SetLeft(Snake[i], Canvas.GetLeft(Snake[i - 1]));
                Canvas.SetTop(Snake[i], Canvas.GetTop(Snake[i - 1]));
            }

            // Move the head to the new position
            Canvas.SetLeft(Snake[0], newHeadX);
            Canvas.SetTop(Snake[0], newHeadY);
        }

        // Method to check if the new head position is out of bounds
        public bool IsOutOfBounds(double newHeadX, double newHeadY)
        {
            return newHeadX < 0 || newHeadX >= Cols * GridSize || newHeadY < 0 || newHeadY >= Rows * GridSize;
        }

        // Method to check if the snake's head collides with its own body
        public bool IsCollisionWithBody(double newHeadX, double newHeadY)
        {
            // Iterate through all body parts of the snake, starting from the second part (index 1)
            for (int i = 1; i < SnakeLength; i++)
            {
                // Check if the head's new position matches any of the body parts' positions
                if (Canvas.GetLeft(Snake[i]) == newHeadX && Canvas.GetTop(Snake[i]) == newHeadY)
                {
                    return true; // Collision detected
                }
            }
            return false; // No collision
        }
    }
}
