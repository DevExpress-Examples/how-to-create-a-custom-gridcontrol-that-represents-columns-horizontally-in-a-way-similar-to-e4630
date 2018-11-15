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


Imports Microsoft.VisualBasic
Imports DevExpress.Data
Imports DevExpress.Xpf.Editors.Settings
Imports DevExpress.Xpf.Grid
Imports System
Imports System.Collections
Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Linq
Imports System.Reflection
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data

Namespace dxExample.VGrid
	Partial Public Class VerticalGridControl
		Inherits GridControl
		Private backItemsSourceEvents As INotifyCollectionChanged

		Private Property InternalItemsSource() As Object
			Get
				Return MyBase.ItemsSource
			End Get
			Set(ByVal value As Object)
				MyBase.ItemsSource = value
			End Set
		End Property
		Private ReadOnly Property InternalColumns() As GridColumnCollection
			Get
				Return MyBase.Columns
			End Get
		End Property

		Private privateRows As VerticalRowCollection
		Public Property Rows() As VerticalRowCollection
			Get
				Return privateRows
			End Get
			Set(ByVal value As VerticalRowCollection)
				privateRows = value
			End Set
		End Property
		Public Property AutoPopulateRows() As Boolean
			Get
				Return CBool(GetValue(AutoPopulateRowsProperty))
			End Get
			Set(ByVal value As Boolean)
				SetValue(AutoPopulateRowsProperty, value)
			End Set
		End Property
		Public Shadows Property ItemsSource() As Object
			Get
				Return CObj(GetValue(ItemsSourceProperty))
			End Get
			Set(ByVal value As Object)
				SetValue(ItemsSourceProperty, value)
			End Set
		End Property

		Public Sub New()
			InitializeComponent()
			InitializeRowsCollection()
			SubscribeItemsSourcePropertyChanged()
		End Sub

		Private Sub InitializeRowsCollection()
			Rows = New VerticalRowCollection()
			AddHandler Rows.CollectionChanged, AddressOf OnRowsCollectionChanged
		End Sub
		Private Sub UpdateRowsCollection()
			If AutoPopulateRows Then
				PopulateRows()
			End If
		End Sub
		Private Sub SubscribeItemsSourcePropertyChanged()
			Dim itemsSourceDropertyDescriptor As DependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(VerticalGridControl.ItemsSourceProperty, GetType(VerticalGridControl))
			itemsSourceDropertyDescriptor.AddValueChanged(Me, New EventHandler(AddressOf OnItemsSourcePropertyChanged))
		End Sub
		Private Sub UpdateInternalColumns()
			Dim itemsSource As ICollection = (TryCast(Me.ItemsSource, ICollection))
			If itemsSource Is Nothing Then
				Columns.Clear()
				Return
			End If
			Columns.BeginUpdate()
			Dim columnsCount As Integer = itemsSource.Count
			If InternalColumns.Count = columnsCount Then
				Return
			End If
			Dim delta As Integer = columnsCount - InternalColumns.Count
			If columnsCount > InternalColumns.Count Then
				For i As Integer = InternalColumns.Count To columnsCount - 1
					InternalColumns.Add(New GridColumn() With {.FieldName = i.ToString(), .UnboundType = UnboundColumnType.Object})
				Next i
			Else
				For i As Integer = InternalColumns.Count - 1 To columnsCount Step -1
					InternalColumns.RemoveAt(i)
				Next i
			End If
			Columns.EndUpdate()
		End Sub
		Private Sub UpdateItemsSourceEventsSubscription()
			If backItemsSourceEvents IsNot Nothing Then
				RemoveHandler backItemsSourceEvents.CollectionChanged, AddressOf OnItemsSourceCollectionChanged
			End If
			If Not(TypeOf ItemsSource Is INotifyCollectionChanged) Then
				Return
			End If
			Dim itemsSourceEvents As INotifyCollectionChanged = (TryCast(ItemsSource, INotifyCollectionChanged))
			AddHandler itemsSourceEvents.CollectionChanged, AddressOf OnItemsSourceCollectionChanged
			backItemsSourceEvents = itemsSourceEvents
		End Sub
		Private Sub PopulateRows()
			Dim itemsSource As IEnumerable = (TryCast(Me.ItemsSource, IEnumerable))
			If itemsSource Is Nothing Then
				Return
			End If
			Dim itemsSourceEnumerator As IEnumerator = itemsSource.GetEnumerator()
			itemsSourceEnumerator.MoveNext()
			Dim item As Object = itemsSourceEnumerator.Current
			If item Is Nothing Then
				Return
			End If
			Dim itemProps() As PropertyInfo = item.GetType().GetProperties()
			For i As Integer = 0 To itemProps.Length - 1
				Rows.Add(VerticalRowData.FromPropertyInfo(itemProps(i)))
			Next i
		End Sub
		Private Sub OnRowsCollectionChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
			InternalItemsSource = Rows
		End Sub
		Private Sub OnItemsSourcePropertyChanged(ByVal sender As Object, ByVal e As EventArgs)
			UpdateInternalColumns()
			UpdateRowsCollection()
			UpdateItemsSourceEventsSubscription()
		End Sub
		Private Sub OnItemsSourceCollectionChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
			If e.Action = NotifyCollectionChangedAction.Add OrElse e.Action = NotifyCollectionChangedAction.Remove Then
				UpdateInternalColumns()
			End If
		End Sub
		Private Sub OnProcessUnboundColumnData(ByVal sender As Object, ByVal e As GridColumnDataEventArgs)
			Dim itemsSource As IList = (TryCast(Me.ItemsSource, IList))
			If itemsSource Is Nothing Then
				Return
			End If
			Dim row As VerticalRowData = Rows(e.ListSourceRowIndex)
			Dim item As Object = itemsSource(Convert.ToInt32(e.Column.FieldName))
			Dim itemProperty As PropertyInfo = item.GetType().GetProperty(row.RowName)
			If itemProperty Is Nothing Then
				Return
			End If
			If e.IsGetData Then
				e.Value = itemProperty.GetValue(item)
			End If
			If e.IsSetData Then
				itemProperty.SetValue(item, e.Value)
			End If
		End Sub

		Public Shared ReadOnly AutoPopulateRowsProperty As DependencyProperty = DependencyProperty.Register("AutoPopulateRows", GetType(Boolean), GetType(VerticalGridControl), New PropertyMetadata(False))
		Public Shared Shadows ReadOnly ItemsSourceProperty As DependencyProperty = DependencyProperty.Register("ItemsSource", GetType(Object), GetType(VerticalGridControl), New PropertyMetadata(Nothing))
	End Class
	Public Class BottomIndicatorRowVisibilityConverter
		Implements IMultiValueConverter
		Public Function Convert(ByVal values() As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
			If values.Count() < 2 Then
				Return Visibility.Collapsed
			End If
			If Not((TypeOf values(0) Is Integer) AndAlso (TypeOf values(1) Is Integer)) Then
				Return Visibility.Collapsed
			End If
			Return If((CInt(Fix(values(0)))) > (CInt(Fix(values(1)))), Visibility.Visible, Visibility.Collapsed)
		End Function
		Public Function ConvertBack(ByVal value As Object, ByVal targetTypes() As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
			Throw New NotImplementedException()
		End Function
	End Class
	Public Class DefaultCellTemplateSelector
		Inherits DataTemplateSelector
		Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) As DataTemplate
			Dim row As VerticalRowData = (TryCast((TryCast(item, EditGridCellData)).RowData.Row, VerticalRowData))
			If row.CellTemplate Is Nothing Then
				Return MyBase.SelectTemplate(item, container)
			End If
			Return row.CellTemplate
		End Function
	End Class
	Public Class VerticalRowData
		Inherits DependencyObject
		Private privateRowName As String
		Public Property RowName() As String
			Get
				Return privateRowName
			End Get
			Set(ByVal value As String)
				privateRowName = value
			End Set
		End Property
		Public Property CellTemplate() As DataTemplate
			Get
				Return CType(GetValue(CellTemplateProperty), DataTemplate)
			End Get
			Set(ByVal value As DataTemplate)
				SetValue(CellTemplateProperty, value)
			End Set
		End Property

		Public Shared ReadOnly CellTemplateProperty As DependencyProperty = DependencyProperty.Register("CellTemplate", GetType(DataTemplate), GetType(VerticalRowData), New PropertyMetadata(Nothing))
		Public Shared Function FromPropertyInfo(ByVal info As PropertyInfo) As VerticalRowData
			Return New VerticalRowData() With {.RowName = info.Name}
		End Function
	End Class
	Public Class VerticalRowCollection
		Inherits ObservableCollection(Of VerticalRowData)
		Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As VerticalRowData)
			Dim existsIndex As Integer = IndexOf(item.RowName)
			If existsIndex > -1 Then
				If Items(existsIndex).CellTemplate IsNot Nothing Then
					Return
				End If
				Items(existsIndex).CellTemplate = item.CellTemplate
				Return
			End If
			MyBase.InsertItem(index, item)
		End Sub

		Private Function IndexOf(ByVal rowName As String) As Integer
			For i As Integer = 0 To Items.Count - 1
				If Items(i).RowName = rowName Then
					Return i
				End If
			Next i
			Return -1
		End Function
	End Class
End Namespace