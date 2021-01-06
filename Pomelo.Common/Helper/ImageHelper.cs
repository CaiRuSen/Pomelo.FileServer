using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace Pomelo.Common.Helper
{
    public class ImageHelper
    {


        /// <summary> 
        /// 图片等比缩放 
        /// </summary> 
        /// <param name="initImage">原图文件流</param> 
        /// <param name="savePath">缩略图存放地址</param> 
        /// <param name="targetWidth">指定的最大宽度</param> 
        /// <param name="targetHeight">指定的最大高度</param> 
        /// <param name="watermarkText">水印文字(为""表示不使用水印)</param> 
        /// <param name="watermarkImage">水印图片路径(为""表示不使用水印)</param> 
        /// <param name="quality">质量（范围0-100）</param> 
        public static void ZoomAuto(Bitmap bp, string savePath, double targetWidth, double targetHeight, string watermarkText, string watermarkImage, int quality)
        {
            //创建目录 
            string dir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);


            //原始圖片格式
            ImageFormat tFormat = bp.RawFormat;



            var initImage = OrientationImage(bp);


            //原图宽高均小于模版，不作处理，直接保存 
            if ((initImage.Width <= targetWidth && initImage.Height <= targetHeight) || targetWidth == 0 || targetHeight == 0)
            {
                #region 水印
                //文字水印 
                if (watermarkText != "")
                {
                    using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(initImage))
                    {
                        System.Drawing.Font fontWater = new Font("黑体", 10);
                        System.Drawing.Brush brushWater = new SolidBrush(Color.White);
                        gWater.DrawString(watermarkText, fontWater, brushWater, 10, 10);
                        gWater.Dispose();
                    }
                }

                //透明图片水印 
                if (watermarkImage != "")
                {
                    if (System.IO.File.Exists(watermarkImage))
                    {
                        //获取水印图片 
                        using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(watermarkImage))
                        {
                            //水印绘制条件：原始图片宽高均大于或等于水印图片 
                            if (initImage.Width >= wrImage.Width && initImage.Height >= wrImage.Height)
                            {
                                Graphics gWater = Graphics.FromImage(initImage);

                                //透明属性 
                                ImageAttributes imgAttributes = new ImageAttributes();
                                ColorMap colorMap = new ColorMap();
                                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                                ColorMap[] remapTable = { colorMap };
                                imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                                float[][] colorMatrixElements = {
                                   new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5 
                                   new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                };

                                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                gWater.DrawImage(wrImage, new Rectangle(initImage.Width - wrImage.Width, initImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);

                                gWater.Dispose();
                            }
                            wrImage.Dispose();
                        }
                    }
                }
                #endregion

                //保存 
                initImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                //释放资源
                initImage.Dispose();
            }
            else
            {
                //缩略图宽、高计算 
                double newWidth = initImage.Width;
                double newHeight = initImage.Height;

                //宽小于高或宽等于高（竖图或正方） 
                if (initImage.Width <= initImage.Height)
                {
                    //如果宽大于模版 
                    if (initImage.Width > targetWidth)
                    {
                        //宽按模版，高按比例缩放 
                        newWidth = targetWidth;
                        newHeight = initImage.Height * (targetWidth / initImage.Width);
                    }
                }
                //宽大于高（横图） 
                else
                {
                    //如果高大于模版 
                    if (initImage.Height > targetHeight)
                    {
                        //高按模版，宽按比例缩放 
                        newHeight = targetHeight;
                        newWidth = initImage.Width * (targetHeight / initImage.Height);
                    }
                }

                //生成新图 
                //新建一个bmp图片 
                System.Drawing.Image newImage = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);

                //新建一个画板 
                System.Drawing.Graphics newG = System.Drawing.Graphics.FromImage(newImage);

                //设置质量 
                newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //置背景色 
                newG.Clear(Color.White);

                //画图 
                newG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);

                #region 水印
                //文字水印 
                if (watermarkText != "")
                {
                    using (System.Drawing.Graphics gWater = System.Drawing.Graphics.FromImage(newImage))
                    {
                        System.Drawing.Font fontWater = new Font("宋体", 10);
                        System.Drawing.Brush brushWater = new SolidBrush(Color.White);
                        gWater.DrawString(watermarkText, fontWater, brushWater, 10, 10);
                        gWater.Dispose();
                    }
                }

                //透明图片水印 
                if (watermarkImage != "")
                {
                    if (System.IO.File.Exists(watermarkImage))
                    {
                        //获取水印图片 
                        using (System.Drawing.Image wrImage = System.Drawing.Image.FromFile(watermarkImage))
                        {
                            //水印绘制条件：原始图片宽高均大于或等于水印图片 
                            if (newImage.Width >= wrImage.Width && newImage.Height >= wrImage.Height)
                            {
                                Graphics gWater = Graphics.FromImage(newImage);

                                //透明属性 
                                ImageAttributes imgAttributes = new ImageAttributes();
                                ColorMap colorMap = new ColorMap();
                                colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
                                colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
                                ColorMap[] remapTable = { colorMap };
                                imgAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                                float[][] colorMatrixElements = {
                                   new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                   new float[] {0.0f,  0.0f,  0.0f,  0.5f, 0.0f},//透明度:0.5 
                                   new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                };

                                ColorMatrix wmColorMatrix = new ColorMatrix(colorMatrixElements);
                                imgAttributes.SetColorMatrix(wmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                gWater.DrawImage(wrImage, new Rectangle(newImage.Width - wrImage.Width, newImage.Height - wrImage.Height, wrImage.Width, wrImage.Height), 0, 0, wrImage.Width, wrImage.Height, GraphicsUnit.Pixel, imgAttributes);
                                gWater.Dispose();
                            }
                            wrImage.Dispose();
                        }
                    }
                }
                #endregion

                //设置压缩质量
                EncoderParameters ep = new EncoderParameters();
                long[] qy = new long[1];
                qy[0] = (long)quality;//设置压缩的比例1-100
                EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
                ep.Param[0] = eParam;

                try
                {
                    ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                    ImageCodecInfo jpegICIinfo = null;

                    for (int x = 0; x < arrayICI.Length; x++)
                    {
                        if (arrayICI[x].FormatDescription.Equals("JPEG"))
                        {
                            jpegICIinfo = arrayICI[x];
                            break;
                        }
                    }

                    if (jpegICIinfo != null)
                    {
                        //保存缩略图
                        newImage.Save(savePath, jpegICIinfo, ep);
                        //newImage.Save(savePath, jpegICIinfo, ep);
                    }
                    else
                    {
                        //保存缩略图
                        newImage.Save(savePath, tFormat);
                    }
                }
                catch
                {

                }
                finally
                {
                    //释放资源 
                    newG.Dispose();
                    newImage.Dispose();
                    initImage.Dispose();
                }
            }
        }


        private static Image OrientationImage(Image image)
        {
            if (Array.IndexOf(image.PropertyIdList, 274) > -1)
            {
                var orientation = (int)image.GetPropertyItem(274).Value[0];
                switch (orientation)
                {
                    case 1:
                        // No rotation required.
                        break;
                    case 2:
                        image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                image.RemovePropertyItem(274);

            }
            return image;
        }
    }
}
