using FLaGLib.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FLaG.IO.Output
{
    public static class StateMachineExtensions
    {
        private const string _FontName = "Times New Roman";

        private static void DrawArrow(Graphics graphics, PointF point, float angle)
        {
            Matrix state = graphics.Transform;

            graphics.TranslateTransform(point.X, point.Y);
            graphics.RotateTransform(angle / (float)Math.PI * 180 + 90);

            const float radius = 40;

            PointF[] points = new PointF[3];

            points[0] = new PointF(radius / 2, (float)-(radius * Math.Sqrt(3) / 2));
            points[1] = new PointF(0, 0);
            points[2] = new PointF(-radius / 2, (float)-(radius * Math.Sqrt(3) / 2));

            graphics.FillPolygon(Brushes.Black, points);

            graphics.Transform = state;
        }

        private static void DrawDirectedLine(Graphics graphics, float x1, float y1, float x2, float y2, float radius, Pen pen, Font font, string text, float offset)
        {
            Matrix state = graphics.Transform;

            float length = (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

            if (length < 2.5f * radius)
            {
                return;
            }

            graphics.TranslateTransform(x1, y1);

            float sin = (y1 - y2) / length;
            float cos = (x2 - x1) / length;

            Matrix matrix = new Matrix(cos, -sin, sin, cos, 0, 0);

            graphics.MultiplyTransform(matrix);

            graphics.TranslateTransform(0, -offset);

            graphics.DrawLine(pen, 1.25f * radius, 0f, length - 1.25f * radius, 0f);

            DrawArrow(graphics, new PointF(length - 1.25f * radius, 0), (float)Math.PI);

            SizeF textSize = graphics.MeasureString(text, font);

            PointF pointToDraw;

            if (cos > 0)
            {
                pointToDraw = new PointF(1.25f * radius + 3.0f * (length - 2.5f * radius) / 4.0f - textSize.Width / 2.0f, -textSize.Height);
            }
            else
            {
                graphics.TranslateTransform(length / 2.0f, 0);
                graphics.RotateTransform(180.0f);
                graphics.TranslateTransform(-length / 2.0f, 0);
                pointToDraw = new PointF(1.25f * radius + (length - 2.5f * radius) / 4.0f - textSize.Width / 2.0f, 0.0f);
            }

            graphics.DrawString(text, font, Brushes.Black, pointToDraw);

            graphics.Transform = state;
        }

        public static SizeF GetStateLabelSize(Graphics g, Font stateFont, Font subscriptFont, SingleLabel state)
        {
            SizeF textSize = g.MeasureString(state.Sign.ToString(), stateFont);

            if (state.SignIndex != null)
            {
                int value = state.SignIndex.Value;

                SizeF indexSize = g.MeasureString(value.ToString(), subscriptFont);

                float xSize = textSize.Width + 2 + indexSize.Width;
                float ySize = Math.Max(textSize.Height, 0.6f * textSize.Height + indexSize.Height);

                textSize = new SizeF(xSize, ySize);
            }

            return textSize;
        }

        public static double GetRadiusByFontAndState(Font stateFont, Font subscriptFont, IEnumerable<SingleLabel> states)
        {
            double radius = 0;

            using (Bitmap bitmap = new Bitmap(1, 1))
            {
                bitmap.SetResolution(600, 600);

                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    foreach (SingleLabel state in states)
                    {
                        SizeF textSize = GetStateLabelSize(graphics, stateFont, subscriptFont, state);

                        if (radius < textSize.Width)
                        {
                            radius = textSize.Width;
                        }

                        if (radius < textSize.Height)
                        {
                            radius = textSize.Height;
                        }
                    }
                }
            }

            return radius;
        }

        public static void DrawCenteredStateName(Graphics g, Font stateFont, Font subscriptFont, RectangleF rect, SingleLabel state)
        {
            SizeF labelSize = GetStateLabelSize(g, stateFont, subscriptFont, state);

            float centerX = (rect.Left + rect.Right) / 2;
            float centerY = (rect.Top + rect.Bottom) / 2;

            float leftX = centerX - labelSize.Width / 2;
            float topY = centerY - labelSize.Height / 2;

            string sign = state.Sign.ToString();

            g.DrawString(sign, stateFont, Brushes.Black, new PointF(leftX, topY));

            SizeF letterSize = g.MeasureString(sign, stateFont);

            if (state.SignIndex != null)
            {
                int v = state.SignIndex.Value;
                leftX += letterSize.Width + 2;
                topY += 0.6f * letterSize.Height;

                g.DrawString(v.ToString(), subscriptFont, Brushes.Black, new PointF(leftX, topY));
            }
        }

        private static string GetArrowText(IEnumerable<char> chars)
        {
            return string.Join(",", chars);
        }

        public static Image DrawDiagram(this FLaGLib.Data.StateMachines.StateMachine stateMachine)
        {
            return null;
            /*

            using (Font stateFont = new Font(_FontName, 100f, FontStyle.Italic, GraphicsUnit.Pixel))
            {
                using (Font transitionFont = new Font(_FontName, 70f, FontStyle.Italic, GraphicsUnit.Pixel))
                {
                    using (Font subscriptFont = new Font(_FontName, 62f, FontStyle.Italic, GraphicsUnit.Pixel))
                    {   
                        IReadOnlyDictionary<Label, SingleLabel> states = 
                            stateMachine.States.Select(s => new KeyValuePair<Label, SingleLabel>(s, s.ExtractSingleLabel())).
                            ToSortedDictionary().AsReadOnly();

                        SingleLabel initialState = states[stateMachine.InitialState];

                        IDictionary<SingleLabel, int> stateIndices = stateMachine.States.Select((s, i) => new KeyValuePair<SingleLabel, int>(states[s], i)).ToDictionary();

                        IReadOnlySet<SingleLabel> finalStates = stateMachine.FinalStates.Select(s => states[s]).ToSortedSet().AsReadOnly();

                        IDictionary<Tuple<SingleLabel, SingleLabel>, ISet<char>> arrows = new Dictionary<Tuple<SingleLabel, SingleLabel>, ISet<char>>();

                        foreach (FLaGLib.Data.StateMachines.Transition transition in stateMachine.Transitions)
                        {
                            Tuple<SingleLabel, SingleLabel> stateTransition = 
                                new Tuple<SingleLabel, SingleLabel>(states[transition.CurrentState], states[transition.NextState]);

                            ISet<char> arrow;

                            if (arrows.ContainsKey(stateTransition))
                            {
                                arrow = arrows[stateTransition];
                            }
                            else
                            {
                                arrow = arrows[stateTransition] = new SortedSet<char>();
                            }

                            arrow.Add(transition.Symbol);
                        }

                        double radius = GetRadiusByFontAndState(stateFont, subscriptFont, states.Values);
                        double startStateRadius = radius - 10.0;

                        double alpha = 2 * Math.PI / states.Count;

                        double length;

                        if (states.Count > 1)
                        {
                            length = radius / (1 - Math.Cos(alpha)) + 2 * radius;
                        }
                        else
                        {
                            length = 0;
                        }

                        double sideSize = 2 * (length + 4 * radius);

                        Bitmap diagram = new Bitmap((int)sideSize, (int)sideSize);

                        diagram.SetResolution(600, 600);

                        using (Graphics graphics = Graphics.FromImage(diagram))
                        {
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.SmoothingMode = SmoothingMode.AntiAlias;

                            using (Pen pen = new Pen(Brushes.Black, 5.0f))
                            {
                                using (Pen boldPen = new Pen(Brushes.Black, 10.0f))
                                {
                                    graphics.TranslateTransform((float)sideSize / 2, (float)sideSize / 2);

                                    foreach (SingleLabel state in states.Values)
                                    {
                                        int index = stateIndices[state];

                                        double left = length * Math.Cos(index * alpha) - radius;
                                        double top = length * Math.Sin(index * alpha) - radius;

                                        Pen drawPen = finalStates.Contains(state) ? boldPen : pen;

                                        RectangleF rect = new RectangleF((float)left, (float)top, (float)(2 * radius), (float)(2 * radius));

                                        graphics.DrawEllipse(drawPen, rect);

                                        if (state == initialState)
                                        {
                                            left = length * Math.Cos(index * alpha) - startStateRadius;
                                            top = length * Math.Sin(index * alpha) - startStateRadius;

                                            rect = new RectangleF((float)left, (float)top, (float)(2 * startStateRadius), (float)(2 * startStateRadius));

                                            graphics.DrawEllipse(pen, rect);
                                        }

                                        DrawCenteredStateName(graphics, stateFont, subscriptFont, rect, state);
                                    }

                                    foreach (KeyValuePair<Tuple<SingleLabel, SingleLabel>, ISet<char>> arrow in arrows)
                                    {
                                        int currentStateIndex = stateIndices[arrow.Key.Item1];
                                        int nextStateIndex = stateIndices[arrow.Key.Item2];

                                        if (currentStateIndex != nextStateIndex)
                                        {
                                            double currentStateX = length * Math.Cos(currentStateIndex * alpha);
                                            double currentStateY = length * Math.Sin(currentStateIndex * alpha);

                                            double nextStateX = length * Math.Cos(nextStateIndex * alpha);
                                            double nextStateY = length * Math.Sin(nextStateIndex * alpha);

                                            float offset = arrows.ContainsKey(new Tuple<SingleLabel, SingleLabel>(arrow.Key.Item2, arrow.Key.Item1)) ? 15.0f : 0.0f;

                                            DrawDirectedLine(graphics, (float)currentStateX, (float)currentStateY, (float)nextStateX, (float)nextStateY, (float)radius, pen, transitionFont, GetArrowText(arrow.Value), offset);
                                        }
                                        else
                                        {
                                            Matrix matrix = graphics.Transform;

                                            float beta = (float)(currentStateIndex * alpha * 180 / Math.PI + 90);

                                            graphics.TranslateTransform((float)(length * Math.Cos(currentStateIndex * alpha)), (float)(length * Math.Sin(currentStateIndex * alpha)));
                                            graphics.RotateTransform(beta);

                                            double arcTop = -radius * (Math.Sqrt(3) + 1.0);

                                            graphics.DrawArc(pen, new RectangleF((float)-radius, (float)arcTop, (float)(2 * radius), (float)(2 * radius)), -225, 270);

                                            float gamma = -225 * (float)Math.PI / 180;

                                            DrawArrow(graphics, new PointF((float)(radius * Math.Cos(gamma)), (float)(arcTop + radius + radius * Math.Sin(gamma))), gamma + (float)Math.PI / 2.0f);

                                            string text = GetArrowText(arrow.Value);

                                            SizeF textSize = graphics.MeasureString(text, transitionFont);

                                            if (Math.Cos(beta * Math.PI / 180) >= 0.0f)
                                            {
                                                PointF pointToDraw = new PointF(-textSize.Width / 2.0f, (float)(arcTop - textSize.Height));

                                                graphics.DrawString(text, transitionFont, Brushes.Black, pointToDraw);
                                            }
                                            else
                                            {
                                                graphics.RotateTransform(180);

                                                PointF pointToDraw = new PointF(-textSize.Width / 2.0f, -(float)(arcTop));

                                                graphics.DrawString(text, transitionFont, Brushes.Black, pointToDraw);
                                            }

                                            graphics.Transform = matrix;
                                        }
                                    }

                                }
                            }
                        }

                        return diagram;
                    }
                }
            } */
        }
    }
}
