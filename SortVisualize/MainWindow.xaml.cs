using System;
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
        private const int ANIMATION_DELAY = 200; //milliseconds
        
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
            t.IsBackground = true; //necessary for thread to quit when the GUI thread quits
            //MessageBox.Show("The GUI thread is going to t.Start(): " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            t.Start();

            //syncRectanglesToData();
            //BubbleSort.IsEnabled = true;
        }


        private void SortDataBubble(ref int[] data, ref SolidColorBrush[] colors)
        {
            //MessageBox.Show("Yes, this thread actually launched. " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            int temp;
            int total = data.Length;
            for (int i = 1; i < total; i++)
            {
                for (int j = 1; j < total - i + 1; j++)
                {
                    colors[j - 1] = green;
                    //http://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
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
                colors[total - i] = blue;
            }
            this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
            this.Dispatcher.Invoke((Action)(() => { BubbleSort.IsEnabled = true; }));
        }


        private void SelectionSort_Click(object sender, RoutedEventArgs e)
        {
            SelectionSort.IsEnabled = false;

            populateDataRandomly(ref data);
            syncRectanglesToData();

            ThreadStart threadStart = delegate ()
            {
                SortDataSelection(ref data, ref dataColors);
            };
            Thread t = new Thread(threadStart);
            t.IsBackground = true; //necessary for thread to quit when the GUI thread quits
            t.Start();
        }

        private void SortDataSelection(ref int[] data, ref SolidColorBrush[] colors)
        {
            int temp;
            int largestIndex;
            int total = data.Length;
            for (int i = 0; i < total; i++)
            {
                largestIndex = 0;
                colors[largestIndex] = green;
                this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                Thread.Sleep(ANIMATION_DELAY);

                for (int j = 1; j < total - i; j++)
                {
                    colors[j] = blue;
                    this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                    Thread.Sleep(ANIMATION_DELAY);

                    if (data[j] >= data[largestIndex])
                    {
                        colors[largestIndex] = violet;
                        largestIndex = j;
                        colors[largestIndex] = green;
                        this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                        Thread.Sleep(ANIMATION_DELAY);
                    }
                    else
                    {
                        colors[j] = violet;
                    }
                }

                if (largestIndex != total - i - 1)
                {
                    colors[total - i - 1] = green;
                    this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                    Thread.Sleep(ANIMATION_DELAY);

                    temp = data[total - i - 1];
                    data[total - i - 1] = data[largestIndex];
                    data[largestIndex] = temp;
                }
                colors[largestIndex] = violet;
                colors[total - i - 1] = blue; //set the location we just put the largest selected item into "blue" for inactive
            }

            for (int i = 0; i < total; i++)
                colors[i] = violet;
            this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
            this.Dispatcher.Invoke((Action)(() => { SelectionSort.IsEnabled = true; }));

        }

        private void MergeSort_Click(object sender, RoutedEventArgs e)
        {
            MergeSort.IsEnabled = false;

            populateDataRandomly(ref data);
            syncRectanglesToData();

            ThreadStart threadStart = delegate ()
            {
                SortDataMerge(ref data, ref dataColors);
            };
            Thread t = new Thread(threadStart);
            t.IsBackground = true; //necessary for thread to quit when the GUI thread quits
            t.Start();
        }

        private void SortDataMerge(ref int[] data, ref SolidColorBrush[] colors)
        {
            mergeSortRecursive(0, data.Length - 1, ref data, ref colors);

            int temp;
            int largestIndex;
            int total = data.Length;
            for (int i = 0; i < total; i++)
            {
                largestIndex = 0;
                colors[largestIndex] = green;
                this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                Thread.Sleep(ANIMATION_DELAY);

                for (int j = 1; j < total - i; j++)
                {
                    colors[j] = blue;
                    this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                    Thread.Sleep(ANIMATION_DELAY);

                    if (data[j] >= data[largestIndex])
                    {
                        colors[largestIndex] = violet;
                        largestIndex = j;
                        colors[largestIndex] = green;
                        this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                        Thread.Sleep(ANIMATION_DELAY);
                    }
                    else
                    {
                        colors[j] = violet;
                    }
                }

                if (largestIndex != total - i - 1)
                {
                    colors[total - i - 1] = green;
                    this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                    Thread.Sleep(ANIMATION_DELAY);

                    temp = data[total - i - 1];
                    data[total - i - 1] = data[largestIndex];
                    data[largestIndex] = temp;
                }
                colors[largestIndex] = violet;
                colors[total - i - 1] = blue; //set the location we just put the largest selected item into "blue" for inactive
            }

            for (int i = 0; i < total; i++)
                colors[i] = violet;
            this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
            this.Dispatcher.Invoke((Action)(() => { MergeSort.IsEnabled = true; }));

        }

        private void mergeSortRecursive(int indexStart, int indexEnd, ref int[] dataSlice, ref SolidColorBrush[] colors)
        {
            for (int i = indexStart; i <= indexEnd; i++)
                colors[i] = green;
            this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
            Thread.Sleep(ANIMATION_DELAY);
            for (int i = indexStart; i <= indexEnd; i++)
                colors[i] = violet;

            if (indexStart == indexEnd)
            {
                return;
            }
            else
            {
                //TODO: I haven't even thought about if my "halving" math is correct here!
                mergeSortRecursive(indexStart, (indexEnd - indexStart) / 2, ref dataSlice, ref colors);
                mergeSortRecursive((indexEnd - indexStart) / 2 + 1, indexEnd, ref dataSlice, ref colors);
            }
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
            bars = new Rectangle[NUM_ITEMS];
            dataColors = new SolidColorBrush[NUM_ITEMS];
            for (int i = 0; i < NUM_ITEMS; i++)
            {
                //create a Row in the proper Grid for each text box
                RowDefinition rowDef = new RowDefinition();
                //rowDef.Height = new GridLength(2.0 * HEIGHT);
                rowDef.Height = new GridLength(1.0, GridUnitType.Star);
                TxtBoxGrid.RowDefinitions.Add(rowDef);
                
                textBoxes[i] = new TextBox();
                textBoxes[i].TextAlignment = TextAlignment.Right;
                TxtBoxGrid.Children.Add(textBoxes[i]);
                textBoxes[i].SetValue(Grid.RowProperty, i); //attach i to the Grid.Row property, ie. <TextBox Grid.Row="i"/>

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
