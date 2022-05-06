using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace ExtensionHistory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string historyPath = "S:\\IT Support Team\\Phone System\\Extension History\\history\\";

        private Extension extensionObject;
        private string extensionNumber;

        //A WPF window constructor, you tell me
        public MainWindow()
        {
            InitializeComponent();
        }

        //Make sure values entered into the extension field are numbers
        private void ExtensionPreview(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Search for the extension, triggerd by button press
        private void SearchExtension(object sender, RoutedEventArgs e)
        {
            _SearchExtension();
        }

        //Search for an extension (but not bound to the button press)
        private void _SearchExtension()
        {
            if (ExtensionInput.Text.Equals(null))
            {
                return;
            }

            extensionNumber = ExtensionInput.Text;

            LoadExtensionHistory(extensionNumber);
            UpdateTable();

            this.Title = "Extension History [Extension: " + extensionNumber + "]";
        }

        //Add history to the extension using the values entered by the user
        private void AddHistory(object sender, RoutedEventArgs e)
        {

            _SearchExtension();

            if (extensionNumber == null)
            {
                return;
            }

            History history = new History();
            history.Name = NameField.Text;
            history.Date = StartDateInput.Text;

            if(NotesInput.Text != "Notes...")
            {
                history.Notes = NotesInput.Text;
            }
            else
            {
                history.Notes = "";
            }

            if(extensionObject.histories == null)
            {
                extensionObject.histories = new List<History>();
            }

            extensionObject.histories.Add(history);

            var jsonString = JsonConvert.SerializeObject(extensionObject, Formatting.Indented);

            File.WriteAllText(historyPath + extensionNumber + ".json", jsonString);

            UpdateTable();
        }

        //Load the extension history from a JSON file
        private void LoadExtensionHistory(string extension)
        {

            string file = historyPath + extension + ".json";

            if (File.Exists(file))
            {
                extensionObject = JsonConvert.DeserializeObject<Extension>(File.ReadAllText(file));
            }
            else
            {
                extensionObject = new Extension();
                extensionObject.histories = new List<History>();
            }
        }

        //Update the table with the current extension history
        private void UpdateTable()
        {

            HistoryGrid.Items.Clear();

            foreach(var history in extensionObject.histories)
            {
                HistoryGrid.Items.Add(history);
            }
        }

    }


    //Some structs to hold the data for extensions and history
    public struct Extension
    {
        public List<History> histories;
    }

    public struct History
    {
        public string Name { set; get; }
        public string Date { set; get; }
        public string Notes { set; get; }
    }
}
