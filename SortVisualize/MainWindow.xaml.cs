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
        private SolidColorBrush blue = new SolidColorBrush(Colors.BlueViolet);
        private SolidColorBrush blue_light = new SolidColorBrush(Colors.CornflowerBlue);
        private SolidColorBrush green = new SolidColorBrush(Colors.SpringGreen);
        private SolidColorBrush yellow = new SolidColorBrush(Colors.GreenYellow);

        private const int NUM_ITEMS = 20;
        private const int MAX_VALUE = 100;
        private const int MIN_WIDTH = 2;
        private int HEIGHT;
        private double WIDTH_SCALE;
        private const int DELAY = 100;

        int count = 0;


        private Rectangle[] bars;
        private TextBox[] textBoxes;
        private int[] data; //CHANGE THIS TO A local variable for each sort type!

        public MainWindow()
        {
            data = new int[NUM_ITEMS];
            InitializeComponent();
        }

        private void BubbleSort_Click(object sender, RoutedEventArgs e)
        {
            BubbleSort.IsEnabled = false;

            Random dice = new Random(); //default constructor is system clock-dependant seed

            for (int i = 0; i < NUM_ITEMS; i++)
            {
                data[i] = dice.Next() % MAX_VALUE;
            }
            //FOR TIMER TESTING: redrawRectangles(data);
            MessageBox.Show("pause");


            //textBoxes[count++].Text = Thread.CurrentThread.ManagedThreadId.ToString();
            //System.Timers.Timer myTimer = new System.Timers.Timer(1000.0);
            //myTimer.SynchronizingObject = this;
            //myTimer.Elapsed += myTimer_Elapsed;
            //myTimer.Start();



            int temp;
            for (int i = 1; i < NUM_ITEMS; i++)
            {
                for (int j = 1; j < NUM_ITEMS - i + 1; j++)
                {
                    bars[j - 1].Fill = green;
                    redrawRectangles(data);
                    SortCanvas.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, emptyDelegate);
                    Thread.Sleep(DELAY);

                    if (data[j - 1] > data[j])
                    {
                        bars[j].Fill = blue_light;
                        //redrawRectangles(data);
                        SortCanvas.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, emptyDelegate);
                        Thread.Sleep(DELAY);

                        temp = data[j - 1];
                        data[j - 1] = data[j];
                        data[j] = temp;

                        bars[j].Fill = green;
                        bars[j - 1].Fill = blue_light;
                        redrawRectangles(data);
                        SortCanvas.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, emptyDelegate);
                        Thread.Sleep(DELAY);
                        bars[j].Fill = blue;



                        // https://msdn.microsoft.com/en-us/library/cc189069(VS.95).aspx
                        //Storyboard storyboard = new Storyboard();
                        ////SortCanvas.Resources.Add("unique_id", storyboard);
                        ////ColorAnimation animation = new ColorAnimation();
                        //DoubleAnimation animateThis = new DoubleAnimation();
                        //DoubleAnimation animateNext = new DoubleAnimation();
                        //Duration quarterSecond = new Duration(TimeSpan.FromSeconds(1));
                        //storyboard.Children.Add(animateThis);
                        //storyboard.Children.Add(animateNext);
                        //storyboard.Duration = quarterSecond;


                        //Storyboard.SetTarget(animateThis, bars[j]);
                        //Storyboard.SetTarget(animateNext, bars[j + 1]);
                        //Storyboard.SetTargetProperty(animateThis, new PropertyPath("(Width)"));
                        //Storyboard.SetTargetProperty(animateNext, new PropertyPath("(Width)"));
                        //animateThis.To = MIN_WIDTH + (data[j] * WIDTH_SCALE);
                        //animateNext.To = MIN_WIDTH + (data[j + 1] * WIDTH_SCALE);
                        //storyboard.Begin();
                        //while (storyboard.GetCurrentState() != ClockState.Stopped)
                        //    ;


                    }


                    //MessageBox.Show(string.Format("{0} bars[j].width = {1}\n{2} bars[j+1].width = {3}", data[j], ((bars[j].Width - MIN_WIDTH) / WIDTH_SCALE), data[j + 1], ((bars[j + 1].Width - MIN_WIDTH) / WIDTH_SCALE)));
                    //redrawRectangles(data);

                    //SortCanvas.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, emptyDelegate);
                    //Thread.Sleep(DELAY);
                    bars[j - 1].Fill = blue;
                }
            }

            BubbleSort.IsEnabled = true;
        }
        private static Action emptyDelegate = delegate () { };

        private void myTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //textBoxes[count++].Text = Thread.CurrentThread.ManagedThreadId.ToString();
            Thread.Sleep(2000);
            System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);
        }

        private void MergeSort_Click(object sender, RoutedEventArgs e)
        {
            bars[4].Width = MIN_WIDTH + (50 * WIDTH_SCALE);
            if (bars[4].Fill == green)
                bars[4].Fill = blue;
            else
                bars[4].Fill = green;
            redrawRectangles(data);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updateScale();


            textBoxes = new TextBox[NUM_ITEMS];
            for (int i = 0; i < NUM_ITEMS; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = new GridLength(2.0 * HEIGHT);
                TxtBoxGrid.RowDefinitions.Add(rowDef);
                textBoxes[i] = new TextBox();
                TxtBoxGrid.Children.Add(textBoxes[i]);
                textBoxes[i].SetValue(Grid.RowProperty, i);
            }


            bars = new Rectangle[NUM_ITEMS];

            for (int i = 0; i < NUM_ITEMS; i++)
            {
                bars[i] = new Rectangle();
                bars[i].Fill = blue;
                SortCanvas.Children.Add(bars[i]);
            }

            redrawRectangles(data);

            this.SizeChanged += Window_SizeChanged;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
                updateScale();
                redrawRectangles(data);
        }

        private void redrawRectangles(int[] data)
        {
            for (int i = 0; i < NUM_ITEMS; i++)
            {
                bars[i].Width = MIN_WIDTH + (data[i] * WIDTH_SCALE);
                bars[i].Height = HEIGHT;
                Canvas.SetLeft(bars[i], HEIGHT);
                Canvas.SetTop(bars[i], i * 2 * HEIGHT);
                bars[i].ToolTip = ((bars[i].Width - MIN_WIDTH) / WIDTH_SCALE);
                textBoxes[i].Text = data[i].ToString();
            }
        }

        private void updateScale()
        {
            HEIGHT = (int)SortCanvas.ActualHeight / (NUM_ITEMS * 2 + 1); //number of bars, with equal space in between for each one, plus the bottom/top margin
            WIDTH_SCALE = (SortCanvas.ActualWidth - 2 * HEIGHT) / MAX_VALUE; //subtract HEIGHT as a left and right margin
        }
    }
}
