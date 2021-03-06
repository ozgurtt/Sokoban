﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sokoban
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ModelLevel levelModel;
        private LevelReader levelReader;
        private Player player;
        private Board board;
        private InfoGrid infoGrid;
        private ModelScore scoreModel;
        private HighScore highScore;
        private HighScoreView highScoreView;
        private LevelEditor levelEditor;
        

        public MainWindow()
        {
            InitializeComponent();

            levelModel = new ModelLevel();
            levelReader = new LevelReader(levelModel);
            highScore = new HighScore();

            highScoreView = new HighScoreView(levelModel, highScore);
            mainGrid.Children.Add(highScoreView);
            highScoreView.Visibility = Visibility.Collapsed;

            fillDropdown();
        }

        //Fill the dropdown menu from the array of maps in levelReader
        private void fillDropdown()
        {
            comboMaps.ItemsSource = levelModel.Maps;
        }

        //Method if highscore clicked
        private void highscore_clicked(object sender, MouseButtonEventArgs e)
        {
            highScoreView.Visibility = Visibility.Visible;
        }

        //Method if mouse hover highscore
        private void highscore_mouseEnter(object sender, MouseEventArgs e)
        {
            labelHighscore.Foreground = Brushes.Black;
        }

        //Method if mouse leave highscore
        private void highscore_mouseLeave(object sender, MouseEventArgs e)
        {
            labelHighscore.Foreground = Brushes.White;
        }

        //Method if start clicked
        private void start_clicked(object sender, MouseButtonEventArgs e)
        {
            startLevel(comboMaps.SelectedItem.ToString());
        }

        //Method if mouse hover start
        private void start_mouseEnter(object sender, MouseEventArgs e)
        {
            labelStart.Foreground = Brushes.Black;
        }

        //Method if mouse leave start
        private void start_mouseLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            labelStart.Foreground = (Brush)bc.ConvertFrom("#FF5EC5F5");
        }

        //Method if level editor clicked
        private void levelEditor_clicked(object sender, MouseButtonEventArgs e)
        {
            startLevelEditor();
        }

        //Method if mouse hover level editor
        private void levelEditor_mouseEnter(object sender, MouseEventArgs e)
        {
            labelLevelEditor.Foreground = Brushes.Black;
        }

        //Method if mouse leave level editor
        private void levelEditor_mouseLeave(object sender, MouseEventArgs e)
        {
            labelLevelEditor.Foreground = Brushes.White;
        }

        private void comboMaps_opened(object sender, EventArgs e)
        {
            fillDropdown();
        }

        public void startLevel(string level)
        {
            levelModel.StartupLevel = level;

            levelReader.readMapString();
            levelReader.readMapObject();
            levelModel.LevelStarted = true;

            //Change the width and height and the title of the window
            this.Width = 16 + (levelModel.ColumnLenght * levelModel.CellSize) + levelModel.InfoGridWidth;
            this.Height = 39 + (levelModel.RowLenght * levelModel.CellSize);
            this.MinHeight = 39 + (8 * levelModel.CellSize);
            this.Title = "Sokoban: " + levelModel.StartupLevel;

            colInfoGrid.Width = new GridLength(levelModel.InfoGridWidth);

            if (board != null)
            {
                board.Visibility = Visibility.Collapsed;
                infoGrid.Visibility = Visibility.Collapsed;
            }

            board = new Board(this.levelModel);
            board.SetValue(Grid.ColumnProperty, 0);
            board.SetValue(Grid.RowProperty, 0);
            gameGrid.Children.Add(board);

            scoreModel = new ModelScore();

            infoGrid = new InfoGrid(this, this.levelModel, scoreModel, highScore);
            infoGrid.SetValue(Grid.ColumnProperty, 1);
            infoGrid.SetValue(Grid.RowProperty, 0);
            gameGrid.Children.Add(infoGrid);

            player = new Player(board, levelModel, scoreModel, this);

            mainGrid.Visibility = Visibility.Collapsed;
            gameGrid.Visibility = Visibility.Visible;
        }

        private void key_Down(object sender, KeyEventArgs e)
        {
            if (levelModel.LevelStarted == true)
            {
                player.keyPressed(e);
            }
        }

        public void mapFinished()
        {
            levelModel.LevelStarted = false;
            InputBox test = new InputBox(this);
            test.VerticalAlignment = VerticalAlignment.Center;
            test.HorizontalAlignment = HorizontalAlignment.Center;
            gameGrid.Children.Add(test);
        }

        public void saveScore(String userName)
        {
            if(userName.Length != 0)
            {
                scoreModel.PlayerName = userName;
            }
            highScore.saveScore(scoreModel, levelModel.StartupLevel);
        }

        public void nextMap()
        {
            for (int i = 0; i < levelModel.Maps.Count(); i++)
            {
                if (levelModel.Maps[i].ToString().Equals(levelModel.StartupLevel))
                {
                    if (i + 1 >= levelModel.Maps.Count())
                    {
                        mainMenu();
                    }
                    else
                    {
                        //Hier komt alle code om een nieuw level te laden. 

                        startLevel(levelModel.Maps[i+1]);

                        string nextmap = levelModel.Maps[i + 1];
                    }
                    break;
                }
            }
        }

        public void mainMenu()
        {
            levelModel.LevelStarted = false;
            board.Visibility = Visibility.Collapsed;
            infoGrid.Visibility = Visibility.Collapsed;
            mainGrid.Visibility = Visibility.Visible;
            
            this.Width = 608;
            this.Height = 439;
        }

        public void startLevelEditor()
        {
            levelEditor = new LevelEditor(levelModel, levelReader);
            windowGrid.Children.Add(levelEditor);
        }
    }
}
