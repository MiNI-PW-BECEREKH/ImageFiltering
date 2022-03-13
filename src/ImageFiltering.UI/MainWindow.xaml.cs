using ImageFiltering.Common.Models;
using ImageFiltering.FunctionFilters.Extensions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using ImageFiltering.Extensions;
using Point = System.Drawing.Point;
using System.Windows.Input;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Color = System.Drawing.Color;
using System.Xml.Serialization;
using Gu.Wpf.DataGrid2D;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace ImageFiltering.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Stack<WriteableBitmap> reDoStack = new Stack<WriteableBitmap>();
        public Stack<WriteableBitmap> unDoStack = new Stack<WriteableBitmap>();
        Kernel kernelContext { get; set; } = new Kernel(new[,] { { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1, 1, 1, 1, 1 } }, new Point(1, 1));
        WriteableBitmap bitmapToProcess = null;
        public MainWindow()
        {
            InitializeComponent();
            ResetUIComponents();
        }

        private void ShouldEnableCheckBox()
        {
            if (unDoStack.Count > 0 && originalImageCanvas.Source != null && !applyOnTopCheckBox.IsEnabled)
            {
                applyOnTopCheckBox.IsEnabled = true;
            }
        }

        private void ResetUIComponents()
        {
            //kernelDataGrid.SetValue(ItemsSource.Array2DProperty, kernelContext.Matrix);
            kernelDataGrid.SetValue(ItemsSource.Array2DProperty, kernelContext.Matrix);
            //kernelDataGrid.DataContext = kernelContext.Matrix;
            offsetSlider.Value = kernelContext.IntensityOffset;
            divisorTextBox.Text = kernelContext.D.ToString();
            divisorCheckBox.IsChecked = true;
        }

        private void LoadImageClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(fileDialog.FileName));
                WriteableBitmap writeable = new WriteableBitmap(bitmap);
                bitmapToProcess = writeable;
                originalImageCanvas.Source = writeable;
                modifiedImageCanvas.Source = new BitmapImage();
            }
        }

        private void inversionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                unDoStack.Push((WriteableBitmap)modifiedImageCanvas.Source);
                var invertedImage = bitmapToProcess.Inversion();
                modifiedImageCanvas.Source = invertedImage;

                ShouldEnableCheckBox();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void contrastEnhancement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                unDoStack.Push((WriteableBitmap)modifiedImageCanvas.Source);
                var contrastEnhancedImage = bitmapToProcess.ContrastEnhancement(contrastEnhancementSlider.Value);
                modifiedImageCanvas.Source = contrastEnhancedImage;
                if (applyOnTopCheckBox.IsChecked == true)
                    bitmapToProcess = contrastEnhancedImage;

                ShouldEnableCheckBox();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void brightnessCorrection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                unDoStack.Push((WriteableBitmap)modifiedImageCanvas.Source);
                var brightnessCorrected = bitmapToProcess.BrightnessCorrection((int)brightnessCorrectionSlider.Value);
                modifiedImageCanvas.Source = brightnessCorrected;
                if (applyOnTopCheckBox.IsChecked == true)
                    bitmapToProcess = brightnessCorrected;

                ShouldEnableCheckBox();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void gammaCorrectionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                unDoStack.Push((WriteableBitmap)modifiedImageCanvas.Source);
                var gammaCorrected = bitmapToProcess.GammaCorrection(gammaCorrectionSlider.Value);
                modifiedImageCanvas.Source = gammaCorrected;
                if (applyOnTopCheckBox.IsChecked == true)
                    bitmapToProcess = gammaCorrected;

                ShouldEnableCheckBox();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            if (unDoStack.Count > 0)
            {
                reDoStack.Push((WriteableBitmap)modifiedImageCanvas.Source);
                modifiedImageCanvas.Source = unDoStack.Pop();
                if (applyOnTopCheckBox.IsChecked ?? false)
                    bitmapToProcess = (WriteableBitmap)modifiedImageCanvas.Source;
            }
        }

        private void applyOnTopCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bitmapToProcess = (WriteableBitmap)modifiedImageCanvas.Source;
        }

        private void applyOnTopCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            bitmapToProcess = (WriteableBitmap)originalImageCanvas.Source;
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            if (reDoStack.Count > 0)
            {
                unDoStack.Push((WriteableBitmap)modifiedImageCanvas.Source);
                modifiedImageCanvas.Source = reDoStack.Pop();
                if (applyOnTopCheckBox.IsChecked ?? false)
                    bitmapToProcess = (WriteableBitmap)modifiedImageCanvas.Source;

            }
        }


        private void convolutionApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                unDoStack.Push((WriteableBitmap)modifiedImageCanvas.Source);
                var convoluted = bitmapToProcess.Convolution(kernelContext);
                modifiedImageCanvas.Source = convoluted;
                if (applyOnTopCheckBox.IsChecked == true)
                    bitmapToProcess = convoluted;


                ShouldEnableCheckBox();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void kernelDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {

            kernelContext.ReComputeD();
            if (divisorCheckBox.IsChecked == true)
            {
                divisorTextBox.Text = kernelContext.D.ToString();
            }
        }

        private void kernelDataGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right && e.ClickCount == 1)
            {
                var dataGrid = sender as DataGrid;
                ContextMenu contextMenu = dataGrid.ContextMenu;
                contextMenu.PlacementTarget = dataGrid;
                contextMenu.IsOpen = true;

            }
        }



        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image files (*.png)|*.png|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                if (saveFileDialog.FileName != string.Empty)
                {
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create((WriteableBitmap)modifiedImageCanvas.Source));
                        encoder.Save(stream);
                    }
                }

            }
        }

        private void removeColumnButton_Click(object sender, RoutedEventArgs e)
        {
            kernelContext.EraseColumn();
            kernelDataGrid.SetValue(ItemsSource.Array2DProperty, kernelContext.Matrix);
            if (divisorCheckBox.IsChecked == true)
            {
                divisorTextBox.Text = kernelContext.D.ToString();
            }
            UpdateAnchorVisual(sender, e);


        }

        private void addColumnButton_Click(object sender, RoutedEventArgs e)
        {
            kernelContext.AddColumn();
            kernelDataGrid.SetValue(ItemsSource.Array2DProperty, kernelContext.Matrix);
            if (divisorCheckBox.IsChecked == true)
            {
                divisorTextBox.Text = kernelContext.D.ToString();
            }
            UpdateAnchorVisual(sender, e);

        }

        private void removeRowButton_Click(object sender, RoutedEventArgs e)
        {
            kernelContext.EraseRow();
            kernelDataGrid.SetValue(ItemsSource.Array2DProperty, kernelContext.Matrix);
            if (divisorCheckBox.IsChecked == true)
            {
                divisorTextBox.Text = kernelContext.D.ToString();
            }
            UpdateAnchorVisual(sender, e);

        }

        private void addRowButton_Click(object sender, RoutedEventArgs e)
        {
            kernelContext.AddRow();
            kernelDataGrid.SetValue(ItemsSource.Array2DProperty, kernelContext.Matrix);
            if (divisorCheckBox.IsChecked == true)
            {
                divisorTextBox.Text = kernelContext.D.ToString();
            }
            UpdateAnchorVisual(sender, e);


        }

        private void SetAnchorMenu_Click(object sender, RoutedEventArgs e)
        {
            if (kernelDataGrid.CurrentCell.Column != null)
            {
                var oldCell = (DataGridCell)kernelDataGrid.GetCell(kernelContext.Anchor.Y, kernelContext.Anchor.X);
                if (oldCell != null)
                    oldCell.Background = new SolidColorBrush(Colors.White);
                var col = kernelDataGrid.CurrentCell.Column.DisplayIndex;
                var row = kernelDataGrid.Items.IndexOf(kernelDataGrid.CurrentItem);
                kernelContext.Anchor = new Point(col, row);
                var cellToAnchor = (DataGridCell)kernelDataGrid.GetCell(row, col);
                cellToAnchor.Background = new SolidColorBrush(Colors.Yellow);

            }


        }

        private void divisorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (divisorCheckBox.IsChecked == false)
            {
                try
                {
                    kernelContext.D = Convert.ToInt32(divisorTextBox.Text);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                divisorTextBox.Text = kernelContext.D.ToString();

        }

        private void divisorCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            kernelContext.ReComputeD();
            divisorTextBox.Text = kernelContext.D.ToString();
        }

        private void offsetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            kernelContext.IntensityOffset = (int)offsetSlider.Value;
        }

        private void blur_Checked(object sender, RoutedEventArgs e)
        {
            kernelContext = new Kernel(new[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } }, new Point(1, 1), 0);
            ResetUIComponents();

        }

        private void gaussianBlur_Checked(object sender, RoutedEventArgs e)
        {
            kernelContext = new Kernel(new[,] { { 0, 1, 0 }, { 1, 4, 1 }, { 0, 1, 0 } }, new Point(1, 1));
            ResetUIComponents();

        }

        private void sharpen_Checked(object sender, RoutedEventArgs e)
        {
            kernelContext = new Kernel(new[,] { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } }, new Point(1, 1));
            ResetUIComponents();

        }

        private void edgeDetection_Checked(object sender, RoutedEventArgs e)
        {
            kernelContext = new Kernel(new[,] { { 0, -1, 0 }, { 0, 1, 0 }, { 0, 0, 0 } }, new Point(1, 1), 128);
            ResetUIComponents();

        }

        private void emboss_Checked(object sender, RoutedEventArgs e)
        {
            kernelContext = new Kernel(new[,] { { -1, -1, -1 }, { 0, 1, 0 }, { 1, 1, 1 } }, new Point(1, 1));
            ResetUIComponents();

        }

        private void openKernel_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                OpenFileDialog openFileDialog = new OpenFileDialog();
                {
                    openFileDialog.Filter = "xml files (*.xml) | *.xml";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;
                    if (openFileDialog.ShowDialog(this) == true)
                    {
                        if (openFileDialog.FileName != "")
                        {
                            var stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                            var xmlSerializer = new XmlSerializer(typeof(Kernel));

                            kernelContext = (Kernel)xmlSerializer.Deserialize(stream);

                            stream.Close();

                            kernelContext.ApplyDeserialization();
                            ResetUIComponents();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void saveKernel_Click(object sender, RoutedEventArgs e)
        {
            kernelContext.PrepareSerialization();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML Files (*.xml)| *.xml";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog(this) == true)
            {
                if (saveFileDialog.FileName != "")
                {
                    var stream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                    var xmlSerializer = new XmlSerializer(typeof(Kernel));
                    xmlSerializer.Serialize(stream, kernelContext);
                    stream.Close();
                }
            }
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            unDoStack.Clear();
            reDoStack.Clear();
            originalImageCanvas.Source = new BitmapImage();
            modifiedImageCanvas.Source = new BitmapImage();
            bitmapToProcess = null;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
            if (dpd != null)
            {
                dpd.AddValueChanged(kernelDataGrid, UpdateAnchorVisual);
            }

            UpdateAnchorVisual(sender, e);
        }

        private void UpdateAnchorVisual(object sender, EventArgs e)
        {

            var cellToAnchor = kernelDataGrid.GetCell(kernelContext.Anchor.Y, kernelContext.Anchor.X);
            if (cellToAnchor == null)
            {
                kernelContext.Anchor = new Point(kernelContext.Width / 2, kernelContext.Height / 2);
                cellToAnchor = kernelDataGrid.GetCell(kernelContext.Height / 2, kernelContext.Width / 2);

            }
            cellToAnchor.Background = new SolidColorBrush(Colors.Yellow);
        }







        //TODO: Find a better way to validate the checkbox for apply on top
    }
}
