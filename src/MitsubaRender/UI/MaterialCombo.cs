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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Rhino.Render;
using Rhino.Render.UI;

namespace MitsubaRender.UI
{
	/// <summary>
	///
	/// </summary>
	[Guid("7230AC25-1955-4fd7-8A45-0BB795244DF7")]
	public partial class MaterialCombo : UserControl, IUserInterfaceSection
	{
		/// <summary>
		/// Basic event.
		/// </summary>
		public event EventHandler OnChange;

		/// <summary>
		/// The data of the comboBox.
		/// </summary>
		private string[] _data;

		/// <summary>
		/// The current selected item.
		/// </summary>
		private string _selectedItem;

		/// <summary>
		///
		/// </summary>
		public string[] Data
		{
			get {
				return _data;
			}
			set {
				_data = value;
				comboBox.DataSource = _data;
			}
		}

		/// <summary>
		/// The current selected item of the custom comboBox for Rhino.
		/// </summary>
		public string SelectedItem
		{
			get {
				return _selectedItem;
			}

			set {
				_selectedItem = value;

				if (!string.IsNullOrEmpty(_selectedItem)) {
					for (int i = 0; i < _data.Length; i++) {
						if (_data[i] == _selectedItem) {
							comboBox.SelectedIndex = i;
							break;
						}
					}
				}
			}
		}

		public MaterialCombo()
		{
			InitializeComponent();
			comboBox.Select(0, 0);
			SelectedItem = comboBox.SelectedText;
		}

		public void UserInterfaceDisplayData(UserInterfaceSection userInterfaceSection,
		                                     RenderContent[] renderContentList)
		{
			//throw new NotImplementedException();
		}

		public void OnUserInterfaceSectionExpanding(UserInterfaceSection userInterfaceSection, bool expanding)
		{
			//throw new NotImplementedException();
		}

		private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			SelectedItem = comboBox.SelectedItem.ToString();

			if (OnChange != null) OnChange(this, null);
		}
	}
}