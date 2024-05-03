﻿using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace Ink_Canvas {
    public partial class MainWindow : Window {
        #region Floating Control

        object lastBorderMouseDownObject;

        private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
            lastBorderMouseDownObject = sender;
        }

        private void FloatingBarIcons_MouseDown_New(object sender, MouseButtonEventArgs e)
        {
            lastBorderMouseDownObject = sender;
            SimpleStackPanel ssp = sender as SimpleStackPanel;
            if (ssp!=null)
            {
                ssp.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/Icons-png/pressdown-background.png"))) { Opacity = 1 };
            }
        }

        bool isStrokeSelectionCloneOn = false;
        private void BorderStrokeSelectionClone_MouseUp(object sender, MouseButtonEventArgs e) {
            if (lastBorderMouseDownObject != sender) return;

            if (isStrokeSelectionCloneOn) {
                BorderStrokeSelectionClone.Background = Brushes.Transparent;

                isStrokeSelectionCloneOn = false;
            } else {
                BorderStrokeSelectionClone.Background = new SolidColorBrush(StringToColor("#FF1ED760"));

                isStrokeSelectionCloneOn = true;
            }
        }

        private void BorderStrokeSelectionCloneToNewBoard_MouseUp(object sender, MouseButtonEventArgs e) {
            if (lastBorderMouseDownObject != sender) return;

            var strokes = inkCanvas.GetSelectedStrokes();
            inkCanvas.Select(new StrokeCollection());
            strokes = strokes.Clone();
            BtnWhiteBoardAdd_Click(null, null);
            inkCanvas.Strokes.Add(strokes);
        }

        private void BorderStrokeSelectionDelete_MouseUp(object sender, MouseButtonEventArgs e) {
            if (lastBorderMouseDownObject != sender) return;
            SymbolIconDelete_MouseUp(sender, e);
        }

        private void GridPenWidthDecrease_MouseUp(object sender, MouseButtonEventArgs e) {
            if (lastBorderMouseDownObject != sender) return;
            ChangeStrokeThickness(0.8);
        }

        private void GridPenWidthIncrease_MouseUp(object sender, MouseButtonEventArgs e) {
            if (lastBorderMouseDownObject != sender) return;
            ChangeStrokeThickness(1.25);
        }

        private void ChangeStrokeThickness(double multipler) {
            foreach (Stroke stroke in inkCanvas.GetSelectedStrokes()) {
                var newWidth = stroke.DrawingAttributes.Width * multipler;
                var newHeight = stroke.DrawingAttributes.Height * multipler;
                if (newWidth >= DrawingAttributes.MinWidth && newWidth <= DrawingAttributes.MaxWidth
                    && newHeight >= DrawingAttributes.MinHeight && newHeight <= DrawingAttributes.MaxHeight) {
                    stroke.DrawingAttributes.Width = newWidth;
                    stroke.DrawingAttributes.Height = newHeight;
                }
            }
        }

        private void GridPenWidthRestore_MouseUp(object sender, MouseButtonEventArgs e) {
            if (lastBorderMouseDownObject != sender) return;

            foreach (Stroke stroke in inkCanvas.GetSelectedStrokes()) {
                stroke.DrawingAttributes.Width = inkCanvas.DefaultDrawingAttributes.Width;
                stroke.DrawingAttributes.Height = inkCanvas.DefaultDrawingAttributes.Height;
            }
        }

        private void ImageFlipHorizontal_MouseUp(object sender, MouseButtonEventArgs e) {
            if (lastBorderMouseDownObject != sender) return;

            Matrix m = new Matrix();

            // Find center of element and then transform to get current location of center
            FrameworkElement fe = e.Source as FrameworkElement;
            Point center = new Point(fe.ActualWidth / 2, fe.ActualHeight / 2);
            center = new Point(inkCanvas.GetSelectionBounds().Left + inkCanvas.GetSelectionBounds().Width / 2,
                inkCanvas.GetSelectionBounds().Top + inkCanvas.GetSelectionBounds().Height / 2);
            center = m.Transform(center);  // 转换为矩阵缩放和旋转的中心点

            // Update matrix to reflect translation/rotation
            m.ScaleAt(-1, 1, center.X, center.Y);  // 缩放

            StrokeCollection targetStrokes = inkCanvas.GetSelectedStrokes();
            StrokeCollection resultStrokes = targetStrokes.Clone();
            foreach (Stroke stroke in resultStrokes) {
                stroke.Transform(m, false);
            }
            _currentCommitType = CommitReason.Rotate;
            inkCanvas.Strokes.Replace(targetStrokes, resultStrokes);
            _currentCommitType = CommitReason.UserInput;
            isProgramChangeStrokeSelection = true;
            inkCanvas.Select(resultStrokes);
            isProgramChangeStrokeSelection = false;

            //updateBorderStrokeSelectionControlLocation();
        }

        private void ImageFlipVertical_MouseUp(object sender, MouseButtonEventArgs e) {
            if (lastBorderMouseDownObject != sender) return;

            Matrix m = new Matrix();

            // Find center of element and then transform to get current location of center
            FrameworkElement fe = e.Source as FrameworkElement;
            Point center = new Point(fe.ActualWidth / 2, fe.ActualHeight / 2);
            center = new Point(inkCanvas.GetSelectionBounds().Left + inkCanvas.GetSelectionBounds().Width / 2,
                inkCanvas.GetSelectionBounds().Top + inkCanvas.GetSelectionBounds().Height / 2);
            center = m.Transform(center);  // 转换为矩阵缩放和旋转的中心点

            // Update matrix to reflect translation/rotation
            m.ScaleAt(1, -1, center.X, center.Y);  // 缩放

            StrokeCollection targetStrokes = inkCanvas.GetSelectedStrokes();
            StrokeCollection resultStrokes = targetStrokes.Clone();
            foreach (Stroke stroke in resultStrokes) {
                stroke.Transform(m, false);
            }
            _currentCommitType = CommitReason.Rotate;
            inkCanvas.Strokes.Replace(targetStrokes, resultStrokes);
            _currentCommitType = CommitReason.UserInput;
            isProgramChangeStrokeSelection = true;
            inkCanvas.Select(resultStrokes);
            isProgramChangeStrokeSelection = false;
        }

        private void ImageRotate45_MouseUp(object sender, MouseButtonEventArgs e) {
            if (lastBorderMouseDownObject != sender) return;

            Matrix m = new Matrix();

            // Find center of element and then transform to get current location of center
            FrameworkElement fe = e.Source as FrameworkElement;
            Point center = new Point(fe.ActualWidth / 2, fe.ActualHeight / 2);
            center = new Point(inkCanvas.GetSelectionBounds().Left + inkCanvas.GetSelectionBounds().Width / 2,
                inkCanvas.GetSelectionBounds().Top + inkCanvas.GetSelectionBounds().Height / 2);
            center = m.Transform(center);  // 转换为矩阵缩放和旋转的中心点

            // Update matrix to reflect translation/rotation
            m.RotateAt(45, center.X, center.Y);  // 旋转

            StrokeCollection targetStrokes = inkCanvas.GetSelectedStrokes();
            StrokeCollection resultStrokes = targetStrokes.Clone();
            foreach (Stroke stroke in resultStrokes) {
                stroke.Transform(m, false);
            }
            _currentCommitType = CommitReason.Rotate;
            inkCanvas.Strokes.Replace(targetStrokes, resultStrokes);
            _currentCommitType = CommitReason.UserInput;
            isProgramChangeStrokeSelection = true;
            inkCanvas.Select(resultStrokes);
            isProgramChangeStrokeSelection = false;
        }

        private void ImageRotate90_MouseUp(object sender, MouseButtonEventArgs e) {
            if (lastBorderMouseDownObject != sender) return;

            Matrix m = new Matrix();

            // Find center of element and then transform to get current location of center
            FrameworkElement fe = e.Source as FrameworkElement;
            Point center = new Point(fe.ActualWidth / 2, fe.ActualHeight / 2);
            center = new Point(inkCanvas.GetSelectionBounds().Left + inkCanvas.GetSelectionBounds().Width / 2,
                inkCanvas.GetSelectionBounds().Top + inkCanvas.GetSelectionBounds().Height / 2);
            center = m.Transform(center);  // 转换为矩阵缩放和旋转的中心点

            // Update matrix to reflect translation/rotation
            m.RotateAt(90, center.X, center.Y);  // 旋转

            StrokeCollection targetStrokes = inkCanvas.GetSelectedStrokes();
            StrokeCollection resultStrokes = targetStrokes.Clone();
            foreach (Stroke stroke in resultStrokes) {
                stroke.Transform(m, false);
            }
            _currentCommitType = CommitReason.Rotate;
            inkCanvas.Strokes.Replace(targetStrokes, resultStrokes);
            _currentCommitType = CommitReason.UserInput;
            isProgramChangeStrokeSelection = true;
            inkCanvas.Select(resultStrokes);
            isProgramChangeStrokeSelection = false;
        }

        #endregion

        bool isGridInkCanvasSelectionCoverMouseDown = false;
        StrokeCollection StrokesSelectionClone = new StrokeCollection();

        private void GridInkCanvasSelectionCover_MouseDown(object sender, MouseButtonEventArgs e) {
            isGridInkCanvasSelectionCoverMouseDown = true;
        }

        private void GridInkCanvasSelectionCover_MouseUp(object sender, MouseButtonEventArgs e) {
            if (isGridInkCanvasSelectionCoverMouseDown) {
                isGridInkCanvasSelectionCoverMouseDown = false;
                GridInkCanvasSelectionCover.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e) {
            forceEraser = true;
            drawingShapeMode = 0;
            inkCanvas.IsManipulationEnabled = false;
            if (inkCanvas.EditingMode == InkCanvasEditingMode.Select) {
                if (inkCanvas.GetSelectedStrokes().Count == inkCanvas.Strokes.Count) {
                    inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                    inkCanvas.EditingMode = InkCanvasEditingMode.Select;
                } else {
                    StrokeCollection selectedStrokes = new StrokeCollection();
                    foreach (Stroke stroke in inkCanvas.Strokes) {
                        if (stroke.GetBounds().Width > 0 && stroke.GetBounds().Height > 0) {
                            selectedStrokes.Add(stroke);
                        }
                    }
                    inkCanvas.Select(selectedStrokes);
                }
            } else {
                inkCanvas.EditingMode = InkCanvasEditingMode.Select;
            }
        }

        double BorderStrokeSelectionControlWidth = 490.0;
        double BorderStrokeSelectionControlHeight = 80.0;
        bool isProgramChangeStrokeSelection = false;

        private void inkCanvas_SelectionChanged(object sender, EventArgs e) {
            if (isProgramChangeStrokeSelection) return;
            if (inkCanvas.GetSelectedStrokes().Count == 0) {
                GridInkCanvasSelectionCover.Visibility = Visibility.Collapsed;
            } else {
                GridInkCanvasSelectionCover.Visibility = Visibility.Visible;
                BorderStrokeSelectionClone.Background = Brushes.Transparent;
                isStrokeSelectionCloneOn = false;
                updateBorderStrokeSelectionControlLocation();
            }
        }

        private void updateBorderStrokeSelectionControlLocation() {
            double borderLeft = (inkCanvas.GetSelectionBounds().Left + inkCanvas.GetSelectionBounds().Right - BorderStrokeSelectionControlWidth) / 2;
            double borderTop = inkCanvas.GetSelectionBounds().Bottom + 15;
            if (borderLeft < 0) borderLeft = 0;
            if (borderTop < 0) borderTop = 0;
            if (Width - borderLeft < BorderStrokeSelectionControlWidth || double.IsNaN(borderLeft)) borderLeft = Width - BorderStrokeSelectionControlWidth;
            if (Height - borderTop < BorderStrokeSelectionControlHeight || double.IsNaN(borderTop)) borderTop = Height - BorderStrokeSelectionControlHeight;

            if (borderTop > 60) borderTop -= 60;
            BorderStrokeSelectionControl.Margin = new Thickness(borderLeft, borderTop, 0, 0);
        }

        private void GridInkCanvasSelectionCover_ManipulationStarting(object sender, ManipulationStartingEventArgs e) {
            e.Mode = ManipulationModes.All;
        }

        private void GridInkCanvasSelectionCover_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e) {

        }

        private void GridInkCanvasSelectionCover_ManipulationDelta(object sender, ManipulationDeltaEventArgs e) {
            try {
                if (dec.Count >= 1) {
                    ManipulationDelta md = e.DeltaManipulation;
                    Vector trans = md.Translation;  // 获得位移矢量
                    double rotate = md.Rotation;  // 获得旋转角度
                    Vector scale = md.Scale;  // 获得缩放倍数

                    Matrix m = new Matrix();

                    // Find center of element and then transform to get current location of center
                    FrameworkElement fe = e.Source as FrameworkElement;
                    Point center = new Point(fe.ActualWidth / 2, fe.ActualHeight / 2);
                    center = new Point(inkCanvas.GetSelectionBounds().Left + inkCanvas.GetSelectionBounds().Width / 2,
                        inkCanvas.GetSelectionBounds().Top + inkCanvas.GetSelectionBounds().Height / 2);
                    center = m.Transform(center);  // 转换为矩阵缩放和旋转的中心点

                    // Update matrix to reflect translation/rotation
                    m.Translate(trans.X, trans.Y);  // 移动
                    m.ScaleAt(scale.X, scale.Y, center.X, center.Y);  // 缩放

                    StrokeCollection strokes = inkCanvas.GetSelectedStrokes();
                    if (StrokesSelectionClone.Count != 0) {
                        strokes = StrokesSelectionClone;
                    } else if (Settings.Gesture.IsEnableTwoFingerRotationOnSelection) {
                        m.RotateAt(rotate, center.X, center.Y);  // 旋转
                    }
                    foreach (Stroke stroke in strokes) {
                        stroke.Transform(m, false);

                        try {
                            stroke.DrawingAttributes.Width *= md.Scale.X;
                            stroke.DrawingAttributes.Height *= md.Scale.Y;
                        } catch { }
                    }

                    updateBorderStrokeSelectionControlLocation();
                }
            } catch { }
        }

        private void GridInkCanvasSelectionCover_TouchDown(object sender, TouchEventArgs e) {
        }

        private void GridInkCanvasSelectionCover_TouchUp(object sender, TouchEventArgs e) {
        }

        Point lastTouchPointOnGridInkCanvasCover = new Point(0, 0);
        private void GridInkCanvasSelectionCover_PreviewTouchDown(object sender, TouchEventArgs e) {
            dec.Add(e.TouchDevice.Id);
            //设备1个的时候，记录中心点
            if (dec.Count == 1) {
                TouchPoint touchPoint = e.GetTouchPoint(null);
                centerPoint = touchPoint.Position;
                lastTouchPointOnGridInkCanvasCover = touchPoint.Position;

                if (isStrokeSelectionCloneOn) {
                    StrokeCollection strokes = inkCanvas.GetSelectedStrokes();
                    isProgramChangeStrokeSelection = true;
                    inkCanvas.Select(new StrokeCollection());
                    StrokesSelectionClone = strokes.Clone();
                    inkCanvas.Select(strokes);
                    isProgramChangeStrokeSelection = false;
                    inkCanvas.Strokes.Add(StrokesSelectionClone);
                }
            }
        }

        private void GridInkCanvasSelectionCover_PreviewTouchUp(object sender, TouchEventArgs e) {
            dec.Remove(e.TouchDevice.Id);
            if (dec.Count >= 1) return;
            isProgramChangeStrokeSelection = false;
            if (lastTouchPointOnGridInkCanvasCover == e.GetTouchPoint(null).Position) {
                if (lastTouchPointOnGridInkCanvasCover.X < inkCanvas.GetSelectionBounds().Left ||
                    lastTouchPointOnGridInkCanvasCover.Y < inkCanvas.GetSelectionBounds().Top ||
                    lastTouchPointOnGridInkCanvasCover.X > inkCanvas.GetSelectionBounds().Right ||
                    lastTouchPointOnGridInkCanvasCover.Y > inkCanvas.GetSelectionBounds().Bottom) {
                    inkCanvas.Select(new StrokeCollection());
                    StrokesSelectionClone = new StrokeCollection();
                }
            } else if (inkCanvas.GetSelectedStrokes().Count == 0) {
                GridInkCanvasSelectionCover.Visibility = Visibility.Collapsed;
                StrokesSelectionClone = new StrokeCollection();
            } else {
                GridInkCanvasSelectionCover.Visibility = Visibility.Visible;
                StrokesSelectionClone = new StrokeCollection();
            }
        }
    }
}
