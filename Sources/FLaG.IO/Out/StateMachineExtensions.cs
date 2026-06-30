using System.Collections.Immutable;
using System.Data;
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

        private const double _StateBigCircleRadiusWithGap = 1.25 * _StateBigCircleRadius;

        private const double _TextLineGap = 15;

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

            GraphMatrix graph = new(states.Count);

            foreach (Transition transition in stateMachine.Transitions)
            {
                graph.Set(
                    stateIndices[states[transition.CurrentState]],
                    stateIndices[states[transition.NextState]],
                    true
                );
                Tuple<SingleLabel, SingleLabel> stateTransition = new(
                    states[transition.CurrentState],
                    states[transition.NextState]
                );

                arrows.GetOrAdd(stateTransition, () => []).Add(transition.Symbol);
            }

            List<int> stateReindex = PermutationFinder.Find(new(graph));
            stateIndices = stateIndices.ToDictionary(kv => kv.Key, kv => stateReindex[kv.Value]);

            (double distanceFromCenter, double degreesBetweenStates) = GetParams(states);
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
            WriteDefs(xmlWriter);
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
                    distanceFromCenter - _StateBigCircleRadius
                );
            }
            foreach (KeyValuePair<Tuple<SingleLabel, SingleLabel>, HashSet<char>> arrow in arrows)
            {
                DrawTransition(
                    xmlWriter,
                    stateIndices[arrow.Key.Item1],
                    stateIndices[arrow.Key.Item2],
                    distanceFromCenter - _StateBigCircleRadius,
                    degreesBetweenStates,
                    arrows.ContainsKey(
                        new Tuple<SingleLabel, SingleLabel>(arrow.Key.Item2, arrow.Key.Item1)
                    ),
                    string.Join(',', arrow.Value)
                );
            }
            xmlWriter.WriteEndElement(); // g
            xmlWriter.WriteEndElement(); // svg
            xmlWriter.WriteEndDocument();
        }

        private static (double distanceFromCenter, double degreesBetweenStates) GetParams(
            ImmutableSortedDictionary<Label, SingleLabel> states
        )
        {
            double degreesBetweenStates = 360.0 / states.Count;

            if (states.Count < 2)
            {
                return (0, degreesBetweenStates);
            }

            return (
                Math.Max(2, 9 - states.Count)
                    * _StateBigCircleRadius
                    / (1 - Math.Cos(degreesBetweenStates * Math.PI / 180)),
                degreesBetweenStates
            );
        }

        private static void WriteDefs(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("defs");
            xmlWriter.WriteStartElement("marker");
            xmlWriter.WriteAttributeString("id", "arrow");
            xmlWriter.WriteAttributeString("viewBox", "0 0 10 10");
            xmlWriter.WriteAttributeString("refX", "5");
            xmlWriter.WriteAttributeString("refY", "5");
            xmlWriter.WriteAttributeString("markerWidth", "6");
            xmlWriter.WriteAttributeString("markerHeight", "6");
            xmlWriter.WriteAttributeString("orient", "auto-start-reverse");
            xmlWriter.WriteStartElement("path");
            xmlWriter.WriteAttributeString("d", "M 0 0 L 10 5 L 0 10 z");
            xmlWriter.WriteAttributeString("fill", "black");
            xmlWriter.WriteEndElement(); // path
            xmlWriter.WriteEndElement(); // marker
            xmlWriter.WriteEndElement(); // defs
        }

        private static void DrawTransition(
            XmlWriter xmlWriter,
            int currentStateIndex,
            int nextStateIndex,
            double distanceFromCenter,
            double degreesBetweenStates,
            bool hasReversedTransition,
            string text
        )
        {
            double alphaDegree = currentStateIndex * degreesBetweenStates;
            if (currentStateIndex == nextStateIndex)
            {
                xmlWriter.WriteStartElement("g");
                xmlWriter.WriteAttributeString(
                    "transform",
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "rotate({0}) translate({1},0)",
                        alphaDegree,
                        distanceFromCenter
                    )
                );
                xmlWriter.WriteAttributeString("stroke", "black");
                xmlWriter.WriteAttributeString(
                    "stroke-width",
                    _StrokeWidth.ToString(CultureInfo.InvariantCulture)
                );
                xmlWriter.WriteAttributeString("fill", "none");
                xmlWriter.WriteStartElement("path");
                double distance = _StateBigCircleRadius * Math.Sqrt(2) / 2;
                xmlWriter.WriteAttributeString(
                    "d",
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "M {0},{1} A {2},{2} 0 1,1 {3},{4}",
                        distance,
                        -distance,
                        _StateBigCircleRadius,
                        distance,
                        distance
                    )
                );
                xmlWriter.WriteAttributeString(
                    "transform",
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "translate({0},0)",
                        _StateBigCircleRadiusWithGap - _StateBigCircleRadius
                    )
                );
                xmlWriter.WriteAttributeString("marker-end", "url(#arrow)");
                xmlWriter.WriteEndElement(); // path
                xmlWriter.WriteStartElement("g");
                xmlWriter.WriteAttributeString(
                    "transform",
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "translate({0},0)",
                        _StateBigCircleRadius * Math.Sqrt(2)
                            + _StateBigCircleRadiusWithGap
                            + _TextLineGap
                    )
                );
                bool rotate = alphaDegree > 180;
                xmlWriter.WriteStartElement("text");
                xmlWriter.WriteAttributeString("x", "0");
                xmlWriter.WriteAttributeString("y", "0");
                xmlWriter.WriteAttributeString("text-anchor", "middle");
                xmlWriter.WriteAttributeString(
                    "dominant-baseline",
                    rotate ? "alphabetic" : "hanging"
                );
                xmlWriter.WriteAttributeString("fill", "black");
                xmlWriter.WriteAttributeString("stroke-width", "1");
                xmlWriter.WriteAttributeString(
                    "transform",
                    string.Format(CultureInfo.InvariantCulture, "rotate({0})", rotate ? 90 : 270)
                );
                xmlWriter.WriteString(text);
                xmlWriter.WriteEndElement(); // text);
                xmlWriter.WriteEndElement(); // g);
                xmlWriter.WriteEndElement(); // g
            }
            else
            {
                double betaDegree = nextStateIndex * degreesBetweenStates;
                double thetaDegree = (alphaDegree + betaDegree) / 2 - 90;
                double phiRad = (alphaDegree - betaDegree) * Math.PI / 360;
                double distance =
                    distanceFromCenter * Math.Cos(phiRad)
                    + (hasReversedTransition ? 15.0 * Math.Sign(phiRad) : 0);
                double x2 = distanceFromCenter * Math.Sin(phiRad);
                double x1 = -x2;
                x1 -= Math.Sign(x1) * _StateBigCircleRadiusWithGap;
                x2 -= Math.Sign(x2) * _StateBigCircleRadiusWithGap;
                xmlWriter.WriteStartElement("g");
                xmlWriter.WriteAttributeString(
                    "transform",
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "rotate({0}) translate(0,{1})",
                        thetaDegree,
                        distance
                    )
                );
                xmlWriter.WriteAttributeString("stroke", "black");
                xmlWriter.WriteAttributeString(
                    "stroke-width",
                    _StrokeWidth.ToString(CultureInfo.InvariantCulture)
                );
                xmlWriter.WriteAttributeString("fill", "none");
                xmlWriter.WriteStartElement("line");
                xmlWriter.WriteAttributeString("x1", x1.ToString(CultureInfo.InvariantCulture));
                xmlWriter.WriteAttributeString("y1", "0");
                xmlWriter.WriteAttributeString("x2", x2.ToString(CultureInfo.InvariantCulture));
                xmlWriter.WriteAttributeString("y2", "0");
                xmlWriter.WriteAttributeString("marker-end", "url(#arrow)");
                xmlWriter.WriteEndElement(); // line
                bool rotate180 = Math.Cos(thetaDegree * Math.PI / 180) < 0;
                bool textEndAnchor = (x2 - x1 > 0) != rotate180;
                bool alphabetic = Math.Sign(phiRad) > 0 == rotate180;
                xmlWriter.WriteStartElement("text");
                xmlWriter.WriteAttributeString("x", "0");
                xmlWriter.WriteAttributeString("y", "0");
                xmlWriter.WriteAttributeString("text-anchor", textEndAnchor ? "end" : "start");
                xmlWriter.WriteAttributeString(
                    "dominant-baseline",
                    alphabetic ? "alphabetic" : "hanging"
                );
                xmlWriter.WriteAttributeString("fill", "black");
                xmlWriter.WriteAttributeString("stroke-width", "1");
                xmlWriter.WriteAttributeString(
                    "transform",
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "translate({0}, {1}) rotate({2})",
                        x1 + (x2 - x1) * 3 / 4,
                        Math.Sign(phiRad) * _TextLineGap,
                        rotate180 ? 180 : 0
                    )
                );
                xmlWriter.WriteString(text);
                xmlWriter.WriteEndElement(); // text
                xmlWriter.WriteEndElement(); // g
            }
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
            xmlWriter.WriteAttributeString(
                "cx",
                distanceFromCenter.ToString(CultureInfo.InvariantCulture)
            );
            xmlWriter.WriteAttributeString("cy", "0");
            xmlWriter.WriteAttributeString(
                "r",
                _StateBigCircleRadius.ToString(CultureInfo.InvariantCulture)
            );
            xmlWriter.WriteEndElement(); // circle
            if (isInitialState)
            {
                xmlWriter.WriteStartElement("circle");
                xmlWriter.WriteAttributeString(
                    "cx",
                    distanceFromCenter.ToString(CultureInfo.InvariantCulture)
                );
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
                    distanceFromCenter,
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
