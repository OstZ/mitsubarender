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
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml;
using MitsubaRender.Emitters;
using MitsubaRender.Materials;
using MitsubaRender.Materials.Wrappers;
using MitsubaRender.RenderSettings;
using MitsubaRender.Settings;
using MitsubaRender.Tools;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace MitsubaRender.Exporter
{
	/// <summary>
	///   This class handles the XML file.
	/// </summary>
	public class MitsubaXml
	{
		#region Private fields

		/// <summary>
		/// </summary>
		private static XmlDocument _document;

		/// <summary>
		///   This fields contains the geometry ID with its Mitsuba material.
		/// </summary>
		public Dictionary<Guid, string> _mitsubaObjects;

		/// <summary>
		/// This is used to align (rotate) the HDR environment in Mitsuba.
		/// </summary>
		public static readonly Transform _toWorldTransform = Transform.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin);

		#endregion

		public MitsubaXml()
		{
			_document = new XmlDocument();
			_document.LoadXml("<?xml version=\"1.0\"?> <scene version=\"" + MitsubaScene.MITSUBA_SCENE_VERSION + "\"/>");
			_mitsubaObjects = new Dictionary<Guid, string>();
		}

		#region Public methods

		/// <summary>
		///   TODO summary
		/// </summary>
		public void AddToXmlRoot(XmlElement element)
		{
			GetRootElement().AppendChild(element);
		}

		/// <summary>
		///   Create an XML reference node
		/// </summary>
		/// <param name="name">Name of the referenced object</param>
		/// <returns>The created XML element</returns>
		public XmlElement MakeReference(string name)
		{
			var element = _document.CreateElement("ref");
			element.SetAttribute("id", name);
			return element;
		}

		/// <summary>
		///   Create default Emitter
		/// </summary>
		/// <returns>XmlElement with a default emitter</returns>
		public XmlElement CreateDefaultEmitter()
		{
			var defaultEmitter = _document.CreateElement("emitter");
			var emitterTrafo = _document.CreateElement("transform");
			var rotateTrafo = _document.CreateElement("rotate");
			var extendProperty = _document.CreateElement("boolean");
			var sunScale = _document.CreateElement("float");
			defaultEmitter.SetAttribute("type", "sunsky");
			extendProperty.SetAttribute("name", "extend");
			extendProperty.SetAttribute("value", "true");
			sunScale.SetAttribute("name", "sunRadiusScale");
			sunScale.SetAttribute("value", "10");
			emitterTrafo.SetAttribute("name", "toWorld");
			rotateTrafo.SetAttribute("x", "1");
			rotateTrafo.SetAttribute("angle", "90");
			emitterTrafo.AppendChild(rotateTrafo);
			defaultEmitter.AppendChild(emitterTrafo);
			defaultEmitter.AppendChild(extendProperty);
			defaultEmitter.AppendChild(sunScale);
			return defaultEmitter;
		}

		/// <summary>
		/// </summary>
		/// <param name="emitter_file"></param>
		/// <returns></returns>
		public void CreateEnvironmentEmitterXml(string emitter_file)
		{
			var emitter = CreateEmitter.EnvironmentEmitter(emitter_file);

			if (emitter != null) AddToXmlRoot(emitter);
		}

		/// <summary>
		/// </summary>
		/// <param name="emitter"></param>
		/// <returns></returns>
		public XmlElement CreateEmitterXml(MitsubaEmitter emitter)
		{
			XmlElement result = null;

			var type = emitter.GetType();

			if (type == typeof(PointLightSource))
				result = CreateEmitter.PointLightSource((PointLightSource)emitter);
			else if (type == typeof(SpotLightSource))
				result = CreateEmitter.SpotLightSource((SpotLightSource)emitter);

			if (result != null) AddToXmlRoot(result);

			return result;
		}

		/// <summary>
		///   TODO summary
		/// </summary>
		public void CreateIntegratorXml()
		{
			XmlElement result;

			if (MitsubaSettings.Integrator == null)
				result = CreateIntegrator.Create(new IntegratorPhotonMapper());

			else if (MitsubaSettings.Integrator is IntegratorAmbientOclusion) {
				var integrator = MitsubaSettings.Integrator as IntegratorAmbientOclusion;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorDirectIlumination) {
				var integrator = MitsubaSettings.Integrator as IntegratorDirectIlumination;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorPathTracer) {
				var integrator = MitsubaSettings.Integrator as IntegratorPathTracer;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorVolumetricPathTracerSimple) {
				var integrator = MitsubaSettings.Integrator as IntegratorVolumetricPathTracerSimple;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorVolumetricPathTracerExtended) {
				var integrator = MitsubaSettings.Integrator as IntegratorVolumetricPathTracerExtended;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorAdjointParticleTracer) {
				var integrator = MitsubaSettings.Integrator as IntegratorAdjointParticleTracer;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorVirtualPointLightRenderer) {
				var integrator = MitsubaSettings.Integrator as IntegratorVirtualPointLightRenderer;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorPhotonMapper) {
				var integrator = MitsubaSettings.Integrator as IntegratorPhotonMapper;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorProgressivePhotonMapper) {
				var integrator = MitsubaSettings.Integrator as IntegratorProgressivePhotonMapper;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorStochasticProgressivePhotonMapper) {
				var integrator = MitsubaSettings.Integrator as IntegratorStochasticProgressivePhotonMapper;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorBidirectionalPathTracer) {
				var integrator = MitsubaSettings.Integrator as IntegratorBidirectionalPathTracer;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorPrimarySampleSpaceMLT) {
				var integrator = MitsubaSettings.Integrator as IntegratorPrimarySampleSpaceMLT;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorSampleSpaceMLT) {
				var integrator = MitsubaSettings.Integrator as IntegratorSampleSpaceMLT;
				result = CreateIntegrator.Create(integrator);
			}
			else if (MitsubaSettings.Integrator is IntegratorEnergyRedisributionPathTracing) {
				var integrator = MitsubaSettings.Integrator as IntegratorEnergyRedisributionPathTracing;
				result = CreateIntegrator.Create(integrator);
			}
			//This Integrator doesn't exist in mitsuba Config UI, but it's documented
			else if (MitsubaSettings.Integrator is IntegratorAdaptativeIntegrator) {
				var integrator = MitsubaSettings.Integrator as IntegratorAdaptativeIntegrator;
				result = CreateIntegrator.Create(integrator);
			}
			//This Integrator doesn't exist in mitsuba Config UI, but it's documented
			else if (MitsubaSettings.Integrator is IntegratorIrradianceCaching) {
				var integrator = MitsubaSettings.Integrator as IntegratorIrradianceCaching;
				result = CreateIntegrator.Create(integrator);
			}
			else
				result = CreateIntegrator.Create(new IntegratorPhotonMapper());

			if (result != null) AddToXmlRoot(result);
		}

		/// <summary>
		/// This method creates a ground in the mitsuba XML, based on the Rhino GroundPlane.
		/// </summary>
		public void CreateGround(MitsubaMaterial material, double altitude)
		{
			var element = _document.CreateElement("shape");
			element.SetAttribute("type", "rectangle");
			var transform = AddElement("transform", "toWorld");
			transform.AppendChild(AddElement("scale", null, new Point3d(1000, 1000, 10)));

			var translate = _document.CreateElement("translate");
			translate.SetAttribute("z", altitude + "");
			transform.AppendChild(translate);

			element.AppendChild(transform);

			if (material != null) {
				var result = CreateMaterialXml(material);

				if (result != null) element.AppendChild(result);
			}

			AddToXmlRoot(element);
		}

		/// <summary>
		///
		/// </summary>
		public void CreateSamplerXml()
		{
			XmlElement result;

			if (MitsubaSettings.Sampler == null)
				result = CreateSampler.Create(new SamplerIndependent());

			else if (MitsubaSettings.Sampler is SamplerHaltonQMC) {
				var sampler = MitsubaSettings.Sampler as SamplerHaltonQMC;
				result = CreateSampler.Create(sampler);
			}
			else if (MitsubaSettings.Sampler is SamplerHammersleyQMC) {
				var sampler = MitsubaSettings.Sampler as SamplerHammersleyQMC;
				result = CreateSampler.Create(sampler);
			}
			else if (MitsubaSettings.Sampler is SamplerIndependent) {
				var sampler = MitsubaSettings.Sampler as SamplerIndependent;
				result = CreateSampler.Create(sampler);
			}
			else if (MitsubaSettings.Sampler is SamplerLowDiscrepancy) {
				var sampler = MitsubaSettings.Sampler as SamplerLowDiscrepancy;
				result = CreateSampler.Create(sampler);
			}
			else if (MitsubaSettings.Sampler is SamplerSobolQMC) {
				var sampler = MitsubaSettings.Sampler as SamplerSobolQMC;
				result = CreateSampler.Create(sampler);
			}
			else if (MitsubaSettings.Sampler is SamplerStraitfield) {
				var sampler = MitsubaSettings.Sampler as SamplerStraitfield;
				result = CreateSampler.Create(sampler);
			}
			else
				result = CreateSampler.Create(new SamplerIndependent());


			if (result != null) AddToXmlRoot(result);

		}

		/// <summary>
		///
		/// </summary>
		/// <param name="material"></param>
		/// <returns></returns>
		public static XmlElement CreateMaterialXml(MitsubaMaterial material)
		{
			XmlElement result = null;

			var materialType = material.GetType();

			if (materialType == typeof(SmoothDiffuseMaterial))
				result = CreateMaterial.SmoothDiffuseMaterial((SmoothDiffuseMaterial)material);
			else if (materialType == typeof(RoughConductorMaterial))
				result = CreateMaterial.RoughConductorMaterial((RoughConductorMaterial)material);
			else if (materialType == typeof(SmoothDielectricMaterial))
				result = CreateMaterial.SmoothDielectricMaterial((SmoothDielectricMaterial)material);
			else if (materialType == typeof(SmoothConductorMaterial))
				result = CreateMaterial.SmoothConductorMaterial((SmoothConductorMaterial)material);
			else if (materialType == typeof(RoughDiffuseMaterial))
				result = CreateMaterial.RoughDiffuseMaterial((RoughDiffuseMaterial)material);
			else if (materialType == typeof(RoughDielectricMaterial))
				result = CreateMaterial.RoughDielectricMaterial((RoughDielectricMaterial)material);
			else if (materialType == typeof(SmoothPlasticMaterial))
				result = CreateMaterial.SmoothPlasticMaterial((SmoothPlasticMaterial)material);
			else if (materialType == typeof(RoughPlasticMaterial))
				result = CreateMaterial.RoughPlasticMaterial((RoughPlasticMaterial)material);

			//else if (materialType == typeof (SmoothDielectricCoatingMaterial))
			//    result = CreateMaterial.SmoothDielectricCoatingMaterial((SmoothDielectricCoatingMaterial) material);

			if (result != null) return result;

			return null;
		}

		/// <summary>
		///   This method defines the default integrator. Must be Photon mapper ?!
		/// </summary>
		public void CreateMaterialXml(MitsubaMaterial material, Guid objId, bool isDuplicated)
		{
			if (!isDuplicated) {
				var result = CreateMaterialXml(material);

				if (result != null) AddToXmlRoot(result);
			}

			//Store the material with the object to create a reference later
			_mitsubaObjects.Add(objId, material.GetMaterialId());
		}

		/// <summary>
		///   The method returns the root tag of the XML file (must be a "scene" tag).
		/// </summary>
		/// <returns>The root tag of the XML file</returns>
		public XmlElement GetRootElement()
		{
			return _document.DocumentElement;
		}

		/// <summary>
		///   It creates a new element in the XML file.
		/// </summary>
		/// <param name="tag">tag for the xml element</param>
		/// <param name="name">name="input name"</param>
		/// <param name="value">value="input value"</param>
		/// <param name="type">type="input type" </param>
		/// <returns></returns>
		public static XmlElement AddElement(string tag, string name, string value = null, string type = null)
		{
			var element = _document.CreateElement(tag);
			element.SetAttribute("name", name);

			if (!String.IsNullOrEmpty(type)) element.SetAttribute("type", type);

			if (!String.IsNullOrEmpty(value)) element.SetAttribute("value", value);

			return element;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public static XmlElement AddElement(string tag, string name, Point3d point)
		{
			var element = _document.CreateElement(tag);

			if (name != null) element.SetAttribute("name", name);

			element.SetAttribute("x", point.X + "");
			element.SetAttribute("y", point.Y + "");
			element.SetAttribute("z", point.Z + "");
			return element;
		}

		/// <summary>
		/// </summary>
		/// <param name="transform"></param>
		/// <returns></returns>
		public static XmlElement AddElement(MitsubaTransform transform)
		{
			var element = _document.CreateElement("transform");
			element.SetAttribute("name", "toWorld");
			var lookat = _document.CreateElement("lookat");
			lookat.SetAttribute("origin", transform.GetOriginForMitsuba());
			lookat.SetAttribute("target", transform.GetTargetForMitsuba());
			element.AppendChild(lookat);
			return element;
		}

		/// <summary>
		///   Export an a perspective or orthographic sensor
		/// </summary>
		/// <returns>True if any content was exported</returns>
		public bool ExportSensor()
		{
			var view = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;
			var perspective = view.IsPerspectiveProjection;
			var orthographic = view.IsParallelProjection;

			if (!perspective && !orthographic) {
				RhinoApp.WriteLine("Warning: camera type not supported -- ignoring.");
				return false;
			}

			double left, right, bottom, top, near, far;
			view.GetFrustum(out left, out right, out bottom, out top, out near, out far);
			var sensorElement = _document.CreateElement("sensor");
			sensorElement.SetAttribute("type", perspective ? "perspective" : "orthographic");

			var toWorld = view.GetTransform(CoordinateSystem.Camera, CoordinateSystem.World);
			toWorld = toWorld * Transform.Mirror(new Plane(new Point3d(0, 0, 0), new Vector3d(0, 0, -1)));
			toWorld = toWorld * Transform.Mirror(new Plane(new Point3d(0, 0, 0), new Vector3d(-1, 0, 0)));

			var toWorldElement = MakeProperty("toWorld", toWorld);

			if (perspective) {
				var focusDistance = view.CameraLocation.DistanceTo(view.CameraTarget);
				double halfDiag, halfVert, halfHoriz;
				view.GetCameraAngle(out halfDiag, out halfVert, out halfHoriz);
				//toWorld = toWorld * Transform.Mirror(new Plane(new Point3d(0, 0, 0), new Vector3d(0, 0, -1)));
				sensorElement.AppendChild(MakeProperty("fovAxis", "diagonal"));
				sensorElement.AppendChild(MakeProperty("fov", 2 * halfDiag * 180 / Math.PI));
				sensorElement.AppendChild(MakeProperty("focusDistance", focusDistance));
			}
			else {
				var scaleNode = _document.CreateElement("scale");
				var scale = (right - left) / 2;
				scaleNode.SetAttribute("x", ToStringHelper(scale));
				scaleNode.SetAttribute("y", ToStringHelper(scale));
				toWorldElement.PrependChild(scaleNode);
			}

			// Some extra room for nativating using the interactive viewer
			near /= 10;
			far *= 10;
			sensorElement.AppendChild(toWorldElement);
			sensorElement.AppendChild(MakeProperty("nearClip", near));
			sensorElement.AppendChild(MakeProperty("farClip", far));

			var filmElement = _document.CreateElement("film");
			filmElement.SetAttribute("type", "hdrfilm");
			//filmElement.AppendChild(MakeProperty("width", m_settings.xres));
			//filmElement.AppendChild(MakeProperty("height", m_settings.yres));
			filmElement.AppendChild(MakeProperty("width", 1024));
			filmElement.AppendChild(MakeProperty("height", 768));
			sensorElement.AppendChild(filmElement);

			var independentSampler = false;
			//if (m_settings.integrator == MitsubaSettings.Integrator.EAdjointParticleTracer ||
			//    m_settings.integrator == MitsubaSettings.Integrator.EKelemenMLT ||
			//    m_settings.integrator == MitsubaSettings.Integrator.EVeachMLT) {
			//    /* These integrators require the independent sampler */
			//    independentSampler = true;
			//}

			var samplerElement = _document.CreateElement("sampler");
			samplerElement.SetAttribute("type", independentSampler ? "independent" : "ldsampler");
			//samplerElement.AppendChild(MakeProperty("sampleCount", m_settings.samplesPerPixel));
			samplerElement.AppendChild(MakeProperty("sampleCount", 4));
			sensorElement.AppendChild(samplerElement);
			GetRootElement().AppendChild(sensorElement);

			return true;
		}

		/// <summary>
		///   This method creates a new shape in the XML file. If the shape has a reference to any BSDF
		///   (Bidirectional scattering distribution function) it creates the ref tag.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="obj"></param>
		/// <param name="meshIndex"></param>
		/// <param name="filename"></param>
		public void CreateShape(XmlElement parent, RhinoObject obj, int meshIndex, string filename)
		{
			var shapeElement = _document.CreateElement("shape");

			if (obj.Name.Length > 0) shapeElement.AppendChild(_document.CreateComment("Rhino object '" + obj.Name + "' "));

			shapeElement.AppendChild(MakeProperty("filename", filename));
			shapeElement.AppendChild(MakeProperty("shapeIndex", meshIndex));
			shapeElement.SetAttribute("type", "serialized");
			parent.AppendChild(shapeElement);

			//BSDF references
			if (_mitsubaObjects.ContainsKey(obj.Id)) {
				//If the object has a Mitsuba object we have to reference it !
				string materialRef;

				if (_mitsubaObjects.TryGetValue(obj.Id, out materialRef)) shapeElement.AppendChild(MakeReference(materialRef));
			}
		}

		/// <summary>
		///   Create a XML property node
		/// </summary>
		/// <param name="name">Name of the property</param>
		/// <param name="value">
		///   Value of the property. An appropriate property tag
		///   will be chosen based on the value's type.
		/// </param>
		/// <returns>The created XML element</returns>
		public static XmlElement MakeProperty(string name, object value)
		{
			var type = value.GetType();
			string elementType;

			if (type == typeof(string)) elementType = "string";
			else if (type == typeof(int)) elementType = "integer";
			else if (type == typeof(string)) elementType = "string";
			else if (type == typeof(float) ||
			         type == typeof(double)) elementType = "float";
			else if (type == typeof(Transform)) elementType = "transform";
			else if (type == typeof(Color)) elementType = "srgb";
			else throw new Exception("Unknown element type!");

			var element = _document.CreateElement(elementType);
			element.SetAttribute("name", name);

			if (type == typeof(Transform)) {
				var matrix = _document.CreateElement("matrix");
				var trafo = (Transform)value;
				var matStr = "";

				for (var i = 0; i < 4; ++i) for (var j = 0; j < 4;
					                                 ++j) matStr += trafo[i, j].ToString(CultureInfo.InvariantCulture) + ", ";

				matrix.SetAttribute("value", matStr.Substring(0, matStr.Length - 2));
				element.AppendChild(matrix);
			}
			else if (type == typeof(Color)) {
				var color = (Color)value;
				element.SetAttribute("value",
				                     ToStringHelper(color.R / 255.0f) + ", " +
				                     ToStringHelper(color.G / 255.0f) + ", " +
				                     ToStringHelper(color.B / 255.0f));
			}
			else if (type == typeof(float)) element.SetAttribute("value", ToStringHelper((float)value));
			else if (type == typeof(double)) element.SetAttribute("value", ToStringHelper((double)value));
			else if (type == typeof(int)) element.SetAttribute("value", ToStringHelper((int)value));
			else element.SetAttribute("value", value.ToString());

			return element;
		}

		/// <summary>
		///   This method writes the XML scene file.
		/// </summary>
		/// <param name="sceneFile">The XML raw text.</param>
		public void WriteData(string sceneFile)
		{
			var output = new FileStream(sceneFile, FileMode.Create);
			var sw = new StreamWriter(output);
			var xmlWriter = new XmlTextWriter(sw) {
				Formatting = Formatting.Indented, Indentation = 4
			};
			_document.WriteTo(xmlWriter);
			sw.Close();
			output.Close();
		}

		#endregion

		#region Private methods

		#region String helpers

		private static string ToStringHelper(int value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		private static string ToStringHelper(double value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		private static string ToStringHelper(float value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		#endregion

		#endregion

		#region Material creation

		/// <summary>
		///   Internal class for handle the material creation.
		/// </summary>
		internal class CreateMaterial
		{
			/// <summary>
			///   TODO summary
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <typeparam name="S"></typeparam>
			/// <param name="element"></param>
			/// <param name="name"></param>
			/// <param name="type"></param>
			private static void MakeMitsubaType<T, S>(ref XmlElement element, string name, MitsubaType<T, S> type)
			{
				if (type.HasTextureOrName) {
					var copied = FileTools.CopyTextureToScenePath(type.SecondParameter as string);

					if (copied != null) {
						type.SecondParameter = (S)Convert.ChangeType(copied, typeof(S));
						var texture = AddElement("texture", name, null, "bitmap");
						texture.AppendChild(AddElement("string", "filename",
						                               Path.GetFileName(type.SecondParameter as string)));
						element.AppendChild(texture);
					}
				}
				else {
					var color = type.GetColorHex();

					if (color != null) {
						if (color != "#000000") element.AppendChild(AddElement("srgb", name, color));
					}
					else {
						var value = (float)Convert.ToDouble(type.FirstParameter);

						if (value > 0) element.AppendChild(AddElement("float", name, value + ""));
					}
				}
			}

			/// <summary>
			/// </summary>
			/// <param name="material"></param>
			/// <returns></returns>
			public static XmlElement SmoothDiffuseMaterial(SmoothDiffuseMaterial material)
			{
				if (_document == null) _document = new XmlDocument();

				var element = _document.CreateElement("bsdf");
				element.SetAttribute("type", "diffuse");
				element.SetAttribute("id", material.GetMaterialId());
				MakeMitsubaType(ref element, "reflectance", material.Reflectance);
				return element;
			}

			/// <summary>
			/// </summary>
			/// <param name="material"></param>
			/// <returns></returns>
			public static XmlElement SmoothDielectricMaterial(SmoothDielectricMaterial material)
			{
				if (_document == null) _document = new XmlDocument();

				var element = _document.CreateElement("bsdf");
				element.SetAttribute("type", "dielectric");
				element.SetAttribute("id", material.GetMaterialId());
				element.AppendChild(AddElement("string", "intIOR", material.IntIOR.SecondParameter + ""));
				element.AppendChild(AddElement("string", "extIOR", material.ExtIOR.SecondParameter + ""));

				//TODO use floats ??
				//element.AppendChild(AddElement("float", "intIOR", material.IntIOR.FirstParameter + ""));
				//element.AppendChild(AddElement("float", "extIOR", material.ExtIOR.FirstParameter + ""));

				return element;
			}

			/// <summary>
			/// </summary>
			/// <param name="material"></param>
			/// <returns></returns>
			public static XmlElement RoughConductorMaterial(RoughConductorMaterial material)
			{
				if (_document == null) _document = new XmlDocument();

				var element = _document.CreateElement("bsdf");
				element.SetAttribute("type", "roughconductor");
				element.SetAttribute("id", material.GetMaterialId());

				if (string.IsNullOrEmpty(material.Distribution))
					material.Distribution = "beckmann"; //Default

				element.AppendChild(AddElement("string", "distribution", material.Distribution));
				MakeMitsubaType(ref element, "alpha", material.Alpha);
				MakeMitsubaType(ref element, "alphaU", material.AlphaU);
				MakeMitsubaType(ref element, "alphaV", material.AlphaV);

				if (string.IsNullOrEmpty(material.Material))
					material.Material = "Cu"; //Default

				element.AppendChild(AddElement("string", "material", material.Material));

				var color = MitsubaMaterial.GetColorHex(material.Eta);

				if (color != "#000000") element.AppendChild(AddElement("srgb", "eta", color));

				color = MitsubaMaterial.GetColorHex(material.K);

				if (color != "#000000") element.AppendChild(AddElement("srgb", "k", color));

				MakeMitsubaType(ref element, "extEta", material.ExtEta);

				return element;
			}

			/// <summary>
			/// </summary>
			/// <param name="material"></param>
			/// <returns></returns>
			public static XmlElement RoughDiffuseMaterial(RoughDiffuseMaterial material)
			{
				if (_document == null) _document = new XmlDocument();

				var element = _document.CreateElement("bsdf");
				element.SetAttribute("type", "roughdiffuse");
				element.SetAttribute("id", material.GetMaterialId());
				MakeMitsubaType(ref element, "reflectance", material.Reflectance);
				MakeMitsubaType(ref element, "alpha", material.Alpha);
				element.AppendChild(AddElement("boolean", "useFastApprox", material.UseFastApprox.ToString().ToLower()));
				return element;
			}

			/// <summary>
			/// </summary>
			/// <param name="material"></param>
			/// <returns></returns>
			public static XmlElement RoughDielectricMaterial(RoughDielectricMaterial material)
			{
				if (_document == null) _document = new XmlDocument();

				var element = _document.CreateElement("bsdf");
				element.SetAttribute("type", "roughdielectric");
				element.SetAttribute("id", material.GetMaterialId());

				if (string.IsNullOrEmpty(material.Distribution))
					material.Distribution = "beckmann"; //Default

				element.AppendChild(AddElement("string", "distribution", material.Distribution));
				MakeMitsubaType(ref element, "alpha", material.Alpha);
				MakeMitsubaType(ref element, "alphaU", material.AlphaU);
				MakeMitsubaType(ref element, "alphaV", material.AlphaV);
				element.AppendChild(AddElement("string", "intIOR", material.IntIOR.SecondParameter + ""));
				element.AppendChild(AddElement("string", "extIOR", material.ExtIOR.SecondParameter + ""));
				//element.AppendChild(AddElement("float", "intIOR", material.IntIOR.FirstParameter + ""));
				//element.AppendChild(AddElement("float", "extIOR", material.ExtIOR.FirstParameter + ""));

				return element;
			}

			/// <summary>
			/// </summary>
			/// <param name="material"></param>
			/// <returns></returns>
			public static XmlElement SmoothPlasticMaterial(SmoothPlasticMaterial material)
			{
				if (_document == null) _document = new XmlDocument();

				var element = _document.CreateElement("bsdf");
				element.SetAttribute("type", "plastic");
				element.SetAttribute("id", material.GetMaterialId());
				element.AppendChild(AddElement("string", "intIOR", material.IntIOR.SecondParameter + ""));
				element.AppendChild(AddElement("string", "extIOR", material.ExtIOR.SecondParameter + ""));
				//element.AppendChild(AddElement("float", "intIOR", material.IntIOR.FirstParameter + ""));
				//element.AppendChild(AddElement("float", "extIOR", material.ExtIOR.FirstParameter + ""));
				MakeMitsubaType(ref element, "diffuseReflectance", material.DiffuseReflectance);
				element.AppendChild(AddElement("boolean", "nonlinear", material.Nonlinear.ToString().ToLower()));

				return element;
			}

			/// <summary>
			/// </summary>
			/// <param name="material"></param>
			/// <returns></returns>
			public static XmlElement RoughPlasticMaterial(RoughPlasticMaterial material)
			{
				if (_document == null) _document = new XmlDocument();

				var element = _document.CreateElement("bsdf");
				element.SetAttribute("type", "roughplastic");
				element.SetAttribute("id", material.GetMaterialId());

				if (string.IsNullOrEmpty(material.Distribution))
					material.Distribution = "beckmann"; //Default

				element.AppendChild(AddElement("string", "distribution", material.Distribution));

				MakeMitsubaType(ref element, "alpha", material.Alpha);
				element.AppendChild(AddElement("string", "intIOR", material.IntIOR.SecondParameter + ""));
				element.AppendChild(AddElement("string", "extIOR", material.ExtIOR.SecondParameter + ""));
				//element.AppendChild(AddElement("float", "intIOR", material.IntIOR.FirstParameter + ""));
				//element.AppendChild(AddElement("float", "extIOR", material.ExtIOR.FirstParameter + ""));
				MakeMitsubaType(ref element, "diffuseReflectance", material.DiffuseReflectance);
				element.AppendChild(AddElement("boolean", "nonlinear", material.Nonlinear.ToString().ToLower()));

				return element;
			}

			/// <summary>
			/// </summary>
			/// <param name="material"></param>
			/// <returns></returns>
			public static XmlElement SmoothConductorMaterial(SmoothConductorMaterial material)
			{
				if (_document == null) _document = new XmlDocument();

				var element = _document.CreateElement("bsdf");
				element.SetAttribute("type", "conductor");
				element.SetAttribute("id", material.GetMaterialId());

				if (string.IsNullOrEmpty(material.Material))
					material.Material = "Cu"; //Default

				element.AppendChild(AddElement("string", "material", material.Material));

				var color = MitsubaMaterial.GetColorHex(material.Eta);

				if (color != "#000000") element.AppendChild(AddElement("srgb", "eta", color));

				color = MitsubaMaterial.GetColorHex(material.K);

				if (color != "#000000") element.AppendChild(AddElement("srgb", "k", color));

				MakeMitsubaType(ref element, "extEta", material.ExtEta);
				return element;
			}

			//public static XmlElement SmoothDielectricCoatingMaterial(SmoothDielectricCoatingMaterial material)
			//{
			//    var element = _document.CreateElement("bsdf");
			//    element.SetAttribute("type", "coating");
			//    element.SetAttribute("id", material.GetMaterialId());

			//    //TODO coating XML

			//    return element;
			//}
		}

		#endregion

		#region Emitter creation

		/// <summary>
		///   Internal class for handle the emitter creation.
		/// </summary>
		internal class CreateEmitter
		{
			/// <summary>
			/// </summary>
			/// <param name="hdr_file"></param>
			/// <returns></returns>
			internal static XmlElement EnvironmentEmitter(string hdr_file)
			{
				var copied = FileTools.CopyTextureToScenePath(hdr_file);
				var element = _document.CreateElement("emitter");
				element.SetAttribute("type", "envmap");
				element.SetAttribute("id", "envmaphdr");
				element.AppendChild(AddElement("string", "filename", copied));
				element.AppendChild(MakeProperty("toWorld", _toWorldTransform));

				return element;
			}

			/// <summary>
			/// </summary>
			/// <param name="emitter"></param>
			/// <returns></returns>
			internal static XmlElement PointLightSource(PointLightSource emitter)
			{
				var element = _document.CreateElement("emitter");
				element.SetAttribute("type", "point");
				element.SetAttribute("id", emitter.EmitterId);
				element.AppendChild(AddElement("point", "position", emitter.Position));
				element.AppendChild(AddElement("spectrum", "intensity", emitter.Intensity + "")); //TODO intensity
				return element;
			}

			/// <summary>
			/// </summary>
			/// <param name="emitter"></param>
			/// <returns></returns>
			internal static XmlElement SpotLightSource(SpotLightSource emitter)
			{
				var element = _document.CreateElement("emitter");
				element.SetAttribute("type", "spot");
				element.SetAttribute("id", emitter.EmitterId);
				element.AppendChild(AddElement(emitter.ToWorld)); //MitsubaTransform
				element.AppendChild(AddElement("spectrum", "intensity", emitter.Intensity + ""));
				element.AppendChild(AddElement("float", "cutoffAngle", emitter.CutOffAngle + ""));
				return element;
			}
		}

		internal static class CreateIntegrator
		{
			public static XmlElement Create(IntegratorAmbientOclusion integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "ao");
				element.AppendChild(AddElement("integer", "shadingSamples", Convert.ToString(integrator.ShadingSamples)));
				element.AppendChild(AddElement("float", "rayLength", Convert.ToString(integrator.OcclusionRayLength)));
				return element;
			}

			public static XmlElement Create(IntegratorDirectIlumination integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "direct");
				element.AppendChild(AddElement("integer", "emitterSamples", Convert.ToString(integrator.EmitterSmaples)));
				element.AppendChild(AddElement("integer", "bsdfSamples", Convert.ToString(integrator.BSDFSamples)));
				element.AppendChild(AddElement("boolean", "strictNormals",
				                               Convert.ToString(integrator.StrictSurfaceNormals).ToLower()));
				element.AppendChild(AddElement("boolean", "hideEmitters",
				                               Convert.ToString(integrator.HideDirectlyVisibleEmitters).ToLower()));
				return element;
			}
			public static XmlElement Create(IntegratorPathTracer integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "path");
				element.AppendChild(AddElement("integer", "maxDepth", Convert.ToString(integrator.MaximumDepth)));
				element.AppendChild(AddElement("integer", "rrDepth", Convert.ToString(integrator.RussianRouletteStartingDepth)));
				element.AppendChild(AddElement("boolean", "strictNormals",
				                               Convert.ToString(integrator.StrictSurfaceNormals).ToLower()));
				element.AppendChild(AddElement("boolean", "hideEmitters",
				                               Convert.ToString(integrator.HideDirectlyVisibleEmitters).ToLower()));
				return element;
			}
			public static XmlElement Create(IntegratorVolumetricPathTracerSimple integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "volpath_simple");
				element.AppendChild(AddElement("integer", "maxDepth", Convert.ToString(integrator.MaximumDepth)));
				element.AppendChild(AddElement("integer", "rrDepth", Convert.ToString(integrator.RussianRouletteStartingDepth)));
				element.AppendChild(AddElement("boolean", "strictNormals",
				                               Convert.ToString(integrator.StrictSurfaceNormals).ToLower()));
				element.AppendChild(AddElement("boolean", "hideEmitters",
				                               Convert.ToString(integrator.HideDirectlyVisibleEmitters).ToLower()));
				return element;
			}
			public static XmlElement Create(IntegratorVolumetricPathTracerExtended integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "volpath");
				element.AppendChild(AddElement("integer", "maxDepth", Convert.ToString(integrator.MaximumDepth)));
				element.AppendChild(AddElement("integer", "rrDepth", Convert.ToString(integrator.RussianRouletteStartingDepth)));
				element.AppendChild(AddElement("boolean", "strictNormals",
				                               Convert.ToString(integrator.StrictSurfaceNormals).ToLower()));
				element.AppendChild(AddElement("boolean", "hideEmitters",
				                               Convert.ToString(integrator.HideDirectlyVisibleEmitters).ToLower()));
				return element;
			}
			public static XmlElement Create(IntegratorAdjointParticleTracer integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "ptracer");
				element.AppendChild(AddElement("integer", "maxDepth", Convert.ToString(integrator.MaximumDepth)));
				element.AppendChild(AddElement("integer", "rrDepth", Convert.ToString(integrator.RussianRouletteStartingDepth)));
				element.AppendChild(AddElement("integer", "granularity", Convert.ToString(integrator.WorkUnitGranularity)));
				element.AppendChild(AddElement("boolean", "bruteForce", Convert.ToString(integrator.BruteForce).ToLower()));
				return element;
			}
			public static XmlElement Create(IntegratorVirtualPointLightRenderer integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "ptracer");
				element.AppendChild(AddElement("integer", "maxDepth", Convert.ToString(integrator.MaximumDepth)));
				element.AppendChild(AddElement("integer", "shadowMapResolution", Convert.ToString(integrator.ShadowMapResolution)));
				element.AppendChild(AddElement("float", "clamping", Convert.ToString(integrator.ClampingFactor).ToLower()));
				return element;
			}
			public static XmlElement Create(IntegratorPhotonMapper integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "photonmapper");
				element.AppendChild(AddElement("integer", "directSamples", Convert.ToString(integrator.DirectSamples)));
				element.AppendChild(AddElement("integer", "glossySamples", Convert.ToString(integrator.GlossySamples)));
				element.AppendChild(AddElement("integer", "maxDepth", Convert.ToString(integrator.MaximumDepth)));
				element.AppendChild(AddElement("integer", "globalPhotons", Convert.ToString(integrator.GlobalPhotons)));
				element.AppendChild(AddElement("integer", "causticPhotons", Convert.ToString(integrator.CausticPhotons)));
				element.AppendChild(AddElement("integer", "volumePhotons", Convert.ToString(integrator.VolumePhotons)));
				element.AppendChild(AddElement("float", "globalLookupRadius", Convert.ToString(integrator.LookupRadiusGlobal)));
				element.AppendChild(AddElement("float", "causticLLookupRadius", Convert.ToString(integrator.LookupRadiusCaustic)));
				element.AppendChild(AddElement("integer", "lookupSize", Convert.ToString(integrator.CausticPhotonMapLookupSize)));
				element.AppendChild(AddElement("integer", "granularity", Convert.ToString(integrator.WorkUnitGranularity)));
				element.AppendChild(AddElement("boolean", "hideEmitters",
				                               Convert.ToString(integrator.HideDirectlyVisibleEmitters).ToLower()));
				element.AppendChild(AddElement("integer", "rrDepth", Convert.ToString(integrator.RussianRouletteSartingDepth)));

				return element;
			}
			public static XmlElement Create(IntegratorProgressivePhotonMapper integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "photonmapper");
				element.AppendChild(AddElement("integer", "photonCount", Convert.ToString(integrator.PhotonsPerIteration)));
				element.AppendChild(AddElement("float", "initialRadius", Convert.ToString(integrator.InitialRadius)));
				element.AppendChild(AddElement("float", "alpha", Convert.ToString(integrator.SizeReductionParameter)));
				element.AppendChild(AddElement("integer", "granularity", Convert.ToString(integrator.WorkUnitGranularity)));
				element.AppendChild(AddElement("integer", "rrDepth", Convert.ToString(integrator.RussianRouletteSartingDepth)));

				//Parameter not present in Mistuba UI Configuration
				//element.AppendChild(AddElement("integer", "maxPasses", Convert.ToString()));

				return element;

			}
			public static XmlElement Create(IntegratorStochasticProgressivePhotonMapper integrator)
			{

				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "sppm");
				element.AppendChild(AddElement("integer", "maxDepth", Convert.ToString(integrator.MaximumDepth)));
				element.AppendChild(AddElement("integer", "photonCount", Convert.ToString(integrator.PhotonsPerIteration)));
				element.AppendChild(AddElement("float", "initialRadius", Convert.ToString(integrator.InitialRadius)));
				element.AppendChild(AddElement("float", "alpha", Convert.ToString(integrator.SizeReductionParameter)));
				element.AppendChild(AddElement("integer", "granularity", Convert.ToString(integrator.WorkUnitGranularity)));
				element.AppendChild(AddElement("integer", "rrDepth", Convert.ToString(integrator.RussianRouletteSartingDepth)));

				//Parameter not present in Mistuba UI Configuration
				//element.AppendChild(AddElement("integer", "maxPasses", Convert.ToString()));

				return element;
			}
			public static XmlElement Create(IntegratorBidirectionalPathTracer integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "bdpt");
				element.AppendChild(AddElement("integer", "maxDepth", Convert.ToString(integrator.MaximumDepth)));
				element.AppendChild(AddElement("integer", "rrDepth", Convert.ToString(integrator.RussianRouletteStartingDepth)));
				element.AppendChild(AddElement("boolean", "lightImage", Convert.ToString(integrator.CreateLightImage).ToLower()));
				element.AppendChild(AddElement("boolean", "sampleDirect",
				                               Convert.ToString(integrator.UseDirectSamplingMethods).ToLower()));
				return element;
			}
			public static XmlElement Create(IntegratorPrimarySampleSpaceMLT integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "pssmlt");
				element.AppendChild(AddElement("boolean", "bidirectional", Convert.ToString(integrator.Bidirectional).ToLower()));
				element.AppendChild(AddElement("integer", "maxDepth", Convert.ToString(integrator.MaximumDepth)));
				element.AppendChild(AddElement("integer", "directSamples", Convert.ToString(integrator.DirectSamples)));
				element.AppendChild(AddElement("integer", "rrDepth", Convert.ToString(integrator.RussianRouletteSartingDepth)));
				element.AppendChild(AddElement("integer", "luminanceSamples", Convert.ToString(integrator.LuminanceSamples)));
				element.AppendChild(AddElement("boolean", "twoStage", Convert.ToString(integrator.TwoStageMLT).ToLower()));
				element.AppendChild(AddElement("float", "pLarge", Convert.ToString(integrator.LargeStepProbability)));

				return element;
			}
			public static XmlElement Create(IntegratorSampleSpaceMLT integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "mlt");
				element.AppendChild(AddElement("integer", "maxDepth", Convert.ToString(integrator.MaximumDepth)));
				element.AppendChild(AddElement("integer", "directSamples", Convert.ToString(integrator.DirectSamples)));
				element.AppendChild(AddElement("integer", "luminanceSamples", Convert.ToString(integrator.LuminanceSamples)));
				element.AppendChild(AddElement("boolean", "twoStage", Convert.ToString(integrator.TwoStageMLT).ToLower()));
				element.AppendChild(AddElement("boolean", "bidirectionalMutation",
				                               Convert.ToString(integrator.BidirectionalMutation).ToLower()));
				element.AppendChild(AddElement("boolean", "lensPerturbation", Convert.ToString(integrator.LensPerturbation).ToLower()));
				element.AppendChild(AddElement("boolean", "multiChainPerturbation",
				                               Convert.ToString(integrator.MultiChainPerturbation).ToLower()));
				element.AppendChild(AddElement("boolean", "causticPerturbation",
				                               Convert.ToString(integrator.CausticPerturbation).ToLower()));
				element.AppendChild(AddElement("boolean", "manifoldPerturbation",
				                               Convert.ToString(integrator.ManifoldPerturbation).ToLower()));
				element.AppendChild(AddElement("float", "lambda", Convert.ToString(integrator.ProbabilityFactor)));


				return element;
			}
			public static XmlElement Create(IntegratorEnergyRedisributionPathTracing integrator)
			{

				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "erpt");
				element.AppendChild(AddElement("integer", "maxDepth", Convert.ToString(integrator.MaximumDepth)));
				element.AppendChild(AddElement("float", "numChains", Convert.ToString(integrator.AverageNumberOfChains)));
				element.AppendChild(AddElement("float", "maxChains", Convert.ToString(integrator.MaxNumberOfChains)));
				element.AppendChild(AddElement("integer", "chainLength", Convert.ToString(integrator.MutationsPerChain)));
				element.AppendChild(AddElement("integer", "directSamples", Convert.ToString(integrator.DirectSamples)));

				//this field appears on Mitsuba UI Config, but there is no key value pair in documentation
				//element.AppendChild(AddElement("integer", "luminanceSamples", Convert.ToString(integrator.LuminanceSamples)));

				element.AppendChild(AddElement("boolean", "bidirectionalMutation",
				                               Convert.ToString(integrator.BidirectionalMutation).ToLower()));
				element.AppendChild(AddElement("boolean", "lensPerturbation", Convert.ToString(integrator.LensPerturbation).ToLower()));
				element.AppendChild(AddElement("boolean", "multiChainPerturbation",
				                               Convert.ToString(integrator.MultiChainPerturbation).ToLower()));
				element.AppendChild(AddElement("boolean", "causticPerturbation",
				                               Convert.ToString(integrator.CausticPerturbation).ToLower()));
				element.AppendChild(AddElement("boolean", "manifoldPerturbation",
				                               Convert.ToString(integrator.ManifoldPerturbation).ToLower()));
				element.AppendChild(AddElement("float", "lambda", Convert.ToString(integrator.ProbabilityFactor)));

				return element;
			}
			//This Integrator doesn't exist in mitsuba Config UI, but it's documented
			public static XmlElement Create(IntegratorAdaptativeIntegrator integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "volpath");
				element.AppendChild(AddElement("float", "maxError", Convert.ToString(integrator.MaximumError)));
				element.AppendChild(AddElement("float", "pValue", Convert.ToString(integrator.PValue)));
				element.AppendChild(AddElement("integer", "maxSampleFactor", Convert.ToString(integrator.MaximumSampleFactor)));
				return element;
			}
			//This Integrator doesn't exist in mitsuba Config UI, but it's documented
			public static XmlElement Create(IntegratorIrradianceCaching integrator)
			{
				var element = _document.CreateElement("integrator");
				element.SetAttribute("type", "irrcache");
				element.AppendChild(AddElement("integer", "resolution", Convert.ToString(integrator.Resolution)));
				element.AppendChild(AddElement("float", "quality", Convert.ToString(integrator.Quality)));
				element.AppendChild(AddElement("boolean", "gradients", Convert.ToString(integrator.Gradients)));
				element.AppendChild(AddElement("boolean", "clampNeighbor", Convert.ToString(integrator.ClampNeighbor).ToLower()));
				element.AppendChild(AddElement("boolean", "clampScreen", Convert.ToString(integrator.ClampScreen).ToLower()));
				element.AppendChild(AddElement("boolean", "overture", Convert.ToString(integrator.Overture).ToLower()));

				element.AppendChild(AddElement("float", "qualityAdjustment", Convert.ToString(integrator.QualityAdjustment)));
				element.AppendChild(AddElement("boolean", "indirectOnly", Convert.ToString(integrator.IndirectOnly).ToLower()));
				element.AppendChild(AddElement("boolean", "debug", Convert.ToString(integrator.Debug).ToLower()));



				return element;
			}

		}

		internal static class CreateSampler
		{
			internal static XmlElement Create(SamplerIndependent sampler)
			{
				var element = _document.CreateElement("sampler");
				element.SetAttribute("type", "independent");
				element.AppendChild(AddElement("integer", "sampleCount", Convert.ToString(sampler.SamplesPerPixel)));
				return element;
			}
			internal static XmlElement Create(SamplerStraitfield sampler)
			{
				var element = _document.CreateElement("sampler");
				element.SetAttribute("type", "stratified");
				element.AppendChild(AddElement("integer", "sampleCount", Convert.ToString(sampler.SamplesPerPixel)));
				element.AppendChild(AddElement("integer", "dimension", Convert.ToString(sampler.EffectiveDimension)));

				return element;
			}
			internal static XmlElement Create(SamplerLowDiscrepancy sampler)
			{
				var element = _document.CreateElement("sampler");
				element.SetAttribute("type", "ldsampler");
				element.AppendChild(AddElement("integer", "sampleCount", Convert.ToString(sampler.SamplesPerPixel)));
				element.AppendChild(AddElement("integer", "dimension", Convert.ToString(sampler.EffectiveDimension)));

				return element;
			}
			internal static XmlElement Create(SamplerHammersleyQMC sampler)
			{
				var element = _document.CreateElement("sampler");
				element.SetAttribute("type", "(hammersley");
				element.AppendChild(AddElement("integer", "sampleCount", Convert.ToString(sampler.SamplesPerPixel)));
				element.AppendChild(AddElement("integer", "scramble", Convert.ToString(sampler.ScrambleValue)));

				return element;
			}
			internal static XmlElement Create(SamplerHaltonQMC sampler)
			{
				var element = _document.CreateElement("sampler");
				element.SetAttribute("type", "(halton");
				element.AppendChild(AddElement("integer", "sampleCount", Convert.ToString(sampler.SamplesPerPixel)));
				element.AppendChild(AddElement("integer", "scramble", Convert.ToString(sampler.ScrambleValue)));

				return element;
			}
			internal static XmlElement Create(SamplerSobolQMC sampler)
			{
				var element = _document.CreateElement("sampler");
				element.SetAttribute("type", "sobol");
				element.AppendChild(AddElement("integer", "sampleCount", Convert.ToString(sampler.SamplesPerPixel)));
				element.AppendChild(AddElement("integer", "scramble", Convert.ToString(sampler.ScrambleValue)));
				return element;
			}
		}
		#endregion

		//TODO implement integrator
		//private XmlElement DefineIntegrator(bool softRays, double darkMatter)
		//{
		//    var toRet = _document.CreateElement("integrator");
		//    toRet.SetAttribute("type", "amazing");
		//    var boolean = _document.CreateElement("boolean");
		//    boolean.SetAttribute("name", "softerRays");
		//    boolean.SetAttribute("value", "true");
		//    var flo = _document.CreateElement("float");
		//    flo.SetAttribute("name", "darkMatter");
		//    flo.SetAttribute("value", "0.4");
		//    toRet.AppendChild(boolean);
		//    toRet.AppendChild(flo);
		//    return toRet;
		//}
	}
}