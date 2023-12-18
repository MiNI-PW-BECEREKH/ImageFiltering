# ImageFiltering

ImageFiltering is a WPF Application done as a part of Computer Graphics Course in Politechnika Warszawska.

Algorithms are written as Extensions methods of `WriteableBitmap` class in WPF

| Working Functionalities | Method |
|--                       | --
| Inversion               | [`WriteableBitmap Inversion(this WriteableBitmap readBitmap)`](https://github.com/MiNI-PW/ImageFiltering/blob/6d3bb2f215d104f50e495e9bb99ef5a69abb1b8c/src/ImageFiltering.FunctionFilters/FunctionFilterExtensions.cs#L10)
| Brightness Correction   | [`WriteableBitmap BrightnessCorrection(this WriteableBitmap readBitmap, int correctionCoefficient)`](https://github.com/MiNI-PW/ImageFiltering/blob/6d3bb2f215d104f50e495e9bb99ef5a69abb1b8c/src/ImageFiltering.FunctionFilters/FunctionFilterExtensions.cs#L55)
| Contrast Enhancement    | [`WriteableBitmap ContrastEnhancement(this WriteableBitmap readBitmap, double contrastCoefficient)`](https://github.com/MiNI-PW/ImageFiltering/blob/6d3bb2f215d104f50e495e9bb99ef5a69abb1b8c/src/ImageFiltering.FunctionFilters/FunctionFilterExtensions.cs#L105)
| Gamma Correction        | [`WriteableBitmap GammaCorrection(this WriteableBitmap readBitmap, double gamma)`](https://github.com/MiNI-PW/ImageFiltering/blob/6d3bb2f215d104f50e495e9bb99ef5a69abb1b8c/src/ImageFiltering.FunctionFilters/FunctionFilterExtensions.cs#L153)
| Convolution Filters     | [`WriteableBitmap Convolution(this WriteableBitmap readBitmap, Kernel kernel)`](https://github.com/MiNI-PW/ImageFiltering/blob/6d3bb2f215d104f50e495e9bb99ef5a69abb1b8c/src/ImageFiltering.ConvolutionalFilters/ConvolutionFilterExtensions.cs#L9)
| Median Filter           | [`WriteableBitmap ConvolutionMedian(this WriteableBitmap readBitmap)`](https://github.com/MiNI-PW/ImageFiltering/blob/6d3bb2f215d104f50e495e9bb99ef5a69abb1b8c/src/ImageFiltering.ConvolutionalFilters/ConvolutionFilterExtensions.cs#L52)
| Average Dithering       | [`WriteableBitmap AverageDithering(this WriteableBitmap readBitmap, int rK, int bK, int gK, int K = 0, bool isGray = false)`](https://github.com/MiNI-PW/ImageFiltering/blob/6d3bb2f215d104f50e495e9bb99ef5a69abb1b8c/src/ImageFiltering.Dithering/DitheringExtensions.cs#L10)
| MedianCut Quantization  | [`WriteableBitmap MedianCutQuantization(this WriteableBitmap bitmap, int colorsInResult)`](https://github.com/MiNI-PW/ImageFiltering/blob/6d3bb2f215d104f50e495e9bb99ef5a69abb1b8c/src/ImageFiltering.MedianCutQuantization/MedianCut.cs#L9)
