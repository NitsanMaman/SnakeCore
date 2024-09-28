using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Snake
{
    public class SnakeViewModel : BaseViewModel
    {

        // Holds an instance of SnakeModel, which manages the game's logic and state.
        private SnakeModel _model;

        //properties exposed to the view for data binding.
        //holds the elements used to draw the game grid.
        public ObservableCollection<UIElement> GameElements => _model.GameElements;

        //representing the snake’s body on the game board.
        public ObservableCollection<Rectangle> Snake => _model.Snake;

        // calculate the dimensions of the game board.
        // properties bound to the canvas dimensions in the XAML to ensure the board scales dynamically.
        public int GameBoardWidth => _model.GridSize * _model.Cols;
        public int GameBoardHeight => _model.GridSize * _model.Rows;

        //commands used to handle user input for moving the snake.
        // properties bound to buttons in the XAML.
        public ICommand MoveUpCommand { get; private set; }
        public ICommand MoveDownCommand { get; private set; }
        public ICommand MoveLeftCommand { get; private set; }
        public ICommand MoveRightCommand { get; private set; }

        public SnakeViewModel()
        {
            // Set The Snake Model
            _model = new SnakeModel(gridSize: 50, rows: 7, cols: 7, snakeLength: 5);

            // Initializes the movement commands using RelayCommand
            MoveUpCommand = new RelayCommand(param => MoveUp());
            MoveDownCommand = new RelayCommand(param => MoveDown());
            MoveLeftCommand = new RelayCommand(param => MoveLeft());
            MoveRightCommand = new RelayCommand(param => MoveRight());

            //ensure movement buttons' enabled/disabled states are correct at the start.
            UpdateButtonStates();
        }

        // Handles moving the snake up if the current direction is not down (prevents reversing into itself)
        private void MoveUp()
        {
            if (_model.CurrentDirection != 3) _model.CurrentDirection = 1; // Set direction to 'up' if not 'down'
            _model.MoveSnake(); // Move the snake
            UpdateButtonStates(); // Update the state of buttons after movement
            OnPropertyChanged(nameof(Snake)); // Notify UI that the snake has moved
        }

        // Handles moving the snake down, similar to MoveUp()
        private void MoveDown()
        {
            if (_model.CurrentDirection != 1) _model.CurrentDirection = 3; // Set direction to 'down' if not 'up'
            _model.MoveSnake();
            UpdateButtonStates();
            OnPropertyChanged(nameof(Snake));
        }

        // Handles moving the snake left
        private void MoveLeft()
        {
            if (_model.CurrentDirection != 2) _model.CurrentDirection = 0; // Set direction to 'left' if not 'right'
            _model.MoveSnake();
            UpdateButtonStates();
            OnPropertyChanged(nameof(Snake));
        }

        // Handles moving the snake right
        private void MoveRight()
        {
            if (_model.CurrentDirection != 0) _model.CurrentDirection = 2;
            _model.MoveSnake();
            UpdateButtonStates();
            OnPropertyChanged(nameof(Snake));
        }

        // Updates the enabled/disabled states of the movement buttons
        private void UpdateButtonStates()
        {
            // Get the current position of the snake's head
            double headX = Canvas.GetLeft(Snake[0]);
            double headY = Canvas.GetTop(Snake[0]);

            // Use GridSize from the model
            int gridSize = _model.GridSize;

            // Predict the next positions for each direction based on the current head position
            double nextLeftX = headX - gridSize;
            double nextRightX = headX + gridSize;
            double nextUpY = headY - gridSize;
            double nextDownY = headY + gridSize;

            // Check boundaries to ensure the snake doesn't move outside the game area
            LeftButtonEnabled = !_model.IsOutOfBounds(nextLeftX, headY) && _model.CurrentDirection != 2;
            RightButtonEnabled = !_model.IsOutOfBounds(nextRightX, headY) && _model.CurrentDirection != 0;
            UpButtonEnabled = !_model.IsOutOfBounds(headX, nextUpY) && _model.CurrentDirection != 3;
            DownButtonEnabled = !_model.IsOutOfBounds(headX, nextDownY) && _model.CurrentDirection != 1;

            // Check for collision with the snake's body to prevent moving into itself
            LeftButtonEnabled &= !_model.IsCollisionWithBody(nextLeftX, headY);
            RightButtonEnabled &= !_model.IsCollisionWithBody(nextRightX, headY);
            UpButtonEnabled &= !_model.IsCollisionWithBody(headX, nextUpY);
            DownButtonEnabled &= !_model.IsCollisionWithBody(headX, nextDownY);

            // Notify the UI that the button states have changed, so it can update their enabled/disabled states
            OnPropertyChanged(nameof(LeftButtonEnabled));
            OnPropertyChanged(nameof(RightButtonEnabled));
            OnPropertyChanged(nameof(UpButtonEnabled));
            OnPropertyChanged(nameof(DownButtonEnabled));
        }

        // Properties that control whether each directional button is enabled or disabled, used for binding in XAML
        public bool LeftButtonEnabled { get; set; }
        public bool RightButtonEnabled { get; set; }
        public bool UpButtonEnabled { get; set; }
        public bool DownButtonEnabled { get; set; }
    }
}
