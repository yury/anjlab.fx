//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;


namespace AnjLab.FX.Wpf.Controls
{
    /// <summary>
    /// The delegate type for handling a selection changed event
    /// </summary>
    public delegate void DateSelectionChangedEventHandler(
        object sender,
        DateSelectionChangedEventArgs e);

    /// <summary>
    /// The inputs to a selection changed event handler
    /// </summary>
    public class DateSelectionChangedEventArgs : RoutedEventArgs
    {
        //-------------------------------------------------------------------
        //
        //  Constructors
        //
        //-------------------------------------------------------------------

        #region Constructors

        /// <summary>
        /// The constructor for date selection changed args
        /// </summary>
        /// <param name="id">The event ID for the event about to fire -- should probably be MonthCalendar.DateSelectionChangedEventID</param>
        /// <param name="removedDates">The dates that were unselected during this event</param>
        /// <param name="addedDates">The dates that were selected during this event</param>
        public DateSelectionChangedEventArgs(
            RoutedEvent id,
            IList<DateTime> removedDates,
            IList<DateTime> addedDates)
        {
            if (removedDates == null)
                throw new ArgumentNullException("removedDates");
            if (addedDates == null)
                throw new ArgumentNullException("addedDates");

            RoutedEvent = id;

            _removedDates = new List<DateTime>(removedDates);
            _addedDates = new List<DateTime>(addedDates);
        }

        internal DateSelectionChangedEventArgs(IList<DateTime> removedDates, IList<DateTime> addedDates)
            : this(MonthCalendar.DateSelectionChangedEvent, removedDates, addedDates)
        {
        }

        internal DateSelectionChangedEventArgs()
        {
            RoutedEvent = MonthCalendar.DateSelectionChangedEvent;
            _removedDates = new List<DateTime>(1);
            _addedDates = new List<DateTime>(1);
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Properties
        //
        //-------------------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// An IList containing the dates that were unselected during this event
        /// </summary>
        public IList<DateTime> RemovedDates
        {
            get { return _removedDates; }
        }

        /// <summary>
        /// An IList containing the dates that were selected during this event
        /// </summary>
        public IList<DateTime> AddedDates
        {
            get { return _addedDates; }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //-------------------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// This method is used to perform the proper type casting in order to
        /// call the type-safe DateSelectionChangedEventHandler delegate for the DateSelectionChangedEvent event.
        /// </summary>
        /// <param name="genericHandler">The handler to invoke.</param>
        /// <param name="genericTarget">The current object along the event's route.</param>
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            var handler = (DateSelectionChangedEventHandler)genericHandler;

            handler(genericTarget, this);
        }

        #endregion

        private List<DateTime> _addedDates;
        private List<DateTime> _removedDates;
    }
}
