using Microsoft.Win32;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Stealthpad
{
    public partial class MainWindow : Window
    {
        double currentOpacity = Properties.Settings.Default.Opacity;
        double startFontsize = Properties.Settings.Default.FontSize;
        double startRowSpacing = Properties.Settings.Default.RowSpacing;
        string? filePath = null;
        Action OnFilePathChanged;
        SoundPlayer soundPlayer = new SoundPlayer();
        public MainWindow()
        {
            InitializeComponent();

            wWindow.Topmost = Properties.Settings.Default.StayOnTop;
            cbPin.IsChecked = wWindow.Topmost;

            SetBackgroundOpacity(currentOpacity);
            sldOpacity.Value = currentOpacity;
            SetRowSpacing(startRowSpacing);
            SetFontSize(startFontsize);
            sldSize.Value = startFontsize;
            sldSpacing.Value = startRowSpacing;
            spSettings.Visibility = Visibility.Collapsed;
            rText.TextChanged += (sender, e) => SetInfo();
            btnSave.IsEnabled = filePath != null;
            OnFilePathChanged += () =>
            {
                btnSave.IsEnabled = filePath != null && File.Exists(filePath);
                btnClose.IsEnabled = filePath != null;
                lblPath.Content = filePath;
            };
        }
        public void OpenFile(string _filePath)
        {
            filePath = _filePath;
            OnFilePathChanged.Invoke();
            string fileContent = File.ReadAllText(filePath);

            TextRange textRange = new TextRange(
                rText.Document.ContentStart,
                rText.Document.ContentEnd
            );

            textRange.Text = fileContent;
        }
        public void DisplayText(string text)
        {

            TextRange textRange = new TextRange(
                rText.Document.ContentStart,
                rText.Document.ContentEnd
            );

            textRange.Text = text;
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void sldOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            currentOpacity = e.NewValue;
            SetBackgroundOpacity(currentOpacity);
        }

        void SetBackgroundOpacity(double opacity)
        {
            rBackground.Opacity = opacity;
            wWindow.InvalidateVisual();
            Properties.Settings.Default.Opacity = (float)opacity;
            Properties.Settings.Default.Save();
        }

        private void sldSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetFontSize(e.NewValue);
        }

        private void SetFontSize(double e)
        {
            if (rText != null) rText.FontSize = (double)e;
            Properties.Settings.Default.FontSize = (float)e;
            Properties.Settings.Default.Save();
        }

        private void cbPin_Click(object sender, RoutedEventArgs e)
        {
            bool pinned = cbPin.IsChecked ?? false;
            SetTopMost(pinned);
        }
        private void SetTopMost(bool topMost)
        {
            wWindow.Topmost = topMost;
            Properties.Settings.Default.StayOnTop = topMost;
            Properties.Settings.Default.Save();
        }

        private void sldSpacing_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetRowSpacing((double)e.NewValue);
        }

        private void SetRowSpacing(double e)
        {
            Properties.Settings.Default.RowSpacing = (float)e;
            Properties.Settings.Default.Save();
            if (rText != null)
            foreach (var block in rText.Document.Blocks)
            {
                block.LineHeight = (double)e;
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            spSettings.Visibility = spSettings.Visibility == Visibility.Visible? Visibility.Collapsed : Visibility.Visible;
            PlayClick(1);
        }
        private void SetInfo()
        {
            //rText.Document.Blocks.
            TextRange textRange = new TextRange(rText.Document.ContentStart, rText.Document.ContentEnd);

            lblInfo.Content = "Character count: " + textRange.Text.Length;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
            var mes = MessageBox.Show("File saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            PlayClick(1);
        }

        private void PlayClick(int e)
        {
            var path = "";
            if (e == 1)
            {
                path = @"E:\\OmatProjektit\\Stealthpad\\Stealthpad\\click.wav";
            }
            if (e == 2)
            {
                path = @"E:\\OmatProjektit\\Stealthpad\\Stealthpad\\click2.wav";
            }
            if (path != "")
            {
                soundPlayer.SoundLocation = path;
                soundPlayer.Play();
            }
        }
        private void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a File to Import";
            if (openFileDialog.ShowDialog() == true)
            {
                // Get the file path selected by the user
                // Read the content of the file (assuming it's a text file)
                OpenFile(openFileDialog.FileName);
            }
        }
        private void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save as";
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"; ;
            if (saveFileDialog.ShowDialog() == true)
            {
                // Get the file path selected by the user
                filePath = saveFileDialog.FileName;
                OnFilePathChanged.Invoke();

                TextRange textRange = new TextRange(
                    rText.Document.ContentStart,
                    rText.Document.ContentEnd
                );

                string fileContent = textRange.Text;
                File.WriteAllText(filePath, fileContent);
            }
        }
        private void Save()
        {
            if (File.Exists(filePath))
            {
                TextRange textRange = new TextRange(
                    rText.Document.ContentStart,
                    rText.Document.ContentEnd
                );

                string fileContent = textRange.Text;
                File.WriteAllText(filePath, fileContent);
            }
        }

        private void rText_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Back || e.Key == Key.Delete)
            //{
            //    PlayClick(2);
            //}
            //else
            //{
            //    PlayClick(1);
            //}


            // Check if Ctrl is held down
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                // Check if the 'D' key is pressed
                if (e.Key == Key.D)
                {
                    // Get the current caret position
                    var caretPosition = rText.CaretPosition;

                    // Find the paragraph where the caret is located
                    var currentParagraph = caretPosition.Paragraph;
                    if (currentParagraph != null)
                    {
                        // Get the text of the current paragraph
                        string currentText = new TextRange(currentParagraph.ContentStart, currentParagraph.ContentEnd).Text;

                        // Create a new paragraph with the same content
                        var newParagraph = new Paragraph();
                        newParagraph.Inlines.Add(currentText);

                        // Insert the new paragraph after the current one
                        rText.Document.Blocks.InsertAfter(currentParagraph, newParagraph);
                        SetRowSpacing(sldSpacing.Value);
                    }

                    // Mark the event as handled to prevent further processing
                    e.Handled = true;
                }
                if (e.Key == Key.F)
                {
                    TextRange textRange = rText.Selection;
                    if(textRange.GetPropertyValue(TextElement.ForegroundProperty) == Brushes.Cyan)
                    {
                        Color customColor = Color.FromArgb(0xFF, 0x0A, 0xFF, 0x5D); // ARGB format
                        SolidColorBrush brush = new SolidColorBrush(customColor);
                        textRange.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
                    }
                    else
                    {
                        textRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Cyan);
                        
                    }
                    e.Handled = true;
                }
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            // Get the current size of the original window
            double originalWidth = this.Width;
            double originalHeight = this.Height;

            if (originalWidth > originalHeight)
            {


                // Set the original window's width to half its original size
                this.Width = originalWidth / 2;

                // Create the new window
                MainWindow newWindow = new MainWindow();

                newWindow.Show();
                // Set the size of the new window to half of the original window's size
                newWindow.Width = originalWidth / 2;
                newWindow.Height = originalHeight;

                // Position the new window next to the original window
                newWindow.Left = this.Left + this.Width;
                newWindow.Top = this.Top;
            }
            else
            {
                // Set the original window's width to half its original size
                this.Height = originalHeight/ 2;

                // Create the new window
                MainWindow newWindow = new MainWindow();

                newWindow.Show();
                // Set the size of the new window to half of the original window's size
                newWindow.Width = originalWidth;
                newWindow.Height = originalHeight / 2;

                // Position the new window next to the original window
                newWindow.Left = this.Left;
                newWindow.Top = this.Top + this.Height;
            }
            // Show the new window
        }

        private void rText_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlayClick(1);
            e.Handled = true;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        private void btnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            filePath = null;
            OnFilePathChanged?.Invoke();
        }



        //private void rText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    // Get the caret position (current insertion point)
        //    var caretPosition = rText.CaretPosition;

        //    // Insert the new character manually (since you're capturing it before it appears)
        //    caretPosition.InsertTextInRun(e.Text);

        //    // Create a TextRange for the newly inserted character
        //    TextRange textRange = new TextRange(caretPosition.GetPositionAtOffset(-e.Text.Length), caretPosition);

        //    // Set color based on the character being inserted
        //    Color customColor = Color.FromArgb(0xFF, 0x0A, 0xFF, 0x5D); // ARGB format
        //    SolidColorBrush brush = new SolidColorBrush(customColor);

        //    if (e.Text == "(" || e.Text == ")")
        //    {
        //        brush = Brushes.Yellow;
        //    }
        //    if (e.Text == "-" || e.Text == "+" || e.Text == "*" || e.Text == "/")
        //    {
        //        brush = Brushes.PowderBlue;
        //    }

        //    // Apply the color to the newly inserted text
        //    textRange.ApplyPropertyValue(TextElement.ForegroundProperty, brush);

        //    // Move the caret to the end of the newly inserted text
        //    rText.CaretPosition = caretPosition.GetPositionAtOffset(1); // Move to the right after the inserted character

        //    e.Handled = true; // Prevent further processing
        //}







    }

}