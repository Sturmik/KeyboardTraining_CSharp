using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace StyleAndTemplates
{
    /// <summary>
    /// Логика взаимодействия для Keyboard.xaml
    /// </summary>
    public partial class Keyboard : Window
    {
        // User statistic
        public List<UserStatistic> _userStat;
        // Date time for checking how long the training lasted
        DateTime startTime;
        // List for text in different difficulties
        List<string> diffText;
        // Variables for charsPerMinute count
        private int _minCount;
        private int _charCount;
        private DispatcherTimer _timerMin;
        private DispatcherTimer _timerChars;
        // Variable, which defines our style mode
        private bool _dark_light;
        // Variables for working with input analysis
        private List<string> pressedKeys;
        // Variables for working with entered keys
        private string _keyReader;
        private string _checkForKey;
        private string _checkForFunctionKey;
        // Variable for capitalization check
        private bool _capitalization;
        // Variable for pressed shift check
        private bool _shift;
        // Variable for fails amount check
        private int _failsCount;

        public Keyboard()
        {
            InitializeComponent();

            _userStat = new List<UserStatistic>();

            buttonCancel.IsEnabled = false;

            pressedKeys = new List<string>();

            _capitalization = false;
            _dark_light = false;

            _failsCount = 0;
            _charCount = 0;
            _minCount = 1;

            _timerMin = new DispatcherTimer();
            _timerMin.Interval = TimeSpan.FromMinutes(1);
            _timerMin.Tick += TimerMinute_Tick;
            _timerMin.Start();

            _timerChars = new DispatcherTimer();
            _timerChars.Interval = TimeSpan.FromSeconds(1);
            _timerChars.Tick += Timer_Tick;
            _timerChars.Start();

            diffText = new List<string>();
            // Level 5 
            diffText.Add("bob-mim-2040=check1000" +
"[[[]]]---===11122233334444555666677788889999" +
"bvoascmaeodpsbmtrgdspovmcae[0w9cndsobsnddsfdsg" +
"23c4xdg2nm,x4khfs4,g0z895v72ycm23x4,.sdfwsc Easy");
            // Level 6
            diffText.Add("aaa bbb ccc DDD EEE = 2 - 3 = -1" +
" 111 222 333 444 5556789 10 30 = [3.4.5]" +
" shot trtrtrtrtrtrtrtrtr , reloading chick chick drin" +
" damn, It jammed, a a a b b b c c c d d d");
            // Level 7
            diffText.Add("ONCE upon a time there was a prince who wanted to marry a princess;" +
    " but she would have to be a real princess. He travelled all over the world to find one," +
    " but nowhere could he get what he wanted. There were princesses enough," +
    " but it was difficult to find out whether they were real ones." +
    " There was always something about them that was not as it should be." +
    " So he came home again and was sad, for he would have liked very much to have a real princess.");
            // Level 8
            diffText.Add("Lorem Ipsum is simply dummy text of the printing and typesetting industry. " +
"Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, " +
"when an unknown printer took a galley of type and scrambled it to make a type specimen book. " +
"It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged." +
" It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently" +
" with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.");
            // Level 9
            diffText.Add("There are many variations of passages of Lorem Ipsum available, " +
                "but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look " +
                "even slightly believable. If you are going to use a passage of Lorem Ipsum, you need to be sure there isn't anything " +
                "embarrassing hidden in the middle of text. All the Lorem Ipsum generators on the Internet tend to repeat predefined " +
                "chunks as necessary, making this the first true generator on the Internet. It uses a dictionary of over 200 Latin words," +
                " combined with a handful of model sentence structures, to generate Lorem Ipsum which looks reasonable. " +
                "The generated Lorem Ipsum is therefore always free from repetition, injected humour, or non-characteristic words etc.");
            // Level 10
            diffText.Add("Contrary to popular belief, Lorem Ipsum is not simply random text. " +
    "It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. " +
    "Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words," +
    " consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature," +
    " discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33" +
    " of de Finibus Bonorum et Malorum The Extremes of Good and Evil by Cicero, written in 45 BC." +
    " This book is a treatise on the theory of ethics, very popular during the Renaissance." +
    " The first line of Lorem Ipsum, 'Lorem ipsum dolor sit amet..', comes from a line in section 1.10.32." +
    "The standard chunk of Lorem Ipsum used since the 1500s is reproduced below for those interested." +
    " Sections 1.10.32 and 1.10.33 from de Finibus Bonorum et Malorum by Cicero are also reproduced in their exact original form," +
    " accompanied by English versions from the 1914 translation by H.Rackham.");

            buttonTheme_Click(default, default);
        }

        #region EventHandle

        private void buttonStatistic_Click(object sender, RoutedEventArgs e)
        {
            Statistic toShow = new Statistic(_userStat);
            toShow.Owner = this;
            toShow.ShowDialog();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            StartProtocol();
            textBoxWorkWith.Text = diffText[(int)sliderDiff.Value - 5];
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            StopProtocol();
        }

        private void buttonTheme_Click(object sender, RoutedEventArgs e)
        {
            _ = _dark_light == true ? _dark_light = false : _dark_light = true;

            Uri uri;

            if (_dark_light == false)
                uri = new Uri("ResourceLibrary\\LightTheme" + ".xaml", UriKind.Relative);
            else
                uri = new Uri("ResourceLibrary\\DarkTheme" + ".xaml", UriKind.Relative);

            // Loading resources
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            // Clearing resource collection
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Clear();
            // Loading new dictionary
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);

            ButtonsColorUpdate();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _keyReader = e.Key.ToString().ToLower();
            if (pressedKeys.Contains(_keyReader)) return;
            if (_keyReader == "capital") Capitalization();
            InputCheck(_keyReader);
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            _keyReader = e.Key.ToString().ToLower();
            InputCheck(_keyReader);

            if (_shift == true)
                if (!pressedKeys.Contains("leftshift") && !pressedKeys.Contains("rightshift"))
                {
                    Capitalization();
                    _shift = false;
                }

            if (buttonStart.IsEnabled == false)
                if (textBoxWorkWith.Text.Length == 0)
                {
                    StopProtocol();
                    textBoxWorkWith.Text = "Time: " + (DateTime.Now - startTime).ToString();
                    MessageBox.Show("Training complete! Your result: " +
                       $"\nTime: {textBoxWorkWith.Text}" +
                       $"\nChars per minute: {textBlockCharsMin.Text}" +
                       $"\nFails count: {textBlockFailsCount.Text}",
                       "Results!", MessageBoxButton.OK, MessageBoxImage.Information);
                    _userStat.Add(new UserStatistic(DateTime.Now,
                        (int)sliderDiff.Value,
                        DateTime.Now - startTime,
                        int.Parse(textBlockCharsMin.Text),
                        int.Parse(textBlockFailsCount.Text)));
                    SaveStat();
                }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            textBlockCharsMin.Text = (_charCount / _minCount).ToString();
        }

        private void TimerMinute_Tick(object sender, EventArgs e)
        {
            _minCount++;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStat();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks key to highlight or disable
        /// </summary>
        /// <returns></returns>
        private bool InputCheck(string keyRead)
        {
            bool checkForInput = false;
            DockPanel dockPan;
            for (int i = 0; i < gridKeyboard.Children.Count; i++)
            {
                dockPan = gridKeyboard.Children[i] as DockPanel;
                for (int j = 0; j < dockPan.Children.Count; j++)
                {
                    // Checking for input

                    _checkForKey = (dockPan.Children[j] as Button).Content.ToString().ToLower();

                    if (keyRead.Equals(_checkForKey))
                        checkForInput = true;
                    if ((dockPan.Children[j] as Button).Tag != null)
                    {
                        _checkForFunctionKey = (dockPan.Children[j] as Button).Tag.ToString().ToLower();
                        if (keyRead.Equals(_checkForFunctionKey))
                            checkForInput = true;
                    }

                    // If input is true, 
                    // the button will be highlighted or disabled

                    if (checkForInput == true)
                    {
                        Button handle = (dockPan.Children[j] as Button);

                        if (pressedKeys.Contains(keyRead))
                        {
                            if (_dark_light == true)
                                handle.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                            else
                                handle.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                            pressedKeys.Remove(keyRead);
                        }
                        else
                        {
                            // Here, we will check user input

                            handle.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                            pressedKeys.Add(keyRead);

                            if (textBoxWorkWith.Text.Length > 0)
                            {
                                if (handle.Content.ToString().Length == 1)
                                {
                                    char enteredVal;
                                    char compareWith;

                                    // Checking for case sensitive 
                                    if (checkSensitive.IsChecked == true)
                                    {
                                        enteredVal = handle.Content.ToString()[0];
                                        compareWith = textBoxWorkWith.Text[0];
                                    }
                                    else
                                    {
                                        enteredVal = handle.Content.ToString().ToLower()[0];
                                        compareWith = textBoxWorkWith.Text.ToLower()[0];
                                    }

                                    if (enteredVal == compareWith)
                                    {
                                        _charCount++;
                                        textBoxWorkWith.Text = textBoxWorkWith.Text.Remove(0, 1);
                                    }
                                    else
                                    {
                                        _failsCount++;
                                        textBlockFailsCount.Text = _failsCount.ToString();
                                    }
                                }
                                else
                                if (handle.Content.ToString() == "Space")
                                {
                                    if (' ' == textBoxWorkWith.Text[0])
                                    {
                                        textBoxWorkWith.Text = textBoxWorkWith.Text.Remove(0, 1);

                                    }
                                    else
                                    {
                                        _failsCount++;
                                        textBlockFailsCount.Text = _failsCount.ToString();
                                    }
                                }
                            }

                        }

                        if (handle.Content.ToString() == "Shift" && _shift != true)
                        {
                            _shift = true;
                            Capitalization();
                        }

                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Capitalization. Changes characters register
        /// </summary>
        /// <param name="small_big">Makes characters small/big</param>
        private void Capitalization()
        {
            DockPanel dockPan;
            Button workWith;
            for (int i = 1; i < gridKeyboard.Children.Count - 1; i++)
            {
                dockPan = gridKeyboard.Children[i] as DockPanel;
                for (int j = 0; j < dockPan.Children.Count; j++)
                {
                    workWith = dockPan.Children[j] as Button;

                    if (char.IsLetter(workWith.Content.ToString()[0]) == true)
                    {
                        if (workWith.Content.ToString().Length > 1) continue;

                        if (_capitalization == true)
                            workWith.Content = workWith.Content.ToString().ToLower();
                        else
                            workWith.Content = workWith.Content.ToString().ToUpper();
                    }
                }
            }

            _ = _capitalization == true ? _capitalization = false : _capitalization = true;
        }

        /// <summary>
        /// Saves user statistic
        /// </summary>
        private void SaveStat()
        {
            if (!Directory.Exists("UserStat"))
                Directory.CreateDirectory("UserStat");
            FileStream FS = new FileStream("UserStat\\" + Tag.ToString(), FileMode.Create, FileAccess.Write);
            BinaryFormatter BF = new BinaryFormatter();
            BF.Serialize(FS, _userStat);
            FS.Flush();
            FS.Close();
        }

        /// <summary>
        /// Loads user statistic
        /// </summary>
        private void LoadStat()
        {
            if (!Directory.Exists("UserStat")) return;
            if (!File.Exists("UserStat\\" + Tag.ToString())) return;
            FileStream FS = new FileStream("UserStat\\" + Tag.ToString(), FileMode.Open, FileAccess.Read);
            BinaryFormatter BF = new BinaryFormatter();
            _userStat = BF.Deserialize(FS) as List<UserStatistic>;
            FS.Flush();
            FS.Close();
        }

        /// <summary>
        /// Function for start protocol
        /// </summary>
        private void StartProtocol()
        {
            startTime = DateTime.Now;
            checkSensitive.IsEnabled = false;
            sliderDiff.IsEnabled = false;
            buttonStart.IsEnabled = false;
            buttonTheme.IsEnabled = false;
            buttonCancel.IsEnabled = true;
            textBlockCharsMin.Text = "0";
            textBlockFailsCount.Text = "0";
            _failsCount = 0;
            _minCount = 1;
            _charCount = 0;
            _timerMin.Start();
            _timerChars.Start();
        }
        /// <summary>
        /// Function for stop protocol
        /// </summary>
        private void StopProtocol()
        {
            checkSensitive.IsEnabled = true;
            sliderDiff.IsEnabled = true;
            buttonStart.IsEnabled = true;
            buttonTheme.IsEnabled = true;
            buttonCancel.IsEnabled = false;
            textBoxWorkWith.Text = "";
            _failsCount = 0;
            _minCount = 1;
            _charCount = 0;
            _timerMin.Stop();
            _timerChars.Stop();
            ButtonsColorUpdate();
        }

        /// <summary>
        /// Buttons update color
        /// </summary>
        private void ButtonsColorUpdate()
        {
            Button handle;
            DockPanel dockPan;
            for (int i = 0; i < gridKeyboard.Children.Count; i++)
            {
                dockPan = gridKeyboard.Children[i] as DockPanel;
                for (int j = 0; j < dockPan.Children.Count; j++)
                {
                    handle = dockPan.Children[j] as Button;

                    if (_dark_light == true)
                        handle.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    else
                        handle.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
            }
        }

        #endregion
    }
}

/// <summary>
/// Statistic, which holds user results
/// </summary>
[Serializable]
public class UserStatistic
{
    public DateTime DateOfPass { get; set; }
    public int DifficultyLevel { get; set; }
    public TimeSpan WorkTime { get; set; }
    public int CharsPerMinute { get; set; }
    public int FailsCount { get; set; }

    public UserStatistic(DateTime dateOfPass, int difficultyLevel,TimeSpan workTime, int charsPerMinute, int failsCount)
    {
        DateOfPass = dateOfPass;
        DifficultyLevel = difficultyLevel;
        WorkTime = workTime;
        CharsPerMinute = charsPerMinute;
        FailsCount = failsCount;
    }
}
