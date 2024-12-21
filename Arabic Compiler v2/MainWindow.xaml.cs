using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Arabic_Compiler_v2
{
    public partial class MainWindow : Window
    {
        private static readonly List<(string Name, string Pattern)> TokenSpecifications = new List<(string, string)>
        {
            ("KEYWORD", @"\b(متغير|طالما|اذا)\b"),
            ("NUM", @"\b\d+\b"),
            ("IDENT", @"\b[\p{L}_][\p{L}\p{N}_]*\b"),
            ("OPERATOR", @"(\+|-|\*|/|==|!=|<=|>=|<|>)"),
            ("ASSIGN", @"="),
            ("LPAREN", @"\("),
            ("RPAREN", @"\)"),
            ("LBRACE", @"\{"),
            ("RBRACE", @"\}"),
            ("SEMICOLON", @";"),
            ("WS", @"\s+"),
            ("UNKNOWN", @"."),
        };

        private static readonly Regex TokenRegex = new Regex(
            string.Join("|", TokenSpecifications.ConvertAll(ts => $"(?<{ts.Name}>{ts.Pattern})")),
            RegexOptions.Compiled
        );

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string inputCode = CodeInputTextBox.Text;

            try
            {
                var tokens = Tokenize(inputCode);

                ScannerResultTextBox.Clear();
                ParserResultTextBox.Clear();

                foreach (var token in tokens)
                {
                    ScannerResultTextBox.AppendText($"({token.TokenType}, \"{token.Value}\")\n");
                }

                var parser = new Parser(tokens);
                string parseResult = parser.ParseProgram();

                ParserResultTextBox.AppendText(parseResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Parsing Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static List<(string TokenType, string Value)> Tokenize(string code)
        {
            var tokens = new List<(string TokenType, string Value)>();

            foreach (Match match in TokenRegex.Matches(code))
            {
                foreach (var tokenSpec in TokenSpecifications)
                {
                    if (match.Groups[tokenSpec.Name].Success)
                    {
                        string tokenType = tokenSpec.Name;
                        string value = match.Groups[tokenSpec.Name].Value;

                        if (tokenType == "WS")
                            break;

                        if (tokenType == "UNKNOWN")
                            throw new Exception($"Unrecognized token: {value}");

                        tokens.Add((tokenType, value));
                        break;
                    }
                }
            }

            return tokens;
        }
    }
}
