using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace SurveyTool
{

    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer : Window
    {

        Image img;
        private double initOffsetX;
        private double initOffsetY;
        private double imgScale = 1;

        public ImageViewer()
        {
            InitializeComponent();
            //loadImage();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updateTitle();
        }

        public void loadImage(string path)
        {
            // pattern of images: 1 2   or  1 2     or  1 2
            //                    . .       3 .         3 4
            
            ImageGrid.ClipToBounds = true;

            TransformGroup group = new TransformGroup();
            ScaleTransform xform = new ScaleTransform();
            group.Children.Add(xform);

            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);

            img = new Image();

            img.HorizontalAlignment = HorizontalAlignment.Left;
            img.VerticalAlignment = VerticalAlignment.Top;
            ImageGrid.ClipToBounds = true;

            ImageGrid.Children.Add(img);
            img.RenderTransform = group;

            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(path, UriKind.Absolute);//"Sunset.jpg", UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            try
            {
                src.EndInit();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Could not load image from " + path + ". Please make sure the path to the image has been specified correctly.");
                return;
            }
              //  catch (filen

            catch (Exception ex) //TODO: for some reason, after excel is loaded, looks in project folder rather than bin/debug... why?
            {
            }
            img.Source = src;
            img.Stretch = Stretch.Uniform;//.Uniform; //.UniformToFill;
            img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            img.VerticalAlignment = System.Windows.VerticalAlignment.Center;

        }

         ScaleTransform scale = new ScaleTransform();
        bool started = false; //hack to get around not-quite-initialized sizes... TODO: fix

        double scaleVal = 1;
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //scaleVal += e.Delta/1000.0;
            //scale.ScaleX = scaleVal;
            //scale.ScaleY = scaleVal;
            if (!started)
            {
                initOffsetX = (ImageGrid.ActualWidth - img.ActualWidth) / 2.0;
                initOffsetY = (ImageGrid.ActualHeight - img.ActualHeight) / 2.0;
                //effectiveOffsetX += initOffsetX;
                //effectiveOffsetY += initOffsetY;
                started = true;
               // return;
            }

                Matrix m = img.RenderTransform.Value;
                Point p = new Point(m.OffsetX, m.OffsetY);//m.//e.MouseDevice.GetPosition(ImageGrid);
                double h = ImageGrid.ActualHeight / 2;
                double w = ImageGrid.ActualWidth / 2;
                if (e.Delta > 0)
                {
                    effectiveOffsetX += 1.1 * img.ActualWidth * imgScale / 2;
                        //1/1.1 *(w - realOffsetX / 2);
                    //effectiveOffsetX -= .0625*(1.1 * img.ActualWidth * imgScale - img.ActualWidth * imgScale) ;
                    //effectiveOffsetY -= .0625*(1.1 * img.ActualHeight * imgScale - img.ActualHeight * imgScale) ;

                    m.ScaleAtPrepend(1.1, 1.1, w - realOffsetX / 2, h - realOffsetY / 2);
                    imgScale *= 1.1;
                    //m.ScaleAtPrepend(1.1, 1.1, initOffsetX + p.X - w, initOffsetY + p.Y - h);
                    //m.sc

                    //update panning position too:
                    //double midPointX = 

                }
                else
                {

                    //effectiveOffsetX += 1.1 * (w - realOffsetX / 2);
                    effectiveOffsetX += 1.1 * img.ActualWidth * imgScale / 2;
                    m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, w - realOffsetX / 2, h - realOffsetY / 2);
                    imgScale *= 1 / 1.1;
                    effectiveOffsetX *= 1/1.1;
                   // effectiveOffsetX += .0625*(1.1 * img.ActualWidth * imgScale - img.ActualWidth * imgScale) ;
                   // effectiveOffsetY += .0625*(1.1 * img.ActualHeight * imgScale - img.ActualHeight * imgScale) ;
                    //m.ScaleAtPrepend(1.0 / 1.1, 1.0 / 1.1, initOffsetX + p.X - w, initOffsetY + p.Y - h);


                }
                
                //m.OffsetX += pos.X - oldPanX;
                //m.OffsetY += pos.Y - oldPanY;
                //x.RenderTransform = new MatrixTransform(m);
                //m.Scale(
                img.RenderTransform = new MatrixTransform(m);
                img.InvalidateArrange();
                //x.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                //x.VerticalAlignment = System.Windows.VerticalAlignment.Center;



                updateTitle();

            //TODO: update effectiveOffsetX and Y here!!!

        }

        private void updateTitle()
        {
            //ViewerWindow.Title = "ActH:" + (int)ActualHeight + " ActW:" + (int)img.ActualWidth + " Scale:" + imgScale + " W:" + (int)(img.ActualWidth * imgScale) + " H:" + (int)(img.ActualHeight * imgScale);
            ViewerWindow.Title = "XPos: " + (int)((ImageGrid.ActualWidth - img.ActualWidth * imgScale) / 2 + effectiveOffsetX) + "YPos: " + (int)((ImageGrid.ActualHeight - img.ActualHeight * imgScale) / 2 + effectiveOffsetY) + " RealOffsetX:" + realOffsetX + " RealOffsetY:" + realOffsetY;
        }

        bool isPanning = false;
        double oldPanX;
        double oldPanY;
        bool firstPosObtained = false; //we must have something to refer to when we begin dragging, so take first value
        double realOffsetX = 0;
        double realOffsetY = 0;
        double effectiveOffsetX = 0;
        double effectiveOffsetY = 0;

        private void ImageGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPanning = true;
        }

        private void ImageGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isPanning = false;
            firstPosObtained = false; //reset
        }

        double oldVisX = 0;
        double oldVisY = 0;
        private void ImageGrid_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            textBox1.Text = "ActWid: " + imgList[0].ActualWidth + " | Wid: " + imgList[0].Width + 
                " | Scale: " + imgScale + " | Size: " + imgList[0].ActualWidth * imgScale + 
                " | Trans: " + imgList[0].RenderTransform.Value.OffsetX + 
                " | CanX: " + canList[0].ActualWidth;
            textBox2.Text = "ActHei: " + imgList[0].ActualHeight + " | Hei: " + imgList[0].Height+
                " | Scale: " + imgScale + " | Size: " + imgList[0].ActualHeight * imgScale +
                " | Trans: " + imgList[0].RenderTransform.Value.OffsetY +
                " | CanY: " + canList[0].ActualHeight;
            */
             
            if (isPanning)
            {
                if (!firstPosObtained)
                {
                    oldPanX = e.GetPosition(ImageGrid).X;
                    oldPanY = e.GetPosition(ImageGrid).Y;

                    //save the initial visible area of the image -- we'll only permit panning if it does not decrease the visible area:
                    oldVisX = calcVisibleX(oldPanX);
                    oldVisY = calcVisibleY(oldPanY);
                    
                    firstPosObtained = true;
                }
                else
                {
                    Point pos = e.GetPosition(ImageGrid);
                    Matrix m = img.RenderTransform.Value;

                    //make sure we're not reducing area by panning before we allow the pan:
                    double newPanX = realOffsetX + pos.X - oldPanX;
                    double newPanY = realOffsetY + pos.Y - oldPanY;

                    double newVisX = calcVisibleX(newPanX);
                    Grid can = ImageGrid;
                   

                    double imgW = img.ActualWidth * imgScale;
                    double imgH = img.ActualHeight * imgScale;

                    if (imgW < can.ActualWidth)
                    {
                        //if (newPanX > 0 && newPanX < imgW)
                        //{
                            m.OffsetX += pos.X - oldPanX;
                            realOffsetX += pos.X - oldPanX;
                            effectiveOffsetX += (pos.X - oldPanX) * imgScale;
                            oldVisX = newVisX;
                            oldPanX = pos.X;
                        //}
                    }
                    else //image is bigger than frame
                    {
                        double maxDiff = can.ActualWidth - imgW;
                        //if (newPanX >= -maxDiff && newPanX <= maxDiff)
                        //{
                            m.OffsetX += pos.X - oldPanX;
                            realOffsetX += pos.X - oldPanX;
                            effectiveOffsetY += (pos.Y - oldPanY) * imgScale;
                            oldVisX = newVisX;
                            oldPanX = pos.X;
                        //}
                    }


                    
                    if (newVisX >= oldPanX)
                    {
                        m.OffsetX += pos.X - oldPanX;
                        realOffsetX += pos.X - oldPanX;
                        oldVisX = newVisX;
                        oldPanX = pos.X;
                    }
                    double newVisY = calcVisibleY(newPanY);
                    if (newVisY >= oldPanY)
                    {
                        m.OffsetY += pos.Y - oldPanY;
                        realOffsetY += pos.Y - oldPanY;
                        oldVisY = newVisY;
                        oldPanY = pos.Y;
                    }

                        img.RenderTransform = new MatrixTransform(m);

                    
                }
                updateTitle();
            }
        }

        double calcVisibleX(double pan)
        {
            Grid can = ImageGrid;
           
            double endBit = 0;

            if (pan >= 0) //if moved in the +ve direction, this end bit is hidden off the edge of the canvas:
            {
                endBit = pan + img.ActualWidth - can.ActualWidth;
            }
            else //negative direction, this part is off the edge
            {
                endBit = -pan;
            }
            if (endBit < 0) endBit = 0; //since you can't have more than all of the image showing
            if (endBit > img.ActualWidth) endBit = img.ActualWidth; //since you can't hide more than the full image

            return img.ActualWidth - endBit;
        }
        double calcVisibleY(double pan)
        {
            Grid can = ImageGrid;
           
            double endBit = 0;

            if (pan >= 0) //if moved in the +ve direction, this end bit is hidden off the edge of the canvas:
            {
                endBit = pan + img.ActualHeight - can.ActualHeight;
            }
            else //negative direction, this part is off the edge
            {
                endBit = -pan;
            }
            if (endBit < 0) endBit = 0; //since you can't have more than all of the image showing
            if (endBit > img.ActualHeight) endBit = img.ActualHeight; //since you can't hide more than the full image

            return img.ActualHeight - endBit;
        }

    }
}
