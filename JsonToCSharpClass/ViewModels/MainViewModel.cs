using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using JsonToCSharpClass.Views;

namespace JsonToCSharpClass.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private string _jsonInput;

        public string JsonInput
        {
            get => _jsonInput;
            set => SetProperty(ref _jsonInput, value);
        }

        private string _cSharpOutput;

        public string CSharpOutput
        {
            get => _cSharpOutput;
            set => SetProperty(ref _cSharpOutput, value);
        }

        private string _rootClassName = "RootObject";

        public string RootClassName
        {
            get => _rootClassName;
            set => SetProperty(ref _rootClassName, value);
        }
        private string _nameSpaceName = "RootProject";
        public string NameSpaceName
        {
            get => _nameSpaceName;
            set => SetProperty(ref _nameSpaceName, value);
        }

        private bool _generateVirtualProperties;

        public bool GenerateVirtualProperties
        {
            get => _generateVirtualProperties;
            set => SetProperty(ref _generateVirtualProperties, value);
        }

        private bool _isSnackbarActive;

        public bool IsSnackbarActive
        {
            get => _isSnackbarActive;
            set => SetProperty(ref _isSnackbarActive, value);
        }

        private string _snackbarMessage;

        public string SnackbarMessage
        {
            get => _snackbarMessage;
            set => SetProperty(ref _snackbarMessage, value);
        }

        public RelayCommand ConvertJsonToCSharpCommand { get; }
        public RelayCommand CopyToClipboardCommand { get; }
        public RelayCommand CopyCommand { get; }
        public RelayCommand ExitCommand { get; }
        public RelayCommand SaveCommand { get; }

        public RelayCommand AboutCommand { get; }

        public MainViewModel()
        {
            ConvertJsonToCSharpCommand = new RelayCommand(ConvertJsonToCSharp);
            CopyToClipboardCommand = new RelayCommand(() => Clipboard.SetText(CSharpOutput));
            CopyCommand = new RelayCommand(CopyToClipboard);
            ExitCommand = new RelayCommand(ExitApplication);
            SaveCommand = new RelayCommand(SaveToFile);
            AboutCommand = new RelayCommand(AboutView);
        }

        private void AboutView()
        {
            About view = new About();
            view.ShowDialog();
        }

        private void SaveToFile()
        {


            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "C# Files (*.cs)|*.cs|All files (*.*)|*.*",
                Title = "Save C# Class",
                FileName = RootClassName
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, CSharpOutput);
                SnackbarMessage = "File saved successfully!";
                IsSnackbarActive = true;
                Task.Delay(3000).ContinueWith(t => IsSnackbarActive = false);
            }
        }

        private void ConvertJsonToCSharp()
        {
            try
            {
                JObject jsonObj = JObject.Parse(JsonInput);
                StringBuilder sb = new StringBuilder();
                sb.Append($"using Newtonsoft.Json;\n\n");
                sb.Append($"namespace {NameSpaceName};\n\n");
                HashSet<string> createdClasses = new HashSet<string>();
                GenerateClass(jsonObj, RootClassName, sb, 0, createdClasses);
                var result = sb.ToString();
                if (result.Contains("ItemClass"))
                {
                    result = "using System.Collections.Generic;\n" + result;
                }
                CSharpOutput = result;

                SnackbarMessage = "The code generation was successful !";
                IsSnackbarActive = true;


                Task.Delay(3000).ContinueWith(t => IsSnackbarActive = false);
            }
            catch (JsonException jsonEx)
            {
                CSharpOutput = "JSON parsing error: " + jsonEx.Message;
            }
            catch (Exception ex)
            {
                CSharpOutput = "Error: " + ex.Message;
            }
        }

        private void GenerateClass(JToken token, string className, StringBuilder sb, int indentLevel, HashSet<string> createdClasses)
        {
            if (createdClasses.Contains(className)) return;

            string classDefinition = $"{new string(' ', indentLevel * 4)}public class {className}\n";
            classDefinition += $"{new string(' ', indentLevel * 4)}{{\n";

            if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    string propName = ToPascalCase(property.Name);
                    string propType = GetCSharpType(property.Value, propName, sb, indentLevel + 1, createdClasses);
                    string propertyModifier = GenerateVirtualProperties ? "virtual " : "";
                    classDefinition += $"{new string(' ', (indentLevel + 1) * 4)}[JsonProperty(\"{property.Name}\")]\n";
                    classDefinition += $"{new string(' ', (indentLevel + 1) * 4)}public {propertyModifier}{propType} {propName} {{ get; set; }}\n";

                    // Recursively handle nested objects and arrays
                    if (property.Value is JObject || property.Value is JArray)
                    {
                        GenerateClass(property.Value, $"{propName}Class", sb, indentLevel, createdClasses);
                    }
                }


                classDefinition += $"{new string(' ', indentLevel * 4)}}}\n\n";
                sb.Append(classDefinition);
                createdClasses.Add(className);
            }

            else if (token is JArray arr)
            {
                foreach (var item in arr)
                {
                    GenerateClass(item, $"{className.Replace("Class", "ItemClass")}", sb, indentLevel, createdClasses);
                    break;
                }
            }



        }

        private void GenerateClassesForArrayItems(JArray array, string className, StringBuilder sb, int indentLevel, HashSet<string> createdClasses)
        {
            string baseItemClassName = $"{className}Item";
            if (array.Count > 0 && !createdClasses.Contains(baseItemClassName))
            {
                JToken firstItem = array.First;
                if (firstItem.Type == JTokenType.Object)
                {
                    GenerateClass(firstItem, baseItemClassName, sb, indentLevel, createdClasses);
                    createdClasses.Add(baseItemClassName);
                }
            }
        }

        private string GetCSharpType(JToken token, string propName, StringBuilder sb, int indentLevel, HashSet<string> createdClasses)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    string className = $"{propName}Class";
                    return className;
                case JTokenType.Array:
                    var arrayType = GetArrayType((JArray)token, propName, sb, indentLevel, createdClasses);
                    return $"List<{arrayType}>";
                case JTokenType.Integer:
                    return "int";
                case JTokenType.Float:
                    return "double";
                case JTokenType.String:
                    return "string";
                case JTokenType.Boolean:
                    return "bool";
                case JTokenType.Date:
                    return "DateTime";
                default:
                    return "dynamic";
            }
        }

        private string GetArrayType(JArray array, string propName, StringBuilder sb, int indentLevel, HashSet<string> createdClasses)
        {
            if (array.Count == 0)
                return "dynamic"; // Empty array, cannot determine type

            var firstElement = array.First;
            return GetCSharpType(firstElement, $"{propName}Item", sb, indentLevel, createdClasses);
        }

        private string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
            return textInfo.ToTitleCase(input).Replace("_", "").Replace("-", "");
        }

        private void CopyToClipboard()
        {
            if (!string.IsNullOrEmpty(CSharpOutput))
            {
                Clipboard.SetText(CSharpOutput);
                SnackbarMessage = "Successfully copied to the clipboard !";
                IsSnackbarActive = true;
                Task.Delay(3000).ContinueWith(t => IsSnackbarActive = false);
            }
        }

        private void ExitApplication()
        {
            Application.Current.Shutdown();
            Environment.Exit(0);
        }
    }
}