using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Xml;
using FLaG.Core.Data;
using FLaG.Core.Data.StateMachines;
using FLaG.Core.Extensions;

namespace FLaG.IO.Out
{
    public static class StateMachineExtensions
    {
        private const double _StateBigCircleRadius = 100;

        private const int _StrokeWidth = 2;

        private const int _BoldStrokeWidth = 6;

        private const int _FontSize = 70;

        private const double _InitialStateSmallCircleRadius = 0.9 * _StateBigCircleRadius;

        public static void DrawDiagram(this StateMachine stateMachine, string fileName)
        {
            ImmutableSortedDictionary<Label, SingleLabel> states = stateMachine
                .States.Select(s => new KeyValuePair<Label, SingleLabel>(s, s.ExtractSingleLabel()))
                .ToImmutableSortedDictionary();

            SingleLabel initialState = states[stateMachine.InitialState];

            Dictionary<SingleLabel, int> stateIndices = stateMachine
                .States.Select((s, i) => new KeyValuePair<SingleLabel, int>(states[s], i))
                .ToDictionary();

            ImmutableSortedSet<SingleLabel> finalStates = stateMachine
                .FinalStates.Select(s => states[s])
                .ToImmutableSortedSet();

            Dictionary<Tuple<SingleLabel, SingleLabel>, HashSet<char>> arrows = [];

            foreach (Transition transition in stateMachine.Transitions)
            {
                Tuple<SingleLabel, SingleLabel> stateTransition = new(
                    states[transition.CurrentState],
                    states[transition.NextState]
                );

                arrows.GetOrAdd(stateTransition, () => []).Add(transition.Symbol);
            }

            double degreesBetweenStates = 360.0 / states.Count;
            double distanceFromCenter =
                states.Count > 1
                    ? _StateBigCircleRadius / (1 - Math.Cos(degreesBetweenStates * Math.PI / 180))
                        + 2 * _StateBigCircleRadius
                    : 0;
            double size = 2 * (distanceFromCenter + 4 * _StateBigCircleRadius);

            using FileStream fileStream = new(
                fileName,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None
            );
            XmlWriterSettings settings = new()
            {
                CloseOutput = true,
                CheckCharacters = true,
                Encoding = new UTF8Encoding(false),
                Indent = true,
            };
            using XmlWriter xmlWriter = XmlWriter.Create(fileStream, settings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("svg", "http://www.w3.org/2000/svg");
            xmlWriter.WriteAttributeString("width", size.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteAttributeString("height", size.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteAttributeString(
                "viewBox",
                string.Format(CultureInfo.InvariantCulture, "0 0 {0} {0}", size)
            );
            xmlWriter.WriteStartElement("g");
            xmlWriter.WriteAttributeString(
                "transform",
                string.Format(CultureInfo.InvariantCulture, "translate({0},{0})", size / 2)
            );
            xmlWriter.WriteAttributeString("font-style", "italic");
            xmlWriter.WriteAttributeString(
                "font-family",
                "DejaVu Serif, Cambria Math, Cambria, serif"
            );
            xmlWriter.WriteAttributeString(
                "font-size",
                _FontSize.ToString(CultureInfo.InvariantCulture)
            );
            foreach (SingleLabel state in states.Values)
            {
                int index = stateIndices[state];
                DrawState(
                    xmlWriter,
                    state,
                    state == initialState,
                    finalStates.Contains(state),
                    degreesBetweenStates,
                    index,
                    distanceFromCenter
                );
            }
            xmlWriter.WriteEndElement(); // g
            xmlWriter.WriteEndElement(); // svg
            xmlWriter.WriteEndDocument();
        }

        private static void DrawState(
            XmlWriter xmlWriter,
            SingleLabel state,
            bool isInitialState,
            bool isFinalState,
            double angleBetweenStates,
            int stateIndex,
            double distanceFromCenter
        )
        {
            double angle = stateIndex * angleBetweenStates;
            double center = distanceFromCenter - _StateBigCircleRadius;

            xmlWriter.WriteStartElement("g");
            xmlWriter.WriteAttributeString(
                "transform",
                string.Format(CultureInfo.InvariantCulture, "rotate({0})", angle)
            );

            int strokeWidth = isFinalState ? _BoldStrokeWidth : _StrokeWidth;
            xmlWriter.WriteStartElement("g");
            xmlWriter.WriteAttributeString("stroke", "black");
            xmlWriter.WriteAttributeString(
                "stroke-width",
                strokeWidth.ToString(CultureInfo.InvariantCulture)
            );
            xmlWriter.WriteAttributeString("fill", "none");
            xmlWriter.WriteStartElement("circle");
            xmlWriter.WriteAttributeString("cx", center.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteAttributeString("cy", "0");
            xmlWriter.WriteAttributeString(
                "r",
                _StateBigCircleRadius.ToString(CultureInfo.InvariantCulture)
            );
            xmlWriter.WriteEndElement(); // circle
            if (isInitialState)
            {
                xmlWriter.WriteStartElement("circle");
                xmlWriter.WriteAttributeString("cx", center.ToString(CultureInfo.InvariantCulture));
                xmlWriter.WriteAttributeString("cy", "0");
                xmlWriter.WriteAttributeString(
                    "r",
                    _InitialStateSmallCircleRadius.ToString(CultureInfo.InvariantCulture)
                );
                xmlWriter.WriteAttributeString(
                    "stroke-width",
                    _StrokeWidth.ToString(CultureInfo.InvariantCulture)
                );
                xmlWriter.WriteEndElement(); // circle
            }
            xmlWriter.WriteStartElement("text");
            xmlWriter.WriteAttributeString("x", "0");
            xmlWriter.WriteAttributeString("y", "0");
            xmlWriter.WriteAttributeString("text-anchor", "middle");
            xmlWriter.WriteAttributeString("dominant-baseline", "middle");
            xmlWriter.WriteAttributeString("fill", "black");
            xmlWriter.WriteAttributeString("stroke-width", "1");
            xmlWriter.WriteAttributeString(
                "transform",
                string.Format(
                    CultureInfo.InvariantCulture,
                    "translate({0},0) rotate({1})",
                    center,
                    -angle
                )
            );
            xmlWriter.WriteString(state.Sign.ToString(CultureInfo.InvariantCulture));
            if (state.SignIndex.HasValue)
            {
                xmlWriter.WriteStartElement("tspan");
                xmlWriter.WriteAttributeString("baseline-shift", "sub");
                xmlWriter.WriteString(state.SignIndex.Value.ToString(CultureInfo.InvariantCulture));
                xmlWriter.WriteEndElement(); // tspan
            }
            xmlWriter.WriteEndElement(); // text
            xmlWriter.WriteEndElement(); // g
            xmlWriter.WriteEndElement(); // g
        }
    }
}
