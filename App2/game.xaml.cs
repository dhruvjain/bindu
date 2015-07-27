using App2.Common;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
//using System.Windows.Media;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace App2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class game : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private string modee;

        int nrows = 6;
        int ncols = 5;
        int multiplier = 70;
        int offset = 20;
        int[,] array;
        int thick = 15;
        int scorea=0, scoreb=0;

        int Box_size = 8;
        Box[,] gameBoxes;


        public game()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
               
     StackPanel panel = new StackPanel();

     int i, j;
     Box_size = 9;
     gameBoxes = new Box[Box_size, Box_size];


            // init
     for (i = 0; i < Box_size; i++)
     {
         for (j = 0; j < Box_size; j++)
         {
             gameBoxes[i,j].left = false;
             gameBoxes[i,j].right = false;
             gameBoxes[i,j].down = false;
             gameBoxes[i, j].up = false;
             gameBoxes[i, j].coins = 1;
             gameBoxes[i, j].i = i;
             gameBoxes[i, j].j = j;
             gameBoxes[i,j].boundaries = 0;
         }
     }

     Ellipse rect;

     for (i = 0; i < ncols; i++)
     {
         for (j = 0; j < nrows; j++)
         {
             rect = new Ellipse();
             rect.Stroke = new SolidColorBrush(Windows.UI.Colors.White);
             rect.Fill = new SolidColorBrush(Windows.UI.Colors.White);
             rect.Width = thick;
             rect.Height = thick;
             Canvas.SetLeft(rect, i * multiplier + offset);
             Canvas.SetTop(rect, j * multiplier + offset);
             rect.StrokeThickness = 2;
             can.Children.Add(rect);
         }
     }
            
            array = new int[nrows, ncols];
            gamearea.Children.Add(panel);
            Start_timer();
        }

        public int draw_line_no(int x, int y, int type, int color)
        {
            // type 0 vertical
            if (type == 0) draw_line(offset + y * multiplier, offset + x * multiplier, offset + (y + 1) * multiplier, offset + x * multiplier, color);
            else draw_line(offset + y * multiplier, offset + x * multiplier, offset + y * multiplier, offset + (x+1) * multiplier, color);
            List<Box> l;
            {
                if (type == 0)
                    l = make_edge('H', x, y);
                else
                    l = make_edge('V', x, y);
            }
            if (score == -999) return -1;
            int i;
            for (i=0; i<l.Count; i++) {
                mark_square(l[i].i, l[i].j, color);
                if (color==0) 
                    onescore.Text = (Convert.ToInt32(onescore.Text) + l[i].coins).ToString();
                else
                    twoscore.Text = (Convert.ToInt32(twoscore.Text) + l[i].coins).ToString();
            }
            if (l.Count > 0) return -500;
            return 0;
        }

        public int count_filled_box()
        {
            int count=0;
            int i, j;
            for (i = 0; i < nrows; i++)
            {
                for (j = 0; j < ncols; j++)
                {
                    if (gameBoxes[i, j].boundaries == 4) count++;
                }
            }
            return count;
        }



        DispatcherTimer timer = new DispatcherTimer();
        public void Start_timer()
        {
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(00, 0, 1);
            bool enabled = timer.IsEnabled;
            timer.Start();
        }

        int turn=0;

        void timer_Tick(object sender, object e)
        {
            int x = draw_random_line(turn);
            if (x == 10)
            {
                if (Convert.ToInt32(onescore.Text) > Convert.ToInt32(twoscore.Text)) 
                    Frame.Navigate(typeof(finish), "Player 1 won.");
                else
                    Frame.Navigate(typeof(finish), "Player 2 won.");
                timer.Stop();
            }
            else if (x != -500)
            {
                if (turn == 0) turn = 1;
                else turn = 0;
            }

        }

        public void initi()
        {

        }

        public int draw_random_line(int x)
        {
            Random r = new Random();
            int a, b, c;
            if (count_filled_box() == (nrows-1) * (ncols-1)) return 10;
            int y;
            do
            {
                while (true)
                {
                    a = r.Next(0, nrows);
                    b = r.Next(0, ncols);
                    c = r.Next(0, 2);
                    if ((a == (nrows - 1) && c == 1) || (b == (ncols - 1) && c == 0))
                        continue;
                    else break;
                }
                y = draw_line_no(a, b, c, x);
            } while (
            y == -1);
            if (y == -500) return -500;
            return 0;
        }

        private void Text(double x, double y, string text, Color color)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = new SolidColorBrush(color);
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y); 
            textBlock.FontSize = 16;
            can.Children.Add(textBlock);
        }

        private void draw_line(int x1, int y1, int x2, int y2 , int x)
        {
            Line line = new Line();
            line.X1 = x1 + thick / 2;
            line.X2 = x2 + thick / 2;
            line.Y1 = y1 + thick / 2;
            line.Y2 = y2 + thick / 2; 
            line.StrokeThickness = 5;
            if (x==0)
                line.Stroke = new SolidColorBrush(Windows.UI.Colors.Red);
            else
                line.Stroke = new SolidColorBrush(Windows.UI.Colors.Green);
            can.Children.Add(line);
        }

        private int find_box(int x, int y)
        {
            int i, j;
            for (i = 0; i < ncols; i++)
            {
                if (x > i * multiplier + offset + thick / 2 && x < multiplier - thick / 2 + i * multiplier + offset + thick / 2) {
                    for (j = 0; j < nrows; j++)
                    {
                        if (y > j * multiplier + offset + thick / 2 && y < j * multiplier + offset + thick / 2 + multiplier - thick / 2) 
                        {
                            return i*100 +j;
                        }
                    }
                }
             }
            return -1;
        }
        public void reseteverything()
        {

        }
        private void add_rand_score(int[,] a, int ncols, int nrows, string mode)
        {
            Random r = new Random();
            Random s = new Random();
            int i, j, temp;
            reseteverything();
            for (i = 1; i < ncols; i++)
            {
                for (j = 1; j < nrows; j++)
                {
                    switch (mode)
                    {
                        case "beginner":
                            gameBoxes[i, j].coins = 1;
                            break;
                        case "intermediate":
                            temp = r.Next(1, 13);
                            gameBoxes[i - 1, j - 1].coins = temp;
                            Text(j * multiplier - 10, i * multiplier - offset, temp.ToString(), Windows.UI.Colors.White);
                            break;
                        case "expert":
                            temp = r.Next(-10, 13);
                            gameBoxes[i - 1, j - 1].coins = temp;
                            Text(j * multiplier - 10, i * multiplier - offset,temp.ToString(), Windows.UI.Colors.White);
                            break;
                        case "huha":
                            temp = r.Next(-10, 13);
                            gameBoxes[i-1, j-1].coins = temp;
                            if (s.Next(1, 10) >3) {
                                Text(j * multiplier - 10, i * multiplier - offset, temp.ToString(), Windows.UI.Colors.White);
                            }
                            break;
                        default:
                            break;
                    }
                    //a[i,j] = r.Next(1, 13);
                    //Text(j*70-10, i*70-20,a[i,j] .ToString(), Windows.UI.Colors.White);
                }
            }
        }

        private void mark_square(int x, int y, int color)
        {
            Rectangle rect;
            rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(Windows.UI.Colors.White);
            if (color==0)
                rect.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
            else
                rect.Fill = new SolidColorBrush(Windows.UI.Colors.Green);
            rect.Width = multiplier - thick/2;
            rect.Height = multiplier - thick/2;
            rect.Opacity = 0.3;
            Canvas.SetLeft(rect, y * multiplier + offset + thick / 2);
            Canvas.SetTop(rect, x * multiplier + offset + thick / 2);
            rect.StrokeThickness = 1;
            can.Children.Add(rect);
        }

        public void setmode (string mode) {
            if (mode == "beginner")
                ;
            else
                add_rand_score(array, nrows, ncols, mode);
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }
        //void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        //{
        //    if (this.canvas1 != null)
        //    {
        //        foreach (TouchPoint _touchPoint in e.GetTouchPoints(this.canvas1))
        //        {
        //            if (_touchPoint.Action == TouchAction.Down)
        //            {
        //                // Clear the canvas and capture the touch to it. 
        //                this.canvas1.Children.Clear();
        //                _touchPoint.TouchDevice.Capture(this.canvas1);
        //            }

        //            else if (_touchPoint.Action == TouchAction.Move && e.GetPrimaryTouchPoint(this.canvas1) != null)
        //            {
        //                // This is the first (primary) touch point. Just record its position. 
        //                if (_touchPoint.TouchDevice.Id == e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id)
        //                {
        //                    pt1.X = _touchPoint.Position.X;
        //                    pt1.Y = _touchPoint.Position.Y;
        //                }

        //                // This is not the first touch point. Draw a line from the first point to this one. 
        //                else if (_touchPoint.TouchDevice.Id != e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id)
        //                {
        //                    pt2.X = _touchPoint.Position.X;
        //                    pt2.Y = _touchPoint.Position.Y;

        //                    Line _line = new Line();
        //                    _line.Stroke = new RadialGradientBrush(Colors.White, Colors.Black);
        //                    _line.X1 = pt1.X;
        //                    _line.X2 = pt2.X;
        //                    _line.Y1 = pt1.Y;
        //                    _line.Y2 = pt2.Y;
        //                    _line.StrokeThickness = 2;
        //                    this.canvas1.Children.Add(_line);
        //                }
        //            }

        //            else if (_touchPoint.Action == TouchAction.Up)
        //            {
        //                // If this touch is captured to the canvas, release it. 
        //                if (_touchPoint.TouchDevice.Captured == this.canvas1)
        //                {
        //                    this.canvas1.ReleaseTouchCapture(_touchPoint.TouchDevice);
        //                }
        //            }
        //        }
        //    }
        //}
        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            //twoscore.Text = e.Parameter as string;
            this.modee = e.Parameter as string;
            this.navigationHelper.OnNavigatedTo(e);
            setmode(modee); 
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(onescore.Text) > Convert.ToInt32(twoscore.Text)) 
                Frame.Navigate(typeof(finish), "Player 2");
            else
                Frame.Navigate(typeof(finish), "Player 1");
        }



        public struct Box
        {
            public bool left;
            public bool right;
            public bool up;
            public bool down;
            public int boundaries;
            public int coins;
            public int i;
            public int j;
        };


        public void fill_coins(int mode)
        {
            int i, j;
            switch (mode)
            {
                case 1: //Normal Box
                    for (i = 0; i < Box_size; i++)
                    {
                        for (j = 0; j < Box_size; j++)
                        {
                            gameBoxes[i, j].coins = 1;
                        }
                    }
                    break;
                case 2: //Weighted Box
                    for (i = 0; i < Box_size; i++)
                    {
                        for (j = 0; j < Box_size; j++)
                        {
                            Random rnd = new Random();
                            gameBoxes[i, j].coins = rnd.Next(1, 10);
                        }
                    }
                    break;
                case 3: //Weighted Box with negative coins
                    for (i = 0; i < Box_size; i++)
                    {
                        for (j = 0; j < Box_size; j++)
                        {
                            Random rnd = new Random();
                            gameBoxes[i, j].coins = rnd.Next(0, 10) - 5;
                        }
                    }
                    break;
                case 4: //Weighted Box with Hidden Blocks
                    for (i = 0; i < Box_size; i++)
                    {
                        for (j = 0; j < Box_size; j++)
                        {
                            Random rnd = new Random();
                            gameBoxes[i, j].coins = rnd.Next(0, 20) - 10;
                        }
                    }
                    break;
            }
        }

        int score;
        public List<Box> make_edge(char c, int row, int col)
        {
            score = 0;
            List<Box> list = new List<Box>();
            if (c == 'V')
            {
                if (col == 0 && !gameBoxes[row, 0].left)
                {
                    gameBoxes[row, 0].left = true;
                    gameBoxes[row, 0].boundaries++;
                    if (gameBoxes[row, 0].boundaries == 4) {
                        score += gameBoxes[row, 0].coins;
                        list.Add(gameBoxes[row, 0]);
                    }
                }
                else if (col == Box_size && !gameBoxes[row, Box_size - 1].right)
                {
                    gameBoxes[row, Box_size - 1].right = true;
                    gameBoxes[row, Box_size - 1].boundaries++;
                    if (gameBoxes[row, Box_size - 1].boundaries == 4) {
                        score += gameBoxes[row, Box_size - 1].coins;
                        list.Add(gameBoxes[row, Box_size-1]);
                    }
                }
                else if (col>0 && !gameBoxes[row, col - 1].right && !gameBoxes[row, col].left)
                {
                    gameBoxes[row, col - 1].right = true;
                    gameBoxes[row, col].left = true;
                    gameBoxes[row, col - 1].boundaries++;
                    gameBoxes[row, col].boundaries++;
                    if (gameBoxes[row, col - 1].boundaries == 4) {
                        score += gameBoxes[row, col - 1].coins;
                        list.Add(gameBoxes[row, col - 1]);
                      }
                    if (gameBoxes[row, col].boundaries == 4) {
                        score += gameBoxes[row, col].coins;
                        list.Add(gameBoxes[row, col]);
                    }
                }
                else
                {
                    score = -999;
                }
            }
            else
            {
                if (row == 0 && !gameBoxes[0, col].up)
                {
                    gameBoxes[0, col].up = true;
                    gameBoxes[0, col].boundaries++;
                    if (gameBoxes[0, col].boundaries == 4)
                    {
                        score += gameBoxes[0, col].coins;
                        list.Add(gameBoxes[0, col]);
                    }
                }
                else if (row == Box_size && !gameBoxes[Box_size - 1, col].down)
                {
                    gameBoxes[Box_size - 1, col].down = true;
                    gameBoxes[Box_size - 1, col].boundaries++;
                    if (gameBoxes[Box_size - 1, col].boundaries == 4)
                    {
                        score += gameBoxes[Box_size - 1, col].coins;
                        list.Add(gameBoxes[Box_size - 1, col]);
                    }
                }
                else if (row > 0 && !gameBoxes[row - 1, col].down && !gameBoxes[row, col].up)
                {
                    gameBoxes[row - 1, col].down = true;
                    gameBoxes[row, col].up = true;
                    gameBoxes[row - 1, col].boundaries++;
                    gameBoxes[row, col].boundaries++;
                    if (gameBoxes[row - 1, col].boundaries == 4)
                    {
                        score += gameBoxes[row - 1, col].coins;
                        list.Add(gameBoxes[row - 1, col]);
                    }
                    if (gameBoxes[row, col].boundaries == 4)
                    {
                        score += gameBoxes[row, col].coins;
                        list.Add(gameBoxes[row, col]);
                    }
                }
                else
                {
                    score = -999;
                }
            }
            return list;
        }
    }
}
