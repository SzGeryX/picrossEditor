using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace Picross
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<List<bool>> solutionList = new List<List<bool>>();
        List<List<bool>> gameList = new List<List<bool>>();
        List<List<Button>> buttons = new List<List<Button>>();
        List<Label> lblRow = new List<Label>();
        List<Label> lblCol = new List<Label>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void generateGame(int size)
        {
            game.Children.Clear();
            game.ColumnDefinitions.Clear();
            game.RowDefinitions.Clear();
            lblCol.Clear();
            lblRow.Clear();
            buttons.Clear();

            for (int i = 0; i < size + 1; i++) 
            {
                game.ColumnDefinitions.Add(new ColumnDefinition());
                game.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < size + 1; i++)
            {
                for (int j = 0; j < size + 1; j++)
                {
                    if (!(i == 0 ^ j == 0)) continue;

                    Label temp = new Label();

                    temp.Content = $"0";

                    temp.Background = Brushes.Gray;
                    temp.HorizontalAlignment = HorizontalAlignment.Center;
                    temp.VerticalAlignment = VerticalAlignment.Center;
                    if (j == 0) temp.FlowDirection = 

                    Grid.SetColumn(temp, i);
                    Grid.SetRow(temp, j);

                    game.Children.Add(temp);

                    if (i == 0) lblCol.Add(temp);
                    if (j == 0) lblRow.Add(temp);
                }
            }

            for (int i = 1; i < size + 1; i++)
            {
                List<Button> tempButtons = new List<Button>();
                for (int j = 1; j < size + 1; j++)
                {
                        Button temp = new Button();

                        temp.Click += flipTile;
                        temp.Background = Brushes.White;
                        temp.Content = $"{i}, {j}";

                        Grid.SetColumn(temp, i);
                        Grid.SetRow(temp, j);

                        game.Children.Add(temp);
                        tempButtons.Add(temp);
                }

                buttons.Add(tempButtons);
            }
        }

        private void sizeSelection_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int size = 0;

            switch (btn.Name)
            {
                case "btn5":
                    size = 5;
                    break;
                case "btn10":
                    size = 10;
                    break;
                case "btn15":
                    size = 15;
                    break;
                case "btn20":
                    size = 20;
                    break;
            }

            clearGame(size);
        }

        private void clearGame(int size)
        {
            gameList.Clear();

            for (int i = 0; i < size; i++)
            {
                List<bool> temp = new List<bool>();
                for (int j = 0; j < size; j++)
                {
                    temp.Add(false);
                }

                gameList.Add(temp);
            }

            generateGame(size);
        }

        private void flipTile(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            int y = Grid.GetColumn(btn) - 1;
            int x = Grid.GetRow(btn) - 1;

            btn.Background = btn.Background == Brushes.White ? Brushes.CadetBlue : Brushes.White;

            gameList[x][y] = !gameList[x][y];

            colHelp();
            rowHelp();
        }

        private void rowHelp()
        {
            for (int i = 0; i < gameList.Count; i++)
            {
                lblCol[i].Content = String.Join(" ", calculateRowHelp(i));
            }   
        }

        private void colHelp()
        {
            for (int i = 0; i < gameList.Count; i++)
            {
                lblRow[i].Content = String.Join(" ", calculateColHelp(i));
            }   
        }

        private List<int> calculateColHelp(int col)
        {
            List<int> result = new List<int>();
            bool chain = false;

            for (int i = 0; i < gameList.Count; i++)
            {
                if (gameList[i][col] == true && chain == true)
                {
                    result[result.Count - 1]++;
                }

                if (gameList[i][col] == true && chain == false)
                {
                    result.Add(1);
                    chain = true;
                }

                if (gameList[i][col] == false)
                {
                    chain = false;
                }
            }

            return result.Count == 0 ? [0] : result;
        }

        private List<int> calculateRowHelp(int row)
        {
            List<int> result = new List<int>();
            bool chain = false;

            for (int i = 0; i < gameList[row].Count; i++)
            {
                if (gameList[row][i] == true && chain == true)
                {
                    result[result.Count - 1]++;
                }

                if (gameList[row][i] == true && chain == false)
                {
                    result.Add(1);
                    chain = true;
                }

                if (gameList[row][i] == false)
                {
                    chain = false;
                }
            }

            return result.Count == 0 ? [0] : result;
        }
        
        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            int size = game.ColumnDefinitions.Count - 1;

            clearGame(size);
        }

        private void loadSolution()
        {
            int size = solutionList.Count;

            clearGame(size);


            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (solutionList[i][j] == true)
                    {
                        buttons[j][i].RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    }
                }
            }

            colHelp();
            rowHelp();
        }

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            solutionList.Clear();

            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() != true) return;

            var file = File.ReadAllLines(ofd.FileName);

            foreach (var line in file )
            {
                Debug.WriteLine(line.Split(';').ToArray()
                    .Select(x => Convert.ToBoolean(Convert.ToInt32(x)))
                    .ToList<bool>());
                    solutionList.Add(line.Split(';').ToArray()
                    .Select(x => Convert.ToBoolean(Convert.ToInt32(x)))
                    .ToList<bool>());
            }

            loadSolution();
        }

        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            if (sfd.ShowDialog() != true) return;

            File.WriteAllLines(sfd.FileName, gameList.Select(x => String.Join(";", x.Select(y => Convert.ToInt32(y)))));
        }
    }
}