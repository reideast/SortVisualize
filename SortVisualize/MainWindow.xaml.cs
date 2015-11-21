﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Media.Animation;
using System.Timers;

namespace SortVisualize
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Color blue = Colors.AliceBlue;
        private SolidColorBrush violet = new SolidColorBrush(Colors.BlueViolet);
        private SolidColorBrush blue = new SolidColorBrush(Colors.CornflowerBlue);
        private SolidColorBrush green = new SolidColorBrush(Colors.SpringGreen);

        private const int NUM_ITEMS = 20;
        private const int MAX_VALUE = 100;
        private const int ZERO_WIDTH = 2;
        private int HEIGHT;
        private double WIDTH_SCALE;
        private const int ANIMATION_DELAY = 100; //milliseconds
        
        private Rectangle[] bars;
        private TextBox[] textBoxes;
        private int[] data; //CHANGE THIS TO A local variable for each sort type?
        private SolidColorBrush[] dataColors;
        private Random dice = new Random(); //default constructor is system clock-dependent seed

        public MainWindow()
        {
            data = new int[NUM_ITEMS];
            InitializeComponent();
        }

        private void BubbleSort_Click(object sender, RoutedEventArgs e)
        {
            BubbleSort.IsEnabled = false; //gray-out button to show user that sort is in progress

            populateDataRandomly(ref data);
            syncRectanglesToData();

            ThreadStart threadStart = delegate ()
            {
                SortDataBubble(ref data, ref dataColors);
            };
            Thread t = new Thread(threadStart);
            //MessageBox.Show("The GUI thread is going to t.Start(): " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            t.Start();


            //BubbleSort.IsEnabled = true;
        }
        //private static Action emptyDelegate = delegate () { };


        private void SortDataBubble(ref int[] data, ref SolidColorBrush[] colors)
        {
            //MessageBox.Show("Yes, this thread actually launched. " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            int temp;
            for (int i = 1; i < NUM_ITEMS; i++)
            {
                for (int j = 1; j < NUM_ITEMS - i + 1; j++)
                {
                    colors[j - 1] = green;
                    this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                    Thread.Sleep(ANIMATION_DELAY);

                    if (data[j - 1] > data[j])
                    {
                        colors[j] = blue;
                        this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                        Thread.Sleep(ANIMATION_DELAY);

                        temp = data[j - 1];
                        data[j - 1] = data[j];
                        data[j] = temp;

                        colors[j] = green;
                        colors[j - 1] = blue;
                        this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                        Thread.Sleep(ANIMATION_DELAY);
                        colors[j] = violet;
                    }
                    colors[j - 1] = violet;
                }
            }
            this.Dispatcher.Invoke((Action)(() => { BubbleSort.IsEnabled = true; }));
        }


        private void MergeSort_Click(object sender, RoutedEventArgs e)
        {
            MergeSort.IsEnabled = false;

            MergeSort.IsEnabled = true;
        }

        private void populateDataRandomly(ref int[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = dice.Next() % (MAX_VALUE + 1);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Upon loading, the window must determine the size of controls,
            //and then programatically create Rectangles and TextBoxes for each item,
            //placing on the grid in the correct location

            updateDrawingScale();

            //update Grid.RowDefinition height for NUM_ITEMS
            TxtGridOffset.Height = new GridLength(NUM_ITEMS * 4.0 - 2, GridUnitType.Star);

            //create text boxes programatically for the integer values being sorted (for the number of items)
            textBoxes = new TextBox[NUM_ITEMS];
            for (int i = 0; i < NUM_ITEMS; i++)
            {
                //create a Row in the proper Grid for each text box
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(2.0 * HEIGHT);
                TxtBoxGrid.RowDefinitions.Add(rowDef);
                
                textBoxes[i] = new TextBox();
                textBoxes[i].TextAlignment = TextAlignment.Right;
                TxtBoxGrid.Children.Add(textBoxes[i]);
                textBoxes[i].SetValue(Grid.RowProperty, i); //attach i to the Grid.Row property, ie. <TextBox Grid.Row="i"/>
            }
            
            bars = new Rectangle[NUM_ITEMS];
            dataColors = new SolidColorBrush[NUM_ITEMS];
            for (int i = 0; i < NUM_ITEMS; i++)
            {
                bars[i] = new Rectangle();
                dataColors[i] = violet;
                //bars[i].Fill = violet;
                SortCanvas.Children.Add(bars[i]);
            }

            syncRectanglesToData();

            this.SizeChanged += Window_SizeChanged; //add event handler now that the Window is loaded
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
                updateDrawingScale();
                syncRectanglesToData();
        }

        private void syncRectanglesToData()
        {
            for (int i = 0; i < NUM_ITEMS; i++)
            {
                bars[i].Fill = dataColors[i];
                bars[i].Width = ZERO_WIDTH + (data[i] * WIDTH_SCALE);
                bars[i].Height = HEIGHT;
                Canvas.SetLeft(bars[i], HEIGHT); //set <Rectangle Canvas.Left="HEIGHT"/> property with HEIGHT as left margin
                Canvas.SetTop(bars[i], i * 2 * HEIGHT); //use HEIGHT as height of rectangle, and equal spacing between them
                bars[i].ToolTip = ((bars[i].Width - ZERO_WIDTH) / WIDTH_SCALE);
                textBoxes[i].Text = data[i].ToString();
            }
        }

        private void updateDrawingScale()
        {
            HEIGHT = (int) (SortCanvas.ActualHeight / (NUM_ITEMS * 2)); //number of bars and equal space in between
            WIDTH_SCALE = (SortCanvas.ActualWidth - (2.0 * HEIGHT)) / MAX_VALUE; //units: pixels/item. subtract HEIGHT as a left and right margin
        }
    }
}
