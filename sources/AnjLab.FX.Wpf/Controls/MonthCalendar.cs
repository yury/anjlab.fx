//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections;              // IEnumerable
using System.Collections.Generic;      // List<T>
using System.Collections.ObjectModel;  // ReadOnlyCollection<T>
using System.Collections.Specialized;  // NotifyCollectionChangedEventArgs
using System.Diagnostics;       // Debug
using System.Globalization;     // CultureInfo
using System.Windows;
using System.Windows.Controls;  // Control
using System.Windows.Controls.Primitives;
using System.Windows.Data;      // IValueConverter
using System.Windows.Input;     // RoutedCommand
using System.Windows.Media;     
using System.Windows.Threading; // DispatcherPriority
using System.Xml;               // XmlAttribute


namespace AnjLab.FX.Wpf.Controls
{
    /// <summary>
    /// The month calendar control implements a calendar-like user interface,
    /// that provides the user with a very intuitive and recognizable method
    /// of selecting a date, a contiguous or discrete ranges of dates using
    /// a visual display. Users can customize the look of the calendar portion
    /// of the control by setting titles, dates, fonts and backgrounds.
    /// </summary>
    [TemplatePart(Name = "PART_VisibleDaysHost", Type = typeof(MonthCalendarContainer))]
    [TemplatePart(Name = "PART_PreviousButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_NextButton", Type = typeof(ButtonBase))]
    public class MonthCalendar : Control, IWeakEventListener
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Static Constructor
        /// </summary>
        static MonthCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthCalendar), new FrameworkPropertyMetadata(typeof(MonthCalendar)));

            _gotoCommand = new RoutedCommand("Goto", typeof(MonthCalendar));
            CommandManager.RegisterClassCommandBinding(typeof(MonthCalendar), new CommandBinding(MonthCalendar.GotoCommand, new ExecutedRoutedEventHandler(OnExecuteGotoCommand), new CanExecuteRoutedEventHandler(OnQueryGotoCommand)));

            _nextCommand = new RoutedCommand("Next", typeof(MonthCalendar));
            CommandManager.RegisterClassCommandBinding(typeof(MonthCalendar), new CommandBinding(MonthCalendar.NextCommand, new ExecutedRoutedEventHandler(OnExecuteNextCommand), new CanExecuteRoutedEventHandler(OnQueryNextCommand)));
            CommandManager.RegisterClassInputBinding(typeof(MonthCalendar), new InputBinding(MonthCalendar.NextCommand, new KeyGesture(Key.PageDown)));

            _previousCommand = new RoutedCommand("Previous", typeof(MonthCalendar));
            CommandManager.RegisterClassCommandBinding(typeof(MonthCalendar), new CommandBinding(MonthCalendar.PreviousCommand, new ExecutedRoutedEventHandler(OnExecutePreviousCommand), new CanExecuteRoutedEventHandler(OnQueryPreviousCommand)));
            CommandManager.RegisterClassInputBinding(typeof(MonthCalendar), new InputBinding(MonthCalendar.PreviousCommand, new KeyGesture(Key.PageUp)));

            IsTabStopProperty.OverrideMetadata(typeof(MonthCalendar), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(MonthCalendar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(MonthCalendar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MonthCalendar()
            : base()
        {}

        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Methods
        //
        //-------------------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// Returns a string representation for this control.
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + " VisibleMonth: " + VisibleMonth.ToShortDateString() + ", SelectedDates.Count: " + SelectedDates.Count.ToString();
        }

        /// <summary>
        /// Called when the Template's tree has been generated
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AttachToVisualTree();
        }

        /// <summary>
        /// Return the UI element corresponding to the given date.
        /// Returns null if the date does not belong to the visible days
        /// or if no UI has been generated for it.
        /// </summary>
        public MonthCalendarItem GetContainerFromDate(DateTime date)
        {
            CalendarDate cdate = GetCalendarDateByDate(date);
            if (cdate != null && _mccContainer != null)
            {
                return _mccContainer.ItemContainerGenerator.ContainerFromItem(cdate) as MonthCalendarItem;
            }
            return null;
        }

        public void RefreshDays()
        {
            _mccContainer.ItemsSource = null;
            _mccContainer.ItemsSource = VisibleDays;
        }
        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Events
        //
        //-------------------------------------------------------------------

        #region Public Events

        /// <summary>
        /// An event fired when the selection changes
        /// </summary>
        public static readonly RoutedEvent DateSelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "DateSelectionChanged", RoutingStrategy.Bubble, typeof(DateSelectionChangedEventHandler), typeof(MonthCalendar));

        /// <summary>
        /// An event fired when this MonthCalendar's selection changes
        /// </summary>
        public event DateSelectionChangedEventHandler DateSelectionChanged
        {
            add { AddHandler(DateSelectionChangedEvent, value); }
            remove { RemoveHandler(DateSelectionChangedEvent, value); }
        }

        /// <summary>
        /// An event fired when the display month switches
        /// </summary>
        public static readonly RoutedEvent VisibleMonthChangedEvent = EventManager.RegisterRoutedEvent(
            "VisibleMonthChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<DateTime>), typeof(MonthCalendar));

        /// <summary>
        /// Add / Remove VisibleMonthChangedEvent handler
        /// </summary>
        public event RoutedPropertyChangedEventHandler<DateTime> VisibleMonthChanged
        {
            add { AddHandler(VisibleMonthChangedEvent, value); }
            remove { RemoveHandler(VisibleMonthChangedEvent, value); }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Commands
        //
        //-------------------------------------------------------------------

        #region Public Commands

        private static RoutedCommand _gotoCommand = null;

        /// <summary>
        /// Go to month
        /// </summary>
        /// <remarks>
        /// if the argument is null,     GotoCommand will switch to DateTime.Now
        /// if the argument is DateTime, GotoCommand will switch to the month from the argument
        /// </remarks>
        public static RoutedCommand GotoCommand
        {
            get { return _gotoCommand; }
        }

        private static void OnQueryGotoCommand(object target, CanExecuteRoutedEventArgs args)
        {
            MonthCalendar mcc = (MonthCalendar)target;

            int offset = 0;

            if (args.Parameter == null)
            {
                offset = MonthCalendarHelper.SubtractByMonth(DateTime.Now, mcc.VisibleMonth);     
            }
            else if (args.Parameter is DateTime)
            {
                offset = MonthCalendarHelper.SubtractByMonth((DateTime)args.Parameter, mcc.VisibleMonth);
            }

            if (offset != 0)
            {
                DateTime newValue = mcc.VisibleMonth.AddMonths(offset);

                args.CanExecute = (MonthCalendarHelper.CompareYearMonth(newValue, mcc.VisibleMonth) != 0
                                   && MonthCalendarHelper.IsWithinRange(newValue, mcc.MinDate, mcc.MaxDate));
            }
        }

        private static void OnExecuteGotoCommand(object target, ExecutedRoutedEventArgs args)
        {
            MonthCalendar mcc = (MonthCalendar)target;

            int offset = 0;
            if (args.Parameter == null)
            {
                offset = MonthCalendarHelper.SubtractByMonth(DateTime.Now, mcc.VisibleMonth);
            }
            else if (args.Parameter is DateTime)
            {
                offset = MonthCalendarHelper.SubtractByMonth((DateTime)args.Parameter, mcc.VisibleMonth);
            }

            Debug.Assert(offset != 0);
            if (offset > 0)
            {
                mcc.ScrollVisibleMonth(1, offset);
            }
            else
            {
                mcc.ScrollVisibleMonth(-1, Math.Abs(offset));
            }
        }


        private static RoutedCommand _nextCommand = null;
        private static RoutedCommand _previousCommand = null;

        /// <summary>
        /// Switch to next month
        /// </summary>
        public static RoutedCommand NextCommand
        {
            get { return _nextCommand; }
        }

        /// <summary>
        /// Switch to previous month
        /// </summary>
        public static RoutedCommand PreviousCommand
        {
            get { return _previousCommand; }
        }

        private static void OnQueryNextCommand(object target, CanExecuteRoutedEventArgs args)
        {
            MonthCalendar mcc = (MonthCalendar)target;

            args.CanExecute = MonthCalendarHelper.CompareYearMonth(mcc.VisibleMonth, mcc.MaxDate) < 0;
        }

        private static void OnExecuteNextCommand(object target, ExecutedRoutedEventArgs args)
        {
            MonthCalendar mcc = (MonthCalendar)target;
            mcc.ScrollVisibleMonth(1, 0);
        }

        private static void OnQueryPreviousCommand(object target, CanExecuteRoutedEventArgs args)
        {
            MonthCalendar mcc = (MonthCalendar)target;
            args.CanExecute = (MonthCalendarHelper.CompareYearMonth(mcc.VisibleMonth, mcc.MinDate) > 0);
        }

        private static void OnExecutePreviousCommand(object target, ExecutedRoutedEventArgs args)
        {
            MonthCalendar mcc = (MonthCalendar)target;
            mcc.ScrollVisibleMonth(-1, 0);
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //-------------------------------------------------------------------

        #region Public Properties

        #region SelectedDates/Date 

        /// <summary>
        /// Selected Dates collection.
        /// </summary>
        public ObservableCollection<DateTime> SelectedDates
        {
            get
            {
                if (_selectedDates == null)
                {
                    _selectedDates = new ObservableCollection<DateTime>();
                    _selectedDates.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSelectedDatesCollectionChanged);
                }
                return _selectedDates;
            }
        }

        /// <summary>
        /// The DependencyProperty for SelectedDate property
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
                DependencyProperty.Register(
                        "SelectedDate",
                        typeof(DateTime?),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                (DateTime?)null,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                new PropertyChangedCallback(OnSelectedDateChanged)),
                        new ValidateValueCallback(IsValidNullableDate));

        /// <summary>
        /// The first date in the current selection or returns null if the selection is empty
        /// </summary>
        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        /// <summary>
        /// Validate input value in MonthCalendar
        /// </summary>
        /// <returns>Returns False if value isn't null and is outside CalendarDataGenerator.MinDate~MaxDate range.  Otherwise, returns True.</returns>
        private static bool IsValidNullableDate(object value)
        {
            DateTime? date = (DateTime?)value;

            return !date.HasValue ||
                MonthCalendarHelper.IsWithinRange(date.Value, CalendarDataGenerator.MinDate, CalendarDataGenerator.MaxDate);
        }

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar mcc = (MonthCalendar)d;

            if (!mcc.SelectionChange.IsActive && 
                !mcc.GetFlag(Flags.IsInternalChangingSelectedDate))
            {
                mcc.SelectionChange.SelectJustThisDate((DateTime?)e.NewValue, true, true);
            }
        }

        #endregion

        #region FirstDayOfWeek

        /// <summary>
        /// The first day of the week as displayed in the month calendar
        /// </summary>
        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
            set { SetValue(FirstDayOfWeekProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for FirstDayOfWeek property
        /// </summary>
        public static readonly DependencyProperty FirstDayOfWeekProperty =
                DependencyProperty.Register(
                        "FirstDayOfWeek",
                        typeof(DayOfWeek),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                DayOfWeek.Sunday /* default value */,
                                new PropertyChangedCallback(OnFirstDayOfWeekChanged)),
                        new ValidateValueCallback(IsValidFirstDayOfWeek));

        private static void OnFirstDayOfWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MonthCalendar)d).InvalidateVisibleDays(0);
        }

        private static bool IsValidFirstDayOfWeek(object value)
        {
            DayOfWeek day = (DayOfWeek)value;

            return day == DayOfWeek.Sunday
                || day == DayOfWeek.Monday
                || day == DayOfWeek.Tuesday
                || day == DayOfWeek.Wednesday
                || day == DayOfWeek.Thursday
                || day == DayOfWeek.Friday
                || day == DayOfWeek.Saturday;
        }

        #endregion FirstDayOfWeek

        #region Max/MinDate

        /// <summary>
        /// The Property for the MinDate property.
        /// </summary>
        public static readonly DependencyProperty MinDateProperty =
                DependencyProperty.Register(
                        "MinDate",
                        typeof(DateTime),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                CalendarDataGenerator.MinDate, /* The default value */
                                new PropertyChangedCallback(OnMinDateChanged)),
                        new ValidateValueCallback(IsValidDate));

        /// <summary>
        /// The min date of MonthCalendar
        /// </summary>
        public DateTime MinDate
        {
            get { return (DateTime)GetValue(MinDateProperty); }
            set { SetValue(MinDateProperty, value); }
        }

        private static void OnMinDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar mcc = (MonthCalendar)d;

            DateTime oldMaxDate = mcc.MaxDate;
            DateTime oldVisibleMonth = mcc.VisibleMonth;
            mcc.CoerceValue(MaxDateProperty);
            mcc.CoerceValue(VisibleMonthProperty);

            //If MaxDate, VisibleMonth hasn't been changed by CoerceValue, then 
            //we should update the IsSelectable and SelectedDates in this method
            if (MonthCalendarHelper.CompareYearMonthDay(oldMaxDate, mcc.MaxDate) == 0
                && MonthCalendarHelper.CompareYearMonth(oldVisibleMonth, mcc.VisibleMonth) == 0)
            {
                mcc.OnMaxMinDateChanged((DateTime)e.NewValue, mcc.MaxDate);
            }
        }

        /// <summary>
        /// Validate input value in MonthCalendar (MinDate, MaxDate, VisibleMonth)
        /// </summary>
        /// <returns>Returns False if value is outside CalendarDataGenerator.MinDate~MaxDate range.  Otherwise, returns True.</returns>
        private static bool IsValidDate(object value)
        {
            DateTime date = (DateTime)value;

            return (date >= CalendarDataGenerator.MinDate) &&
                    (date <= CalendarDataGenerator.MaxDate);
        }


        /// <summary>
        /// The Property for the MaxDate property.
        /// </summary>
        public static readonly DependencyProperty MaxDateProperty =
                DependencyProperty.Register(
                        "MaxDate",
                        typeof(DateTime),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                CalendarDataGenerator.MaxDate, /* The default value */
                                new PropertyChangedCallback(OnMaxDateChanged),
                                new CoerceValueCallback(CoerceMaxDate)),
                        new ValidateValueCallback(IsValidDate));

        /// <summary>
        /// The max date of MonthCalendar
        /// </summary>
        public DateTime MaxDate
        {
            get { return (DateTime)GetValue(MaxDateProperty); }
            set { SetValue(MaxDateProperty, value); }
        }

        private static object CoerceMaxDate(DependencyObject d, object value)
        {
            MonthCalendar mcc = (MonthCalendar)d;
            DateTime newValue = (DateTime)value;

            DateTime min = mcc.MinDate;
            if (newValue < min)
            {
                return min;
            }

            return value;
        }

        private static void OnMaxDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar mcc = (MonthCalendar)d;

            DateTime oldVisibleMonth = mcc.VisibleMonth;
            mcc.CoerceValue(VisibleMonthProperty);

            //If VisibleMonth hasn't been changed by CoerceValue, 
            //we should update the IsSelectable and SelectedDates in this method
            if (MonthCalendarHelper.CompareYearMonth(oldVisibleMonth, mcc.VisibleMonth) == 0)
            {
                mcc.OnMaxMinDateChanged(mcc.MinDate, (DateTime)e.NewValue);
            }
        }

        /// <summary>
        /// Update the IsSelectable property of visible days and selected dates when max/min date has been changed
        /// </summary>
        /// <param name="minDate">new MinDate</param>
        /// <param name="maxDate">new MaxDate</param>
        private void OnMaxMinDateChanged(DateTime minDate, DateTime maxDate)
        {
            int count = VisibleDays.Count;
            for (int i = 0; i < count; ++i)
            {
                VisibleDays[i].IsSelectable =
                    MonthCalendarHelper.IsWithinRange(VisibleDays[i].Date, minDate, maxDate); 
            }

            //Update the selected dates if the new Max/MinDate value is within visible days
            if (minDate > FirstVisibleDate || maxDate < LastVisibleDate)
            {
                SelectionChange.Begin();
                bool succeeded = false;
                try
                {
                    foreach (DateTime dt in SelectedDates)
                    {
                        if (!MonthCalendarHelper.IsWithinRange(dt, minDate, maxDate))
                        {
                            SelectionChange.Unselect(dt);
                        }
                    }

                    SelectionChange.End(true, true);
                    succeeded = true;
                }
                finally
                {
                    if (!succeeded)
                    {
                        SelectionChange.Cancel();
                    }
                }
            }
        }

        #endregion

        #region VisibleMonth

        /// <summary>
        /// The DependencyProperty for VisibleMonth property
        /// </summary>
        public static readonly DependencyProperty VisibleMonthProperty =
                DependencyProperty.Register(
                        "VisibleMonth",
                        typeof(DateTime),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                DateTime.Today /* default value */,
                                new PropertyChangedCallback(OnVisibleMonthChanged),
                                new CoerceValueCallback(CoerceVisibleMonth)),
                        //Note: Though only year/month work on VisibleMonth, we'll still compare its value with CalendarDataGenerator.Max/MinDate
                        new ValidateValueCallback(IsValidDate)); 

        /// <summary>
        /// The first visible month
        /// </summary>
        /// <remarks>
        /// Only the Year and Month field is used, not guarantee the day is 1!
        /// </remarks>
        public DateTime VisibleMonth
        {
            get { return (DateTime)GetValue(VisibleMonthProperty); }
            set { SetValue(VisibleMonthProperty, value); }
        }


        private static object CoerceVisibleMonth(DependencyObject d, object value)
        {
            MonthCalendar mcc = (MonthCalendar)d;
            DateTime newValue = (DateTime)value;

            DateTime min = mcc.MinDate;
            if (newValue < min)
            {
                return min;
            }

            DateTime max = mcc.MaxDate;
            if (newValue > max)
            {
                return max;
            }

            return newValue;
        }

        private static void OnVisibleMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar mcc = (MonthCalendar)d;
            DateTime oldDate = (DateTime)e.OldValue;
            DateTime newDate = (DateTime)e.NewValue;

            //oldDate != newDate in Year/Month field
            if (MonthCalendarHelper.CompareYearMonth(oldDate, newDate) != 0)
            {
                try
                {
                    //raise the event
                    mcc.OnVisibleMonthChanged(new RoutedPropertyChangedEventArgs<DateTime>(oldDate, newDate, VisibleMonthChangedEvent));
                }
                finally
                {
                    mcc.SetFlag(Flags.IsSwitchingMonth, true);
                    try
                    {
                        mcc.InvalidateVisibleDays(MonthCalendarHelper.SubtractByMonth(newDate, oldDate));
                    }
                    finally
                    {
                        mcc.SetFlag(Flags.IsSwitchingMonth, false);
                    }
                }
            }
        }

        #endregion VisibleMonth

        #region MaxSelectionCount

        /// <summary>
        /// The DependencyProperty for MaxSelectionCount property
        /// </summary>
        public static readonly DependencyProperty MaxSelectionCountProperty =
                DependencyProperty.Register(
                        "MaxSelectionCount",
                        typeof(int),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                int.MaxValue /* default value */,
                                new PropertyChangedCallback(OnMaxSelectionCountChanged)),
                        new ValidateValueCallback(IsValidMaxSelectionCount));

        /// <summary>
        /// The maximum number of days that can be selected in a month calendar control
        /// </summary>
        public int MaxSelectionCount
        {
            get { return (int)GetValue(MaxSelectionCountProperty); }
            set { SetValue(MaxSelectionCountProperty, value); }
        }

        private static void OnMaxSelectionCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar mcc = (MonthCalendar)d;
            int newValue = (int)e.NewValue;

            if (newValue < mcc.SelectedDates.Count)
            {
                mcc.TrimSelectedDates(newValue);
            }
        }

        private static bool IsValidMaxSelectionCount(object o)
        {
            return (int)o > 0;
        }

        /// <summary> 
        /// Delete redundant selected dates to fit the new MaxSelectionCount value
        /// </summary>
        private void TrimSelectedDates(int limit)
        {
            if (SelectionChange.IsActive)
            {
                return;
            }

            SelectionChange.Begin();
            bool succeeded = false;
            try
            {
                int count = SelectedDates.Count;
                for (int i = count - 1; i >= limit; i--)
                {
                    SelectionChange.Unselect(SelectedDates[i]);
                }

                SelectionChange.End(true, true);
                succeeded = true;
            }
            finally
            {
                if (!succeeded)
                {
                    SelectionChange.Cancel();
                }
            }
        }


        #endregion

        #region ShowsTitle/WeekNumbers/DayHeader Properties

        /// <summary>
        /// The DependencyProperty for ShowsTitle property
        /// </summary>
        public static readonly DependencyProperty ShowsTitleProperty =
                DependencyProperty.Register(
                        "ShowsTitle",
                        typeof(bool),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Indicating whether the control displays the title or not
        /// </summary>
        public bool ShowsTitle
        {
            get { return (bool)GetValue(ShowsTitleProperty); }
            set { SetValue(ShowsTitleProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// The DependencyProperty for ShowsWeekNumbers property
        /// </summary>
        public static readonly DependencyProperty ShowsWeekNumbersProperty =
                DependencyProperty.Register(
                        "ShowsWeekNumbers",
                        typeof(bool),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Indicating whether the control displays the week numbers or not
        /// </summary>
        public bool ShowsWeekNumbers
        {
            get { return (bool)GetValue(ShowsWeekNumbersProperty); }
            set { SetValue(ShowsWeekNumbersProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        /// The DependencyProperty for ShowsDayHeaders property
        /// </summary>
        public static readonly DependencyProperty ShowsDayHeadersProperty =
                DependencyProperty.Register(
                        "ShowsDayHeaders",
                        typeof(bool),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Indicating whether the control displays the day header or not
        /// </summary>
        public bool ShowsDayHeaders
        {
            get { return (bool)GetValue(ShowsDayHeadersProperty); }
            set { SetValue(ShowsDayHeadersProperty, BooleanBoxes.Box(value)); }
        }

        #endregion

        #region DayTemplate/DayTemplateSelector/DayContainerStyle/DayContainerStyleSelector

        /// <summary>
        /// The DependencyProperty for the DayTemplate property.
        /// Flags:              none
        /// Default Value:      null
        /// </summary>
        public static readonly DependencyProperty DayTemplateProperty =
                DependencyProperty.Register(
                        "DayTemplate",
                        typeof(DataTemplate),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                (DataTemplate)null,
                                new PropertyChangedCallback(OnDayTemplateChanged)));

        /// <summary>
        /// DayTemplate is the template that describes how to convert Items into UI elements.
        /// </summary>
        /// <remarks>
        /// DayTemplate must bind with Date, so it can be updated when scroll month
        /// here is a sample:
        /// &lt;DataTemplate x:Key="dayTemplate"&gt;
        ///     &lt;TextBlock Text="{Binding Date}"/&gt;
        /// &lt;/DataTemplate&gt;
        /// </remarks>
        public DataTemplate DayTemplate
        {
            get { return (DataTemplate)GetValue(DayTemplateProperty); }
            set { SetValue(DayTemplateProperty, value); }
        }

        /// <summary>
        /// Called when DayTemplateProperty is invalidated on "d."
        /// </summary>
        private static void OnDayTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar mcc = (MonthCalendar)d;
            mcc.RefreshDayTemplate();
        }

        /// <summary>
        /// The DependencyProperty for the DayTemplateSelector property.
        /// Flags:              none
        /// Default Value:      null
        /// </summary>
        public static readonly DependencyProperty DayTemplateSelectorProperty =
                DependencyProperty.Register(
                        "DayTemplateSelector",
                        typeof(DataTemplateSelector),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                (DataTemplateSelector)null,
                                new PropertyChangedCallback(OnDayTemplateSelectorChanged)));

        /// <summary>
        /// DayTemplateSelector allows the app writer to provide custom template selection logic
        /// for a template to apply to each item.
        /// </summary>
        public DataTemplateSelector DayTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DayTemplateSelectorProperty); }
            set { SetValue(DayTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Called when DayTemplateSelectorProperty is invalidated on "d."
        /// </summary>
        private static void OnDayTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar mcc = (MonthCalendar)d;
            mcc.RefreshDayTemplate();
        }

        /// <summary>
        /// The DependencyProperty for the DayContainerStyle property.
        /// Flags:              none
        /// Default Value:      null
        /// </summary>
        public static readonly DependencyProperty DayContainerStyleProperty =
                DependencyProperty.Register(
                        "DayContainerStyle",
                        typeof(Style),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                (Style)null,
                                new PropertyChangedCallback(OnDayContainerStyleChanged)));

        /// <summary>
        /// 
        /// </summary>
        public Style DayContainerStyle
        {
            get { return (Style)GetValue(DayContainerStyleProperty); }
            set { SetValue(DayContainerStyleProperty, value); }
        }

        /// <summary>
        /// Called when DayContainerStyleProperty is invalidated on "d."
        /// </summary>
        private static void OnDayContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MonthCalendar)d).RefreshDayTemplate();
        }

        /// <summary>
        ///     The DependencyProperty for the DayContainerStyleSelector property.
        ///     Flags:              none
        ///     Default Value:      null
        /// </summary>
        public static readonly DependencyProperty DayContainerStyleSelectorProperty =
                DependencyProperty.Register(
                        "DayContainerStyleSelector",
                        typeof(StyleSelector),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                (StyleSelector)null,
                                new PropertyChangedCallback(OnDayContainerStyleChanged)));

        /// <summary>
        ///     DayContainerStyleSelector allows the application writer to provide custom logic
        ///     to choose the style to apply to each generated day element.
        /// </summary>
        /// <remarks>
        ///     This property is ignored if <seealso cref="DayContainerStyle"/> is set.
        /// </remarks>
        public StyleSelector DayContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(DayContainerStyleSelectorProperty); }
            set { SetValue(DayContainerStyleSelectorProperty, value); }
        }

        #endregion

        #region TitleStyle/DayHeaderStyle/WeekNumberStyle

        /// <summary>
        /// The DependencyProperty for the TitleStyle property.
        /// Flags:              none
        /// Default Value:      null
        /// </summary>
        public static readonly DependencyProperty TitleStyleProperty =
                DependencyProperty.Register(
                        "TitleStyle",
                        typeof(Style),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                (Style)null));

        /// <summary>
        /// TitleStyle property
        /// </summary>
        public Style TitleStyle
        {
            get { return (Style)GetValue(TitleStyleProperty); }
            set { SetValue(TitleStyleProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for the DayHeaderStyle property.
        /// Flags:              none
        /// Default Value:      null
        /// </summary>
        public static readonly DependencyProperty DayHeaderStyleProperty =
                DependencyProperty.Register(
                        "DayHeaderStyle",
                        typeof(Style),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                (Style)null));

        /// <summary>
        /// DayHeaderStyle property
        /// </summary>
        public Style DayHeaderStyle
        {
            get { return (Style)GetValue(DayHeaderStyleProperty); }
            set { SetValue(DayHeaderStyleProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for the WeekNumberStyle property.
        /// Flags:              none
        /// Default Value:      null
        /// </summary>
        public static readonly DependencyProperty WeekNumberStyleProperty =
                DependencyProperty.Register(
                        "WeekNumberStyle",
                        typeof(Style),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                (Style)null));

        /// <summary>
        /// WeekNumberStyle property
        /// </summary>
        public Style WeekNumberStyle
        {
            get { return (Style)GetValue(WeekNumberStyleProperty); }
            set { SetValue(WeekNumberStyleProperty, value); }
        }

        #endregion

        #region Previous/NextButtonStyle

        /// <summary>
        /// The DependencyProperty for the PreviousButtonStyle property.
        /// Flags:              none
        /// Default Value:      null
        /// </summary>
        public static readonly DependencyProperty PreviousButtonStyleProperty =
                DependencyProperty.Register(
                        "PreviousButtonStyle",
                        typeof(Style),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                (Style)null, new PropertyChangedCallback(OnPreviousButtonStyleChanged)));

        /// <summary>
        /// PreviousButtonStyle property
        /// </summary>
        public Style PreviousButtonStyle
        {
            get { return (Style)GetValue(PreviousButtonStyleProperty); }
            set { SetValue(PreviousButtonStyleProperty, value); }
        }

        private static void OnPreviousButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MonthCalendar)d).RefreshPreviousButtonStyle();
        }

        /// <summary>
        /// The DependencyProperty for the NextButtonStyle property.
        /// Flags:              none
        /// Default Value:      null
        /// </summary>
        public static readonly DependencyProperty NextButtonStyleProperty =
                DependencyProperty.Register(
                        "NextButtonStyle",
                        typeof(Style),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                (Style)null, new PropertyChangedCallback(OnNextButtonStyleChanged)));

        /// <summary>
        /// NextButtonStyle property
        /// </summary>
        public Style NextButtonStyle
        {
            get { return (Style)GetValue(NextButtonStyleProperty); }
            set { SetValue(NextButtonStyleProperty, value); }
        }

        private static void OnNextButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MonthCalendar)d).RefreshNextButtonStyle();
        }

        #endregion

        #region DataSource/DataSourceDataPath/XPath

        /// <summary>
        /// The DependencyProperty for DataSource property
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty
            = DependencyProperty.Register("DataSource", typeof(IEnumerable), typeof(MonthCalendar),
                                          new FrameworkPropertyMetadata((IEnumerable)null,
                                                                new PropertyChangedCallback(OnDataSourceChanged)));

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable DataSource
        {
            get { return (IEnumerable)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar mcc = (MonthCalendar)d;
            
            // WeakEventManager is used here to avoid a potential memory leak
            // Please see the comments in WeakEventManager/WeakEventManagerTemplate for more details
            if (e.OldValue is INotifyCollectionChanged)
            {
                CollectionChangedEventManager.RemoveListener((INotifyCollectionChanged)e.OldValue, mcc);
            }
            if (e.NewValue is INotifyCollectionChanged)
            {
                CollectionChangedEventManager.AddListener((INotifyCollectionChanged)e.NewValue, mcc);
            }

            mcc.UpdateDataSource();
        }

        /// <summary>
        ///     The DependencyProperty for the DataSourceDatePath property.
        ///     Flags:              none
        ///     Default Value:      null
        /// </summary>
        public static readonly DependencyProperty DataSourceDatePathProperty =
                DependencyProperty.Register(
                        "DataSourceDatePath",
                        typeof(PropertyPath),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                (PropertyPath)null,
                                new PropertyChangedCallback(OnDataSourceDatePathChanged)));

        /// <summary>
        /// The source path (for CLR bindings).which is used to get the date property from each item of DataSource
        /// </summary>
        public PropertyPath DataSourceDatePath
        {
            get { return (PropertyPath)GetValue(DataSourceDatePathProperty); }
            set { SetValue(DataSourceDatePathProperty, value); }
        }

        private static void OnDataSourceDatePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MonthCalendar)d).UpdateDataSource();
        }

        /// <summary>
        ///     The DependencyProperty for the DataSourceDateXPath property.
        ///     Flags:              none
        ///     Default Value:      null
        /// </summary>
        public static readonly DependencyProperty DataSourceDateXPathProperty =
                DependencyProperty.Register(
                        "DataSourceDateXPath",
                        typeof(string),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                string.Empty,
                                new PropertyChangedCallback(OnDataSourceDatePathChanged)));

        /// <summary>
        /// The XPath path (for XML bindings).which is used to get the date property from each item of DataSource
        /// </summary>
        public string DataSourceDateXPath
        {
            get { return (string)GetValue(DataSourceDateXPathProperty); }
            set { SetValue(DataSourceDateXPathProperty, value); }
        }

        #endregion

        internal bool IsRecentInputDeviceKeyboard { get; set; }
        #endregion

        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// Raise VisibleMonthChanged event.
        /// </summary>
        /// <param name="e">RoutedPropertyChangedEventArgs contains the old and new value.</param>
        protected virtual void OnVisibleMonthChanged(RoutedPropertyChangedEventArgs<DateTime> e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// Raise DateSelectionChanged event.
        /// </summary>
        protected virtual void OnDateSelectionChanged(DateSelectionChangedEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// This is the method that responds to the PreviewKeyDown event.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        /// <remarks>
        /// Override OnPreviewKeyDown isn't recommended for Control Author, it's reserved for customer
        /// Because MonthCalenarContainer already handles the PageUp/PageDown/Home/End, we have to use Preview here.
        /// </remarks>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            IsRecentInputDeviceKeyboard = true;

            switch (e.Key)
            {
                case Key.PageUp:
                case Key.PageDown:
                    {
                        e.Handled = true;
                        if (((Keyboard.Modifiers & ModifierKeys.Control) != (ModifierKeys.Control))
                            && ((Keyboard.Modifiers & ModifierKeys.Shift) != (ModifierKeys.Shift)))
                        {
                            if (e.Key == Key.PageUp)
                            {
                                PreviousCommand.Execute(null, this);
                            }
                            else
                            {
                                NextCommand.Execute(null, this);
                            }
                        }
                        break;
                    }
                case Key.Home:
                case Key.End:
                    //Find the first/last focusable date as target when Home/End is pressed
                    DateTime? targetDate = null;
                    DateTime firstDate = (FirstDate > MinDate.Date) ? FirstDate : MinDate;
                    DateTime lastDate = (LastDate < MaxDate) ? LastDate.Date : MaxDate;
                    if (e.Key == Key.Home)
                    {
                        targetDate = FindFocusableDate(firstDate, lastDate, true);
                    }
                    else
                    {
                        targetDate = FindFocusableDate(firstDate, lastDate, false);
                    }
                    Debug.Assert(targetDate.HasValue);

                    if (((Keyboard.Modifiers & ModifierKeys.Control) != (ModifierKeys.Control))
                        && ((Keyboard.Modifiers & ModifierKeys.Shift) != (ModifierKeys.Shift)))
                    {
                        SelectionChange.SelectJustThisDate(targetDate, true, true);
                    }
                    else
                    {
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == (ModifierKeys.Control))
                        {
                            MonthCalendarItem mci = GetContainerFromDate(targetDate.Value);
                            if (mci != null)
                            {
                                Keyboard.Focus(mci);
                            }
                        }
                        else // Shift is pressed
                        {
                            MonthCalendarItem orignalItem = e.OriginalSource as MonthCalendarItem;
                            CalendarDate cdate = null;
                            if (orignalItem != null)
                            {
                                cdate = _mccContainer.ItemContainerGenerator.ItemFromContainer(orignalItem) as CalendarDate;
                            }

                            if (e.Key == Key.Home)
                            {
                                SetSelectionByRange(targetDate, new DateTime?(cdate.Date));
                            }
                            else
                            {
                                SetSelectionByRange(new DateTime?(cdate.Date), targetDate);
                            }
                        }
                    }
                    e.Handled = true;
                    break;
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            IsRecentInputDeviceKeyboard = false;

            base.OnPreviewMouseDown(e);
        }

        
        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Methods
        //
        //-------------------------------------------------------------------

        #region Private Methods

        /// <summary>
        /// Generate the visible days collection based on the input firstdate, lastdate and firstdayofweek
        /// </summary>
        private ObservableCollection<CalendarDate> CreateVisibleDaysCollection(DateTime firstDate, DateTime lastDate, DayOfWeek firstDayOfWeek)
        {
            DateTime leadingDate = CalendarDataGenerator.CalculateLeadingDate(firstDate, firstDayOfWeek);
            DateTime trailingDate = CalendarDataGenerator.CalculateTrailingDate(firstDate, lastDate, firstDayOfWeek);
            int totalDay = trailingDate.Subtract(leadingDate).Days + 1;

            ObservableCollection<CalendarDate> collection = new ObservableCollection<CalendarDate>();
            for (int i = 0; i < totalDay; ++i)
            {
                CalendarDate cdate = new CalendarDate(leadingDate.AddDays(i));
                cdate.IsOtherMonth = cdate.Date < FirstDate || cdate.Date > LastDate;
                cdate.IsSelectable = MonthCalendarHelper.IsWithinRange(cdate.Date, MinDate, MaxDate);

                collection.Add(cdate);
            }

            return collection;
        }

        /// <summary>
        /// Invalidate the visible days when switch month
        /// </summary>
        /// <param name="scrollChange"></param>
        private void InvalidateVisibleDays(int scrollChange)
        {
            if (_mccContainer != null)
            {
                ObservableCollection<CalendarDate> newVisibleDays = CreateVisibleDaysCollection(FirstDate, LastDate, FirstDayOfWeek);
                Debug.Assert(newVisibleDays.Count == 42);

                //Clear the selection UI
                SetFlag(Flags.IsChangingSelectorSelection, true);
                try
                {
                    _mccContainer.SelectedItems.Clear();
                }
                finally
                {
                    SetFlag(Flags.IsChangingSelectorSelection, false);
                }

                //Generate new VisibleDays collection and update it
                if (DayTemplateSelector == null)
                {
                    for (int i = 0; i < VisibleDays.Count; ++i)
                    {
                        VisibleDays[i].InternalUpdate(newVisibleDays[i]);
                    }
                }
                else
                {
                    SetFlag(Flags.IsChangingSelectorSelection, true);
                    try
                    {
                        VisibleDays = newVisibleDays;
                        _mccContainer.ItemsSource = VisibleDays;
                    }
                    finally
                    {
                        SetFlag(Flags.IsChangingSelectorSelection, false);
                    }
                }

                RestoreSelection(scrollChange);
            }
            else
            {
                //If selected date is added before UIReady(_mccContainer is null)
                //VisibleDays and SelectedDates must be updated
                VisibleDays = CreateVisibleDaysCollection(FirstDate, LastDate, FirstDayOfWeek);
                RestoreSelection(scrollChange);
            }

            UpdateDataSourceToCalendarDates();
        }

        /// <summary>
        /// Walk to visual tree to find the MonthCalendarContainer. Set the GroupFactory and GroupPanel.
        /// Register the selection change event if it is a Selector.
        /// </summary>
        private void AttachToVisualTree()
        {
            DetachFromVisualTree();

            // Walk the visual tree to find the MonthCalendarContainer.
            _mccContainer = GetTemplateChild(c_VisibleDaysHostTemplateName) as MonthCalendarContainer;

            if (_mccContainer != null)
            {
                _mccContainer.ItemsSource = VisibleDays;

                RefreshDayTemplate();

                _mccContainer.LayoutUpdated += new EventHandler(OnContainerLayoutUpdated);
                _mccContainer.SelectionChanged += new SelectionChangedEventHandler(OnContainerSelectionChanged);
                //If MonthCalendar's visualtree is restyled, set this flag to true to update the SelectedDates on new UI
                SetFlag(Flags.IsVisualTreeUpdated, true);
            }

            RefreshPreviousButtonStyle();
            RefreshNextButtonStyle();
        }

        /// <summary>
        /// Clear our layout-specific data, and detach our current itemscontrol from monthcalendar
        /// </summary>
        private void DetachFromVisualTree()
        {
            if (_mccContainer != null)
            {
                _mccContainer.LayoutUpdated -= new EventHandler(OnContainerLayoutUpdated);
                _mccContainer.SelectionChanged -= new SelectionChangedEventHandler(OnContainerSelectionChanged);
                _mccContainer = null;
            }
        }

        /// <summary>
        /// Scroll the current visible month of MothCalendar based on delta and direction
        /// </summary>
        /// <param name="direction">1:increase; -1: decrease</param>
        /// <param name="delta">how many months should be scrolled, 0 means use the default value</param>
        private void ScrollVisibleMonth(int direction, int delta)
        {
            //NOTE:
            // To read the graph below, please use fixed width font.
            //
            // Date range:                              Min                               Max
            //                                           |--------------------------------|
            //                                           .                                .
            // Valid cases:                              .                                .
            //                                           .                                .
            // Case 1:                                   .     [..................]       .
            //                                           .  FstVsM                        .
            //                                           .                                .
            // Case 2:                                   [..................]             .
            //                                        FstVsM                              .
            //                                           .                                .
            // Case 3:                                   .             [..................]
            //                                           .          FstVsM                .
            //                                           .                                .
            // Case 4:                                   [.........................................]
            //                                        FstVsM                              .
            //                                           .                                .
            //                                           .                                .
            // Case 5:                                   .             [.......................]
            //                                           .           FstVsM               .
            //                                           .                                .
            // Valid initial but no returnable cases:    .                                .
            //                                           .                                .
            // Case 6:                      [..................]                          .
            //                           FstVsM                                           .
            //                                           .                                .
            // Case 7:                                   .                                .       [..................]
            //                                           .                                .    FstVsM
            //
            // Invalid case(s)
            //                                           .                                .
            // Case 8:                          [.........................................]
            //                               FstVsM

            int monthCount = 1;

            if (delta == 0)
            {
                delta = monthCount;
            }

            DateTime firstMonth = VisibleMonth;
            bool checkFirstMonth = false;

            // Scroll to the previous page
            if (direction == -1)
            {
                if (MonthCalendarHelper.CompareYearMonth(VisibleMonth, MinDate) > 0) // case 1/3/5
                {
                    checkFirstMonth = true;
                    try
                    {
                        firstMonth = VisibleMonth.AddMonths(delta * direction);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        firstMonth = MinDate; // case 2/4
                    }
                }
            }
            // Scroll to the next page
            else
            {
                try
                {
                    if (MonthCalendarHelper.CompareYearMonth(LastDate, MaxDate) < 0) // case 1-4
                    {
                        firstMonth = VisibleMonth.AddMonths(delta * direction);
                        DateTime lastMonth = firstMonth.AddMonths(monthCount - 1);

                        // if lastMonth is greater than MaxDate, scroll back to the appropriate position
                        if (MonthCalendarHelper.CompareYearMonth(lastMonth, MaxDate) > 0) // case 5
                        {
                            checkFirstMonth = true;
                            firstMonth = MaxDate.AddMonths(-(monthCount - 1)); // case 3
                        }
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    checkFirstMonth = true;
                    firstMonth = MaxDate.AddMonths(-(monthCount - 1)); // case 3
                }
            }

            // check in case the firstMonth is less than MinDate
            if (checkFirstMonth && MonthCalendarHelper.CompareYearMonth(firstMonth, MinDate) < 0) // case 8
            {
                firstMonth = MinDate; // change to case 4
            }

            VisibleMonth = firstMonth;
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

        /// <summary>
        /// True if the date is leading or traing day
        /// </summary>
        private bool IsLeadingTrailingDay(DateTime date)
        {
            Debug.Assert(date >= FirstVisibleDate && date <= LastVisibleDate);
            return !MonthCalendarHelper.IsWithinRange(date, FirstDate, LastDate);
        }

        /// <summary>
        /// Update the SelectedDates to UI if those dates are added before UI ready
        /// </summary>
        private void OnContainerLayoutUpdated(object sender, EventArgs e)
        {
            if (GetFlag(Flags.IsVisualTreeUpdated))
            {
                SetFlag(Flags.IsVisualTreeUpdated, false);

                if (SelectedDates.Count > 0)
                {
                    SetFlag(Flags.IsChangingSelectorSelection, true);
                    try
                    {
                        foreach (DateTime dt in SelectedDates)
                        {
                            _mccContainer.SelectedItems.Add(GetCalendarDateByDate(dt));
                        }
                    }
                    finally
                    {
                        SetFlag(Flags.IsChangingSelectorSelection, false);
                    }
                }
            }
        }

        /// <summary>
        /// Refresh the ItemTemplate/ItemTemplateSelector/ItemContainerStyle if DayTemplate/DayTemplateSelecotr/DayContainerStyle is set
        /// </summary>
        private void RefreshDayTemplate()
        {
            if (_mccContainer != null)
            {
                //if both DayTemplate/Selector is null, use the default DayTemplate
                if (DayTemplate == null && DayTemplateSelector == null)
                {
                    if (s_MonthCalendarDayTemplate == null)
                    {
                        s_MonthCalendarDayTemplate = new DataTemplate();
                        FrameworkElementFactory txt = new FrameworkElementFactory(typeof(TextBlock));
                        txt.SetBinding(TextBlock.TextProperty, new Binding("Date.Day"));
                        s_MonthCalendarDayTemplate.VisualTree = txt;
                    }

                    _mccContainer.ItemTemplateSelector = null;
                    _mccContainer.ItemTemplate = s_MonthCalendarDayTemplate;
                }
                else
                {
                    _mccContainer.ItemTemplate = DayTemplate;
                    _mccContainer.ItemTemplateSelector = DayTemplateSelector;
                }

                SetFlag(Flags.IsVisibleDaysUpdated, true);
            }
        }

        private void RefreshPreviousButtonStyle()
        {
            ButtonBase previousButton = GetTemplateChild(c_PreviousButtonName) as ButtonBase;
            if (previousButton != null)
            {
                if (PreviousButtonStyle == null)
                {
                    if (_defaultPreviousButtonStyle == null)
                    {
                        _defaultPreviousButtonStyle = FindResource(new ComponentResourceKey(typeof(MonthCalendar), "PreviousButtonStyleKey")) as Style;
                    }
                    previousButton.Style = _defaultPreviousButtonStyle;
                }
                else
                {
                    previousButton.Style = PreviousButtonStyle;
                }
            }
        }

        private void RefreshNextButtonStyle()
        {
            ButtonBase nextButton = GetTemplateChild(c_NextButtonName) as ButtonBase;
            if (nextButton != null)
            {
                if (NextButtonStyle == null)
                {
                    if (_defaultNextButtonStyle == null)
                    {
                        _defaultNextButtonStyle = FindResource(new ComponentResourceKey(typeof(MonthCalendar), "NextButtonStyleKey")) as Style;
                    }
                    nextButton.Style = _defaultNextButtonStyle;
                }
                else
                {
                    nextButton.Style = NextButtonStyle;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction">true: start &lt;= end; false: start &gt;= end</param>
        /// <returns>Focusable date, null if no focusable date exists</returns>
        private DateTime? FindFocusableDate(DateTime start, DateTime end, bool direction)
        {
            DateTime date = direction ? start : end;

            bool gotFocusableDate = false;
            MonthCalendarItem mci = null;
            TimeSpan tspan = end - start;
            for (int i = 0; i < Math.Abs(tspan.Days); ++i)
            {
                mci = GetContainerFromDate(date);
                Debug.Assert(mci != null);
                if (IsFocusable(mci))
                {
                    gotFocusableDate = true;
                    break;
                }
                date = date.AddDays(direction ? 1 : -1);
            }

            return gotFocusableDate ? new DateTime?(date) : null;
        }

        /// <summary>
        /// True if the element can be focused
        /// </summary>
        private bool IsFocusable(FrameworkElement fe)
        {
            return fe != null && fe.Focusable && (bool)fe.GetValue(IsTabStopProperty) && fe.IsEnabled && fe.Visibility == Visibility.Visible;
        }

        /// <summary>
        /// Select the dates between start and end. If one of them is null, only select the other one
        /// </summary>
        private void SetSelectionByRange(DateTime? start, DateTime? end)
        {
            if (SelectedDates.Count == MaxSelectionCount)
            {
                return;
            }

            if (!start.HasValue)
            {
                SelectedDate = end;
            }
            else if (!end.HasValue)
            {
                SelectedDate = start;
            }
            else
            {
                SelectionChange.Begin();
                bool succeeded = false;
                try
                {
                    foreach (DateTime dt in SelectedDates)
                    {
                        if (!MonthCalendarHelper.IsWithinRange(dt, start.Value, end.Value))
                        {
                            SelectionChange.Unselect(dt);
                        }
                    }

                    DateTime date = start.Value;
                    while (date <= end.Value && SelectedDates.Count < MaxSelectionCount)
                    {
                        if (!SelectedDates.Contains(date))
                        {
                            SelectionChange.Select(date, true);
                        }
                        date = date.AddDays(1);
                    }

                    SelectionChange.End(true, true);
                    succeeded = true;
                }
                finally
                {
                    if (!succeeded)
                    {
                        SelectionChange.Cancel();
                    }
                }
            }
        }

        #region Selection

        /// <summary>
        /// Raise the SelectionChanged event.
        /// </summary>
        private void InvokeDateSelectedChangedEvent(List<DateTime> unselected, List<DateTime> selected)
        {
            DateSelectionChangedEventArgs args = new DateSelectionChangedEventArgs(unselected, selected);
            args.Source = this;
            OnDateSelectionChanged(args);
        }

        /// <summary>
        /// Get the CalendarDate by date
        /// </summary>
        /// <returns>null if the date exceeds First/LastVisibleDate</returns>
        private CalendarDate GetCalendarDateByDate(DateTime date)
        {
            TimeSpan ts = date - FirstVisibleDate;
            return VisibleDays.Count > ts.Days && ts.Days >= 0 ? VisibleDays[ts.Days] : null;
        }

        /// <summary>
        /// Handle SelectedDates collection changed event
        /// </summary>
        private void OnSelectedDatesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectionChange.IsActive)
            {
                return;
            }

            SelectionChange.Begin();
            bool succeeded = false;
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems.Count != 1)
                            throw new NotSupportedException(c_exRangeActionsNotSupported);
                        SelectionChange.Select((DateTime)e.NewItems[0], false /* assumeInItemsCollection */);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems.Count != 1)
                            throw new NotSupportedException(c_exRangeActionsNotSupported);
                        SelectionChange.Unselect((DateTime)e.OldItems[0]);
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        SelectionChange.Clear();
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        if (e.NewItems.Count != 1 || e.OldItems.Count != 1)
                            throw new NotSupportedException(c_exRangeActionsNotSupported);
                        SelectionChange.Unselect((DateTime)e.OldItems[0]);
                        SelectionChange.Select((DateTime)e.NewItems[0], false /* assumeInItemsCollection */);
                        break;

                    default:
                        throw new NotSupportedException("Unexpected collection change action '" + e.Action.ToString() + "'.");
                }

                SelectionChange.End(true, false);

                succeeded = true;
            }
            finally
            {
                if (!succeeded)
                {
                    SelectionChange.Cancel();
                }
            }
        }

        /// <summary>
        /// Update the selected dates status when user changes it by UI
        /// </summary>
        private void OnContainerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GetFlag(Flags.IsChangingSelectorSelection) || SelectionChange.IsActive
                || _mccContainer.TemplatedParent == null || e.OriginalSource != _mccContainer)
            {
                return;
            }
            
            SetFlag(Flags.IsChangingSelectorSelection, true); //temporary disable the listener

            try
            {
                //Convert e.RemovedItems to strong type removedDates
                List<DateTime> removedDates = new List<DateTime>(e.RemovedItems.Count);
                foreach (CalendarDate cdate in e.RemovedItems)
                {
                    removedDates.Add(cdate.Date);
                }

                //Click leading/trailing date to switch the month
                if (_mccContainer.SelectedItems.Count == 1)
                {
                    Debug.Assert(_mccContainer.SelectedItem is CalendarDate);
                    CalendarDate cdate = _mccContainer.SelectedItem as CalendarDate;
                    Debug.Assert(cdate != null);
                    if (IsLeadingTrailingDay(cdate.Date))
                    {
                        SelectionChange.SelectJustThisDate(cdate.Date, false, true);
                        return;
                    }
                }

                //Convert e.AddedItems to strong type addedDates
                List<DateTime> addedDates = new List<DateTime>(e.AddedItems.Count);
                foreach (CalendarDate cdate in e.AddedItems)
                {
                    addedDates.Add(cdate.Date);
                }

                //If addedDates.Count+SelectedDates.Count-removedDates.Count > MaxSelectionCount
                //The redundant selected dates will be removed from addedDates, 
                //The purpose is to restrict the count of addedItems, not to unselect the existing dates
                int selectedCount = addedDates.Count + SelectedDates.Count - removedDates.Count;
                int unselectCount = selectedCount - MaxSelectionCount;

                if (unselectCount > 0)
                {
                    List<DateTime> newAddedDates = new List<DateTime>(addedDates);
                    int count = addedDates.Count;
                    for (int i = count - 1; i >= count - unselectCount; i--)
                    {
                        MonthCalendarItem mci = _mccContainer.ItemContainerGenerator.ContainerFromItem(e.AddedItems[i]) as MonthCalendarItem;
                        Debug.Assert(mci != null);
                        mci.IsSelected = false;
                        newAddedDates.Remove(addedDates[i]);
                    }
                    addedDates = new List<DateTime>(newAddedDates);
                }

                // check if still contains change. If yes, raise event.
                if (removedDates.Count > 0 || addedDates.Count > 0)
                {
                    SelectionChange.Begin();
                    bool succeeded = false;
                    try
                    {
                        foreach (DateTime dt in removedDates)
                        {
                            SelectionChange.Unselect(dt);
                        }

                        foreach (DateTime dt in addedDates)
                        {
                            SelectionChange.Select(dt, true);
                        }

                        SelectionChange.End(false, true);
                        succeeded = true;
                    }
                    finally
                    {
                        if (!succeeded)
                        {
                            SelectionChange.Cancel();
                        }
                    }
                }

            }
            finally
            {
                SetFlag(Flags.IsChangingSelectorSelection, false); // re-enable the listener
            }
        }

        /// <summary>
        /// Restore the selection UI after switching month
        /// </summary>
        private void RestoreSelection(int scrollChange)
        {
            if (SelectionChange.IsActive || SelectedDates.Count == 0)
            {
                return;
            }

            SelectionChange.Begin();
            bool succeeded = false;
            try
            {
                //Restore the selected dates that are within Max/MinDate, First/LastVisibleDate
                //And unselect the dates that exceeds the range
                foreach (DateTime dt in SelectedDates)
                {
                    if (MonthCalendarHelper.IsWithinRange(dt, FirstVisibleDate, LastVisibleDate)
                        && MonthCalendarHelper.IsWithinRange(dt, MinDate, MaxDate))
                    {
                        SelectionChange.RestoreSelect(dt);
                    }
                    else
                    {
                        SelectionChange.Unselect(dt);
                    }
                }

                SelectionChange.End(true, true);
                succeeded = true;
            }
            finally
            {
                if (!succeeded)
                {
                    SelectionChange.Cancel();
                }
            }
        }

        #endregion

        #region DataSource

        /// <summary>
        /// Update the CalendarDate.Data property when DataSource collection is changed
        /// </summary>
        private void OnDataSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems.Count != 1)
                        throw new NotSupportedException(c_exRangeActionsNotSupported);
                    AddToDictionary(e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count != 1)
                        throw new NotSupportedException(c_exRangeActionsNotSupported);
                    RemoveFromDictionary(e.OldItems[0]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    UpdateDataSource();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems.Count != 1 || e.OldItems.Count != 1)
                        throw new NotSupportedException(c_exRangeActionsNotSupported);
                    RemoveFromDictionary(e.OldItems[0]);
                    AddToDictionary(e.NewItems[0]);
                    break;
            }
        }

        /// <summary>
        /// Add value to data source dictionary
        /// </summary>
        private void AddToDictionary(object value)
        {
            DateTime? key = GetKeyFromDataSourceItem(value);
            if (key.HasValue && !_dictDataSource.ContainsKey(key.Value))
            {
                // Add date/data pair to data source dictionary, if the date is within current display dates, 
                // update CalendarDate.Data immediatelly
                _dictDataSource.Add(key.Value, value);
                if (MonthCalendarHelper.IsWithinRange(key.Value, FirstVisibleDate, LastVisibleDate))
                {
                    CalendarDate cdate = GetCalendarDateByDate(key.Value);
                    cdate.Data = value;
                }
            }
        }

        /// <summary>
        /// Remove value from data source dictionary
        /// </summary>
        private void RemoveFromDictionary(object value)
        {
            DateTime? key = GetKeyFromDataSourceItem(value);
            if (key.HasValue)
            {
                // Remove date/data pair from data source dictionary, if the date is within current display dates, 
                // update CalendarDate.Data to null immediatelly
                _dictDataSource.Remove(key.Value);
                if (MonthCalendarHelper.IsWithinRange(key.Value, FirstVisibleDate, LastVisibleDate))
                {
                    CalendarDate cdate = GetCalendarDateByDate(key.Value);
                    cdate.Data = null;
                }
            }
        }

        private void UpdateDataSource()
        {
            if (_dictDataSource == null)
            {
                _dictDataSource = new Dictionary<DateTime, object>();
            }
            else
            {
                bool bUpdate = _dictDataSource.Count > 0;
                _dictDataSource.Clear();
                if (bUpdate)
                {
                    UpdateDataSourceToCalendarDates();
                }
            }

            // Generate the data source dictionary with DataSource and DataSourceDatePath/XPath,
            // And update the data in the dictionary to the specific date
            if (DataSource != null && (DataSourceDatePath != null || (DataSourceDateXPath != null && DataSourceDateXPath.Length != 0)))
            {
                foreach (object obj in DataSource)
                {
                    DateTime? key = GetKeyFromDataSourceItem(obj);
                    if (key.HasValue && !_dictDataSource.ContainsKey(key.Value))
                    {
                        _dictDataSource.Add(key.Value, obj);
                    }
                }

                UpdateDataSourceToCalendarDates();
            }
        }

        /// <summary>
        /// Update the data source data to the specific calendar dates
        /// </summary>
        private void UpdateDataSourceToCalendarDates()
        {
            if (_dictDataSource == null) return;

            foreach (CalendarDate cdate in VisibleDays)
            {
                if (_dictDataSource.ContainsKey(cdate.Date))
                {
                    cdate.Data = _dictDataSource[cdate.Date];
                }
                else
                {
                    cdate.Data = null;
                }
            }
        }

        /// <summary>
        /// Get the DateTime value from the value with DataSourceDatePath/XPath
        /// the value works as a key for data source dictionary
        /// </summary>
        private DateTime? GetKeyFromDataSourceItem(object value)
        {
            if (_dummyItem == null)
            {
                _dummyItem = new FrameworkElement();
            }

            Binding bind = new Binding();
            if (DataSourceDatePath != null)//TODO:bug#1662127
            {
                bind.Path = DataSourceDatePath;
            }
            bind.XPath = DataSourceDateXPath;
            bind.Source = value;
            _dummyItem.SetBinding(FrameworkElement.TagProperty, bind);

            object tag = _dummyItem.Tag;
            if (_dummyItem.Tag is XmlAttribute)
            {
                tag = ((XmlAttribute)_dummyItem.Tag).Value;
            }
            else if (_dummyItem.Tag is XmlElement)
            {
                tag = ((XmlElement)_dummyItem.Tag).InnerText;
            }

            DateTime? key = new DateTime?();
            if (tag is string)
            {
                try
                {
                    key = DateTime.Parse((string)tag, Language.GetSpecificCulture().DateTimeFormat);
                }
                catch (FormatException)
                {
                }
            }
            else if (tag is DateTime)
            {
                key = (DateTime)_dummyItem.Tag;
            }

            BindingOperations.ClearBinding(_dummyItem, FrameworkElement.TagProperty);

            return key;
        }

        /// <summary>
        /// Handle events from the centralized event table
        /// </summary>
        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(CollectionChangedEventManager))
            {
                OnDataSourceCollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
            }
            else
            {
                return false;
            }
            return true;
        }

        #endregion

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Fields
        //
        //-------------------------------------------------------------------

        #region Private Fields

        private MonthCalendarContainer _mccContainer;

        private DateTime FirstVisibleDate { get { return VisibleDays[0].Date; } }

        private DateTime LastVisibleDate { get { return VisibleDays[VisibleDays.Count - 1].Date; } }

        /// <summary>
        /// first day of the first currently visible month
        /// </summary>
        private DateTime FirstDate
        {
            get { return new DateTime(VisibleMonth.Year, VisibleMonth.Month, 1); }
        }

        /// <summary>
        /// last day of the last currently visible month
        /// </summary>
        private DateTime LastDate
        {
            get
            {
                try
                {
                    return FirstDate.AddMonths(1).AddMilliseconds(-1);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return DateTime.MaxValue;
                }
            }
        }

        private ObservableCollection<CalendarDate> VisibleDays
        {
            get 
            {
                if (_visibleDays == null)
                {
                    _visibleDays = CreateVisibleDaysCollection(FirstDate, LastDate, FirstDayOfWeek);
                }
                return _visibleDays;
            }
            set 
            {
                _visibleDays = value;
            }
        }

        private ObservableCollection<CalendarDate> _visibleDays;

        /// <summary>
        /// Allows batch processing of selection changes so that only one SelectionChanged event is fired
        /// </summary>
        private SelectionChanger SelectionChange
        {
            get
            {
                if (_selectionChangeInstance == null)
                {
                    _selectionChangeInstance = new SelectionChanger(this);
                }
                return _selectionChangeInstance;
            }
        }

        private SelectionChanger _selectionChangeInstance;

        private DateTime? InternalSelectedDate
        {
            get { return InternalSelectedDates.Count == 0 ? null : new DateTime?(InternalSelectedDates[0]); }
        }

        private List<DateTime> InternalSelectedDates
        {
            get
            {
                if (_internalSelectedDates == null)
                {
                    _internalSelectedDates = new List<DateTime>();
                }

                return _internalSelectedDates;
            }
        }

        private List<DateTime> _internalSelectedDates;

        private ObservableCollection<DateTime> _selectedDates;

        private Dictionary<DateTime, object> _dictDataSource; // DataSource dictionary
        private FrameworkElement _dummyItem; // a dummy item works with binding to get the DateTime from DataSource

        [Flags]
        private enum Flags
        {
            // True when InternalVisibleDays is updated
            // This flag is used to know when we should hide redundant dates
            IsVisibleDaysUpdated = 0x00000001, 
            // True to disable the selector selectionchanged listener, False to enable
            IsChangingSelectorSelection = 0x00000002,
            // True when the SelectedDate property is updated internally
            IsInternalChangingSelectedDate = 0x00000004,
            // True when the month is switching
            IsSwitchingMonth = 0x00000008,
            //True when MonthCalendar is firstly created, or its visualtree is restyled
            IsVisualTreeUpdated = 0x00000010,
        }

        private Flags _flags;

        private Style _defaultPreviousButtonStyle, _defaultNextButtonStyle;
        private static DataTemplate s_MonthCalendarDayTemplate;

        // Part name used in the style. The class TemplatePartAttribute should use the same name
        private const string c_VisibleDaysHostTemplateName = "PART_VisibleDaysHost";
        private const string c_PreviousButtonName = "PART_PreviousButton";
        private const string c_NextButtonName = "PART_NextButton";

        private const string c_exRangeActionsNotSupported = "Range actions are not supported.";

        #endregion

        #region SelectionChanger

        /// <summary>
        /// Helper class for selection change batching.
        /// </summary>
        private class SelectionChanger
        {
            /// <summary>
            /// Create a new SelectionChanger -- there should only be one instance per MonthCalendar.
            /// </summary>
            internal SelectionChanger(MonthCalendar mcc)
            {
                Debug.Assert(mcc != null);
                _owner = mcc;
                InitFlags();
                _toSelect = new List<DateTime>(1);
                _toRestoreSelect = new List<DateTime>(1);
                _toUnselect = new List<DateTime>(1);
            }

            #region Internal Properties/Methods

            /// <summary>
            /// True if there is a SelectionChange currently in progress.
            /// </summary>
            internal bool IsActive
            {
                get { return _isActive; }
            }

            /// <summary>
            /// Begin tracking selection changes.
            /// </summary>
            internal void Begin()
            {
                Debug.Assert(_owner.CheckAccess());
                Debug.Assert(!_isActive, "Cannot begin a new SelectionChange when another one is active.");

                _isActive = true;
                _toSelect.Clear();
                _toRestoreSelect.Clear();
                _toUnselect.Clear();
            }

            /// <summary>
            /// Cancels the currently active SelectionChanger.
            /// </summary>
            internal void Cancel()
            {
                Debug.Assert(_owner.CheckAccess());

                InitFlags();
                if (_toSelect.Count > 0)
                {
                    _toSelect.Clear();
                }
                if (_toRestoreSelect.Count > 0)
                {
                    _toRestoreSelect.Clear();
                }
                if (_toUnselect.Count > 0)
                {
                    _toUnselect.Clear();
                }
            }

            /// <summary>
            /// Queue something to be added to the selection.  Does nothing if the date is already selected.
            /// </summary>
            internal void Select(DateTime date, bool assumeInCollection)
            {
                Debug.Assert(_owner.CheckAccess());
                Debug.Assert(_isActive, "No SelectionChange is active.");

                // To support Unselect(date) / Select(date) where date is already selected.
                if (_toUnselect.Remove(date)) return;

                //if date is already in select queue, ignore this operation
                if (_toSelect.Contains(date)) return;

                // Disallow selecting the date that exceeds Max/MinDate
                if (!IsSelectable(date)) return;

                if (!_owner.InternalSelectedDates.Contains(date))
                {
                    if (_owner.SelectedDates.Count == 0)
                    {
                        //If new selected date exceeds the First/LastDate when SelectedDates.Count is 0, switch month will be triggered
                        _isSwitchingMonthTriggered = !MonthCalendarHelper.IsWithinRange(date, _owner.FirstDate, _owner.LastDate);
                    }
                    else
                    {
                        //If new selected date exceeds the First/LastVisibleDate, switch month will be triggered
                        _isSwitchingMonthTriggered = !MonthCalendarHelper.IsWithinRange(date, _owner.FirstVisibleDate, _owner.LastVisibleDate);
                    }
                    _toSelect.Add(date);
                }
            }

            /// <summary>
            /// Restore the selected dates after switching month
            /// </summary>
            internal void RestoreSelect(DateTime date)
            {
                Debug.Assert(_owner.CheckAccess());

                _toRestoreSelect.Add(date);
            }

            /// <summary>
            /// Queue something to be removed from the selection.  Does nothing if the date is not already selected.
            /// </summary>
            internal void Unselect(DateTime date)
            {
                Debug.Assert(_owner.CheckAccess());
                Debug.Assert(_isActive, "No SelectionChange is active.");

                // To support Select(date) / Unselect(date) where date is not already selected.
                if (_toSelect.Remove(date)) return;

                //if date is already in unselect queue, ignore this operation
                if (_toUnselect.Contains(date)) return;

                if (_owner.InternalSelectedDates.Contains(date))
                {
                    _toUnselect.Add(date);
                }
            }

            internal void Clear()
            {
                Debug.Assert(_owner.CheckAccess());

                _toUnselect = new List<DateTime>(_owner.InternalSelectedDates);
            }

            /// <summary>
            /// Select just this date; all other dates in SelectedDates will be removed.
            /// </summary>
            internal void SelectJustThisDate(DateTime? date, bool isSynchronizedWithUI, bool isSynchronizedWithCollection)
            {
                Begin();
                Clear();  //Copy SelectedDates to _toUnselect

                try
                {
                    if (date.HasValue)
                    {
                        if (_toUnselect.Contains(date.Value))
                        {
                            _toUnselect.Remove(date.Value);
                        }
                        else
                        {
                            Select(date.Value, false);
                        }
                    }
                }
                finally
                {
                    End(isSynchronizedWithUI, isSynchronizedWithCollection);
                }
            }

            /// <summary>
            /// Commit selection changes.
            /// </summary>
            /// <param name="isSynchronizedWithUI">
            /// True if the SelectionChange need to sync up with the UI
            /// </param>
            /// <param name="isSynchronizedWithCollection">
            /// True if the SelectionChange need to sync up with the SelectedDates property
            /// </param>
            internal void End(bool isSynchronizedWithUI, bool isSynchronizedWithCollection)
            {
                Debug.Assert(_owner.CheckAccess());
                Debug.Assert(_isActive, "There must be a selection change active when you call SelectionChange.End()");

                if (_toUnselect.Count > 0 || _toSelect.Count > 0 || _toRestoreSelect.Count > 0)
                {
                    //Update the dates in SelectedDates and InternalSelectedDates
                    foreach (DateTime dt in _toUnselect)
                    {
                        if (isSynchronizedWithCollection)
                        {
                            _owner.SelectedDates.Remove(dt);
                        }

                        _owner.InternalSelectedDates.Remove(dt);
                    }

                    foreach (DateTime dt in _toSelect)
                    {
                        if (isSynchronizedWithCollection)
                        {
                            _owner.SelectedDates.Add(dt);
                        }

                        Debug.Assert(!_owner.InternalSelectedDates.Contains(dt));
                        _owner.InternalSelectedDates.Add(dt);
                    }

                    //If the selection change will cause a month switch, don't synchronize with the UI
                    if (!_isSwitchingMonthTriggered && isSynchronizedWithUI)
                    {
                        SynchronizeWithUI();
                    }
                }

                SynchronizeInternalSelectedDates();

                InitFlags();

                if (_toUnselect.Count > 0 || _toSelect.Count > 0)
                {
                    //internal update SelectedDate property
                    _owner.SetFlag(Flags.IsInternalChangingSelectedDate, true);
                    try
                    {
                        _owner.SelectedDate = _owner.InternalSelectedDate;
                    }
                    finally
                    {
                        _owner.SetFlag(Flags.IsInternalChangingSelectedDate, false);
                    }

                    _owner.InvokeDateSelectedChangedEvent(_toUnselect, _toSelect);
                }

                //If the Selection change isn't caused by a month switching
                //then the following code is to check should we do a month switching or not
                if (!_owner.GetFlag(Flags.IsSwitchingMonth))
                {
                    if (_owner.SelectedDates.Count == 1)
                    {
                        //If only one selected date and it exceeds First/LastDate, switch month
                        if (!MonthCalendarHelper.IsWithinRange(_owner.SelectedDate.Value, _owner.FirstDate, _owner.LastDate))
                        {
                            _owner.VisibleMonth = _owner.SelectedDate.Value;
                        }
                    }
                    else if (_owner.SelectedDates.Count > 1)
                    {
                        //If one date exceeds the First/LastVisibleDate of MonthCalendar, switch month
                        foreach (DateTime dt in _owner.SelectedDates)
                        {
                            if (!MonthCalendarHelper.IsWithinRange(dt, _owner.FirstVisibleDate, _owner.LastVisibleDate))
                            {
                                _owner.VisibleMonth = dt;
                                break;
                            }
                        }
                    }
                }
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Update the selected dates in MonthCalenarContainer
            /// </summary>
            private void SynchronizeWithUI()
            {
                MonthCalendarContainer mccContainer = _owner._mccContainer;
                if (mccContainer != null)
                {
                    foreach (DateTime dt in _toUnselect)
                    {
                        mccContainer.SelectedItems.Remove(_owner.GetCalendarDateByDate(dt));
                    }
                    foreach (DateTime dt in _toSelect)
                    {
                        Debug.Assert(_owner.GetCalendarDateByDate(dt) != null);
                        mccContainer.SelectedItems.Add(_owner.GetCalendarDateByDate(dt));
                    }
                    foreach (DateTime dt in _toRestoreSelect)
                    {
                        Debug.Assert(_owner.GetCalendarDateByDate(dt) != null);
                        mccContainer.SelectedItems.Add(_owner.GetCalendarDateByDate(dt));
                    }

                    //Forward focus to the SelectedDate
                    if (_owner.InternalSelectedDate.HasValue && _owner.IsKeyboardFocusWithin)
                    {
                        _owner.Dispatcher.BeginInvoke(DispatcherPriority.Input,
                            (DispatcherOperationCallback)delegate(object arg)
                            {
                                object[] args = (object[])arg;
                                MonthCalendar mcc = args[0] as MonthCalendar;
                                DateTime date = (DateTime)args[1];
                                MonthCalendarItem focusedItem = mcc.GetContainerFromDate(date);
                                if (focusedItem != null && focusedItem.Focusable && focusedItem.IsEnabled && focusedItem.IsVisible)
                                {
                                    mcc.SetFlag(Flags.IsChangingSelectorSelection, true);
                                    try
                                    {
                                        Keyboard.Focus(focusedItem);

                                        if (mcc.SelectedDates.Count > 1)
                                        {
                                            foreach (DateTime dt in mcc.SelectedDates)
                                            {
                                                if (MonthCalendarHelper.CompareYearMonthDay(dt, date) != 0)
                                                {
                                                    CalendarDate cdate = mcc.GetCalendarDateByDate(dt);
                                                    if (cdate != null && !mcc._mccContainer.SelectedItems.Contains(cdate))
                                                    {
                                                        mcc._mccContainer.SelectedItems.Add(cdate);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        mcc.SetFlag(Flags.IsChangingSelectorSelection, false);
                                    }
                                }
                                else
                                {
                                    mcc._mccContainer.Focus();
                                }
                                return null;
                            },
                            new object[] { _owner, _owner.InternalSelectedDate.Value });
                    }
                }
            }

            /// <summary>
            /// Update the InternalSelectedDates, so SelecteDates == InternalSelectedDates
            /// </summary>
            private void SynchronizeInternalSelectedDates()
            {
                // Dictionary is used for duplicate datetimes in SelectedDates
                Dictionary<DateTime, object> userSelectedDatesTable = new Dictionary<DateTime, object>(_owner.SelectedDates.Count);
                IList<DateTime> userSelectedDates = _owner.SelectedDates;
                for (int i = 0; i < userSelectedDates.Count; ++i)
                {
                    if (!_owner.InternalSelectedDates.Contains(userSelectedDates[i])
                        || userSelectedDatesTable.ContainsKey(userSelectedDates[i]))
                    {
                        userSelectedDates.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        userSelectedDatesTable.Add(userSelectedDates[i], null);
                    }
                }
            }

            private void InitFlags()
            {
                _isActive = false;
                _isSwitchingMonthTriggered = false;
            }

            /// <summary>
            /// True if the date can be selected
            /// </summary>
            private bool IsSelectable(DateTime date)
            {
                return (_owner.InternalSelectedDates.Count - _toUnselect.Count) < _owner.MaxSelectionCount
                    && MonthCalendarHelper.IsWithinRange(date, _owner.MinDate, _owner.MaxDate);
            }

            #endregion

            #region Private Fields

            private MonthCalendar _owner;
            private List<DateTime> _toSelect;
            private List<DateTime> _toRestoreSelect;
            private List<DateTime> _toUnselect;
            private bool _isActive;
            // True if switching month will be triggered by a new selected date
            private bool _isSwitchingMonthTriggered; 

            #endregion
        }

        

        #endregion
    }
}