using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace krestik_nolik_yakovchuk
{
    public partial class MainWindow : Window
    {
        private readonly string[] board = new string[9];   // поле 3x3
        private string currentPlayer = "X";                // чей сейчас ход
        private bool isGameActive = false;                // идёт ли игра
        private bool isPlayingWithAI = false;             // режим: true – против ПК
        private bool lastModeSet = false;                 // режим уже выбирали
        private Button[] boardButtons = null!;            // ссылки на кнопки поля

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
        }

        // Инициализация массива кнопок и очистка поля
        private void InitializeBoard()
        {
            boardButtons = new Button[]
            {
                Btn0, Btn1, Btn2,
                Btn3, Btn4, Btn5,
                Btn6, Btn7, Btn8
            };

            for (int i = 0; i < 9; i++)
            {
                board[i] = "";
                boardButtons[i].Content = "";
                boardButtons[i].IsEnabled = true;
                boardButtons[i].Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        // Кнопка "Против ПК"
        private void PlayWithAI_Click(object sender, RoutedEventArgs e)
        {
            isPlayingWithAI = true;
            lastModeSet = true;
            StartGame();
        }

        // Кнопка "Два игрока"
        private void PlayWithPlayer_Click(object sender, RoutedEventArgs e)
        {
            isPlayingWithAI = false;
            lastModeSet = true;
            StartGame();
        }

        // Общий запуск игры (и при выборе режима, и при новой игре)
        private void StartGame()
        {
            GameInfo.Visibility = Visibility.Visible;

            isGameActive = true;
            currentPlayer = "X";

            InitializeBoard();
            UpdateGameStatus();
        }

        // Клик по ячейке поля
        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (!isGameActive) return;
            if (sender is not Button button) return;

            int index = Array.IndexOf(boardButtons, button);
            if (index < 0) return;

            if (board[index] != "") return;

            // Ход текущего игрока (X или O)
            board[index] = currentPlayer;
            button.Content = currentPlayer;
            button.IsEnabled = false;

            // Проверка победы
            if (CheckWin(currentPlayer))
            {
                GameStatus.Text = $"Игрок {currentPlayer} победил!";
                isGameActive = false;
                return;
            }

            // Проверка на ничью
            if (IsBoardFull())
            {
                GameStatus.Text = "Ничья!";
                isGameActive = false;
                return;
            }

            if (isPlayingWithAI)
            {
                // Против ПК: человек всегда X, потом ходит ИИ (O)
                AIMove();
            }
            else
            {
                // Переключаем игрока в режиме 2‑х людей
                currentPlayer = currentPlayer == "X" ? "O" : "X";
                GameStatus.Text = $"Ход игрока {currentPlayer}";
            }
        }

        // Ход компьютера (O)
        private void AIMove()
        {
            if (!isGameActive) return;

            // Попытка выиграть
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == "")
                {
                    board[i] = "O";
                    if (CheckWin("O"))
                    {
                        boardButtons[i].Content = "O";
                        boardButtons[i].IsEnabled = false;
                        FinishAfterAIMove();
                        return;
                    }
                    board[i] = "";
                }
            }

            // Блокировка победы X
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == "")
                {
                    board[i] = "X";
                    if (CheckWin("X"))
                    {
                        board[i] = "O";
                        boardButtons[i].Content = "O";
                        boardButtons[i].IsEnabled = false;
                        FinishAfterAIMove();
                        return;
                    }
                    board[i] = "";
                }
            }

            // Центр
            if (board[4] == "")
            {
                board[4] = "O";
                boardButtons[4].Content = "O";
                boardButtons[4].IsEnabled = false;
                FinishAfterAIMove();
                return;
            }

            // Углы
            int[] corners = { 0, 2, 6, 8 };
            foreach (int corner in corners)
            {
                if (board[corner] == "")
                {
                    board[corner] = "O";
                    boardButtons[corner].Content = "O";
                    boardButtons[corner].IsEnabled = false;
                    FinishAfterAIMove();
                    return;
                }
            }

            // Первая свободная ячейка
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == "")
                {
                    board[i] = "O";
                    boardButtons[i].Content = "O";
                    boardButtons[i].IsEnabled = false;
                    FinishAfterAIMove();
                    return;
                }
            }
        }

        // Проверка состояния после хода ИИ
        private void FinishAfterAIMove()
        {
            if (CheckWin("O"))
            {
                GameStatus.Text = "Компьютер победил!";
                isGameActive = false;
                return;
            }

            if (IsBoardFull())
            {
                GameStatus.Text = "Ничья!";
                isGameActive = false;
                return;
            }

            GameStatus.Text = "Ваш ход (X)";
        }

        // Проверка победы
        private bool CheckWin(string player)
        {
            // Горизонтали
            for (int i = 0; i < 9; i += 3)
            {
                if (board[i] == player && board[i + 1] == player && board[i + 2] == player)
                    return true;
            }

            // Вертикали
            for (int i = 0; i < 3; i++)
            {
                if (board[i] == player && board[i + 3] == player && board[i + 6] == player)
                    return true;
            }

            // Диагонали
            if (board[0] == player && board[4] == player && board[8] == player)
                return true;

            if (board[2] == player && board[4] == player && board[6] == player)
                return true;

            return false;
        }

        // Проверка на заполненность поля
        private bool IsBoardFull()
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == "")
                    return false;
            }
            return true;
        }

        // Кнопка "Новая игра"
        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            // Если режим уже выбирали – перезапускаем его
            if (!lastModeSet)
            {
                // по умолчанию пусть будет режим двух игроков
                isPlayingWithAI = false;
                lastModeSet = true;
            }

            StartGame();
        }

        // Обновление статуса в меню
        private void UpdateGameStatus()
        {
            GameStatus.Text = isPlayingWithAI
                ? "Ваш ход (X) против ПК"
                : "Ход игрока X";
        }
    }
}
