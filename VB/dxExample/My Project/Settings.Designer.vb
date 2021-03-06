﻿' Developer Express Code Central Example:
' How to create a custom GridControl that represents columns horizontally in a way similar to the WinForms VerticalGrid control
' 
' This example demonstrates how to create a GridControl descendant with
' horizontally oriented columns.
' To use it, simply add the
' VerticalGridControl.xaml and VerticalGridControl.xaml.cs files in your
' project.
' 
' In addition, this example demonstrates how to customize grid cells
' using data templates.
' 
' You can find sample updates and versions for different programming languages here:
' http://www.devexpress.com/example=E4630

'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.18033
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Namespace My


	<Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(), Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")>
	Friend NotInheritable Partial Class Settings
		Inherits System.Configuration.ApplicationSettingsBase

		Private Shared defaultInstance As Settings = (CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New Settings()), Settings))

		Public Shared ReadOnly Property [Default]() As Settings
			Get
				Return defaultInstance
			End Get
		End Property
	End Class
End Namespace
