using System;
using System.Drawing;
using System.Windows.Forms;
using forza4_csharp;

namespace TestProject;

public partial class Form1 : Form
{
    private readonly Forza4 game = new(6, 7);
    private int currentPlayer = 1;
    private readonly Label statusLabel;
    private readonly Button[] columnButtons;
    private bool isGameOver;
    private readonly List<Point> winningCells = new();
    public Form1()
    {
        Text = "Forza 4";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 420;
        Height = 500;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        BackColor = Color.FromArgb(20, 20, 30);
        MaximizeBox = false;
        MinimizeBox = false;

        var title = new Label
        {
            Text = "Forza 4",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 18)
        };

        statusLabel = new Label
        {
            Text = "Giocatore 1: scegli una colonna",
            ForeColor = Color.LightGray,
            Font = new Font("Segoe UI", 11),
            AutoSize = true,
            Location = new Point(20, 60)
        };

        var panel = new Panel
        {
            Location = new Point(20, 95),
            Size = new Size(380, 300),
            BackColor = Color.FromArgb(40, 40, 55),
            BorderStyle = BorderStyle.FixedSingle
        };

        var boardTable = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 7,
            RowCount = 6,
            BackColor = Color.Transparent,
        };

        for (int c = 0; c < 7; c++)
        {
            boardTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28F));
        }

        for (int r = 0; r < 6; r++)
        {
            boardTable.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66F));
        }

        for (int r = 0; r < 6; r++)
        {
            for (int c = 0; c < 7; c++)
            {
                var cell = new Label
                {
                    Text = "",
                    Font = new Font("Segoe UI", 20, FontStyle.Bold),
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.FromArgb(60, 60, 80)
                };
                boardTable.Controls.Add(cell, c, r);
            }
        }

        panel.Controls.Add(boardTable);

        columnButtons = new Button[7];
        var buttonsPanel = new FlowLayoutPanel
        {
            Location = new Point(20, 410),
            Size = new Size(380, 60),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent
        };

        for (int c = 0; c < 7; c++)
        {
            int column = c;
            var btn = new Button
            {
                Text = (c + 1).ToString(),
                Width = 45,
                Height = 40,
                BackColor = Color.FromArgb(90, 140, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.Click += (_, _) => DropToken(column);
            columnButtons[c] = btn;
            buttonsPanel.Controls.Add(btn);
        }

        Controls.Add(title);
        Controls.Add(statusLabel);
        Controls.Add(panel);
        Controls.Add(buttonsPanel);

        KeyPreview = true;
        KeyDown += Form1_KeyDown;

        game.createbord();
        UpdateBoard();
    }

    private void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        if (TryGetColumnFromKey(e.KeyCode, out int column))
        {
            DropToken(column);
            e.SuppressKeyPress = true;
        }
    }

    private static bool TryGetColumnFromKey(Keys keyCode, out int column)
    {
        if (keyCode >= Keys.D1 && keyCode <= Keys.D7)
        {
            column = keyCode - Keys.D1;
            return true;
        }

        if (keyCode >= Keys.NumPad1 && keyCode <= Keys.NumPad7)
        {
            column = keyCode - Keys.NumPad1;
            return true;
        }

        column = -1;
        return false;
    }

    private void DropToken(int column)
    {
        if (isGameOver)
        {
            ShowPlayAgainDialog();
            return;
        }

        int result = game.drop(column, currentPlayer);
        if (result == -1)
        {
            statusLabel.Text = "Colonna piena: scegli un'altra colonna.";
            return;
        }

        UpdateBoard();

        int winner = game.checkwin();
        if (winner != -1)
        {
            isGameOver = true;
            winningCells.Clear();
            winningCells.AddRange(FindWinningCells(winner));
            DisableColumnButtons();
            UpdateBoard();
            statusLabel.Text = $"Ha vinto il Giocatore {winner}!";
            ShowPlayAgainDialog();
            return;
        }

        if (game.IsBoardFull())
        {
            isGameOver = true;
            DisableColumnButtons();
            statusLabel.Text = "PAREGGIO!";
            ShowPlayAgainDialog();
            return;
        }

        currentPlayer = currentPlayer == 1 ? 2 : 1;
        statusLabel.Text = $"Giocatore {currentPlayer}: scegli una colonna";
    }

    private void DisableColumnButtons()
    {
        foreach (var btn in columnButtons)
        {
            btn.Enabled = false;
        }
    }

    private List<Point> FindWinningCells(int player)
    {
        for (int r = 0; r < 6; r++)
        {
            for (int c = 0; c < 7; c++)
            {
                if (game.Board[r, c] != player)
                {
                    continue;
                }

                if (c + 3 < 7 &&
                    game.Board[r, c + 1] == player &&
                    game.Board[r, c + 2] == player &&
                    game.Board[r, c + 3] == player)
                {
                    return new List<Point>
                    {
                        new(r, c),
                        new(r, c + 1),
                        new(r, c + 2),
                        new(r, c + 3)
                    };
                }

                if (r + 3 < 6 &&
                    game.Board[r + 1, c] == player &&
                    game.Board[r + 2, c] == player &&
                    game.Board[r + 3, c] == player)
                {
                    return new List<Point>
                    {
                        new(r, c),
                        new(r + 1, c),
                        new(r + 2, c),
                        new(r + 3, c)
                    };
                }

                if (r + 3 < 6 && c + 3 < 7 &&
                    game.Board[r + 1, c + 1] == player &&
                    game.Board[r + 2, c + 2] == player &&
                    game.Board[r + 3, c + 3] == player)
                {
                    return new List<Point>
                    {
                        new(r, c),
                        new(r + 1, c + 1),
                        new(r + 2, c + 2),
                        new(r + 3, c + 3)
                    };
                }

                if (r + 3 < 6 && c - 3 >= 0 &&
                    game.Board[r + 1, c - 1] == player &&
                    game.Board[r + 2, c - 2] == player &&
                    game.Board[r + 3, c - 3] == player)
                {
                    return new List<Point>
                    {
                        new(r, c),
                        new(r + 1, c - 1),
                        new(r + 2, c - 2),
                        new(r + 3, c - 3)
                    };
                }
            }
        }

        return new List<Point>();
    }

    private void UpdateBoard()
    {
        foreach (Control control in Controls)
        {
            if (control is Panel panel && panel.Controls.Count > 0 && panel.Controls[0] is TableLayoutPanel table)
            {
                for (int r = 0; r < 6; r++)
                {
                    for (int c = 0; c < 7; c++)
                    {
                        var cell = table.GetControlFromPosition(c, r) as Label;
                        if (cell is null)
                        {
                            continue;
                        }

                        int value = game.Board[r, c];

                        cell.Text = value switch
                        {
                            1 => "X",
                            2 => "O",
                            _ => string.Empty
                        };

                        cell.ForeColor = value switch
                        {
                            1 => Color.DodgerBlue,
                            2 => Color.Red,
                            _ => Color.White
                        };

                        bool isWinningCell = winningCells.Contains(new Point(r, c));
                        cell.BackColor = isWinningCell
                            ? Color.Gold
                            : Color.FromArgb(60, 60, 80);
                        if (isWinningCell)
                        {
                            cell.ForeColor = Color.Black;
                        }
                    }
                }
            }
        }
    }
    private void ShowPlayAgainDialog()
    {
        var dialog = new Form
        {
            Text = "Gioca ancora?",
            StartPosition = FormStartPosition.Manual,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MinimizeBox = false,
            MaximizeBox = false,
            Width = 260,
            Height = 140,
            TopMost = true,
            Location = new Point(Left + Width + 10, Top + 120)
        };

        var message = new Label
        {
            Text = "Vuoi giocare di nuovo?",
            AutoSize = true,
            Location = new Point(20, 20),
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.White
        };

        var okButton = new Button
        {
            Text = "Sì",
            Location = new Point(70, 60),
            Width = 70,
            ForeColor = Color.White,
            DialogResult = DialogResult.OK
        };

        var cancelButton = new Button
        {
            Text = "No",
            Location = new Point(150, 60),
            Width = 70,
            ForeColor = Color.White,
            DialogResult = DialogResult.Cancel
        };

        okButton.Click += (_, _) => dialog.Close();
        cancelButton.Click += (_, _) => dialog.Close();

        dialog.Controls.Add(message);
        dialog.Controls.Add(okButton);
        dialog.Controls.Add(cancelButton);
        dialog.BackColor = Color.FromArgb(30, 30, 40);

        var result = dialog.ShowDialog(this);
        if (result == DialogResult.OK)
        {
            Application.Restart();
        }
        else
        {
            Application.Exit();
        }
    }
}
