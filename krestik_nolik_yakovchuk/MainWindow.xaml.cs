#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace krestik_nolik_yakovchuk
{
    public partial class MainWindow : Window
    {
        private readonly string[] board = new string[9];
        private string currentPlayer = "X";
        private bool isGameActive = false;
        private bool isPlayingWithAI = false;
        private int scoreX = 0;
        private int scoreO = 0;
        private int scoreDraw = 0;

        // Поля для кастомизации
        private string symbolX = "❌";
        private string symbolO = "⭕";
        private Color colorX = Color.FromRgb(0xFF, 0x6B, 0x6B);
        private Color colorO = Color.FromRgb(0x4E, 0xCD, 0xC4);

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            ClearBoard();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                board[i] = " ";
            }
        }

        private void ClearBoard()
        {
            foreach (Button btn in FindVisualChildren<Button>(GameBoard))
            {
                btn.Content = " ";
                btn.IsEnabled = true;
                btn.Background = new SolidColorBrush(Color.FromRgb(0x3D, 0x3D, 0x5C));
            }
        }

        private void PlayWithAI_Click(object sender, RoutedEventArgs e)
        {
            isPlayingWithAI = true;
            StartGame();
            GameStatus.Text = "🤖 Игра против компьютера";
        }

        private void PlayWithPlayer_Click(object sender, RoutedEventArgs e)
        {
            isPlayingWithAI = false;
            StartGame();
            GameStatus.Text = "👥 Игра двух игроков";
        }

        private void StartGame()
        {
            InitializeBoard();
            ClearBoard();
            currentPlayer = "X";
            isGameActive = true;
            UpdateTurnIndicator();
            GameStatusCenter.Text = "Ход игрока: ❌";
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (!isGameActive) return;

            Button btn = sender as Button;
            if (btn == null) return;

            int index = GetButtonIndex(btn);

            if (index == -1 || board[index] != " " || !btn.IsEnabled)
                return;

            MakeMove(index, currentPlayer);

            if (CheckWinner(currentPlayer))
            {
                EndGame(currentPlayer);
                return;
            }

            if (CheckDraw())
            {
                EndGame("draw");
                return;
            }

            currentPlayer = currentPlayer == "X" ? "O" : "X";
            UpdateTurnIndicator();

            if (isPlayingWithAI && currentPlayer == "O" && isGameActive)
            {
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(500);
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    MakeAIMove();
                };
                timer.Start();
            }
        }

        private void MakeMove(int index, string player)
        {
            board[index] = player;
            Button btn = GetButtonByIndex(index);
            if (btn == null) return;

            // Применяем кастомизацию
            if (player == "X")
            {
                btn.Content = symbolX;
                btn.Foreground = new SolidColorBrush(colorX);
            }
            else
            {
                btn.Content = symbolO;
                btn.Foreground = new SolidColorBrush(colorO);
            }

            btn.IsEnabled = false;

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            btn.BeginAnimation(UIElement.OpacityProperty, fadeAnimation);
        }

        private void MakeAIMove()
        {
            if (!isGameActive) return;

            int moveIndex = FindBestMove();

            if (moveIndex != -1)
            {
                MakeMove(moveIndex, "O");

                if (CheckWinner("O"))
                {
                    EndGame("O");
                    return;
                }

                if (CheckDraw())
                {
                    EndGame("draw");
                    return;
                }

                currentPlayer = "X";
                UpdateTurnIndicator();
            }
        }

        private int FindBestMove()
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == " ")
                {
                    board[i] = "O";
                    if (CheckWinner("O", false))
                    {
                        board[i] = " ";
                        return i;
                    }
                    board[i] = " ";
                }
            }

            for (int i = 0; i < 9; i++)
            {
                if (board[i] == " ")
                {
                    board[i] = "X";
                    if (CheckWinner("X", false))
                    {
                        board[i] = " ";
                        return i;
                    }
                    board[i] = " ";
                }
            }

            if (board[4] == " ") return 4;

            int[] corners = { 0, 2, 6, 8 };
            foreach (int corner in corners)
            {
                if (board[corner] == " ") return corner;
            }

            for (int i = 0; i < 9; i++)
            {
                if (board[i] == " ") return i;
            }

            return -1;
        }

        private bool CheckWinner(string player, bool updateScore = true)
        {
            int[,] winPatterns = {
                {0, 1, 2}, { 3, 4, 5}, {6, 7, 8},
                {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
                {0, 4, 8}, {2, 4, 6}
            };

            for (int i = 0; i < 8; i++)
            {
                if (board[winPatterns[i, 0]] == player &&
                    board[winPatterns[i, 1]] == player &&
                    board[winPatterns[i, 2]] == player)
                {
                    if (updateScore)
                    {
                        HighlightWinningCells(winPatterns[i, 0], winPatterns[i, 1], winPatterns[i, 2]);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool CheckDraw()
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == " ") return false;
            }
            return true;
        }

        private void HighlightWinningCells(int a, int b, int c)
        {
            Button btnA = GetButtonByIndex(a);
            Button btnB = GetButtonByIndex(b);
            Button btnC = GetButtonByIndex(c);

            if (btnA != null) btnA.Background = new SolidColorBrush(Color.FromRgb(0x4E, 0xCD, 0xC4));
            if (btnB != null) btnB.Background = new SolidColorBrush(Color.FromRgb(0x4E, 0xCD, 0xC4));
            if (btnC != null) btnC.Background = new SolidColorBrush(Color.FromRgb(0x4E, 0xCD, 0xC4));
        }

        private void EndGame(string result)
        {
            isGameActive = false;

            if (result == "draw")
            {
                GameStatusCenter.Text = "🤝 Ничья!";
                GameStatusCenter.Foreground = new SolidColorBrush(Color.FromRgb(0xA0, 0xA0, 0xA0));
                scoreDraw++;
                ScoreDraw.Text = scoreDraw.ToString();
                GameStatus.Text = "🤝 Игра закончилась вничью";
            }
            else
            {
                string winnerText = result == "X" ? "Игрок X" : (isPlayingWithAI ? "Компьютер" : "Игрок O");
                GameStatusCenter.Text = "🎉 Победил " + winnerText + "!";
                GameStatusCenter.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xD9, 0x3D));

                if (result == "X")
                {
                    scoreX++;
                    ScoreX.Text = scoreX.ToString();
                }
                else
                {
                    scoreO++;
                    ScoreO.Text = scoreO.ToString();
                }

                GameStatus.Text = "🏆 Победил " + winnerText + "!";
            }

            foreach (Button btn in FindVisualChildren<Button>(GameBoard))
            {
                btn.IsEnabled = false;
            }
        }

        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            InitializeBoard();
            ClearBoard();
            currentPlayer = "X";
            isGameActive = true;
            GameStatusCenter.Text = "Ход игрока: ❌";  
            GameStatusCenter.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xD9, 0x3D));

            GameStatus.Text = isPlayingWithAI ? "🤖 Игра против компьютера" : "👥 Игра двух игроков";

            UpdateTurnIndicator();
        }

        private void ResetScore_Click(object sender, RoutedEventArgs e)
        {
            scoreX = 0;
            scoreO = 0;
            scoreDraw = 0;
            ScoreX.Text = "0";
            ScoreO.Text = "0";
            ScoreDraw.Text = "0";

            MessageBox.Show("📊 Счёт сброшен!", "Сброс счёта",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateTurnIndicator()
        {
            if (currentPlayer == "X")
            {
                GameStatusCenter.Text = "Ход игрока: ❌";  
                GameStatusCenter.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x6B, 0x6B));
            }
            else
            {
                GameStatusCenter.Text = isPlayingWithAI ? "Ход компьютера: ⭕" : "Ход игрока: ⭕";  
                GameStatusCenter.Foreground = new SolidColorBrush(Color.FromRgb(0x4E, 0xCD, 0xC4));
            }
        }

        private int GetButtonIndex(Button btn)
        {
            if (btn == Btn0) return 0;
            if (btn == Btn1) return 1;
            if (btn == Btn2) return 2;
            if (btn == Btn3) return 3;
            if (btn == Btn4) return 4;
            if (btn == Btn5) return 5;
            if (btn == Btn6) return 6;
            if (btn == Btn7) return 7;
            if (btn == Btn8) return 8;
            return -1;
        }

        private Button GetButtonByIndex(int index)
        {
            switch (index)
            {
                case 0: return Btn0;
                case 1: return Btn1;
                case 2: return Btn2;
                case 3: return Btn3;
                case 4: return Btn4;
                case 5: return Btn5;
                case 6: return Btn6;
                case 7: return Btn7;
                case 8: return Btn8;
                default: return null;
            }
        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T childOfType)
                {
                    yield return childOfType;
                }
                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }

        // Метод для открытия окна кастомизации
        private void OpenCustomization_Click(object sender, RoutedEventArgs e)
        {
            var customizationWindow = new CustomizationWindow();
            customizationWindow.Owner = this;

            if (customizationWindow.ShowDialog() == true)
            {
                // Применяем настройки
                symbolX = customizationWindow.SymbolX;
                colorX = customizationWindow.ColorX;
                symbolO = customizationWindow.SymbolO;
                colorO = customizationWindow.ColorO;

                // Применяем к уже сделанным ходам
                ApplyCustomizationToBoard();
            }
        }

        // Применяем кастомизацию к существующим ячейкам
        private void ApplyCustomizationToBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == "X")
                {
                    Button btn = GetButtonByIndex(i);
                    if (btn != null)
                    {
                        btn.Content = symbolX;
                        btn.Foreground = new SolidColorBrush(colorX);
                    }
                }
                else if (board[i] == "O")
                {
                    Button btn = GetButtonByIndex(i);
                    if (btn != null)
                    {
                        btn.Content = symbolO;
                        btn.Foreground = new SolidColorBrush(colorO);
                    }
                }
            }
            UpdateTurnIndicator();
        }
    }
}