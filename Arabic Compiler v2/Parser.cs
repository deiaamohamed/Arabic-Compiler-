public class Parser
{
    private List<(string TokenType, string Value)> tokens;
    private int currentTokenIndex;
    private List<string> rulesApplied;

    public Parser(List<(string TokenType, string Value)> tokens)
    {
        this.tokens = tokens;
        this.currentTokenIndex = 0;
        this.rulesApplied = new List<string>();
    }

    public string ParseProgram()
    {
        try
        {
            ParseStatementList();
            return string.Join("\n", rulesApplied);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    private void ParseStatementList()
    {
        if (currentTokenIndex < tokens.Count)
        {
            if (tokens[currentTokenIndex].TokenType == "RBRACE")
            {
                return;
            }

            ParseStatement();
            ParseStatementList();
        }
    }

    private void ParseStatement()
    {
        if (currentTokenIndex >= tokens.Count)
        {
            throw new Exception("Unexpected end of input. Token list is exhausted.");
        }

        if (tokens[currentTokenIndex].TokenType == "KEYWORD")
        {
            string keyword = tokens[currentTokenIndex].Value;

            if (keyword == "متغير")
            {
                ParseDeclaration();
            }
            else if (keyword == "اذا")
            {
                ParseIfStatement();
            }
            else if (keyword == "طالما")
            {
                ParseWhileStatement();
            }
            else
            {
                throw new Exception($"Unexpected keyword '{keyword}'. Expected 'متغير', 'اذا', or 'طالما'.");
            }
        }
        else if (tokens[currentTokenIndex].TokenType == "IDENT")
        {
            ParseAssignment();
        }
        else
        {
            throw new Exception($"Unexpected token: '{tokens[currentTokenIndex].Value}' at position {currentTokenIndex}. Expected a valid statement keyword or identifier.");
        }
    }

    private void ParseDeclaration()
    {
        MatchToken("KEYWORD", "متغير");
        MatchToken("IDENT");

        if (currentTokenIndex < tokens.Count && tokens[currentTokenIndex].TokenType == "ASSIGN")
        {
            MatchToken("ASSIGN");
            ParseExpression();
        }

        MatchToken("SEMICOLON");
        rulesApplied.Add("Declaration Rule: متغير IDENT [ASSIGN Expression] ;");
    }

    private void ParseAssignment()
    {
        MatchToken("IDENT");
        MatchToken("ASSIGN");
        ParseExpression();
        MatchToken("SEMICOLON");
        rulesApplied.Add("Assignment Rule: IDENT ASSIGN Expression ;");
    }

    private void ParseExpression()
    {
        ParseTerm();

        while (currentTokenIndex < tokens.Count && tokens[currentTokenIndex].TokenType == "OPERATOR")
        {
            ParseOperator();
            ParseTerm();
        }
    }

    private void ParseTerm()
    {
        if (tokens[currentTokenIndex].TokenType == "IDENT" || tokens[currentTokenIndex].TokenType == "NUM")
        {
            currentTokenIndex++;
        }
        else if (tokens[currentTokenIndex].TokenType == "LPAREN")
        {
            currentTokenIndex++;
            ParseExpression();
            MatchToken("RPAREN");
        }
        else
        {
            throw new Exception($"Expected IDENT, NUM, or LPAREN, found: {tokens[currentTokenIndex].Value}");
        }
    }

    private void ParseOperator()
    {
        if (tokens[currentTokenIndex].TokenType == "OPERATOR" && (tokens[currentTokenIndex].Value == "+" || tokens[currentTokenIndex].Value == "-" || tokens[currentTokenIndex].Value == "*" || tokens[currentTokenIndex].Value == "/"))
        {
            currentTokenIndex++;
        }
        else
        {
            throw new Exception("Expected operator, found: " + tokens[currentTokenIndex].Value);
        }
    }

    private void ParseIfStatement()
    {
        MatchToken("KEYWORD", "اذا");
        MatchToken("LPAREN");
        ParseCondition();
        MatchToken("RPAREN");
        MatchToken("LBRACE");
        ParseStatementList();
        MatchToken("RBRACE");
        rulesApplied.Add("If Statement Rule: اذا (Condition) { Statement List }");
    }

    private void ParseCondition()
    {
        ParseTerm();
        ParseRelationalOperator();
        ParseTerm();
    }

    private void ParseRelationalOperator()
    {
        if (tokens[currentTokenIndex].TokenType == "OPERATOR" &&
            (tokens[currentTokenIndex].Value == "==" || tokens[currentTokenIndex].Value == "!=" ||
             tokens[currentTokenIndex].Value == ">" || tokens[currentTokenIndex].Value == ">=" ||
             tokens[currentTokenIndex].Value == "<" || tokens[currentTokenIndex].Value == "<="))
        {
            currentTokenIndex++;
        }
        else
        {
            throw new Exception($"Expected relational operator, found: {tokens[currentTokenIndex].Value}");
        }
    }

    private void ParseWhileStatement()
    {
        MatchToken("KEYWORD", "طالما");
        MatchToken("LPAREN");
        ParseCondition();
        MatchToken("RPAREN");
        MatchToken("LBRACE");
        ParseStatementList();
        MatchToken("RBRACE");
        rulesApplied.Add("While Statement Rule: طالما (Condition) { Statement List }");
    }

    private void MatchToken(string expectedType, string expectedValue = null)
    {
        if (currentTokenIndex < tokens.Count && tokens[currentTokenIndex].TokenType == expectedType)
        {
            if (expectedValue == null || tokens[currentTokenIndex].Value == expectedValue)
            {
                currentTokenIndex++;
            }
            else
            {
                throw new Exception($"Expected {expectedValue}, found: {tokens[currentTokenIndex].Value}");
            }
        }
        else
        {
            throw new Exception($"Expected {expectedType}, found: {tokens[currentTokenIndex].Value}");
        }
    }
}
