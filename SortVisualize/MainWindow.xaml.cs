using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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

        private const int NUM_ITEMS = 32;
        private const int MAX_VALUE = 100;
        private const int ZERO_WIDTH = 2;
        private double HEIGHT;
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
            bool swapOccured = true;
            for (int i = 1; i < total && swapOccured; i++)
            {
                swapOccured = false;
                for (int j = 1; j < total - i + 1; j++)
                {
                    colors[j] = blue;
                    colors[j - 1] = blue;
                    // http://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                    this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                    Thread.Sleep(ANIMATION_DELAY);

                    if (data[j - 1] > data[j])
                    {
                        swapOccured = true;
                        temp = data[j - 1];
                        data[j - 1] = data[j];
                        data[j] = temp;

                        colors[j] = green;
                        colors[j - 1] = green;
                        this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                        Thread.Sleep(ANIMATION_DELAY);
                    }
                    colors[j] = violet;
                    colors[j - 1] = violet;
                }
                colors[total - i] = blue;
            }

            for (int i = 0; i < data.Length; i++)
                colors[i] = violet;
            this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
            this.Dispatcher.Invoke((Action)(() => { BubbleSort.IsEnabled = true; }));
        }

        private void CombSort_Click(object sender, RoutedEventArgs e)
        {
            CombSort.IsEnabled = false; //gray-out button to show user that sort is in progress

            populateDataRandomly(ref data);
            syncRectanglesToData();

            ThreadStart threadStart = delegate () { SortDataComb(ref data, ref dataColors); };
            Thread t = new Thread(threadStart);
            t.IsBackground = true; //necessary for thread to quit when the GUI thread quitst.Start();
            t.Start();
        }


        private void SortDataComb(ref int[] data, ref SolidColorBrush[] colors)
        {
            // Comb sort implemented via algorithm on https://en.wikipedia.org/wiki/Comb_sort
            int gap = data.Length; //distance between items being compared (note: starts at Length, but first loop will actually shrink it down to gap/shrinkFactor, so no indexOutOfBounds possible)
            double shrinkFactor = 1.3; // optimal shrink size from algorithm creators

            int temp;
            bool swapOccured = true;
            while (!(gap == 1 && !swapOccured)) // only stop when gap has reached 1 and there are no more swaps
            {
                gap = (int)(gap / shrinkFactor); // reduce gap between items being compared to an integer value....
                if (gap < 1) gap = 1; //...which is always 1 or more

                swapOccured = false;
                for (int i = 0; i + gap < data.Length; i++) //loop through, looking at indices [i] and [gap more than i]
                {
                    colors[i] = blue;
                    colors[i + gap] = blue;
                    this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                    Thread.Sleep(ANIMATION_DELAY);

                    if (data[i] > data[i + gap])
                    {
                        swapOccured = true;
                        temp = data[i];
                        data[i] = data[i + gap];
                        data[i + gap] = temp;

                        colors[i] = green;
                        colors[i + gap] = green;
                        this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                        Thread.Sleep(ANIMATION_DELAY);
                    }
                    colors[i] = violet;
                    colors[i + gap] = violet;
                }
            }
            this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
            this.Dispatcher.Invoke((Action)(() => { CombSort.IsEnabled = true; }));
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

            ThreadStart threadStart = delegate () { SortDataMerge(ref data, ref dataColors); };
            Thread t = new Thread(threadStart);
            t.IsBackground = true; //necessary for thread to quit when the GUI thread quits
            t.Start();
        }

        private void SortDataMerge(ref int[] data, ref SolidColorBrush[] colors)
        {
            mergeSortRecursive(0, data.Length - 1, ref data, ref colors);
            
            for (int i = 0; i < data.Length; i++)
                colors[i] = violet;
            this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
            this.Dispatcher.Invoke((Action)(() => { MergeSort.IsEnabled = true; }));

        }

        private void mergeSortRecursive(int indexStart, int indexEnd, ref int[] dataSlice, ref SolidColorBrush[] colors)
        {
            //How much does it cost to do this test on every recursion?
            //If the initial call is error check, and if my math for the indices on the recursive calls is correct, no error checking is needed.
            if (indexStart > indexEnd || indexStart < 0 || indexEnd >= dataSlice.Length)
                throw new ArgumentOutOfRangeException("Merge Sort Recursive: Indices must be in order, above 0, and within the bounds of the array.");

            //show what range is being tested in this recursion
            for (int i = indexStart; i <= indexEnd; i++)
                colors[i] = green;
            this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
            Thread.Sleep(ANIMATION_DELAY);

            if (indexStart == indexEnd)
            {
                return;
            }
            else
            {
                for (int i = indexStart; i <= indexEnd; i++)
                    colors[i] = violet;
                //double checked math with example [0-19], works for odd and even integers
                mergeSortRecursive(indexStart, (indexEnd - indexStart) / 2 + indexStart, ref dataSlice, ref colors);
                mergeSortRecursive((indexEnd - indexStart) / 2 + indexStart + 1, indexEnd, ref dataSlice, ref colors);


                for (int i = indexStart; i <= indexEnd; i++)
                    colors[i] = green;
                this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                Thread.Sleep(ANIMATION_DELAY);

                //merge using temporary array
                int lower = indexStart;
                int lowerMax = (indexEnd - indexStart) / 2 + indexStart;
                int upper = (indexEnd - indexStart) / 2 + indexStart + 1;
                int upperMax = indexEnd;

                //pick out items in order
                int[] properValues = new int[indexEnd - indexStart + 1];
                int[] properOrder = new int[indexEnd - indexStart + 1]; //for animation purposes: to remember where the values are in the original array
                for (int i = 0; i < properOrder.Length; i++)
                {
                    if ((upper > upperMax) || (lower <= lowerMax && data[lower] <= data[upper]))
                    {
                        properValues[i] = data[lower];
                        properOrder[i] = lower;
                        lower++;
                    }
                    else
                    {
                        properValues[i] = data[upper];
                        properOrder[i] = upper;
                        upper++;
                    }
                }

                //wipe out visualization of values to show that they are being placed back in
                for (int i = indexStart; i <= indexEnd; i++)
                {
                    data[i] = 0;
                    colors[i] = green;
                }
                this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                Thread.Sleep(ANIMATION_DELAY);

                for (int i = 0; i < properValues.Length; i++)
                {
                    colors[i + indexStart] = blue;
                    data[i + indexStart] = properValues[i];
                    this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                    Thread.Sleep(ANIMATION_DELAY);

                    //MessageBox.Show("");
                }

                //pointer math for each array?
            }
        }
        
        private void QuickSort_Click(object sender, RoutedEventArgs e)
        {
            QuickSort.IsEnabled = false;

            populateDataRandomly(ref data);
            syncRectanglesToData();

            ThreadStart threadStart = delegate () { SortDataQuick(ref data, ref dataColors); };
            Thread t = new Thread(threadStart);
            t.IsBackground = true; //necessary for thread to quit when the GUI thread quits
            t.Start();
        }

        private void SortDataQuick(ref int[] data, ref SolidColorBrush[] colors)
        {
            quickSortRecursive(0, data.Length - 1, ref data, ref colors);

            for (int i = 0; i < data.Length; i++)
                colors[i] = violet;
            this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
            this.Dispatcher.Invoke((Action)(() => { QuickSort.IsEnabled = true; }));

        }

        private void quickSortRecursive(int indexStart, int indexEnd, ref int[] dataSlice, ref SolidColorBrush[] colors)
        {
            /*
            https://en.wikipedia.org/wiki/Quicksort
            quicksort(A, lo, hi)
              if lo < hi
                p = partition(A, lo, hi)
                quicksort(A, lo, p - 1)
                quicksort(A, p + 1, hi)

            partition(A, lo, hi)
                pivot = A[hi]
                i = lo #place for swapping
                for j = lo to hi - 1
                    if A[j] <= pivot
                        swap A[i] with A[j]
                        i = i + 1
                swap A[i] with A[hi]
                return i
            */

            //show what range is being tested in this recursion
            for (int i = indexStart; i <= indexEnd; i++)
                colors[i] = blue;
            this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
            Thread.Sleep(ANIMATION_DELAY);
            
            if (indexStart < indexEnd)
            {
                int partitionIndex = quickSortPartition(indexStart, indexEnd, ref data, ref colors); //partitioning is where the swapping takes place
                for (int i = indexStart; i <= indexEnd; i++)
                    colors[i] = violet;
                quickSortRecursive(indexStart, partitionIndex - 1, ref data, ref colors);
                quickSortRecursive(partitionIndex + 1, indexEnd, ref data, ref colors);
            }
        }

        private int quickSortPartition(int indexStart, int indexEnd, ref int[] data, ref SolidColorBrush[] colors)
        {
            /*
            partition(A, lo, hi)
                pivot = A[hi]
                i = lo #place for swapping
                for j = lo to hi - 1
                    if A[j] <= pivot
                        swap A[i] with A[j]
                        i = i + 1
                swap A[i] with A[hi]
                return i
            */
            int temp;
            int pivotDatum = data[indexEnd]; // save the data magnitude we are using for pivot in a temporary variable (last point chosen as pivotDatum is per "Lomuto partition scheme")
            colors[indexEnd] = green;
            int currPivotIndex = indexStart; // index that will become our actual halfway point (all data less than pivotDatum will go before halfway point)
            for (int i = indexStart; i <= indexEnd - 1; i++) // from indexStart to one less than index of pivotDatum
            {
                colors[i] = green;
                this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                Thread.Sleep(ANIMATION_DELAY);

                if (data[i] <= pivotDatum)
                {
                    colors[currPivotIndex] = green;
                    // swap data[j] and data[currPivotIndex]
                    temp = data[currPivotIndex];
                    data[currPivotIndex] = data[i];
                    data[i] = temp;

                    this.Dispatcher.Invoke((Action)(() => syncRectanglesToData()), System.Windows.Threading.DispatcherPriority.Render);
                    Thread.Sleep(ANIMATION_DELAY);
                    colors[currPivotIndex] = blue;

                    currPivotIndex++;
                }

                colors[i] = blue;
            }
            // swap pivotDatum into the actual pivot index
            //temp = data[indexEnd];
            data[indexEnd] = data[currPivotIndex];
            data[currPivotIndex] = pivotDatum; //temp

            return currPivotIndex; //inform quick sort we have decided on a pivot index
        }

        private string printArray(int[] arr)
        {
            StringBuilder result = new StringBuilder();
            foreach (int item in arr)
                result.Append(item + ",");
            return result.ToString();
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
            HEIGHT = (SortCanvas.ActualHeight / (NUM_ITEMS * 2)); //number of bars and equal space in between
            WIDTH_SCALE = (SortCanvas.ActualWidth - (2.0 * HEIGHT)) / MAX_VALUE; //units: pixels/item. subtract HEIGHT as a left and right margin
        }
    }
}
