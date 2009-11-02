//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Diagnostics;       // Debug
using System.Globalization;     // CultureInfo
using System.Windows;
using System.Windows.Controls;  // Control
using System.Windows.Controls.Primitives; //ButtonBase
using System.Windows.Data;      // IValueConverter
using System.Windows.Input;
using System.Windows.Media;     
using System.Windows.Threading; // DispatcherPriority

using AnjLab.FX.Wpf.Controls;


namespace AnjLab.FX.Wpf.Controls
{
    /// <summary>
    /// The DatePicker control allows the user to enter or select a date and display it in 
    /// the specified format. User can limit the date that can be selected by setting the 
    /// selection range.  You might consider using a DatePicker control instead of a MonthCalendar 
    /// if you need custom date formatting and limit the selection to just one date.
    /// </summary>
    [TemplatePart(Name = "PART_EditableTextBox", Type = typeof(MaskedTextBox))]
    [TemplatePart(Name = "PART_DatePickerCalendar", Type = typeof(MonthCalendar))]
    public class DatePicker : Control
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Static Constructor
        /// </summary>
        static DatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatePicker), new FrameworkPropertyMetadata(typeof(DatePicker)));

            EventManager.RegisterClassHandler(typeof(DatePicker), Keyboard.KeyDownEvent, new KeyEventHandler(KeyDownHandler), true);
            EventManager.RegisterClassHandler(typeof(DatePicker), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true);

            // Listen for ContextMenu openings/closings
            EventManager.RegisterClassHandler(typeof(DatePicker), ContextMenuService.ContextMenuOpeningEvent, new ContextMenuEventHandler(OnContextMenuOpen), true);
            EventManager.RegisterClassHandler(typeof(DatePicker), ContextMenuService.ContextMenuClosingEvent, new ContextMenuEventHandler(OnContextMenuClose), true);
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DatePicker() : base()
        {
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Methods
        //
        //-------------------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// Called when the Template's tree has been generated
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AttachToVisualTree();
        }

        /// <summary>
        /// Returns a string representation for this control.
        /// "...DatePicker, Value:06/02/2006"
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();

            if (Value.HasValue)
            {
                s += ", Value:" + Value.Value.ToShortDateString();
            }

            return s;
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //-------------------------------------------------------------------

        #region Public Properties

        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register(
                "Mask",
                typeof (string),
                typeof (DatePicker),
                new PropertyMetadata("00/00/0000"));

        public string Mask
        {
            get { return (string) GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        #region IsDropDownOpen

        /// <summary>
        /// The DependencyProperty for the IsDropDownOpen property
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(
                "IsDropDownOpen",
                typeof(bool),
                typeof(DatePicker),
                new FrameworkPropertyMetadata(
                    BooleanBoxes.FalseBox,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnIsDropDownOpenChanged),
                    new CoerceValueCallback(CoerceIsDropDownOpen)));

        /// <summary>
        /// Whether or not the "popup" for this control is currently open
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// Coerce IsDropDownOpen with IsLoaded, so set IsDropDownOpen to true before UI ready can work
        /// </summary>
        private static object CoerceIsDropDownOpen(DependencyObject d, object value)
        {
            if ((bool)value)
            {
                DatePicker dp = (DatePicker)d;
                if (!dp.IsLoaded)
                {
                    //Defer setting IsDropDownOpen to true after Loaded event is fired to show popup window correctly
                    dp.Loaded += new RoutedEventHandler(dp.OpenOnLoad);
                    return BooleanBoxes.FalseBox;
                }
            }

            return value;
        }

        private void OpenOnLoad(object sender, RoutedEventArgs e)
        {
            CoerceValue(IsDropDownOpenProperty);
            Loaded -= new RoutedEventHandler(OpenOnLoad);
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)d;

            if ((bool)e.NewValue)
            {
                //Remember the previous Value for cancel action (Key.Escape)
                datepicker._previousValue = datepicker.Value;

                //In edit mode, if the text has been changed before opening the drop-down content
                //parse the text and get the correct Value before the popup window is showed
                if (datepicker.GetFlag(Flags.IsTextChanged) && datepicker.CanEdit && datepicker.EditableTextBoxSite != null)
                {
                    datepicker.DoParse(datepicker.EditableTextBoxSite.Text);
                }

                //If Value != MCC.SelectedDate, set SelectedDate = Value 
                if (datepicker.MonthCalendar != null && datepicker.Value != datepicker.MonthCalendar.SelectedDate)
                {
                    datepicker.SetFlag(Flags.IgnoreDateSelectionChanged, true);
                    try
                    {
                        if(datepicker.Value != null)
                            datepicker.MonthCalendar.SelectedDate = datepicker.Value.Value.Date;
                    }
                    finally
                    {
                        datepicker.SetFlag(Flags.IgnoreDateSelectionChanged, false);
                    }
                }

                // When the drop down opens, take capture
                Mouse.Capture(datepicker, CaptureMode.SubTree);

                // Popup.IsOpen is databound to IsDropDownOpen.  We don't know
                // if IsDropDownOpen will be invalidated before Popup.IsOpen.
                // If we are invalidated first and we try to focus the item, we
                // might succeed. When the popup finally opens, Focus
                // will be sent to null because Core doesn't know what else to do.
                // So, we must focus the element only after we are sure the popup
                // has opened. We will queue an operation (at Send priority) to
                // do this work -- this is the soonest we can make this happen.
                if (datepicker.MonthCalendar != null && datepicker.Value.HasValue)
                {
                    datepicker.Dispatcher.BeginInvoke(
                        DispatcherPriority.Send,
                        (DispatcherOperationCallback)delegate(object arg)
                                                         {
                                                             DatePicker dp = (DatePicker)arg;
                                                             if (dp.IsKeyboardFocusWithin)
                                                             {
                                                                 MonthCalendarItem item = dp.MonthCalendar.GetContainerFromDate(dp.Value.Value);
                                                                 if (item != null)
                                                                 {
                                                                     item.Focus();
                                                                 }
                                                             }
                                                             return null;
                                                         },
                        datepicker);
                }

                datepicker.OnDropDownOpened(new RoutedEventArgs(DropDownOpenedEvent));
            }
            else
            {
                // If focus is within the subtree, make sure we have the focus so that focus isn't in the disposed hwnd
                if (datepicker.IsKeyboardFocusWithin)
                {
                    // If use Mouse to select a date, DateSelectionChanged is fired in ListBox.MakeSingleSelection
                    // Then ListBoxItem.Focus() will be called which will grab the focus from DatePicker
                    // So use Dispatcher.BeginInvoke to set Focus to DatePicker after ListBoxItem.Focus()
                    datepicker.Dispatcher.BeginInvoke(
                        DispatcherPriority.Loaded,
                        (DispatcherOperationCallback)delegate(object arg)
                                                         {
                                                             DatePicker dp = (DatePicker)arg;
                                                             if (dp.IsKeyboardFocusWithin)
                                                             {
                                                                 dp.Focus();
                                                             }
                                                             return null;
                                                         },
                        datepicker);

                    if (datepicker.HasCapture)
                    {
                        // It's not editable, make sure the datepicker has focus
                        datepicker.Focus();
                    }
                }

                if (datepicker.HasCapture)
                {
                    Mouse.Capture(null);
                }

                datepicker.OnDropDownClosed(new RoutedEventArgs(DropDownClosedEvent));
            }
        }

        #endregion

        #region CanEdit

        /// <summary>
        /// The DependencyProperty for the CanEdit property
        /// </summary>
        public static readonly DependencyProperty CanEditProperty =
            DependencyProperty.Register(
                "CanEdit",
                typeof(bool),
                typeof(DatePicker),
                new FrameworkPropertyMetadata(
                    BooleanBoxes.FalseBox,
                    new PropertyChangedCallback(OnCanEditChanged)));


        /// <summary>
        /// True if this DatePicker is editable.
        /// </summary>
        public bool CanEdit
        {
            get { return (bool)GetValue(CanEditProperty); }
            set { SetValue(CanEditProperty, BooleanBoxes.Box(value)); }
        }

        private static void OnCanEditChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)d;

            if ((bool)e.NewValue)
            {
                datepicker.UpdateEditableTextBox(datepicker.Text);
            }
        }

        #endregion

        #region IsReadOnly

        /// <summary>
        /// The DependencyProperty for the IsReadOnly Property
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            TextBox.IsReadOnlyProperty.AddOwner(typeof(DatePicker));

        /// <summary>
        /// When the DatePicker is Editable, if the TextBox within it is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, BooleanBoxes.Box(value)); }
        }

        #endregion

        #region IsValid

        /// <summary>
        /// The key needed set a read-only property.
        /// </summary>
        private static readonly DependencyPropertyKey IsValidPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IsValid",
                typeof(bool),
                typeof(DatePicker),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// The DependencyProperty for the IsValid property.
        /// </summary>
        public static readonly DependencyProperty IsValidProperty = IsValidPropertyKey.DependencyProperty;

        /// <summary>
        /// A property indicating whether the Value is valid or not
        /// </summary>
        public bool IsValid
        {
            get { return Value.HasValue; }
        }

        #endregion

        #region Text

        /// <summary>
        /// The key needed set a read-only property.
        /// </summary>
        private static readonly DependencyPropertyKey TextPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "Text",
                typeof(string),
                typeof(DatePicker),
                new FrameworkPropertyMetadata(""));

        /// <summary>
        /// The DependencyProperty for the Text property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = TextPropertyKey.DependencyProperty;

        /// <summary>
        /// Text store the formated Value, if the Value is null, it should store the NullValueText property
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
        }

        #endregion

        #region Value

        /// <summary>
        /// The DependencyProperty for the Value property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(DateTime?),
                typeof(DatePicker),
                new FrameworkPropertyMetadata(
                    (DateTime?)null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    new PropertyChangedCallback(OnValueChanged),
                    new CoerceValueCallback(CoerceValue)));

        /// <summary>
        /// The DateTime value of DatePicker
        /// </summary>
        public DateTime? Value
        {
            get
            {
                //If someone is inputing string when Value.get is called, DatePicker will parsing the input string immediatelly.
                if (GetFlag(Flags.IsTextChanged) && CanEdit && EditableTextBoxSite != null)
                {
                    DoParse(EditableTextBoxSite.Text);
                }

                return (DateTime?)GetValue(ValueProperty);
            }
            set { SetValue(ValueProperty, value); }
        }

        private static object CoerceValue(DependencyObject d, object value)
        {
            DatePicker datepicker = (DatePicker)d;

            if (value != null)
            {
                DateTime newValue = (DateTime)value;

                DateTime min = datepicker.MinDate;
                if (newValue < min)
                {
                    return min;
                }

                DateTime max = datepicker.MaxDate;
                if (newValue > max)
                {
                    return max;
                }
            }
            return value;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            //Invalid the IsValid and Text property when Value is changed
            datepicker.SetValue(IsValidPropertyKey, newValue.HasValue);
            datepicker.DoFormat(newValue);

            if (datepicker.MonthCalendar != null && datepicker.MonthCalendar.SelectedDate != newValue)
            {
                datepicker.SetFlag(Flags.IgnoreDateSelectionChanged, true);
                try
                {
                    if(newValue != null)
                        datepicker.MonthCalendar.SelectedDate = newValue.Value.Date;
                    else
                        datepicker.MonthCalendar.SelectedDate = newValue;
                }
                finally
                {
                    datepicker.SetFlag(Flags.IgnoreDateSelectionChanged, false);
                }
            }

            RoutedPropertyChangedEventArgs<DateTime?> routedArgs =
                new RoutedPropertyChangedEventArgs<DateTime?>(oldValue, newValue, ValueChangedEvent);

            datepicker.OnValueChanged(routedArgs);
        }

        #endregion

        #region DateConverter

        /// <summary>
        /// The DependencyProperty for the DateConverter Property
        /// </summary>
        public static readonly DependencyProperty DateConverterProperty =
            DependencyProperty.Register(
                "DateConverter",
                typeof(IValueConverter),
                typeof(DatePicker),
                new FrameworkPropertyMetadata(
                    (IValueConverter)null,
                    new PropertyChangedCallback(OnDateConverterChanged)));


        /// <summary>
        /// This property is used to parse/format between Value and text
        /// </summary>
        /// <remarks>
        /// ConvertBack is used to customize the parsing logic
        /// Convert is used to customimze the formatting logic
        /// If the converter can't parse the input text correctly, throw FormatException will fire InvalidEntry event
        /// </remarks>
        public IValueConverter DateConverter
        {
            get { return (IValueConverter)GetValue(DateConverterProperty); }
            set { SetValue(DateConverterProperty, value); }
        }

        private static void OnDateConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DatePicker)d).DoFormat();
        }

        #endregion

        #region NullValueText

        /// <summary>
        /// The DependencyProperty for the NullValueText Property
        /// </summary>
        public static readonly DependencyProperty NullValueTextProperty =
            DependencyProperty.Register(
                "NullValueText",
                typeof(string),
                typeof(DatePicker),
                new FrameworkPropertyMetadata(
                    "",
                    new PropertyChangedCallback(OnNullValueTextChanged)));

        /// <summary>
        /// This property indicates which input string should convert the Value of DatePicker into the null value.
        /// </summary>
        public string NullValueText
        {
            get { return (string)GetValue(NullValueTextProperty); }
            set { SetValue(NullValueTextProperty, value); }
        }

        private static void OnNullValueTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)d;

            if (!datepicker.Value.HasValue)
            {
                datepicker.DoFormat(null);
            }
        }

        #endregion

        #region Max/MinDate

        /// <summary>
        /// The Property for the MinDate property.
        /// </summary>
        public static readonly DependencyProperty MinDateProperty =
            MonthCalendar.MinDateProperty.AddOwner(typeof(DatePicker),
                                                   new FrameworkPropertyMetadata(
                                                       new DateTime(1753, 1, 1), /* The default value */
                                                       new PropertyChangedCallback(OnMinDateChanged)));

        /// <summary>
        /// The min date of DatePicker
        /// </summary>
        public DateTime MinDate
        {
            get { return (DateTime)GetValue(MinDateProperty); }
            set { SetValue(MinDateProperty, value); }
        }

        private static void OnMinDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)d;

            datepicker.CoerceValue(MaxDateProperty);
            datepicker.CoerceValue(ValueProperty);
        }

        /// <summary>
        /// The Property for the MaxDate property.
        /// </summary>
        public static readonly DependencyProperty MaxDateProperty =
            MonthCalendar.MaxDateProperty.AddOwner(typeof(DatePicker),
                                                   new FrameworkPropertyMetadata(
                                                       new DateTime(9998, 12, 31), /* The default value */
                                                       new PropertyChangedCallback(OnMaxDateChanged),
                                                       new CoerceValueCallback(CoerceMaxDate)));

        /// <summary>
        /// The max date of DatePicker
        /// </summary>
        public DateTime MaxDate
        {
            get { return (DateTime)GetValue(MaxDateProperty); }
            set { SetValue(MaxDateProperty, value); }
        }

        private static void OnMaxDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)d;

            datepicker.CoerceValue(ValueProperty);
        }

        private static object CoerceMaxDate(DependencyObject d, object value)
        {
            DatePicker datepicker = (DatePicker)d;
            DateTime newValue = (DateTime)value;

            DateTime min = datepicker.MinDate;
            if (newValue < min)
            {
                return min;
            }

            return value;
        }

        #endregion

        #region MonthCalendarStyle

        /// <summary>
        /// The DependencyProperty for the MonthCalendarStyle Property
        /// </summary>
        public static readonly DependencyProperty MonthCalendarStyleProperty =
            DependencyProperty.Register(
                "MonthCalendarStyle",
                typeof(Style),
                typeof(DatePicker),
                new FrameworkPropertyMetadata(
                    (Style)null,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The style of drop-down MonthCalendar
        /// </summary>
        public Style MonthCalendarStyle
        {
            get { return (Style)GetValue(MonthCalendarStyleProperty); }
            set { SetValue(MonthCalendarStyleProperty, value); }
        }

        #endregion

        #region DropDownButtonStyle

        /// <summary>
        /// The DependencyProperty for the DropDownButtonStyle property.
        /// Flags:              none
        /// Default Value:      null
        /// </summary>
        public static readonly DependencyProperty DropDownButtonStyleProperty =
            DependencyProperty.Register(
                "DropDownButtonStyle",
                typeof(Style),
                typeof(DatePicker),
                new FrameworkPropertyMetadata(
                    (Style)null, new PropertyChangedCallback(OnDropDownButtonStyleChanged)));

        /// <summary>
        /// DropDownButtonStyle property
        /// </summary>
        public Style DropDownButtonStyle
        {
            get { return (Style)GetValue(DropDownButtonStyleProperty); }
            set { SetValue(DropDownButtonStyleProperty, value); }
        }

        private static void OnDropDownButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DatePicker)d).RefreshDropDownButtonStyle();
        }

        #endregion

        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Events
        //
        //-------------------------------------------------------------------

        #region Public Events


        /// <summary>
        /// DropDownOpened event
        /// </summary>
        public static readonly RoutedEvent DropDownOpenedEvent = EventManager.RegisterRoutedEvent("DropDownOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DatePicker));

        /// <summary>
        /// DropDownClosed event
        /// </summary>
        public static readonly RoutedEvent DropDownClosedEvent = EventManager.RegisterRoutedEvent("DropDownClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DatePicker));

        /// <summary>
        /// Add / Remove DropDownOpened handler
        /// </summary>
        public event RoutedEventHandler DropDownOpened
        {
            add { AddHandler(DropDownOpenedEvent, value); }
            remove { RemoveHandler(DropDownOpenedEvent, value); }
        }

        /// <summary>
        /// Add / Remove DropDownClosed handler
        /// </summary>
        public event RoutedEventHandler DropDownClosed
        {
            add { AddHandler(DropDownClosedEvent, value); }
            remove { RemoveHandler(DropDownClosedEvent, value); }
        }

        /// <summary>
        /// InvalidEntry event
        /// </summary>
        public static readonly RoutedEvent InvalidEntryEvent = EventManager.RegisterRoutedEvent("InvalidEntry", RoutingStrategy.Bubble, typeof(InvalidEntryEventHandler), typeof(DatePicker));

        /// <summary>
        /// Add / Remove InvalidEntry handler
        /// </summary>
        public event InvalidEntryEventHandler InvalidEntry
        {
            add { AddHandler(InvalidEntryEvent, value); }
            remove { RemoveHandler(InvalidEntryEvent, value); }
        }

        /// <summary>
        /// An event reporting that the Value property changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        /// <summary>
        /// Event ID correspond to Value changed event
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<DateTime?>), typeof(DatePicker));

        #endregion

        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //-------------------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// Raise DropDownOpened event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDropDownOpened(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// Raise DropDownClosed event
        /// </summary>
        protected virtual void OnDropDownClosed(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// This event is invoked when datepicker can't parse the input string correctly
        /// </summary>
        protected virtual void OnInvalidEntry(InvalidEntryEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// This method is invoked when the Value property changes.
        /// </summary>
        /// <param name="e">RoutedPropertyChangedEventArgs contains the old and new value.</param>
        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// Called when this element gets focus.
        /// </summary>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            // If we're an editable datepicker, forward focus to the TextBox element
            if (!e.Handled && e.NewFocus == this)
            {
                if (CanEdit && EditableTextBoxSite != null)
                {
                    EditableTextBoxSite.Focus();
                    e.Handled = true;
                }
            }
        }

        #region Mouse

        /// <summary>
        /// Close the dropdown content if DatePicker lost the mouse capture
        /// </summary>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (Mouse.Captured != this)
            {
                if (e.OriginalSource == this)
                {
                    // If capture is null or it's not below the datepicker, close.
                    if (Mouse.Captured == null || !DatePickerHelper.IsDescendant(this, Mouse.Captured as Visual))
                    {
                        IsDropDownOpen = false;
                    }
                }
                else
                {
                    if (DatePickerHelper.IsDescendant(this, e.OriginalSource as Visual))
                    {
                        // Take capture if one of our children gave up capture (by closing their drop down)
                        if (IsDropDownOpen && Mouse.Captured == null)
                        {
                            Mouse.Capture(this, CaptureMode.SubTree);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        IsDropDownOpen = false;
                    }
                }
            }
        }

        /// <summary>
        /// When datepicker is editable, clicks the entry box should close the datepicker..
        /// </summary>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (CanEdit && IsDropDownOpen && EditableTextBoxSite != null)
            {
                Visual originalSource = e.OriginalSource as Visual;

                if (originalSource != null && EditableTextBoxSite.IsAncestorOf(originalSource))
                {
                    IsDropDownOpen = false;
                }
            }
        }

        #endregion

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Methods
        //
        //-------------------------------------------------------------------

        #region Private Methods

        /// <summary>
        /// Detaches the EditableTextBox, MonthCalendar from old child tree and attaches them to a new one
        /// </summary>
        private void AttachToVisualTree()
        {
            DetachFromVisualTree();

            EditableTextBoxSite = GetTemplateChild(c_EditableTextBoxTemplateName) as MaskedTextBox;
            if (EditableTextBoxSite != null)
            {
                EditableTextBoxSite.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnEditableTextBoxLostFocus);
                EditableTextBoxSite.KeyDown += new KeyEventHandler(OnEditableTextBoxKeyDown);
                EditableTextBoxSite.TextChanged += new TextChangedEventHandler(OnEditableTextBoxTextChanged);

                if (CanEdit)//set Text to EditableTextBoxSite.Text in editable mode
                {
                    UpdateEditableTextBox(Text);
                }
            }

            MonthCalendar = GetTemplateChild(c_DatePickerCalendarTemplateName) as MonthCalendar;
            if (MonthCalendar != null)
            {
                MonthCalendar.DateSelectionChanged += new DateSelectionChangedEventHandler(OnDateSelectionChanged);
                MonthCalendar.VisibleMonthChanged += new RoutedPropertyChangedEventHandler<DateTime>(OnMonthCalendarVisibleMonthChanged);
                CommandManager.AddPreviewExecutedHandler(MonthCalendar, new ExecutedRoutedEventHandler(OnMonthCalendarCommandPreviewExecuted));
            }

            RefreshDropDownButtonStyle();
        }

        /// <summary>
        /// Clear the event, and detach our current EditableTextBox from ComboBox
        /// </summary>
        private void DetachFromVisualTree()
        {
            if (EditableTextBoxSite != null)
            {
                EditableTextBoxSite.LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(OnEditableTextBoxLostFocus);
                EditableTextBoxSite.KeyDown -= new KeyEventHandler(OnEditableTextBoxKeyDown);
                EditableTextBoxSite.TextChanged -= new TextChangedEventHandler(OnEditableTextBoxTextChanged);
                EditableTextBoxSite = null;
            }

            if (MonthCalendar != null)
            {
                MonthCalendar.DateSelectionChanged -= new DateSelectionChangedEventHandler(OnDateSelectionChanged);
                MonthCalendar.VisibleMonthChanged -= new RoutedPropertyChangedEventHandler<DateTime>(OnMonthCalendarVisibleMonthChanged);
                CommandManager.RemovePreviewExecutedHandler(MonthCalendar, new ExecutedRoutedEventHandler(OnMonthCalendarCommandPreviewExecuted));
                MonthCalendar = null;
            }
        }

        private void RefreshDropDownButtonStyle()
        {
            ButtonBase dropdownButton = GetTemplateChild(c_DropDownButtonName) as ButtonBase;
            if (dropdownButton != null)
            {
                if (DropDownButtonStyle == null)
                {
                    if (_defaultDropDownButtonStyle == null)
                    {
                        _defaultDropDownButtonStyle = FindResource(new ComponentResourceKey(typeof(DatePicker), "DropDownButtonStyleKey")) as Style;
                    }
                    dropdownButton.Style = _defaultDropDownButtonStyle;
                }
                else
                {
                    dropdownButton.Style = DropDownButtonStyle;
                }
            }
        }

        /// <summary>
        /// If EditableTextBoxSite loses focus and Text has been changed, DatePicker will parse Text
        /// </summary>
        private void OnEditableTextBoxLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (EditableTextBoxSite != null && 
                GetFlag(Flags.IsTextChanged) && 
                !GetFlag(Flags.IsContextMenuOpen))
            {
                DoParse(EditableTextBoxSite.Text);
            }
        }

        /// <summary>
        /// If Key.Enter is pressed, DatePicker will parse the Text no matter it changed or not
        /// </summary>
        private void OnEditableTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && EditableTextBoxSite != null)
            {
                DoParse(EditableTextBoxSite.Text);
                e.Handled = true;
            }
        }

        /// <summary>
        /// If Text has been changed, the flag Flags.IsTextChanged will be set to true, so DatePicker can decide to parse the Text or not
        /// </summary>
        private void OnEditableTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            SetFlag(Flags.IsTextChanged, true);
        }

        /// <summary>
        /// If we (or one of our children) are clicked, claim the focus
        /// </summary>
        //Note: Can't use override OnMouseButtonDown because some other controls in DatePicker
        //may handle the event first, so we can't get it
        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DatePicker datepicker = (DatePicker)sender;

            // If we (or one of our children) are clicked, claim the focus (don't steal focus if our context menu is clicked)
            if (!datepicker.IsKeyboardFocusWithin)
            {
                if (datepicker.EditableTextBoxSite != null && !datepicker.EditableTextBoxSite.IsFocused)
                {
                    datepicker.Focus();
                }
            }

            e.Handled = true;   // Always handle so that parents won't take focus away

            if (Mouse.Captured == datepicker && e.OriginalSource == datepicker)
            {
                // When we have capture, all clicks off the popup will have the datepicker as
                // the OriginalSource.  So when the original source is the datepicker, that
                // means the click was off the popup and we should dismiss.
                datepicker.IsDropDownOpen = false;
            }
            else
            {
                // If mouse click the selected date, close the popup
                FrameworkElement fe = e.OriginalSource as FrameworkElement;
                if (fe != null && fe.DataContext is CalendarDate)
                {
                    if (datepicker.Value.HasValue
                        && datepicker.Value.Value == ((CalendarDate)fe.DataContext).Date)
                    {
                        datepicker.IsDropDownOpen = false;
                    }
                }
            }
        }

        private static void OnContextMenuOpen(object sender, ContextMenuEventArgs e)
        {
            ((DatePicker)sender).SetFlag(Flags.IsContextMenuOpen, true);
        }

        private static void OnContextMenuClose(object sender, ContextMenuEventArgs e)
        {
            ((DatePicker)sender).SetFlag(Flags.IsContextMenuOpen, false);
        }

        #region MonthCalendar

        private void OnMonthCalendarCommandPreviewExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            if (args.Command == MonthCalendar.PreviousCommand
                || args.Command == MonthCalendar.NextCommand
                || args.Command == MonthCalendar.GotoCommand)
            {
                SetFlag(Flags.IsNormalVisibleMonthChanged, Value.HasValue && IsDropDownOpen);
            }
        }

        private void OnMonthCalendarVisibleMonthChanged(object sender, RoutedPropertyChangedEventArgs<DateTime> e)
        {
            if (IsDropDownOpen && Value.HasValue && GetFlag(Flags.IsNormalVisibleMonthChanged))
            {
                int monthInterval = (e.NewValue.Year - e.OldValue.Year) * 12 + (e.NewValue.Month - e.OldValue.Month);
                MonthCalendar.SelectedDate = Value.Value.AddMonths(monthInterval);
            }
        }

        private void OnDateSelectionChanged(object sender, DateSelectionChangedEventArgs e)
        {
            if (IsDropDownOpen && !GetFlag(Flags.IgnoreDateSelectionChanged))
            {
                Value = MonthCalendar.SelectedDate;

                //
                if (!_datepickerCalendar.IsRecentInputDeviceKeyboard
                    && 
                         !GetFlag(Flags.IsNormalVisibleMonthChanged))
                {
                    IsDropDownOpen = false;
                }
            }

            SetFlag(Flags.IsNormalVisibleMonthChanged, false);
        }

        #endregion

        #region Keyboard

        /// <summary>
        /// Called when a key event occurs.
        /// </summary>
        private static void KeyDownHandler(object sender, KeyEventArgs e)
        {
            ((DatePicker)sender).KeyDownHandler(e);
        }

        private void KeyDownHandler(KeyEventArgs e)
        {
            bool handled = false;
            Key key = e.Key;

            // Only process key events if they haven't been handled or are from our text box
            if (e.Handled == false || e.OriginalSource == EditableTextBoxSite)
            {
                // We want to handle Alt key. Get the real key if it is Key.System.
                if (key == Key.System)
                {
                    key = e.SystemKey;
                }

                switch (key)
                {
                    case Key.Up:
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                        {
                            KeyboardToggleDropDown(!IsDropDownOpen, true /* commitSelection */);
                            handled = true;
                        }
                        break;

                    case Key.Down:
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                        {
                            KeyboardToggleDropDown(!IsDropDownOpen, true /* commitSelection */);
                            handled = true;
                        }
                        else if (IsDropDownOpen)
                        {
                            SelectFocusableDate();
                            handled = true;
                        }
                        break;

                    case Key.F4:
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == 0)
                        {
                            KeyboardToggleDropDown(!IsDropDownOpen, true /* commitSelection */);
                            handled = true;
                        }
                        break;

                    case Key.Escape:
                        if (IsDropDownOpen)
                        {
                            KeyboardToggleDropDown(false, false /* commitSelection */);
                            handled = true;
                        }
                        break;

                    case Key.Enter:
                        if (IsDropDownOpen)
                        {
                            KeyboardToggleDropDown(false, true /* commitSelection */);
                            handled = true;
                        }
                        break;

                    case Key.Tab:
                        if (IsDropDownOpen)
                        {
                            IsDropDownOpen = false;
                        }
                        break;

                    default:
                        handled = false;
                        break;
                }

                if (e.Key < Key.D0 || e.Key > Key.D9)
                    handled = true;

                if (handled)
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Close the dropdown and commit the selection if requested.
        /// Make sure to set the selection after the dropdown has closed
        /// Don't trigger any unnecessary navigation as a result of changing the selection.
        /// </summary>
        private void KeyboardToggleDropDown(bool openDropDown, bool commitSelection)
        {
            IsDropDownOpen = openDropDown;

            if (!openDropDown)
            {
                if (commitSelection && MonthCalendar != null)
                {
                    Value = MonthCalendar.SelectedDate;
                }
                else
                {
                    Value = _previousValue;
                }
            }
        }

        /// <summary>
        /// Select the focusable date
        /// </summary>
        private void SelectFocusableDate()
        {
            if (MonthCalendar == null)
            {
                return;
            }

            //If Value isn't null, select it; if not, select the first focusable date
            MonthCalendarItem focusableItem = null;
            if (Value.HasValue)
            {
                focusableItem = MonthCalendar.GetContainerFromDate(Value.Value);
            }
            else
            {
                DateTime firstDayOfMonth = new DateTime(MonthCalendar.VisibleMonth.Year, MonthCalendar.VisibleMonth.Month, 1);

                for (int i = 0; i < DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month); ++i)
                {
                    focusableItem = MonthCalendar.GetContainerFromDate(firstDayOfMonth);
                    if (IsFocusable(focusableItem))
                    {
                        break;
                    }
                    firstDayOfMonth = firstDayOfMonth.AddDays(1);
                }
            }

            if (focusableItem != null)
            {
                focusableItem.IsSelected = true;
                focusableItem.Focus();
            }
        }

        /// <summary>
        /// True if the element can be focused
        /// </summary>
        private bool IsFocusable(FrameworkElement fe)
        {
            return fe != null && fe.Focusable && (bool)fe.GetValue(IsTabStopProperty) && fe.IsEnabled && fe.Visibility == Visibility.Visible;
        }

        #endregion

        /// <summary>
        /// Parse the input string, if the input string is a valid date, return the Date, else return null
        /// </summary>
        /// <param name="text">the input string</param>
        /// <remarks>
        /// If the input entry equals NullValueText, Value will be set to null
        /// If the input entry is a valid date, Value will be set to the input date
        /// If the input entry isn't a valid date, InvalidEntry event will be fired, Value will still keep the old value (don't set to null)
        /// </remarks>
        private void DoParse(string text)
        {
            if (GetFlag(Flags.IsParsing))
            {
                return;
            }

            DateTime? date = null;
            bool isValidDate = true;
            SetFlag(Flags.IsTextChanged, false);
            SetFlag(Flags.IsParsing, true);

            if (text == NullValueText)
            {
                SetFlag(Flags.IsParsing, false);
            }
            else
            {
                //If user provides DateConverter, use it to parse; if not, use default converter
                object ret = null;
                try
                {
                    CultureInfo cultureInfo = Language != null ? Language.GetSpecificCulture() : null;
                    if (DateConverter != null)
                    {
                        ret = DateConverter.ConvertBack(text, typeof(DateTime), null, cultureInfo);
                    }
                    else
                    {
                        ret = _defaultDateConverter.ConvertBack(text, typeof(DateTime), null, cultureInfo);
                    }
                }
                catch (FormatException)
                {
                    isValidDate = false;
                }
                finally
                {
                    SetFlag(Flags.IsParsing, false);
                }

                if (ret is DateTime)
                {
                    date = new DateTime?((DateTime)ret);
                }
            }

            if (isValidDate)
            {
                //If the input entry is a valid date
                //Note: Since DatePicker use coercion for Value/MaxDate/MinDate
                //Value = date can't change the Value if date exceeds the range of Max/MinDate
                //But in this case, we need to update the EditableTextBox.Text to Value(call UpdateEditableTextBox(DoFormat(Value)))
                DateTime? oldValue = Value;
                Value = date;
                if (oldValue == Value)
                {
                    DoFormat();
                }
            }
            else
            {
                //If the input entry isn't a valid date, fire InvalidEntry event
                SetFlag(Flags.IgnoreUpdateEditableTextBox, true);
                try
                {
                    Value = null;
                }
                finally
                {
                    SetFlag(Flags.IgnoreUpdateEditableTextBox, false);
                }

                if (EditableTextBoxSite != null)
                {
                    if (!EditableTextBoxSite.IsFocused)
                    {
                        EditableTextBoxSite.Focus();
                    }
                    EditableTextBoxSite.SelectAll();
                }

                InvalidEntryEventArgs args = new InvalidEntryEventArgs(InvalidEntryEvent, text);
                OnInvalidEntry(args);
            }
        }

        /// <summary>
        /// Format Value property to a formatted string
        /// </summary>
        private string DoFormat()
        {
            return DoFormat(Value);
        }

        protected virtual string GetDateFormat(CultureInfo cultureInfo)
        {
            return cultureInfo.DateTimeFormat.ShortDatePattern;
        }

        private string DoFormat(DateTime? date)
        {
            string text;

            if (date.HasValue)
            {
                CultureInfo cultureInfo = Language != null ? Language.GetSpecificCulture() : null;
                object o = null;
                if (DateConverter != null)
                {
                    o = DateConverter.Convert(date.Value, typeof(string), GetDateFormat(cultureInfo), cultureInfo);
                }
                else
                {
                    o = _defaultDateConverter.Convert(date.Value, typeof(string), GetDateFormat(cultureInfo), cultureInfo);
                }

                text = Convert.ToString(o, cultureInfo);
            }
            else
            {
                text = NullValueText;
            }

            SetValue(TextPropertyKey, text);
            if (CanEdit && !GetFlag(Flags.IgnoreUpdateEditableTextBox))
            {
                UpdateEditableTextBox(text);
            }

            return text;
        }

        /// <summary>
        /// Update the Text to the TextBox in editable mode
        /// </summary>
        private void UpdateEditableTextBox(string text)
        {
            // If the DatePicker is editable, it must have an EditableTextBoxSite
            if (EditableTextBoxSite != null)
            {
                if (!string.Equals(EditableTextBoxSite.Text, text, StringComparison.Ordinal))
                {
                    EditableTextBoxSite.Text = text;
                    SetFlag(Flags.IsTextChanged, false);//Ignore internal set Text fired TextChanged event
                }

                // If we have focus and the IsDropDownOpen is false, set the focus to the TextBox
                if (IsKeyboardFocusWithin)
                {
                    if (!IsDropDownOpen)
                    {
                        EditableTextBoxSite.Focus();
                    }
                    EditableTextBoxSite.SelectAll();
                }
            }
        }

        private bool GetFlag(Flags flag)
        {
            return (_flags & flag) == flag;
        }

        private void SetFlag(Flags flag, bool set)
        {
            if (set)
            {
                _flags |= flag;
            }
            else
            {
                _flags &= (~flag);
            }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Fields
        //
        //-------------------------------------------------------------------

        #region Private Fields

        protected MaskedTextBox EditableTextBoxSite
        {
            get { return _editableTextBoxSite; }
            set { _editableTextBoxSite = value; }
        }

        private MaskedTextBox _editableTextBoxSite;

        private MonthCalendar MonthCalendar
        {
            get { return _datepickerCalendar; }
            set { _datepickerCalendar = value; }
        }

        private MonthCalendar _datepickerCalendar;

        private bool HasCapture
        {
            get { return Mouse.Captured == this; }
        }

        //If Key.Escape is pressed, Value will roll back to the previous value
        private DateTime? _previousValue = null;
        private Style _defaultDropDownButtonStyle;

        [Flags]
        private enum Flags
        {
            //True if user has changed the text of TextBox
            IsTextChanged = 0x00000001,
            //Avoid reentry the parse process in DoParse()
            IsParsing = 0x00000002,
            IsContextMenuOpen = 0x00000004,
            //True if VisibleMonthChanged event is fired by clicking Next/Previous button or pressing PageUp/PageDown
            IsNormalVisibleMonthChanged = 0x00000008,
            //True to ignore the DateSelectionChanged event which is fired when SelectedDate is set a new value
            IgnoreDateSelectionChanged = 0x00000010,
            //True to ignore updating EditableTextBox when Value is changed
            IgnoreUpdateEditableTextBox = 0x00000020,
        }

        private Flags _flags;

        //Default DateConverter, it's used if user doesn't provide the DateConverter
        private readonly IValueConverter _defaultDateConverter = new DateTimeValueConverter();

        // Part names used in the style. The class TemplatePartAttribute should use the same names
        private const string c_EditableTextBoxTemplateName = "PART_EditableTextBox";
        private const string c_DatePickerCalendarTemplateName = "PART_DatePickerCalendar";
        private const string c_DropDownButtonName = "PART_DropDownButton";

        #endregion
    }
}