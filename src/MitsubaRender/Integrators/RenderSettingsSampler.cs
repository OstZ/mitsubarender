﻿// This file is part of MitsubaRenderPlugin project.
//  
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation; either version 3 of the License, or (at your
// option) any later version. This program is distributed in the hope that
// it will be useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details. 
// 
// You should have received a copy of the GNU General Public License
// along with MitsubaRenderPlugin.  If not, see <http://www.gnu.org/licenses/>.
// 
// Copyright 2014 TDM Solutions SL

using System;
using System.ComponentModel;
using System.IO;
using MitsubaRender.Settings;

namespace MitsubaRender.Integrators
{   
    public static class SamplerObjectInstances
    {
        public static SamplerIndependent SamplerIndependent;
        public static SamplerStraitfield SamplerStraitfield;
        public static SamplerLowDiscrepancy SamplerLowDiscrepancy;
        public static SamplerHammersleyQMC SamplerHammersleyQMC;
        public static SamplerHaltonQMC SamplerHaltonQMC;
        public static SamplerSobolQMC SamplerSobolQMC;
       
        public static ISave[] GetSamplersDefaultInstances()
        {
            return new ISave[] { 
                new SamplerIndependent(), 
                new SamplerStraitfield(), 
                new SamplerLowDiscrepancy(),
                new SamplerHammersleyQMC(),
                new SamplerHaltonQMC(),
                new SamplerSobolQMC(),
            };
        }
    }

    public class SamplerIndependent : ISave
    {
        public SamplerIndependent()
        {
            SamplesPerPixel = 64;
        }

        [DisplayName(@"Samples per pixel")]
        [Description("Number of samples per pixel")]
        public int SamplesPerPixel { get; set; }

        public bool Save(string name)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
                name = GetType().Name;

            if (!Directory.Exists(MitsubaSettings.FolderSamplersFolder))
                return false;

            var filePath = Path.Combine(MitsubaSettings.FolderSamplersFolder, name) + LibrarySamplers.Extension;
            return Tools.FileTools.SaveObject(filePath, this);
        }

        public bool Save()
        {
            return Save(null);
        }
    }
    public class SamplerStraitfield : ISave
    {
        public SamplerStraitfield()
        {
            SamplesPerPixel = 64;
            EffectiveDimension = 4;
        }

        [DisplayName(@"Samples per pixel")]
        [Description("Number of samples per pixel; should be a power of two (e.g. 1, 2, 4, 8, 16, etc.), or it will be rounded up to the next one")]
        public int SamplesPerPixel { get; set; }

        [DisplayName(@"Effective dimension")]
        [Description("Effective dimension, up to which stratified samples are provided. The number here is to be interpreted as the number of subsequent 1D or 2D sample requests that can be satisfied using good samples. Higher high values increase both storage and computational costs."
            )]
        public int EffectiveDimension { get; set; }
       
        public bool Save(string name)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
                name = GetType().Name;

            if (!Directory.Exists(MitsubaSettings.FolderSamplersFolder))
                return false;

            var filePath = Path.Combine(MitsubaSettings.FolderSamplersFolder, name) + LibrarySamplers.Extension;
            return Tools.FileTools.SaveObject(filePath, this);
        }

        public bool Save()
        {
            return Save(null);
        }
    }
    public class SamplerLowDiscrepancy : ISave
    {
        public SamplerLowDiscrepancy()
        {
            SamplesPerPixel = 64;
            EffectiveDimension = 4;
        }

        [DisplayName(@"Samples per pixel")]
        [Description(
            "Number of samples per pixel; should be a power of two (e.g. 1, 2, 4, 8, 16, etc.), or it will be rounded up to the next one"
            )]
        public int SamplesPerPixel { get; set; }

        [DisplayName(@"Effective dimension")]
        [Description(
            "Effective dimension, up to which low discrepancy samples are provided. The number here is to be interpreted as the number of subsequent 1D or 2D sample requests that can be satisfied using good samples. Higher high values increase both storage and computational costs."
            )]
        public int EffectiveDimension { get; set; }
       
        public bool Save(string name)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
                name = GetType().Name;

            if (!Directory.Exists(MitsubaSettings.FolderSamplersFolder))
                return false;

            var filePath = Path.Combine(MitsubaSettings.FolderSamplersFolder, name) + LibrarySamplers.Extension;
            return Tools.FileTools.SaveObject(filePath, this);
        }

        public bool Save()
        {
            return Save(null);
        }
    }
    public class SamplerHammersleyQMC : ISave
    {
        public SamplerHammersleyQMC()
        {
            SamplesPerPixel = 64;
            ScrambleValue = -1;
        }

        [DisplayName(@"Samples per pixel")]
        [Description("Number of generated samples / samples per pixel")]
        public int SamplesPerPixel { get; set; }

        [DisplayName(@"Scramble value")]
        [Description(
            "This plugin can operate in one of three scrambling modes: <ul><li>When set to <tt>0</tt>, the implementation will provide the standard Hammersley sequence.</li><li>When set to <tt>-1</tt>, the implementation will compute a scrambled variant of the Hammersley sequence based on permutations by Faure, which has better equidistribution properties in high dimensions.</li><li>When set to a value greater than one, a random permutation is chosen based on this number. This is useful to break up temporally coherent noise when rendering the frames of an animation—in this case, simply set the parameter to the current frame index. </li></ul>Default: <tt>-1</tt>, i.e. use the Faure permutations. Note that permutations rely on a precomputed table that consumes approximately 7 MiB of additional memory at run time."
            )]
        public int ScrambleValue { get; set; }

        public bool Save(string name)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
                name = GetType().Name;

            if (!Directory.Exists(MitsubaSettings.FolderSamplersFolder))
                return false;

            var filePath = Path.Combine(MitsubaSettings.FolderSamplersFolder, name) + LibrarySamplers.Extension;
            return Tools.FileTools.SaveObject(filePath, this);
        }

        public bool Save()
        {
            return Save(null);
        }
    }
    public class SamplerHaltonQMC : ISave
    {
        public SamplerHaltonQMC()
        {
            SamplesPerPixel = 64;
            ScrambleValue = -1;
        }

        [DisplayName(@"Samples per pixel")]
        [Description("Number of generated samples / samples per pixel")]
        public int SamplesPerPixel { get; set; }

        [DisplayName(@"Scramble value")]
        [Description(
            "This plugin can operate in one of three scrambling modes: <ul><li>When set to <tt>0</tt>, the implementation will provide the standard Hammersley sequence.</li><li>When set to <tt>-1</tt>, the implementation will compute a scrambled variant of the Hammersley sequence based on permutations by Faure, which has better equidistribution properties in high dimensions.</li><li>When set to a value greater than one, a random permutation is chosen based on this number. This is useful to break up temporally coherent noise when rendering the frames of an animation—in this case, simply set the parameter to the current frame index. </li></ul>Default: <tt>-1</tt>, i.e. use the Faure permutations. Note that permutations rely on a precomputed table that consumes approximately 7 MiB of additional memory at run time."
            )]
        public int ScrambleValue { get; set; }

        public bool Save(string name)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
                name = GetType().Name;

            if (!Directory.Exists(MitsubaSettings.FolderSamplersFolder))
                return false;

            var filePath = Path.Combine(MitsubaSettings.FolderSamplersFolder, name) + LibrarySamplers.Extension;
            return Tools.FileTools.SaveObject(filePath, this);
        }

        public bool Save()
        {
            return Save(null);
        }
    }
    public class SamplerSobolQMC : ISave
    {
        public SamplerSobolQMC()
        {
            SamplesPerPixel = 64;
            ScrambleValue = 0;
        }

        [DisplayName(@"Samples per pixel")]
        [Description("Number of samples per pixel")]
        public int SamplesPerPixel { get; set; }

        [DisplayName(@"Scramble value")]
        [Description(
            "Scramble value that can be used to break up temporally coherent noise patterns. For stills, this parameter is irrelevant. When rendering an animation, simply set it to the current frame index."
            )]
        public int ScrambleValue { get; set; }

        public bool Save(string name)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
                name = GetType().Name;

            if (!Directory.Exists(MitsubaSettings.FolderSamplersFolder))
                return false;

            var filePath = Path.Combine(MitsubaSettings.FolderSamplersFolder, name) + LibrarySamplers.Extension;
            return Tools.FileTools.SaveObject(filePath, this);
        }

        public bool Save()
        {
            return Save(null);
        }
    }
}