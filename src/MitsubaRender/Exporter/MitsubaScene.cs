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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using MitsubaRender.Emitters;
using MitsubaRender.Materials;
using Rhino;
using Rhino.DocObjects;
using Rhino.Render;

namespace MitsubaRender.Exporter
{
    /// <summary>
    ///   This class handles the Mitsuba scene.
    ///   It contains the XML file handler and the "geometry translator" between Rhino and Mitsuba.
    /// </summary>
    internal class MitsubaScene
    {
        #region Public fields

        /// <summary>
        ///   This constant
        /// </summary>
        public const string MITSUBA_SCENE_VERSION = "0.4.0";

        //TODO default values ?? 
        //TODO use the Setting value perhaps ????
        //TODO summaries !

        /// <summary>
        /// </summary>
        public static string BasePath { get; set; }

        /// <summary>
        /// </summary>
        public static string FileName { get; set; }

        #endregion

        #region Private fields

        /// <summary>
        ///   The instance for create the geometry that Mitsuba understand.
        /// </summary>
        private readonly MeshStore _meshStore;

        /// <summary>
        ///   The XML file handler.
        /// </summary>
        private readonly MitsubaXml _mitsubaXml;

        /// <summary>
        ///   This list handles the different materials of the scene and reuse any material if needed.
        /// </summary>
        private List<string> _materialsUsed;

        /// <summary>
        ///   This list handles the different emitters of the scene and reuse any emitter if needed.
        /// </summary>
        private List<MitsubaEmitter> _emittersUsed;

        #endregion

        /// <summary>
        ///   Main ctor.
        /// </summary>
        /// <param name="basePath">The path of the scene folder.</param>
        /// <param name="fileName">The path of the scene XML file.</param>
        public MitsubaScene(string basePath, string fileName)
        {
            BasePath = basePath;
            FileName = fileName;
            _meshStore = new MeshStore(BasePath);
            _mitsubaXml = new MitsubaXml();
        }

        #region Public methods

        /// <summary>
        ///   This method creates everything that the Mitsuba scene needs.
        /// </summary>
        /// <returns>A string with the XML data.</returns>
        public string ExportSceneFile()
        {
            /* Measure the time spent in Export() */
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            //Create the serialized meshes file
            _meshStore.Create(Path.GetFileNameWithoutExtension(FileName) + ".serialized");
            var sceneFile = Path.Combine(BasePath, FileName);
            try
            {
                _mitsubaXml.CreateDefaultIntegrator();
                ExportAllMaterials();

                //ExportAllLights();
                /* Export all instance defitions and references */
                //ExportAllInstaceDefReference(doc, docRoot);
                /* Export the remaining geometry */
                ExportGeometry();
                /* Create a sensor for the current view */
                _mitsubaXml.ExportSensor();
                /* Export current environment */

                //TODO environment
                ExportEmitters();

                ExportIntegrator();

                //if (RhinoDoc.ActiveDoc.RenderEnvironments.Count > 0) ExportEnvironment(doc, docRoot);
                //else
                //{
                //    /* Create default emitter */
                //    docRoot.AppendChild(CreateEmitter());
                //}
                //Write the XML file in the disk
                _mitsubaXml.WriteData(sceneFile);
            }
            finally
            {
                _meshStore.Close();
                stopwatch.Stop();
                RhinoApp.WriteLine("Export is done (took " + stopwatch.ElapsedMilliseconds + " ms)");
            }
            return sceneFile;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///   Export the geometry to the XML file.
        /// </summary>
        private void ExportGeometry()
        {
            var objects = new List<RhinoObject>(RhinoDoc.ActiveDoc.Objects.FindByObjectType(ObjectType.Brep));
            objects.AddRange(RhinoDoc.ActiveDoc.Objects.FindByObjectType(ObjectType.Extrusion));
            objects.AddRange(RhinoDoc.ActiveDoc.Objects.FindByObjectType(ObjectType.Mesh));

            foreach (var o in objects) ExportRenderMesh(_mitsubaXml.GetRootElement(), o);
        }

        /// <summary>
        ///   Export the render mesh associated with a certain object
        /// </summary>
        /// <param name="parent">The parent node in the output XML document</param>
        /// <param name="obj">The RhinoObject instance to be exported</param>
        /// <returns>true if any content was exported</returns>
        private void ExportRenderMesh(XmlElement parent, RhinoObject obj)
        {
            var type = obj.ObjectType;
            if (type != ObjectType.Surface &&
                type != ObjectType.Brep &&
                type != ObjectType.Mesh &&
                type != ObjectType.Extrusion)
            {
                RhinoApp.WriteLine("Not exporting object of type " + type);
                return;
            }
            var meshes = RhinoObject.GetRenderMeshes(new[] {obj}, true, true);
            if (meshes == null) return;

            foreach (var meshRef in meshes)
            {
                if (meshRef == null) continue;
                var mesh = meshRef.Mesh();
                var index = _meshStore.Store(mesh, obj.Name);
                _mitsubaXml.CreateShape(parent, obj, index, _meshStore.Filename);
            }
        }

        /// <summary>
        ///   This method iterates the Rhino active document and export all Mitsuba material to the XML file.
        /// </summary>
        private void ExportAllMaterials()
        {
            _materialsUsed = new List<string>();

            foreach (var obj in RhinoDoc.ActiveDoc.Objects.GetObjectList(ObjectType.AnyObject))
            {
                if (!obj.Visible) continue;
                var material = obj.RenderMaterial as MitsubaMaterial;
                if (material != null)
                {
                    //!_materialsUsed.Contains(material.GetMaterialId())
                    var isDuplicated = _materialsUsed.Contains(material.GetMaterialId());
                    _mitsubaXml.CreateMaterialXml(material, obj.Id, isDuplicated);
                    _materialsUsed.Add(material.GetMaterialId());
                }
            }

            //foreach (var mat in RhinoDoc.ActiveDoc.RenderMaterials)
            //{
            //    var index = mat.Fields.GetField("index");
            //    var material = mat as MitsubaMaterial;
            //    if (material != null)
            //    {
            //        //if (MaterialsUsed.Contains(material.GetMaterialId()))
            //        _mitsubaXml.CreateMaterialXml(material, idx);
            //    }
            //    idx++;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        private void ExportIntegrator()
        {
            //Pillar datos de 
            //_mitsubaXml.ExportIntegrator(Integrator);
            
        }

        /// <summary>
        /// </summary>
        private void ExportEmitters()
        {
            _emittersUsed = new List<MitsubaEmitter>();

            if (RhinoDoc.ActiveDoc.RenderEnvironments.Count > 0)
            {
                //HDR environment
                var texture = RenderEnvironment.CurrentEnvironment.FindChild("texture");
                if (texture != null)
                {
                    var env_file = texture.Fields.GetField("filename").ValueAsObject();
                    _mitsubaXml.CreateEnvironmentEmitterXml(env_file.ToString());
                }

                //var env = RhinoDoc.ActiveDoc.RenderEnvironments[0];
                //SimulatedEnvironment envSim = new SimulatedEnvironment();
                //env.SimulateEnvironment(ref envSim, true);
            }

            //Lights
            foreach (var obj in RhinoDoc.ActiveDoc.Objects.GetObjectList(ObjectType.Light))
            {
                if (!obj.Visible) continue;

                var objRef = new ObjRef(obj);
                var light = objRef.Light();

                MitsubaEmitter emitter = null;

                if (light.IsPointLight)
                {
                    //var location = light.Location;
                    //var spectrum = light.Diffuse;
                    //var sampleWeight = light.Intensity;
                    emitter = new PointLightSource(light.Location, (float) light.Intensity*100);
                }
                else if (light.IsSpotLight)
                {
                    var origin = light.Location;
                    var target = light.Direction;
                    var cutoffAngle = (float) RhinoMath.ToDegrees(light.SpotAngleRadians);
                    var intensity = (float)light.Intensity * 50000; //TODO Multiplicador SpotLight ???
                    emitter = new SpotLightSource(origin, target, intensity, cutoffAngle);
                }
                else if (light.IsSunLight)
                {
                    //TODO SunLight
                }

                if (emitter != null) _mitsubaXml.CreateEmitterXml(emitter);
            }
        }

        #endregion
    }
}