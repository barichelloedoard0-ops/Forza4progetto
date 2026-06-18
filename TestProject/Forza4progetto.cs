namespace forza4_csharp;

// Classe principale del gioco: gestisce il tabellone e le regole base.
public class Forza4
{
    int rows= 6;
    int columns = 7;
    int[,] board;

    // Costruttore: crea una matrice con il numero di righe e colonne richieste.
    public Forza4(int rows, int columns)
    {
        this.rows = rows;
        this.columns = columns;
        board = new int[rows, columns];
    }

    // Espone il tabellone al programma grafico senza usare reflection.
    public int[,] Board => board;

    // Inizializza tutte le celle del tabellone a 0, cioè vuote.
    public int createbord()
    { 
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                board[i, j] = 0;
            }
        }

        return 0;
    }

    // Inserisce un gettone nella colonna scelta dal giocatore.
    // Scorre dal fondo verso l'alto per trovare la prima cella libera.
    public int drop(int column, int player)
    {
        for (int i = rows - 1; i >= 0; i--)
        {
            if (board[i, column] == 0)
            {
                board[i, column] = player;
                return 0;
            }
        }

        return -1;
    }

    // Mostra il tabellone corrente nel terminale usando X e O.
    public void PrintBoard()
    {
        for (int i = 0; i < rows; i++)
        {
            Console.Write("|");
            for (int j = 0; j < columns; j++)
            {
                char symbol = board[i, j] switch
                {
                    1 => 'X',
                    2 => 'O',
                    _ => ' '
                };
                Console.Write($" |{symbol}| ");
            }

            Console.WriteLine("|");
        }

        Console.Write("  ");
        for (int j = 0; j < columns; j++)
        {
            Console.Write($"  {j}  ");
        }

        Console.WriteLine();
    }

    // Controlla se il tabellone è pieno: in quel caso la partita finisce in pareggio.
    public bool IsBoardFull()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (board[i, j] == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Controlla se un giocatore ha ottenuto 4 gettoni in fila.
    public int checkwin()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (board[i, j] == 0)
                {
                    continue;
                }

                int player = board[i, j];

                // Controlla una vittoria orizzontale: 4 gettoni sulla stessa riga.
                if (j + 3 < columns && board[i, j + 1] == player && board[i, j + 2] == player && board[i, j + 3] == player)
                {
                    return player;
                }

                // Controlla una vittoria verticale: 4 gettoni sulla stessa colonna.
                if (i + 3 < rows && board[i + 1, j] == player && board[i + 2, j] == player && board[i + 3, j] == player)
                {
                    return player;
                }

                // Controlla una vittoria in diagonale 
                if (i + 3 < rows && j + 3 < columns &&
                    board[i + 1, j + 1] == player &&
                    board[i + 2, j + 2] == player &&
                    board[i + 3, j + 3] == player)
                {
                    return player;
                }
                if (i + 3 < rows && j - 3 >= 0 &&
                    board[i + 1, j - 1] == player &&
                    board[i + 2, j - 2] == player &&
                    board[i + 3, j - 3] == player)
                {
                    return player;
                }
                
            }
        }
        return -1;
    }

}
