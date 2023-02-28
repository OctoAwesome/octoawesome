using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sprache;

namespace OctoAwesome.Client;

internal class CommandParser
{
    private static Parser<char> ParseValidIdentifierChars = Parse.LetterOrDigit.Or(Parse.Char('_'));

    private static Parser<string> ParseIdentifier = Parse.Letter.AtLeastOnce().Concat(ParseValidIdentifierChars.Many()).Text();

    private static Parser<string> ParseDoubleQuotationString = ParseEscapedString('"', '\\');
    private static Parser<string> ParseSingleQuotationString = ParseEscapedString('\'', '\\');
    
    private static Parser<uint> ParseUInt = Parse.Digit.AtLeastOnce().Text().SelectWhere(x => (uint.TryParse(x, out var res), res));
    
    private static Parser<int> ParseInt = Parse.Char('-')
        .Optional()
        .SelectMany(op => ParseUInt, 
            (op, num) => (op.IsDefined ? -1 : 1) * (int)num);

    private static Parser<IEnumerable<char>> ParseFloatingStringWithoutNegative()
    {
        return Parse.Digit.Many().Concat(Parse.Char('.').Once().Concat(Parse.Digit.Many()));
    }

    private static Parser<IEnumerable<char>> ParseFloatingString()
    {
        return Parse.Char('-')
            .Optional()
            .SelectMany(op => ParseFloatingStringWithoutNegative(), 
                (op, num) => (op.IsDefined ? num.Prepend(op.Get()) : num));
    }

    private static Parser<string> ParseFloatingWithPostfix()
    {
        return ParseFloatingString().Concat(Parse.IgnoreCase('f').Once()).Text();
    }

    private static Parser<double> ParseDouble = ParseFloatingString().Text().SelectWhere(s => (double.TryParse(s, CultureInfo.InvariantCulture, out var res), res));
    private static Parser<float> ParseFloat = ParseFloatingWithPostfix().SelectWhere(x => (float.TryParse(x, CultureInfo.InvariantCulture, out var res), res));

    private static Parser<T> ParseDelimitedWithErrors<T, TU>(Parser<TU> del, bool forExecuting, Func<T?, TU, T> createNode, Func<T?, string, T> createErrorNode)
    {
        var delimiter = Parse.Char('.');
        var baseParser = del.XDelimitedBy(delimiter).Select(x => (success: true, x));
        
        var completeParser =
            forExecuting ? baseParser : baseParser.Or(del.DelimitedBy(delimiter).Select(x => (success: false, x)));
        
        return completeParser.Select((tpl) =>
                                     {
                                         var nodes = tpl.x;
                                         T? parent = default;
        
                                         foreach (var node in nodes)
                                         {
                                             parent = createNode(parent, node);
                                         }
        
                                         if (!tpl.success)
                                         {
                                             parent = createErrorNode(parent, "");
                                         }
        
                                         if (parent is null)
                                             throw new ArgumentNullException();
        
                                         return parent;
                                     })
            ;
    }
    
    private static Parser<Node> ParseMethodOrIdentifier(bool forExecuting)
    {
        var r = ParseIdentifier.SelectMany<string, IOption<TupleNode>, (string id, TupleNode? tupleNode)>(
            _ => ParseTuple(forExecuting, ParseNode(forExecuting)).XOptional(),
            (id, option) => (id, option.GetOrDefault()));

        static Node CreateNode(Node? parent, (string id, TupleNode? tupleNode) node)
        {
            return node.tupleNode is null
                ? new ReferenceNode(parent, node.id)
                : new MethodNode(parent, node.id, node.tupleNode);
        }

        return ParseDelimitedWithErrors<Node, (string id, TupleNode? tupleNode)>(r, forExecuting, CreateNode, (node, s) => new ErrorNode(node, s));
    }

    private static Parser<Primitive> ParseValue = ParseDoubleQuotationString.Token().Select(x => new Primitive(x))
        .Or(ParseFloat.Token().Select(x => new Primitive(x)))
        .Or(ParseDouble.Token().Select(x => new Primitive(x)))
        .Or(ParseInt.Token().Select(x => new Primitive(x)))
        .Or(ParseUInt.Token().Select(x => new Primitive(x))).Positioned();

    private static Parser<Node> ParseNode(bool forExecuting) => ParseValue.Select(x => (Node)x)
            .Or(ParseMethodOrIdentifier(forExecuting))
            .Positioned();

    private static Parser<string> ParseEscapedString(char quotationMark, char escapeChar)
    {
        var quotationParser = Parse.Char(quotationMark);
        var escapeParser = Parse.Char(escapeChar);

        var textWithoutQuotes =
            Parse.AnyChar.Except(quotationParser);

        var escapedQuote = from _ in escapeParser
                                        from c in Parse.AnyChar
                                        select c;

        var stringContent = escapedQuote.Or(textWithoutQuotes).Many().Text();
        
        return stringContent.Contained(quotationParser, quotationParser);
    }

    private static Parser<TupleNode> ParseTuple(bool forExecuting, Parser<Node> nodeParser)
    {
        var openBracket = Parse.Char('(').Token();
        var closeBracket = Parse.Char(')').Token();
        return ParseEnclosedTuple(nodeParser, forExecuting, openBracket, closeBracket);
    }

    private static Parser<Node> ParseTypeIdentifier(bool forExecuting)
    {
        var typeIdentifierParser =
            ParseDelimitedWithErrors<Node, string>(ParseIdentifier, forExecuting, (parent, id) => new ReferenceNode(parent, id),
                (parent, id) => new ErrorNode(parent, id));
        var typeParser = Parse.Ref(() => ParseTupleType(forExecuting)).Select(x => (Node)x)
            .XOr(typeIdentifierParser);
            
        return typeParser;
    }
        private static Parser<TupleNode> ParseTupleType(bool forExecuting)
    {
        var openBracket = Parse.Char('(').Token();
        var closeBracket = Parse.Char(')').Token();

        var tupleItem = ParseTypeIdentifier(forExecuting).Token()
            .SelectMany(x => ParseIdentifier.Optional(), 
            (typeName, itemName) => (typeName, itemName)).Select(x => new TupleTypeElement(x.typeName, x.itemName.GetOrDefault()));
        
        return ParseEnclosedTuple(tupleItem, forExecuting, openBracket, closeBracket);
    }
    private static Parser<TupleNode> ParseTypeTuple(bool forExecuting)
    {
        var openBracket = Parse.Char('<').Token();
        var closeBracket = Parse.Char('>').Token();
        var typeParser = ParseTypeIdentifier(forExecuting);
        return ParseEnclosedTuple(typeParser, forExecuting, openBracket, closeBracket);
    }
    private static Parser<TupleNode> ParseEnclosedTuple(Parser<Node> innerNode, bool forExecuting, Parser<char> openBracket, Parser<char> closeBracket)
    {
        var listParse = innerNode.Token()
            .Select(x => (matched: true, node: x))
            .DelimitedBy(Parse.Chars(','));

        if (!forExecuting)
            listParse = listParse.Or(innerNode.Token().Optional()
                    .Select(x => (matched: x.IsDefined, node: x.GetOrDefault()))
                    .Token().DelimitedBy(Parse.Chars(',')))
                .Or(ParseNode(forExecuting).Token().Optional()
                    .Select(x => (matched: x.IsDefined, node: x.GetOrDefault()))
                    .Token().DelimitedBy(Parse.Chars(',')));

        var listParseOpt = listParse.Optional();
        

        var containedParser = forExecuting
            ? listParseOpt.Contained(openBracket, closeBracket).Select(x => (isClosed: true, value: x))
            : listParseOpt.ContainedOptional(openBracket, closeBracket);
        
        return containedParser.Select(x =>
        {
            var nodes = Array.Empty<Node>();
            if (x.value.IsDefined)
            {
                int errorNodeCount = 0;
                var nodesList = new List<Node>();
                foreach (var n in x.value.Get())
                {
                    if (n.matched)
                    {
                        nodesList.Add(n.node);
                    }
                    else
                    {
                        nodesList.Add(new ErrorNode(null, ""));
                        errorNodeCount++; // TODO: better workaround?
                    }
                }

                if (errorNodeCount != 1 || nodesList.Count != 1)
                {
                    nodes = nodesList.ToArray();
                }
            }
            return x.isClosed
                ? new TupleNode(nodes)
                : new TupleNode(nodes.Append(new ErrorNode(null, ")")).ToArray());
        });
    }

    public IResult<Node> ParseCommand(string input, bool forExecuting)
    {
        return ParseNode(forExecuting).TryParse(input);
    }

    public TRes Execute<TRes>(Node node, INodeVisitor<TRes> visitor)
    {
        return node.Accept(visitor);
    }

    public void ParseCommand()
    {
        var input = "something(asdf.haha.bla(), 1.23, \"huhu \\\"\" )";
        var value = ParseNode(false).Parse(input);
        //Console.WriteLine($"Parse: \"{a.Parse("\\\"")}\"");
        Console.WriteLine($"Parse: \"{value}\" {value.StartPos}");
    }
}