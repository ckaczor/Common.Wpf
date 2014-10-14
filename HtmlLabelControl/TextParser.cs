﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Common.Wpf.HtmlLabelControl
{
    public class TextParser
    {
        private HtmlLabel _parentControl;

        public Collection<TextLine> Parse(HtmlLabel parentControl, string text)
        {
            _parentControl = parentControl;

            // Escape the supplied text
            text = SecurityElement.Escape(text);

            // Add a root tag so the parser is happy
            text = string.Format(CultureInfo.InvariantCulture, "<body>{0}</body>", text);

            // Normalize line endings
            text = text.Replace("\r\n", "\n");

            // Create an XML document and load it with the text
            var xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.LoadXml(text);

            // Create a list of text lines
            var lines = new Collection<TextLine>();

            // Walk over the nodes and build up the fragment list
            WalkNodes(xmlDocument.ChildNodes, lines);

            return lines;
        }

        private readonly Stack<TextFragmentStyle> _attributeStack = new Stack<TextFragmentStyle>();

        private void WalkNodes(XmlNodeList xmlNodeList, Collection<TextLine> textLines)
        {
            if (textLines.Count == 0)
                textLines.Add(new TextLine());

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                TextFragmentStyle style;

                switch (xmlNode.Name.ToUpperInvariant())
                {
                    case "#WHITESPACE":
                    case "#TEXT":

                        // Split the fragment and the line endings
                        var lines = xmlNode.Value.Split('\n');

                        var firstLine = true;

                        foreach (var line in lines)
                        {
                            var textLine = (firstLine ? textLines[textLines.Count - 1] : new TextLine());

                            // Create a new fragment and fill the style information
                            var textFragment = new TextFragment(_parentControl) { Text = line };

                            foreach (var s in _attributeStack)
                            {
                                s.Apply(textFragment);
                            }

                            // Add the fragment to the list
                            textLine.FragmentList.Add(textFragment);

                            if (!firstLine)
                                textLines.Add(textLine);

                            firstLine = false;
                        }

                        break;

                    case "B":

                        style = new TextFragmentStyle { Weight = FontWeights.Bold };
                        _attributeStack.Push(style);

                        break;

                    case "U":

                        style = new TextFragmentStyle { Underline = true };
                        _attributeStack.Push(style);

                        break;

                    case "CITE":

                        style = new TextFragmentStyle { Style = FontStyles.Italic };
                        _attributeStack.Push(style);

                        break;

                    case "FONT":
                        style = new TextFragmentStyle();

                        if (xmlNode.Attributes != null)
                        {
                            foreach (XmlAttribute attribute in xmlNode.Attributes)
                            {
                                switch (attribute.Name.ToUpperInvariant())
                                {
                                    case "SIZE":
                                        style.Size = Convert.ToDouble(attribute.Value, CultureInfo.InvariantCulture);
                                        break;

                                    case "COLOR":
                                        style.Color = (Brush) new BrushConverter().ConvertFromString(attribute.Value);
                                        break;
                                }
                            }

                            _attributeStack.Push(style);
                        }

                        break;
                }

                if (xmlNode.ChildNodes.Count > 0)
                    WalkNodes(xmlNode.ChildNodes, textLines);

                if (_attributeStack.Count > 0)
                    _attributeStack.Pop();
            }
        }
    }
}