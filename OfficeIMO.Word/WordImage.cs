using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using System.Linq;
using Anchor = DocumentFormat.OpenXml.Drawing.Wordprocessing.Anchor;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using ShapeProperties = DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties;
using A = DocumentFormat.OpenXml.Drawing;
using Pic = DocumentFormat.OpenXml.Drawing.Pictures;
using A14 = DocumentFormat.OpenXml.Office2010.Drawing;
using Wp14 = DocumentFormat.OpenXml.Office2010.Word.Drawing;
using DocumentFormat.OpenXml.Office2010.Word.Drawing;
using SixLabors.ImageSharp.Processing;


namespace OfficeIMO.Word {
    public enum WrapImageText {
        InLineWithText,
        BehindText,
        InFrontText
        // not defined
        //Square,
        //Through,
        //Tight,
        //TopAndBottom
    }

    public class WordImage {
        private const double EnglishMetricUnitsPerInch = 914400;
        private const double PixelsPerInch = 96;

        internal Drawing _Image;
        internal ImagePart _imagePart;

        private WordDocument _document;

        public BlipCompressionValues? CompressionQuality {
            get {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    return picture.BlipFill.Blip.CompressionState;
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        return picture.BlipFill.Blip.CompressionState;
                    }
                }
                return null;
            }
            set { }
        }

        public string RelationshipId {
            get {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    return picture.BlipFill.Blip.Embed;
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        return picture.BlipFill.Blip.Embed;
                    }
                }
                return null;
            }
            set {

            }
        }

        public string FilePath { get; set; }

        /// <summary>
        /// Get or sets the image's file name
        /// </summary>
        public string FileName {
            get {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    return picture.NonVisualPictureProperties.NonVisualDrawingProperties.Name;
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        return picture.NonVisualPictureProperties.NonVisualDrawingProperties.Name;
                    }
                }
                return null;
            }
            set {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    picture.NonVisualPictureProperties.NonVisualDrawingProperties.Name = value;
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        picture.NonVisualPictureProperties.NonVisualDrawingProperties.Name = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the image's description.
        /// </summary>
        public string Description {
            get {

                if (_Image.Inline != null) {
                    return _Image.Inline.DocProperties.Description;

                } else if (_Image.Anchor != null) {
                    var anchoDocPropertiesr = _Image.Anchor.OfType<DocProperties>().FirstOrDefault();
                    return anchoDocPropertiesr.Description;
                }

                return null;

                //if (_Image.Inline != null) {
                //    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                //    return picture.NonVisualPictureProperties.NonVisualDrawingProperties.Description;
                //} else if (_Image.Anchor != null) {
                //    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                //    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                //        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                //        return picture.NonVisualPictureProperties.NonVisualDrawingProperties.Description;
                //    }
                //}
                //return null;
            }
            set {
                if (_Image.Inline != null) {
                    _Image.Inline.DocProperties.Description = value;
                } else if (_Image.Anchor != null) {
                    var anchoDocPropertiesr = _Image.Anchor.OfType<DocProperties>().FirstOrDefault();
                    anchoDocPropertiesr.Description = value;
                }

                //if (_Image.Inline != null) {
                //    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                //    picture.NonVisualPictureProperties.NonVisualDrawingProperties.Description = value;
                //} else if (_Image.Anchor != null) {
                //    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                //    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                //        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                //        picture.NonVisualPictureProperties.NonVisualDrawingProperties.Description = value;
                //    }
                //}
            }
        }

        /// <summary>
        /// Gets or sets Width of an image
        /// </summary>
        public double? Width {
            get {
                if (_Image.Inline != null) {
                    var extents = _Image.Inline.Extent;
                    var cX = extents.Cx;
                    return cX / EnglishMetricUnitsPerInch * PixelsPerInch;
                } else if (_Image.Anchor != null) {
                    var extents = _Image.Anchor.Extent;
                    var cX = extents.Cx;
                    return cX / englishMetricUnitsPerInch * pixelsPerInch;
                }
                return null;
            }
            set {
                double emuWidth = value.Value * EnglishMetricUnitsPerInch / PixelsPerInch;
                if (_Image.Inline != null) {
                    var extents = _Image.Inline.Extent;
                    extents.Cx = (long)emuWidth;
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    picture.ShapeProperties.Transform2D.Extents.Cx = (Int64Value)emuWidth;
                } else if (_Image.Anchor != null) {
                    var extents = _Image.Anchor.Extent;
                    extents.Cx = (long)emuWidth;
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        picture.ShapeProperties.Transform2D.Extents.Cx = (Int64Value)emuWidth;
                    }
                }
                // _Image.Inline.Extent.Cx = (Int64Value)emuWidth;
                //var picture = _Image.Inline.Graphic.GraphicData.ChildElements.OfType<DocumentFormat.OpenXml.Drawing.Pictures.Picture>().First();
            }
        }

        /// <summary>
        /// Gets or sets Height of an image
        /// </summary>
        public double? Height {
            get {
                if (_Image.Inline != null) {
                    var extents = _Image.Inline.Extent;
                    var cY = extents.Cy;
                    return cY / EnglishMetricUnitsPerInch * PixelsPerInch;
                } else if (_Image.Anchor != null) {
                    var extents = _Image.Anchor.Extent;
                    var cY = extents.Cy;
                    return cY / englishMetricUnitsPerInch * pixelsPerInch;
                }
                return null;
            }
            set {
                if (_Image.Inline != null) {
                    double emuHeight = value.Value * EnglishMetricUnitsPerInch / PixelsPerInch;
                    _Image.Inline.Extent.Cy = (Int64Value)emuHeight;
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    picture.ShapeProperties.Transform2D.Extents.Cy = (Int64Value)emuHeight;
                } else if (_Image.Anchor != null) {
                    double emuHeight = value.Value * englishMetricUnitsPerInch / pixelsPerInch;
                    _Image.Anchor.Extent.Cy = (Int64Value)emuHeight;
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        picture.ShapeProperties.Transform2D.Extents.Cy = (Int64Value)emuHeight;
                    }
                }
            }
        }

        public double? EmuWidth {
            get {
                if (_Image.Inline != null) {
                    var extents = _Image.Inline.Extent;
                    return extents.Cx;
                } else if (_Image.Anchor != null) {
                    var extents = _Image.Anchor.Extent;
                    return extents.Cx;
                }
                return null;
            }
        }
        public double? EmuHeight {
            get {
                if (_Image.Inline != null) {
                    var extents = _Image.Inline.Extent;
                    return extents.Cy;
                } else if (_Image.Anchor != null) {
                    var extents = _Image.Anchor.Extent;
                    return extents.Cy;
                }
                return null;
            }
        }

        public ShapeTypeValues? Shape {
            get {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    var presetGeometry = picture.ShapeProperties.GetFirstChild<PresetGeometry>();
                    return presetGeometry.Preset;
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        var presetGeometry = picture.ShapeProperties.GetFirstChild<PresetGeometry>();
                        return presetGeometry.Preset;
                    }
                }

                return null;
            }
            set {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    var presetGeometry = picture.ShapeProperties.GetFirstChild<PresetGeometry>();
                    presetGeometry.Preset = value;
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        var presetGeometry = picture.ShapeProperties.GetFirstChild<PresetGeometry>();
                        presetGeometry.Preset = value;
                    }
                }

            }
        }

        /// <summary>
        /// Microsoft Office does not seem to fully support this attribute, and ignores this setting.
        /// More information: http://officeopenxml.com/drwSp-SpPr.php
        /// </summary>
        public BlackWhiteModeValues? BlackWiteMode {
            get {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    return picture.ShapeProperties.BlackWhiteMode.Value;
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        return picture.ShapeProperties.BlackWhiteMode.Value;
                    }
                }

                return null;
            }
            set {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();

                    if (value == null) {
                        // delete?
                    } else {
                        if (picture.ShapeProperties.BlackWhiteMode == null) {
                            picture.ShapeProperties.BlackWhiteMode = new EnumValue<BlackWhiteModeValues>();
                        }
                        picture.ShapeProperties.BlackWhiteMode.Value = value.Value;
                    }
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (value == null) {
                        // delete?
                    } else {
                        if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                            var picture = anchorGraphic.GraphicData
                                .GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                            if (picture.ShapeProperties.BlackWhiteMode == null) {
                                picture.ShapeProperties.BlackWhiteMode = new EnumValue<BlackWhiteModeValues>();
                            }

                            picture.ShapeProperties.BlackWhiteMode.Value = value.Value;
                        }
                    }
                }
            }
        }
        public bool? VerticalFlip {
            get {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    return picture.ShapeProperties.Transform2D.VerticalFlip;
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        return picture.ShapeProperties.Transform2D.VerticalFlip;
                    }
                }

                return null;
            }
            set {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    picture.ShapeProperties.Transform2D.VerticalFlip = value.Value;
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        picture.ShapeProperties.Transform2D.VerticalFlip = value.Value;
                    }
                }
            }
        }
        public bool? HorizontalFlip {
            get {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData
                        .GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    return picture.ShapeProperties.Transform2D.HorizontalFlip;
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        return picture.ShapeProperties.Transform2D.HorizontalFlip;
                    }
                }

                return null;
            }
            set {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    picture.ShapeProperties.Transform2D.HorizontalFlip = value.Value;
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        picture.ShapeProperties.Transform2D.HorizontalFlip = value.Value;
                    }
                }
            }
        }


        public int? Rotation {
            get {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    if (picture.ShapeProperties.Transform2D.Rotation != null) {
                        return picture.ShapeProperties.Transform2D.Rotation / 10000;
                    }
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        if (picture.ShapeProperties.Transform2D.Rotation != null) {
                            return picture.ShapeProperties.Transform2D.Rotation / 10000;
                        }
                    }
                }

                return null;
            }
            set {
                if (_Image.Inline != null) {
                    var picture = _Image.Inline.Graphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                    if (value == null) {
                        picture.ShapeProperties.Transform2D.Rotation = null;
                    } else {
                        picture.ShapeProperties.Transform2D.Rotation = value.Value * 10000;
                    }
                } else if (_Image.Anchor != null) {
                    var anchorGraphic = _Image.Anchor.OfType<Graphic>().FirstOrDefault();
                    if (anchorGraphic != null && anchorGraphic.GraphicData != null) {
                        var picture = anchorGraphic.GraphicData.GetFirstChild<DocumentFormat.OpenXml.Drawing.Pictures.Picture>();
                        if (value == null) {
                            picture.ShapeProperties.Transform2D.Rotation = null;
                        } else {
                            picture.ShapeProperties.Transform2D.Rotation = value.Value * 10000;
                        }
                    }
                }
            }
        }

        public bool? Wrap {
            // this doesn't work
            get {
                return null;
            }
            set {

                //if (_Image.Anchor == null) {
                //    var inline = _Image.Inline.CloneNode(true);

                //    IEnumerable<OpenXmlElement> clonedElements = _Image.Inline
                //        .Elements()
                //        .Select(e => e.CloneNode(true))
                //        .ToList();

                //    var childInline = inline.Descendants();
                //    Anchor anchor1 = new Anchor() { BehindDoc = true };
                //    WrapNone wrapNone1 = new WrapNone();
                //    anchor1.Append(wrapNone1);
                //    _Image.Append(anchor1);

                //    _Image.Inline.Remove();

                //    _Image.Anchor.Append(clonedElements);
                //} else {
                //    _Image.Anchor.AllowOverlap = true;
                //}
            }
        }



        public WordImage(WordDocument document, string filePath, ShapeTypeValues shape = ShapeTypeValues.Rectangle, BlipCompressionValues compressionQuality = BlipCompressionValues.Print) {
            double width;
            double height;
            using (var img = SixLabors.ImageSharp.Image.Load(filePath)) {
                width = img.Width;
                height = img.Height;
            }
        }

        public WordImage(WordDocument document, Paragraph paragraph) {

        }

        public WordImage(
            WordDocument document,
            string filePath,
            double? width,
            double? height,
            ShapeTypeValues shape = ShapeTypeValues.Rectangle,
            BlipCompressionValues compressionQuality = BlipCompressionValues.Print) {
            FilePath = filePath;
            var fileName = System.IO.Path.GetFileName(filePath);
            using var imageStream = new FileStream(filePath, FileMode.Open);
            AddImage(document, imageStream, fileName, width, height, shape, compressionQuality);
        }

        public WordImage(
            WordDocument document,
            Stream imageStream,
            string fileName,
            double? width,
            double? height,
            ShapeTypeValues shape = ShapeTypeValues.Rectangle,
            BlipCompressionValues compressionQuality = BlipCompressionValues.Print) {
            FilePath = fileName;
            AddImage(document, imageStream, fileName, width, height, shape, compressionQuality);
        }

        public WordImage(WordDocument document, Drawing drawing) {
            _document = document;
            _Image = drawing;
            var imageParts = document._document.MainDocumentPart.ImageParts;
            foreach (var imagePart in imageParts) {
                var relationshipId = document._wordprocessingDocument.MainDocumentPart.GetIdOfPart(imagePart);
                if (this.RelationshipId == relationshipId) {
                    this._imagePart = imagePart;
                }
            }
        }

        /// <summary>
        /// Extract image from Word Document and save it to file
        /// </summary>
        /// <param name="fileToSave"></param>
        public void SaveToFile(string fileToSave) {
            using (FileStream outputFileStream = new FileStream(fileToSave, FileMode.Create)) {
                var stream = this._imagePart.GetStream();
                stream.CopyTo(outputFileStream);
                stream.Close();
            }
        }

        /// <summary>
        /// Remove image from a Word Document
        /// </summary>
        public void Remove() {
            if (this._imagePart != null) {
                _document._wordprocessingDocument.MainDocumentPart.DeletePart(_imagePart);
            }

            if (this._Image != null) {
                this._Image.Remove();
            }
        }

        private void AddImage(
            WordDocument document,
            Stream imageStream,
            string fileName,
            double? width,
            double? height,
            ShapeTypeValues shape,
            BlipCompressionValues compressionQuality) {
            _document = document;

            // Size - https://stackoverflow.com/questions/8082980/inserting-image-into-docx-using-openxml-and-setting-the-size

            // if widht/height are not set we check ourselves
            // but probably will need better way
            var imageСharacteristics = Helpers.GetImageСharacteristics(imageStream);
            if (width == null || height == null) {
                width = imageСharacteristics.Width;
                height = imageСharacteristics.Height;
            }

            _imagePart = document._wordprocessingDocument.MainDocumentPart.AddImagePart(imageСharacteristics.Type);
            _imagePart.FeedData(imageStream);

            var relationshipId = document._wordprocessingDocument.MainDocumentPart.GetIdOfPart(_imagePart);

            //calculate size in emu
            var emuWidth = width.Value * EnglishMetricUnitsPerInch / PixelsPerInch;
            var emuHeight = height.Value * EnglishMetricUnitsPerInch / PixelsPerInch;

            var shapeProperties = new ShapeProperties(
                new Transform2D(new Offset {X = 0L, Y = 0L},
                    new Extents {
                        Cx = (Int64Value) emuWidth,
                        Cy = (Int64Value) emuHeight
                    }
                ),
                new PresetGeometry(new AdjustValueList()) {Preset = shape}
            );

            var imageName = System.IO.Path.GetFileNameWithoutExtension(fileName);

            var drawing = new Drawing(
                //new Anchor(
                //    new WrapNone()
                //    ) { BehindDoc = true },
                new Inline(
                    new Extent {Cx = (Int64Value) emuWidth, Cy = (Int64Value) emuHeight},
                    new EffectExtent {
                        LeftEdge = 0L,
                        TopEdge = 0L,
                        RightEdge = 0L,
                        BottomEdge = 0L
                    },
                    new DocProperties {
                        Id = (UInt32Value) 1U,
                        Name = imageName
                    },
                    new DocumentFormat.OpenXml.Drawing.Wordprocessing.NonVisualGraphicFrameDrawingProperties(
                        new GraphicFrameLocks {NoChangeAspect = true}),
                    new Graphic(
                        new GraphicData(
                            new DocumentFormat.OpenXml.Drawing.Pictures.Picture(
                                new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties(
                                    new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties {
                                        Id = (UInt32Value) 0U,
                                        Name = fileName
                                    },
                                    new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties()),
                                new DocumentFormat.OpenXml.Drawing.Pictures.BlipFill(
                                    new Blip(new BlipExtensionList(new BlipExtension {
                                            // https://stackoverflow.com/questions/33521914/value-of-blipextension-schema-uri-28a0092b-c50c-407e-a947-70e740481c1c
                                            Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                        })
                                    ) {
                                        Embed = relationshipId,
                                        CompressionState = compressionQuality
                                    },
                                    new Stretch(new FillRectangle())),
                                shapeProperties)
                        ) {Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture"})
                ) {
                    DistanceFromTop = (UInt32Value) 0U,
                    DistanceFromBottom = (UInt32Value) 0U,
                    DistanceFromLeft = (UInt32Value) 0U,
                    DistanceFromRight = (UInt32Value) 0U,
                    EditId = "50D07946"
                });

            _Image = drawing;
            Shape = shape;
        }
    }
}
